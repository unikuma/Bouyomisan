using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Livet;
using NAudio.Wave;

namespace Bouyomisan.Models
{
    public class BouyomisanEngine : NotificationObject, IDisposable
    {
        public void PlayPronunciation()
        {
            AppSetting.WritePreset();
            Process.Start(
                AquesTalkPath,
                $"/T \"{Pronunciation.Replace(Environment.NewLine, string.Empty)}\" " +
                $"/P \"{AppSetting.Voices[AppSetting.SelectedVoiceIndex].Name}\"");
        }

        public async Task CreateFileAsync()
        {
            AppSetting.WritePreset();
            VoiceSetting voice = AppSetting.Voices[AppSetting.SelectedVoiceIndex];
            OutputSetting output = AppSetting.Outputs[AppSetting.SelectedOutputIndex];

            string audioOut = Path.GetFullPath(output.AudioOut);
            if (!Directory.Exists(audioOut))
            {
                Directory.CreateDirectory(audioOut);
            }

            WaveData wave = await CreateWaveAsync(CreateWavePath(voice, audioOut));
            if (ShouldOutputWavOnly)
            {
                LastGeneratedFile = wave.WavePath;
                return;
            }

            if (AppSetting.IsEnabledTxtOutput)
            {
                CreateText(wave);
            }
            LastGeneratedFile = CreateExo(wave, voice, output);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static readonly string AquesTalkPath = Path.GetFullPath("./AquesTalkPlayer/AquesTalkPlayer.exe");

        public static BouyomisanEngine Instance
        {
            get => _instance;
        }

        public ApplicationSetting AppSetting
        {
            get => _appSetting;
        }

        public string Subtitles
        {
            get => _subtitles;
            set
            {
                RaisePropertyChangedIfSet(ref _subtitles, value);

                if (_subtitles == string.Empty)
                {
                    ShouldCopySubtitles = true;
                }
                if (ShouldCopySubtitles)
                {
                    Pronunciation = Subtitles;
                }
            }
        }

        public string Pronunciation
        {
            get => _pronunciation;
            set => RaisePropertyChangedIfSet(ref _pronunciation, value);
        }

        public bool ShouldCopySubtitles
        {
            get => _shouldCopySubtitles;
            set
            {
                RaisePropertyChangedIfSet(ref _shouldCopySubtitles, value);

                if (_shouldCopySubtitles)
                {
                    Pronunciation = Subtitles;
                }
            }
        }

        public bool ShouldOutputWavOnly
        {
            get => _shouldOutputWavOnly;
            set => RaisePropertyChangedIfSet(ref _shouldOutputWavOnly, value);
        }

        public string LastGeneratedFile
        {
            get => _lastGeneratedFile;
            private set => RaisePropertyChangedIfSet(ref _lastGeneratedFile, value);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    AppSetting.Serialize();
                }

                _disposed = true;
            }
        }

        private BouyomisanEngine()
        {
            if (!File.Exists(ApplicationSetting.SavePath))
            {
                _appSetting = new()
                {
                    Voices = { new() { Name = "プログラムにより追加" } },
                    Outputs = { new() { Name = "プログラムにより追加" } }
                };
                return;
            }

            var temp = ApplicationSetting.Deserialize();
            temp ??= new();

            if (temp.Voices.Count == 0)
            {
                temp.Voices.Add(new() { Name = "プログラムにより追加" });
            }
            if (temp.Outputs.Count == 0)
            {
                temp.Outputs.Add(new() { Name = "プログラムにより追加" });
            }

            _appSetting = temp;
        }

        private string CreateWavePath(VoiceSetting voice, string audioOut)
        {
            var now = DateTime.Now;
            return Path.Combine(audioOut, $"{now:yyyyMMdd}_{now:HHmmss}_{voice.Name}.wav");
        }

        private async Task<WaveData> CreateWaveAsync(string wavePath)
        {
            Process.Start(
                AquesTalkPath,
                $"/T {Pronunciation.Replace("\r\n", string.Empty)} " +
                $"/P {AppSetting.Voices[AppSetting.SelectedVoiceIndex].Name} " +
                $"/W {wavePath}");

            FileStream? fs = null;
            for (int i = 0; i < 256; i++)
            {
                await Task.Delay(50);
                try
                {
                    fs = new(wavePath, FileMode.Open);
                    Debug.WriteLine(i);
                    break;
                }
                catch { }
            }

            if (fs == null)
            {
                throw new TimeoutException($"{wavePath} が見つかりませんでした。");
            }

            WaveFileReader wfr = new(fs);
            double waveTime = wfr.TotalTime.TotalMilliseconds;
            wfr.Dispose();
            fs.Dispose();

            return new(wavePath, waveTime);
        }

        private string CreateExo(WaveData wave, VoiceSetting voice, OutputSetting output)
        {
            string rowSubtitles =
                BitConverter.ToString(Encoding.Unicode.GetBytes(Subtitles)).Replace("-", string.Empty).ToLower().PadRight(4096, '0');
            int endFrame = (int)(Math.Floor((wave.WaveTime + voice.CorrectionMillisecond) / 100d) / 10 * output.AviUtlFps);

            if (endFrame < 1)
            {
                endFrame = 1;
            }

            string exoPath = Path.ChangeExtension(wave.WavePath, ".exo");
            File.WriteAllText(
                exoPath,
                string.Format(voice.ExoTemplate, rowSubtitles, wave.WavePath, endFrame),
                Encoding.GetEncoding("shift_jis"));

            return exoPath;
        }

        private void CreateText(WaveData wave)
        {
            string textPath = Path.ChangeExtension(wave.WavePath, ".txt");
            File.WriteAllText(textPath, Subtitles, Encoding.GetEncoding("shift_jis"));
        }

        private static readonly BouyomisanEngine _instance = new();
        private bool _disposed = false;
        private readonly ApplicationSetting _appSetting;
        private string _subtitles = string.Empty;
        private string _pronunciation = string.Empty;
        private bool _shouldCopySubtitles = true;
        private bool _shouldOutputWavOnly = false;
        private string _lastGeneratedFile = string.Empty;

        private class WaveData
        {
            public WaveData(string path, double time)
            {
                WavePath = path;
                WaveTime = time;
            }

            public string WavePath { get; init; }
            public double WaveTime { get; init; }
        }
    }
}

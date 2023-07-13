using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bouyomisan.Models
{
    internal class NewVoiceCreator
    {
        public static readonly string AquesTalkPlayerPath = Path.GetFullPath(@".\AquesTalkPlayer\AquesTalkPlayer.exe");

        public string SubtitleText { get; set; } = string.Empty;
        public string VoiceText { get; set; } = string.Empty;
        public VoiceSetting SelectedVoice { get; set; } = new();
        public OutputSetting SelectedOutput { get; set; } = new();

        public double TotalWavMilliseconds { get; private set; }

        private string _wavPath = string.Empty;

        public async Task<string> CreateWavAsync()
        {
            _wavPath = CreateWavPath();

            Process.Start(AquesTalkPlayerPath,
                          $"/T \"{VoiceText.Replace("\r\n", string.Empty)}\" " +
                          $"/P \"{SelectedVoice.Name}\" " +
                          $"/W \"{_wavPath}\"");

            FileStream? fs = null;
            for (int i = 0; i < 256; i++)
            {
                await Task.Delay(50);
                try
                {
                    fs = new(_wavPath, FileMode.Open);
                    break;
                }
                catch { }
            }

            if (fs == null)
                throw new TimeoutException(_wavPath + "is not found.");

            using (var wfr = new WaveFileReader(fs))
            {
                TotalWavMilliseconds = wfr.TotalTime.TotalMilliseconds;
                fs.Dispose();
            }

            return _wavPath;
        }

        public string CreateExo()
        {
            string rowSubtitle = BitConverter.ToString(Encoding.Unicode.GetBytes(SubtitleText))
                                             .Replace("-", string.Empty)
                                             .ToLower()
                                             .PadRight(4096, '0');
            int endFrame = (int)(Math.Floor((TotalWavMilliseconds + SelectedVoice.CorrectionMillisecond) / 100d) / 10 * SelectedOutput.AviUtlFps);

            if (endFrame < 1)
                endFrame = 1;

            string exoPath = Path.ChangeExtension(_wavPath, ".exo");
            File.WriteAllText(exoPath,
                              string.Format(SelectedVoice.ExoTemplate, rowSubtitle, _wavPath, endFrame),
                              Encoding.GetEncoding("shift_jis"));
            return exoPath;
        }
        
        public string CreateTxt()
        {
            string txtPath = Path.ChangeExtension(_wavPath, ".txt");
            File.WriteAllText(txtPath, SubtitleText, Encoding.GetEncoding("shift_jis"));
            return txtPath;
        }

        private string CreateWavPath()
        {
            var dtNow = DateTime.Now;
            return Path.Combine(Path.GetFullPath(SelectedOutput.AudioOut),
                                $"{dtNow.ToString("yyyyMMdd")}_{dtNow.ToString("HHmmss")}_{SelectedVoice.Name}_" +
                                $"{SubtitleText[..(SubtitleText.Length <= 17 ? SubtitleText.Length : 17)].Replace("\r\n", string.Empty)}.wav");
        }
    }
}

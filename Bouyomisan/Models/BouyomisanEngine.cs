using System;
using System.Diagnostics;
using System.IO;
using Livet;

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

        public void CreateFile()
        {
            AppSetting.WritePreset();

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

        private void ThrowIfPronunciationIsNullOrWhitespace()
        {
            if (string.IsNullOrWhiteSpace(Pronunciation))
            {
                throw new Exception("読み上げる文字列がありません");
            }
        }

        private static readonly BouyomisanEngine _instance = new();
        private bool _disposed = false;
        private readonly ApplicationSetting _appSetting;
        private string _subtitles = string.Empty;
        private string _pronunciation = string.Empty;
        private bool _shouldCopySubtitles = true;
        private bool _shouldOutputWavOnly = false;
        private string _lastGeneratedFile = string.Empty;
    }
}

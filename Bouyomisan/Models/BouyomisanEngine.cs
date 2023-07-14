using System;
using System.IO;
using Livet;

namespace Bouyomisan.Models
{
    public class BouyomisanEngine : NotificationObject, IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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

            for (int i = 0; i < temp.Voices.Count; i++)
            {
                temp.Voices[i].ExoTemplate = temp.Voices[i].ExoTemplate.Replace("\n", "\r\n");
            }
            _appSetting = temp;
        }

        private static readonly BouyomisanEngine _instance = new();
        private bool _disposed = false;
        private readonly ApplicationSetting _appSetting;
        private string _subtitles = string.Empty;
        private string _pronunciation = string.Empty;
        private bool _shouldCopySubtitles = true;
        private bool _shouldOutputWavOnly = false;
    }
}

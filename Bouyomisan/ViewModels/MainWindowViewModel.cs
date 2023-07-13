using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Bouyomisan.Models;
using Livet;
using Livet.EventListeners;
using Livet.Messaging;

namespace Bouyomisan.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public void Initialize()
        {
            CompositeDisposable.Add(
                new PropertyChangedEventListener(_engine, (s, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(_engine.Subtitles):
                            RaisePropertyChanged(nameof(Subtitles));
                            break;

                        case nameof(_engine.Pronunciation):
                            RaisePropertyChanged(nameof(Pronunciation));
                            break;

                        case nameof(_engine.ShouldCopySubtitles):
                            RaisePropertyChanged(nameof(ShouldCopySubtitles));
                            break;

                        case nameof(_engine.ShouldOutputWavOnly):
                            RaisePropertyChanged(nameof(ShouldOutputWavOnly));
                            break;
                    }
                }));

            CompositeDisposable.Add(
                new PropertyChangedEventListener(_engine, (s, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(_engine.AppSetting.Voices):
                            RaisePropertyChanged(nameof(VoiceSettings));
                            break;

                        case nameof(_engine.AppSetting.Outputs):
                            RaisePropertyChanged(nameof(OutputSettings));
                            break;

                        case nameof(_engine.AppSetting.SelectedVoiceIndex):
                            RaisePropertyChanged(nameof(SelectedVoice));
                            break;

                        case nameof(_engine.AppSetting.SelectedOutputIndex):
                            RaisePropertyChanged(nameof(SelectedOutput));
                            break;
                    }
                }));
        }

        public void ShowSettingWindow()
        {
            var settingViewModel = new SettingWindowViewModel();
            Messenger.Raise(
                new TransitionMessage(typeof(Views.SettingWindow), settingViewModel, TransitionMode.NewOrActive, "ShowSettingWindow"));
        }

        public void DisableCopy()
        {
            ShouldCopySubtitles = false;
        }

        // 読み上げ用文字列をAquesTalkPlayerで再生する
        public void PlayVoiceText()
        {
            if (string.IsNullOrWhiteSpace(Pronunciation))
            {
                MessageBox.Show("読み上げる文字列が無い為、音声を再生できません",
                                "Bouyomisan",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }
            if (!File.Exists(NewVoiceCreator.AquesTalkPlayerPath))
            {
                MessageBox.Show($"指定の場所にAquesTalkPlayer.exeが存在しない為、音声を再生できません",
                                "Bouyomisan",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            // 声設定を基に.presetファイルを作成する
            PresetCreator.Create(VoiceSettings);

            Process.Start(NewVoiceCreator.AquesTalkPlayerPath,
                          $"/T \"{Pronunciation.Replace("\r\n", string.Empty)}\" " +
                          $"/P \"{VoiceSettings[SelectedVoice].Name}\"");
        }

        public async void CreateExoFile(DependencyObject dragSource)
        {
            if (string.IsNullOrWhiteSpace(Pronunciation))
            {
                MessageBox.Show("読み上げる文字列が無い為、音声ファイルを作成できません",
                                "Bouyomisan",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }
            if (!File.Exists(NewVoiceCreator.AquesTalkPlayerPath))
            {
                MessageBox.Show($"指定の場所にAquesTalkPlayer.exeが存在しない為、音声ファイルを作成できません",
                                "Bouyomisan",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            try
            {
                if (!Directory.Exists(Path.GetFullPath(OutputSettings[SelectedOutput].AudioOut)))
                    Directory.CreateDirectory(Path.GetFullPath(OutputSettings[SelectedOutput].AudioOut));

                // 声設定を基に.presetファイルを作成する
                PresetCreator.Create(VoiceSettings);

                // 現在の設定を基に音声ファイルとExoファイルを作成する
                nvc.SubtitleText = Subtitles;
                nvc.VoiceText = Pronunciation;
                nvc.SelectedVoice = VoiceSettings[SelectedVoice];
                nvc.SelectedOutput = OutputSettings[SelectedOutput];
                string wavPath = await nvc.CreateWavAsync();

                DragDrop.DoDragDrop(dragSource,
                                    new DataObject(DataFormats.FileDrop, new string[] { ShouldOutputWavOnly ? wavPath : nvc.CreateExo() }),
                                    DragDropEffects.Copy);
            }
            catch (TimeoutException e)
            {
                MessageBox.Show(e.Message, "Bouyomisan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string Version => $"v{Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)}";

        public string Subtitles
        {
            get => _engine.Subtitles;
            set => _engine.Subtitles = value;
        }

        public string Pronunciation
        {
            get => _engine.Pronunciation;
            set => _engine.Pronunciation = value;
        }

        public bool ShouldCopySubtitles
        {
            get => _engine.ShouldCopySubtitles;
            set => _engine.ShouldCopySubtitles = value;
        }

        public ObservableCollection<VoiceSetting> VoiceSettings
        {
            get => _engine.AppSetting.Voices;
            set => _engine.AppSetting.Voices = value;
        }

        public ObservableCollection<OutputSetting> OutputSettings
        {
            get => _engine.AppSetting.Outputs;
            set => _engine.AppSetting.Outputs = value;
        }

        public int SelectedVoice
        {
            get => _engine.AppSetting.SelectedVoiceIndex;
            set => _engine.AppSetting.SelectedVoiceIndex = value;
        }

        public int SelectedOutput
        {
            get => _engine.AppSetting.SelectedOutputIndex;
            set => _engine.AppSetting.SelectedOutputIndex = value;
        }

        public bool ShouldOutputWavOnly
        {
            get => _engine.ShouldOutputWavOnly;
            set => _engine.ShouldOutputWavOnly = value;
        }

        // ウィンドウが閉じた
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _engine.Dispose();
                }

                _disposed = true;
                base.Dispose(disposing);
            }
        }

        private bool _disposed = false;
        private readonly BouyomisanEngine _engine = BouyomisanEngine.Instance;
        private readonly NewVoiceCreator nvc = new();
    }
}

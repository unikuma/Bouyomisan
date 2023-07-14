using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

                        case nameof(_engine.LastGeneratedFile):
                            Messenger.Raise(new());
                            break;
                    }
                }));

            CompositeDisposable.Add(
                new PropertyChangedEventListener(_engine.AppSetting, (s, e) =>
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
                            RaisePropertyChanged(nameof(SelectedVoiceIndex));
                            break;

                        case nameof(_engine.AppSetting.SelectedOutputIndex):
                            RaisePropertyChanged(nameof(SelectedOutputIndex));
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

        public void PlayPronunciation()
        {
            if (!File.Exists(BouyomisanEngine.AquesTalkPath))
            {
                Messenger.Raise(
                    new InformationMessage(
                        "指定の場所にAquesTalkPlayer.exeが存在しない為、音声の再生が出来ません",
                        "Bouyomisan エラー",
                        MessageBoxImage.Error,
                        "BEngineError"));
                return;
            }
            if (string.IsNullOrWhiteSpace(Pronunciation))
            {
                Messenger.Raise(
                    new InformationMessage(
                        "読み上げる文字列が無いため再生できません",
                        "Bouyomisan エラー",
                        MessageBoxImage.Error,
                        "BEngineError"));
                return;
            }

            _engine.PlayPronunciation();
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
                if (!Directory.Exists(Path.GetFullPath(OutputSettings[SelectedOutputIndex].AudioOut)))
                    Directory.CreateDirectory(Path.GetFullPath(OutputSettings[SelectedOutputIndex].AudioOut));

                // 声設定を基に.presetファイルを作成する
                PresetCreator.Create(VoiceSettings);

                // 現在の設定を基に音声ファイルとExoファイルを作成する
                nvc.SubtitleText = Subtitles;
                nvc.VoiceText = Pronunciation;
                nvc.SelectedVoice = VoiceSettings[SelectedVoiceIndex];
                nvc.SelectedOutput = OutputSettings[SelectedOutputIndex];
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

#pragma warning disable CA1822
        public string Title => "Bouyomisan v4.0.0 Preview 1";
#pragma warning restore CA1822

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

        public int SelectedVoiceIndex
        {
            get => _engine.AppSetting.SelectedVoiceIndex;
            set => _engine.AppSetting.SelectedVoiceIndex = value;
        }

        public int SelectedOutputIndex
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

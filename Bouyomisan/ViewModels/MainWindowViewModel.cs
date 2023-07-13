using Bouyomisan.Models;
using Livet;
using Livet.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Bouyomisan.ViewModels
{
    public class MainWindowViewModel : ViewModel, IDisposable
    {
        #region VoiceSettingsプロパティ
        /// <summary>
        /// 声設定
        /// </summary>
        public ObservableCollection<VoiceSetting> VoiceSettings
        {
            get => _voiceSettings;
            set => RaisePropertyChangedIfSet(ref _voiceSettings, value);
        }

        private ObservableCollection<VoiceSetting> _voiceSettings = new();
        #endregion

        #region SelectedVoiceプロパティ
        /// <summary>
        /// 現在選択中の声設定のインデックス
        /// </summary>
        public int SelectedVoice
        {
            get => _selectedVoice;
            set => RaisePropertyChangedIfSet(ref _selectedVoice, value < 0 ? 0 : value);
        }

        private int _selectedVoice = 0;
        #endregion

        #region OutputSettingsプロパティ
        /// <summary>
        /// 出力設定
        /// </summary>
        public ObservableCollection<OutputSetting> OutputSettings
        {
            get => _outputSettings;
            set => RaisePropertyChangedIfSet(ref _outputSettings, value);
        }

        private ObservableCollection<OutputSetting> _outputSettings = new();
        #endregion

        #region SelectedOutputプロパティ
        /// <summary>
        /// 現在選択中の出力設定のインデックス
        /// </summary>
        public int SelectedOutput
        {
            get => _selectedOutput;
            set => RaisePropertyChangedIfSet(ref _selectedOutput, value < 0 ? 0 : value);
        }

        private int _selectedOutput = 0;
        #endregion

        #region WordDictionaryプロパティ
        /// <summary>
        /// 辞書データ
        /// </summary>
        public ObservableCollection<WordPair> WordDictionary
        {
            get => _wordDictionary;
            set => RaisePropertyChangedIfSet(ref _wordDictionary, value);
        }

        private ObservableCollection<WordPair> _wordDictionary = new();
        #endregion

        #region ShouldWavOnlyOutputプロパティ
        /// <summary>
        /// Wavファイルのみ出力するべきか否か
        /// </summary>
        public bool ShouldWavOnlyOutput
        {
            get => _shouldWavOnlyOutput;
            set => RaisePropertyChangedIfSet(ref _shouldWavOnlyOutput, value);
        }

        private bool _shouldWavOnlyOutput = false;
        #endregion

        /// <summary>
        /// 棒読みさんのバージョン
        /// </summary>
        public string Version => $"v{Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)}";

        private ApplicationSettings _appSettings = new();

        private readonly NewVoiceCreator nvc = new();

        public void Initialize()
        {
            // ファイル読み書きなどでsjisを使えるようにする。
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (!File.Exists("Settings.xml"))
            {
                VoiceSettings.Add(new VoiceSetting() { Name = "プログラムより自動追加" });
                OutputSettings.Add(new OutputSetting() { Name = "プログラムより自動追加" });

                MessageBox.Show("Settings.xml が見つかりません\r\n各設定タブから設定を追加してください",
                                "Bouyomisan",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            // 設定ファイルを読み込んで適用する
            // _appSettings = Expansion.StaticXmlSerializer.Deserialize<ApplicationSettings>("Settings.xml") ?? new ApplicationSettings();

            VoiceSettings = _appSettings.Voices;
            OutputSettings = _appSettings.Outputs;
            WordDictionary = _appSettings.WordDictionary;
            SelectedVoice = _appSettings.SelectedIndex.voice;
            SelectedOutput = _appSettings.SelectedIndex.output;
        }

        #region SubtitleTextプロパティ
        /// <summary>
        /// AviUtl上での字幕になる文字列
        /// </summary>
        public string SubtitleText
        {
            get => _subtitleText;
            set
            {
                if (ShouldCopyText)
                    ApplyDictionary(value);

                RaisePropertyChangedIfSet(ref _subtitleText, value);

                if (value == string.Empty)
                    ShouldCopyText = true;
            }
        }

        private string _subtitleText = string.Empty;
        #endregion

        #region VoiceTextプロパティ
        /// <summary>
        /// AquesTalkPlayerを通して読み上げる文字列
        /// </summary>
        public string VoiceText
        {
            get => _voiceText;
            set => RaisePropertyChangedIfSet(ref _voiceText, value);
        }

        private string _voiceText = string.Empty;
        #endregion

        // 読み上げ用文字列をAquesTalkPlayerで再生する
        public void PlayVoiceText()
        {
            if (string.IsNullOrWhiteSpace(VoiceText))
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
            PresetCreator.CreateIni(_appSettings);

            Process.Start(NewVoiceCreator.AquesTalkPlayerPath,
                          $"/T \"{VoiceText.Replace("\r\n", string.Empty)}\" " +
                          $"/P \"{VoiceSettings[SelectedVoice].Name}\"");
        }

        public async void CreateExoFile(DependencyObject dragSource)
        {
            if (string.IsNullOrWhiteSpace(VoiceText))
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
                PresetCreator.CreateIni(_appSettings);

                // 現在の設定を基に音声ファイルとExoファイルを作成する
                nvc.SubtitleText = SubtitleText;
                nvc.VoiceText = VoiceText;
                nvc.SelectedVoice = VoiceSettings[SelectedVoice];
                nvc.SelectedOutput = OutputSettings[SelectedOutput];
                string wavPath = await nvc.CreateWavAsync();

                if (_appSettings.IsEnabledTxtOutput)
                    nvc.CreateTxt();

                DragDrop.DoDragDrop(dragSource,
                                    new DataObject(DataFormats.FileDrop, new string[] { ShouldWavOnlyOutput ? wavPath : nvc.CreateExo() }),
                                    DragDropEffects.Copy);
            }
            catch (TimeoutException e)
            {
                MessageBox.Show(e.Message, "Bouyomisan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region ShouldCopyTextプロパティ
        /// <summary>
        /// 字幕用文字列を読み上げ用文字列にコピーするべきか否か
        /// </summary>
        public bool ShouldCopyText
        {
            get => _shouldCopyText;
            set
            {
                if (value)
                    ApplyDictionary(SubtitleText);
                RaisePropertyChangedIfSet(ref _shouldCopyText, value);
            }
        }

        private bool _shouldCopyText = true;
        #endregion

        public void DisableCopy()
        {
            ShouldCopyText = false;
        }

        // 設定ウィンドウを開く
        public void ShowSettingWindow()
        {
            var settingViewModel = new SettingWindowViewModel(_appSettings)
            {
                VoiceSettings = VoiceSettings,
                SelectedVoice = SelectedVoice,
                OutputSettings = OutputSettings,
                SelectedOutput = SelectedOutput,
                WordDictionary = WordDictionary,
            };
            var message = new TransitionMessage(typeof(Views.SettingWindow), settingViewModel, TransitionMode.NewOrActive, "ShowSettingWindow");
            Messenger.Raise(message);
        }

        // ウィンドウが閉じた
        public new void Dispose()
        {
            _appSettings.Voices = VoiceSettings;
            _appSettings.Outputs = OutputSettings;
            _appSettings.WordDictionary = WordDictionary;
            _appSettings.SelectedIndex = (SelectedVoice, SelectedOutput);

            // Expansion.StaticXmlSerializer.Serialize("Settings.xml", _appSettings);
        }

        // 読み上げ用文字列に辞書を適用
        private void ApplyDictionary(string value)
        {
            string temp = value;

            foreach (var data in WordDictionary)
                temp = data.IsEnable ? Regex.Replace(temp, data.Before, data.After, RegexOptions.IgnoreCase) : temp;

            VoiceText = temp;
        }

        private readonly BouyomisanEngine _engine = BouyomisanEngine.Instance;
    }
}

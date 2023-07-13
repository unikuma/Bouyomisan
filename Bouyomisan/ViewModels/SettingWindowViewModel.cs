using Bouyomisan.Models;
using Livet;
using System.Collections.ObjectModel;
using System.Windows;

namespace Bouyomisan.ViewModels
{
    public class SettingWindowViewModel : ViewModel
    {
        private ApplicationSetting _appSetting = new();

        public SettingWindowViewModel()
        {
        }

        public SettingWindowViewModel(ApplicationSetting appSetting)
        {
            _appSetting = appSetting;
            IsEnabledTxtOutput = _appSetting.IsEnabledTxtOutput;
        }

        public void Initialize()
        {
        }

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
        public int SelectedVoice { get; init; }
        #endregion

        // 声設定を追加します
        public void AddVoice()
        {
            VoiceSettings.Add(new VoiceSetting());
        }

        // 声設定を削除します
        public void RemoveVoice(VoiceSetting voice)
        {
            if (MessageBoxResult.Yes == MessageBox.Show($"{voice.Name} を削除してもよろしいですか？",
                                                        "Bouyomisan",
                                                        MessageBoxButton.YesNo,
                                                        MessageBoxImage.Warning))
            {
                int index = VoiceSettings.IndexOf(voice);

                if (SelectedVoice == index)
                    MessageBox.Show("現在選択中の声設定を削除する事はできません", "Bouyomisan", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    VoiceSettings.RemoveAt(index);
            }
        }

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
        public int SelectedOutput { get; init; }
        #endregion

        // 出力設定を追加します
        public void AddOutput()
        {
            OutputSettings.Add(new OutputSetting());
        }

        // 出力設定を削除します
        public void RemoveOutput(OutputSetting output)
        {
            if (MessageBoxResult.Yes == MessageBox.Show($"{output.Name} を削除してもよろしいですか？",
                                                        "Bouyomisan",
                                                        MessageBoxButton.YesNo,
                                                        MessageBoxImage.Warning))
            {
                int index = OutputSettings.IndexOf(output);

                if (SelectedOutput == index)
                    MessageBox.Show("現在選択中の出力設定を削除する事はできません", "Bouyomisan", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    OutputSettings.RemoveAt(index);
            }
        }

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

        #region SelectedWordPairプロパティ
        /// <summary>
        /// 現在選択中の辞書データのインデックス
        /// </summary>
        public int SelectedWordPair { get; set; }
        #endregion

        // 登録された辞書データの優先度を上げます
        public void IncreaseWordPairPriority()
        {
            if (0 < SelectedWordPair && SelectedWordPair < WordDictionary.Count)
            {
                // 配列を入れ替えるとDataGridの選択が解除されてしまう為、現在のインデックスを予め記録しておく
                int n = SelectedWordPair;

                (WordDictionary[SelectedWordPair], WordDictionary[SelectedWordPair - 1]) = (WordDictionary[SelectedWordPair - 1], WordDictionary[SelectedWordPair]);
                SelectedWordPair = --n;
            }
        }

        // 登録された辞書データの優先度を下げます
        public void DecreaseWordPairPriority()
        {
            if (SelectedWordPair < WordDictionary.Count - 1)
            {
                // 配列を入れ替えるとDataGridの選択が解除されてしまう為、現在のインデックスを予め記録しておく
                int n = SelectedWordPair;

                (WordDictionary[SelectedWordPair + 1], WordDictionary[SelectedWordPair]) = (WordDictionary[SelectedWordPair], WordDictionary[SelectedWordPair + 1]);
                SelectedWordPair = ++n;
            }
        }

        #region IsEnabledTxtOutputプロパティ
        public bool IsEnabledTxtOutput
        {
            get => _appSetting.IsEnabledTxtOutput;
            set
            {
                _appSetting.IsEnabledTxtOutput = value;
                RaisePropertyChanged();
            }
        }
        #endregion
    }
}

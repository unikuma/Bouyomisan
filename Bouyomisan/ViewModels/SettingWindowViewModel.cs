using Bouyomisan.Models;
using Livet;
using Livet.EventListeners;
using System.Collections.ObjectModel;
using System.Windows;

namespace Bouyomisan.ViewModels
{
    public class SettingWindowViewModel : ViewModel
    {
        public void Initialize()
        {
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
                        RaisePropertyChanged(nameof(SelectedVoice));
                        break;

                    case nameof(_engine.AppSetting.SelectedOutputIndex):
                        RaisePropertyChanged(nameof(SelectedOutput));
                        break;
                }
            }));
        }

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

        public ObservableCollection<WordPair> WordDictionary
        {
            get => _engine.AppSetting.Words;
            set => _engine.AppSetting.Words = value;
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

        #region SelectedWordPairプロパティ
        /// <summary>
        /// 現在選択中の辞書データのインデックス
        /// </summary>
        public int SelectedWordPair { get; set; }
        #endregion

        public bool IsEnabledTxtOutput
        {
            get => _engine.AppSetting.IsEnabledTxtOutput;
            set => _engine.AppSetting.IsEnabledTxtOutput = value;
        }

        private readonly BouyomisanEngine _engine = BouyomisanEngine.Instance;
    }
}

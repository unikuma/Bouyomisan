using Bouyomisan.Models;
using Livet;
using Livet.EventListeners;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace Bouyomisan.ViewModels
{
    public class SettingWindowViewModel : ViewModel
    {
        public SettingWindowViewModel()
        {
            foreach (var vtype in Enum.GetValues<VoiceType>())
            {
                string engnName =
                    vtype.GetType().GetField(vtype.ToString())?.GetCustomAttribute<VoiceEngineAttribute>()?.EngineName ?? "null";

                string description =
                    vtype.GetType().GetField(vtype.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "null";

                Types.Add($"{engnName} / {description}", vtype);
            }
        }

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
                            RaisePropertyChanged(nameof(SelectedVoiceIndex));
                            break;

                        case nameof(_engine.AppSetting.SelectedOutputIndex):
                            RaisePropertyChanged(nameof(SelectedOutputIndex));
                            break;

                        case nameof(_engine.AppSetting.IsEnabledTxtOutput):
                            RaisePropertyChanged(nameof(IsEnabledTxtOutput));
                            break;
                    }
                }));
        }

        public void AddVoice()
        {
            VoiceSettings.Add(new VoiceSetting());
        }

        public void RemoveVoice(VoiceSetting voice)
        {
            if (VoiceSettings.IndexOf(voice) == SelectedVoiceIndex)
            {
                Messenger.Raise(
                    new InformationMessage(
                        "現在選択中の声設定を削除することは出来ません",
                        "Bouyomisan エラー",
                        MessageBoxImage.Error,
                        "CannotRemove"));
                return;
            }

            var confirmes = new ConfirmationMessage(voice.Name + " を削除してもよろしいですか？", "Bouyomisan", "CanRemove")
            {
                Button = MessageBoxButton.YesNo,
                Image = MessageBoxImage.Warning
            };
            Messenger.Raise(confirmes);

            if (confirmes.Response == true)
            {
                VoiceSettings.Remove(voice);
            }
        }

        public void AddOutput()
        {
            OutputSettings.Add(new OutputSetting());
        }

        public void RemoveOutput(OutputSetting output)
        {
            if (OutputSettings.IndexOf(output) == SelectedOutputIndex)
            {
                Messenger.Raise(
                    new InformationMessage(
                        "現在選択中の出力設定を削除することは出来ません",
                        "Bouyomisan エラー",
                        MessageBoxImage.Error,
                        "CannotRemove"));
                return;
            }

            var confirmes = new ConfirmationMessage(output.Name + " を削除してもよろしいですか？", "Bouyomisan", "CanRemove")
            {
                Button = MessageBoxButton.YesNo,
                Image = MessageBoxImage.Warning
            };
            Messenger.Raise(confirmes);

            if (confirmes.Response == true)
            {
                OutputSettings.Remove(output);
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

        public Dictionary<string, VoiceType> Types { get; } = new();

        private readonly BouyomisanEngine _engine = BouyomisanEngine.Instance;
    }
}

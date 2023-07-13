using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Livet;

namespace Bouyomisan.Models
{
    [XmlRoot("BouyomisanSettings")]
    public class ApplicationSetting : NotificationObject
    {
        // 声設定コレクション
        [XmlArray("Voices"), XmlArrayItem("Voice")]
        public ObservableCollection<VoiceSetting> Voices
        {
            get => _voices;
            set => RaisePropertyChangedIfSet(ref _voices, value);
        }

        // 出力設定コレクション
        [XmlArray("Outputs"), XmlArrayItem("Output")]
        public ObservableCollection<OutputSetting> Outputs
        {
            get => _outputs;
            set => RaisePropertyChangedIfSet(ref _outputs, value);
        }

        // 辞書
        [XmlArray("Words"), XmlArrayItem("Word")]
        public ObservableCollection<WordPair> Words
        {
            get => _words;
            set => RaisePropertyChangedIfSet(ref _words, value);
        }

        // *.txt出力が有効か否か
        [XmlElement("IsEnabledTxtOutput")]
        public bool IsEnabledTxtOutput
        {
            get => _isEnabledTxtOutput;
            set => RaisePropertyChangedIfSet(ref _isEnabledTxtOutput, value);
        }

        private ObservableCollection<VoiceSetting> _voices = new();
        private ObservableCollection<OutputSetting> _outputs = new();
        private ObservableCollection<WordPair> _words = new();
        private bool _isEnabledTxtOutput;
    }
}

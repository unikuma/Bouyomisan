using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Extension.Serialization;
using Livet;

namespace Bouyomisan.Models
{
    [XmlRoot("BouyomisanSettings")]
    public class ApplicationSetting : NotificationObject
    {
        public static ApplicationSetting? Deserialize()
        {
            return StaticXmlSerializer.Deserialize<ApplicationSetting>(SavePath);
        }

        public void Serialize()
        {
            StaticXmlSerializer.Serialize(SavePath, this);
        }

        public void WritePreset()
        {
            if (!Directory.Exists(PresetDir))
            {
                Directory.CreateDirectory(PresetDir);
            }

            using var stw = new StreamWriter(PresetDir + "AquesTalkPlayer.preset", append: false, Encoding.GetEncoding("shift_jis"));
            stw.WriteLine("プリセット名,棒読み,エンジン,声種,話速,音量,高さ,アクセント,声質,音程,メモ");
            foreach (var voice in Voices)
            {
                stw.WriteLine(voice.ToString());
            }
        }

        public static readonly string SavePath = Path.GetFullPath("./Settings.xml");
        public static readonly string PresetDir = Path.GetFullPath("./AquesTalkPlayer/");

        [XmlArray("Voices"), XmlArrayItem("Voice")]
        public ObservableCollection<VoiceSetting> Voices
        {
            get => _voices;
            set => RaisePropertyChangedIfSet(ref _voices, value);
        }

        [XmlArray("Outputs"), XmlArrayItem("Output")]
        public ObservableCollection<OutputSetting> Outputs
        {
            get => _outputs;
            set => RaisePropertyChangedIfSet(ref _outputs, value);
        }

        [XmlArray("Words"), XmlArrayItem("Word")]
        public ObservableCollection<WordPair> Words
        {
            get => _words;
            set => RaisePropertyChangedIfSet(ref _words, value);
        }

        public int SelectedVoiceIndex
        {
            get => _selectedVoiceIndex;
            set => RaisePropertyChangedIfSet(ref _selectedVoiceIndex, value);
        }

        public int SelectedOutputIndex
        {
            get => _selectedOutputIndex;
            set => RaisePropertyChangedIfSet(ref _selectedOutputIndex, value);
        }

        [XmlElement("IsEnabledTxtOutput")]
        public bool IsEnabledTxtOutput
        {
            get => _isEnabledTxtOutput;
            set => RaisePropertyChangedIfSet(ref _isEnabledTxtOutput, value);
        }

        private ObservableCollection<VoiceSetting> _voices = new();
        private ObservableCollection<OutputSetting> _outputs = new();
        private ObservableCollection<WordPair> _words = new();
        private int _selectedVoiceIndex = 0;
        private int _selectedOutputIndex = 0;
        private bool _isEnabledTxtOutput = false;
    }
}

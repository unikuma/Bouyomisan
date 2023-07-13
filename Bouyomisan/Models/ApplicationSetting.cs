using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

namespace Bouyomisan.Models
{
    [XmlRoot("BouyomisanSettings")]
    public class ApplicationSetting
    {
        // 声設定コレクション
        [XmlArray("Voices"), XmlArrayItem("Voice")]
        public ObservableCollection<VoiceSetting> Voices { get; set; } = new ObservableCollection<VoiceSetting>();

        // 出力設定コレクション
        [XmlArray("Outputs"), XmlArrayItem("Output")]
        public ObservableCollection<OutputSetting> Outputs { get; set; } = new ObservableCollection<OutputSetting>();

        // 辞書
        [XmlArray("Words"), XmlArrayItem("Word")]
        public ObservableCollection<WordPair> WordDictionary { get; set; } = new ObservableCollection<WordPair>();

        // 削除ダイアログを表示するかどうか
        public bool CanShowRemoveDialog { get; set; } = true;

        // 選択中の声、出力設定
        [XmlElement("SelectedIndex")]
        public (int voice, int output) SelectedIndex { get; set; } = (0, 0);

        // *.txt出力が有効か否か
        [XmlElement("IsEnabledTxtOutput")]
        public bool IsEnabledTxtOutput { get; set; } = false;

        // リサンプル可能か
        [XmlElement("CanResample")]
        public bool CanResample { get; set; } = false;

        // リサンプルモード
        [XmlElement("ResampleMode")]
        public ResampleMode ResampleMode { get; set; } = ResampleMode.Sinc;

        // リサンプル周波数
        [XmlElement("ResampleFS")]
        public ResampleFS ResampleFS { get; set; } = ResampleFS.Small;

        public static Dictionary<string, ResampleMode> ResampleModes { get; } = new();
        public static Dictionary<string, ResampleFS> ResampleFSs { get; } = new();

        static ApplicationSetting()
        {
            foreach (var mode in Enum.GetValues<ResampleMode>())
            {
                var description = mode.GetType().GetField(mode.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "null";
                ResampleModes.Add(mode.ToString() + " / " + description ?? string.Empty, mode);
            }

            foreach (var fs in Enum.GetValues<ResampleFS>())
            {
                var description = fs.GetType().GetField(fs.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "null";
                ResampleFSs.Add(description ?? string.Empty, fs);
            }
        }
    }

    public enum ResampleMode
    {
        [Description("オリジナルの音質を維持したままリサンプリングします。")] Sinc,
        [Description("3次多項式で補間します。YMM4のデフォルト設定と同等。")] Cubic,
        [Description("線形補間します。AviUtilと同等。")] Linear,
    }

    public enum ResampleFS
    {
        [Description("44100hz")] Small = 44100,
        [Description("48000hz")] Large = 48000,
    }
}

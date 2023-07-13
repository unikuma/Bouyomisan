using Livet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

namespace Bouyomisan.Models
{
    public class OutputSetting : NotificationObject
    {
        private string _Name = "デフォルト30";
        private byte _AviUtlFps = 30;
        private string _AudioOut = @".\AudioOut";

        [XmlElement("Name")]
        public string Name
        {
            get => _Name;
            set => RaisePropertyChangedIfSet(ref _Name, value);
        }

        /// <summary>
        /// AviUtlで設定されているFPS
        /// </summary>
        [XmlElement("AviUtlFps")]
        public byte AviUtlFps
        {
            get => _AviUtlFps;
            set => RaisePropertyChangedIfSet(ref _AviUtlFps, value);
        }

        /// <summary>
        /// 音声出力先。Exo, Txtファイルもここに出力される
        /// </summary>
        [XmlElement("AudioOut")]
        public string AudioOut
        {
            get => _AudioOut;
            set => RaisePropertyChangedIfSet(ref _AudioOut, value);
        }
    }

    public class VoiceSetting : NotificationObject
    {
        private string _Name = "デフォルト";
        private VoiceType _SelectedType = VoiceType.F1;
        private bool _IsBouyomi = false;

        private ushort _Speed = 100;
        private ushort _Volume = 100;
        private ushort _Pitch = 100;

        private int _CorrectionMillisecond = 500;
        private string _ExoTemplate = @"[0]
start=1
end={2}
layer=1
overlay=1
audio=1
[0.0]
_name=音声ファイル
再生位置=0.00
再生速度=100.0
ループ再生=0
動画ファイルと連携=0
file={1}
[0.1]
_name=標準再生
音量=100.0
左右=0.0";

        [XmlElement("Name")]
        public string Name
        {
            get => _Name;
            set => RaisePropertyChangedIfSet(ref _Name, value);
        }

        [XmlElement("VoiceType")]
        public VoiceType SelectedType
        {
            get => _SelectedType;
            set => RaisePropertyChangedIfSet(ref _SelectedType, value);
        }

        [XmlElement("Bouyomi")]
        public bool IsBouyomi
        {
            get => _IsBouyomi;
            set => RaisePropertyChangedIfSet(ref _IsBouyomi, value);
        }

        [XmlElement("Speed")]
        public ushort Speed
        {
            get => _Speed;
            set => RaisePropertyChangedIfSet(ref _Speed, value);
        }

        [XmlElement("Volume")]
        public ushort Volume
        {
            get => _Volume;
            set => RaisePropertyChangedIfSet(ref _Volume, value);
        }

        [XmlElement("Pitch")]
        public ushort Pitch
        {
            get => _Pitch;
            set => RaisePropertyChangedIfSet(ref _Pitch, value);
        }

        [XmlElement("CorrectionMillisecond")]
        public int CorrectionMillisecond
        {
            get => _CorrectionMillisecond;
            set => RaisePropertyChangedIfSet(ref _CorrectionMillisecond, value);
        }

        [XmlElement("ExoTemplate")]
        public string ExoTemplate
        {
            get => _ExoTemplate;
            set => RaisePropertyChangedIfSet(ref _ExoTemplate, value);
        }

        public static Dictionary<string, VoiceType> Types { get; } = new Dictionary<string, VoiceType>();

        static VoiceSetting()
        {
            foreach (var vtype in Enum.GetValues<VoiceType>())
            {
                // vtype(VoiceType構造体)からFieldInfoを取得し、
                // GetCustomAttribute<T>関数でDescriptionAttributeのDescriptionプロパティを取得
                var engineName = vtype.GetType().GetField(vtype.ToString())?.GetCustomAttribute<VoiceEngineAttribute>()?.EngineName ?? "null";
                var description = vtype.GetType().GetField(vtype.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "null";
                Types.Add((engineName + " / " + description) ?? string.Empty, vtype);
            }
        }

        /// <summary>
        /// 自身のプリセット文字列を返す
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // プリセット名,棒読み,エンジン,声種,話速,音量,高さ,アクセント,声質,音程,メモ
            return $"\"{Name}\"," +
                   $"\"{IsBouyomi}\"," +
                   $"\"{SelectedType.GetType().GetField(SelectedType.ToString())?.GetCustomAttribute<VoiceEngineAttribute>()?.EngineName}\"," +
                   $"\"{SelectedType.ToString().ToLower()}\"," +
                   $"{Speed}," +
                   $"{Volume}," +
                   $"100," +
                   $"100," +
                   $"100," +
                   $"{Pitch}," +
                   $"\"Bouyomisan\"";
        }
    }

    public enum VoiceType
    {
        // AquesTalk1声種(AquesTalk1は11~18)
        [Description("女性１"), VoiceEngine("AquesTalk1")] F1 = 11,
        [Description("女性２"), VoiceEngine("AquesTalk1")] F2,
        [Description("男性１"), VoiceEngine("AquesTalk1")] M1,
        [Description("男性２"), VoiceEngine("AquesTalk1")] M2,
        [Description("ロボット１"), VoiceEngine("AquesTalk1")] R1,
        [Description("中性"), VoiceEngine("AquesTalk1")] Imd1,
        [Description("機械"), VoiceEngine("AquesTalk1")] Dvd,
        [Description("特殊"), VoiceEngine("AquesTalk1")] Jgr,
        // AquesTalk2声種(AquesTalk2は21~30)
        [Description("女性A"), VoiceEngine("AquesTalk2")] Aq_defo1 = 21,
        [Description("女性C"), VoiceEngine("AquesTalk2")] Aq_f1c,
        [Description("女性D"), VoiceEngine("AquesTalk2")] Aq_huskey,
        [Description("女性E"), VoiceEngine("AquesTalk2")] Aq_momo1,
        [Description("女性F"), VoiceEngine("AquesTalk2")] Aq_rb2,
        [Description("女性G"), VoiceEngine("AquesTalk2")] Aq_rm,
        [Description("女性H"), VoiceEngine("AquesTalk2")] Aq_yukkuri,
        [Description("男性B"), VoiceEngine("AquesTalk2")] Aq_m4b,
        [Description("ロボット２"), VoiceEngine("AquesTalk2")] Aq_robo,
        [Description("中性２"), VoiceEngine("AquesTalk2")] Aq_teto1,
    }

    public class VoiceEngineAttribute : Attribute
    {
        public string EngineName { get; set; } = string.Empty;

        public VoiceEngineAttribute(string engineName)
        {
            EngineName = engineName;
        }
    }

    [XmlRoot("BouyomisanSettings")]
    public class ApplicationSettings
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

        static ApplicationSettings()
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

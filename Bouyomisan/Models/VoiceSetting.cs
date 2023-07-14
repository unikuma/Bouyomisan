using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using Livet;

namespace Bouyomisan.Models
{
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

    [AttributeUsage(AttributeTargets.Field)]
    public class VoiceEngineAttribute : Attribute
    {
        public string EngineName { get; set; } = string.Empty;

        public VoiceEngineAttribute(string engineName)
        {
            EngineName = engineName;
        }
    }
}

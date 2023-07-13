using Livet;
using System.Xml.Serialization;

namespace Bouyomisan.Models
{
    /// <summary>
    /// 文字列置換用の辞書データクラスです。
    /// </summary>
    public class WordPair : NotificationObject
    {
        private bool _IsEnable = true;
        private string _Before = string.Empty;
        private string _After = string.Empty;
        private string _Remarks = string.Empty;

        [XmlElement("Enable")]
        public bool IsEnable
        {
            get => _IsEnable;
            set => RaisePropertyChangedIfSet(ref _IsEnable, value);
        }

        [XmlElement("Before")]
        public string Before
        {
            get => _Before;
            set => RaisePropertyChangedIfSet(ref _Before, value);
        }

        [XmlElement("After")]
        public string After
        {
            get => _After;
            set => RaisePropertyChangedIfSet(ref _After, value);
        }

        [XmlElement("Remarks")]
        public string Remarks
        {
            get => _Remarks;
            set => RaisePropertyChangedIfSet(ref _Remarks, value);
        }
    }
}

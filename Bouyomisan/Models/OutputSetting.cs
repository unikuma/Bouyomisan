using System.Xml.Serialization;
using Livet;

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
}

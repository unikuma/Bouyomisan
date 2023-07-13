using Livet;

namespace Bouyomisan.Models
{
    /// <summary>
    /// 棒読みさんの核となる音声生成・字幕生成を担当するクラスです。
    /// </summary>
    public class BouyomisanEngine : NotificationObject
    {
        public static BouyomisanEngine Instance
        {
            get => _instance;
        }

        private BouyomisanEngine() { }
        private static readonly BouyomisanEngine _instance = new();
    }
}

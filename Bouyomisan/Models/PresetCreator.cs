using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Bouyomisan.Models
{
    internal static class PresetCreator
    {
        /// <summary>
        /// 渡された声設定コレクションからAquesTalkPlayer.presetファイルを作成します。
        /// </summary>
        /// <param name="settings"></param>
        public static void Create(IReadOnlyCollection<VoiceSetting> settings)
        {
            using (var stw = new StreamWriter("AquesTalkPlayer\\AquesTalkPlayer.preset", append: false, Encoding.GetEncoding("shift_jis")))
            {
                stw.WriteLine("プリセット名,棒読み,エンジン,声種,話速,音量,高さ,アクセント,声質,音程,メモ");
                foreach (var setting in settings)
                    stw.WriteLine(setting.ToString());
            }
        }

        public static void CreateIni(ApplicationSettings appSetting)
        {
            string aqInit = File.ReadAllText("AquesTalkPlayer\\AquesTalkPlayer.ini", Encoding.GetEncoding("shift_jis"));

            string pattern = @"bResample=\d
resample_method=\d
resample_fs=\d{5}
";

            aqInit = Regex.Replace(aqInit,
                                   pattern,
                                   $"bResample={Convert.ToInt32(appSetting.CanResample)}\r\n" +
                                   $"resample_method={((int)appSetting.ResampleMode).ToString()}\r\n" +
                                   $"resample_fs={((int)appSetting.ResampleFS).ToString()}\r\n");

            File.WriteAllText("AquesTalkPlayer\\AquesTalkPlayer.ini", aqInit, Encoding.GetEncoding("shift_jis"));
        }
    }
}

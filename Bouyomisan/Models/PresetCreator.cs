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
    }
}

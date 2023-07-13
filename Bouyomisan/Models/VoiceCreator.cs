using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bouyomisan.Models
{
    internal class VoiceCreator
    {
        public static readonly string AquesTalkPlayerPath = Path.GetFullPath("AquesTalkPlayer\\AquesTalkPlayer.exe");

        /// <summary>
        /// 非同期で音声・Exoファイルを作成します。
        /// </summary>
        /// <param name="subtitle">字幕用文字列</param>
        /// <param name="speak">読み上げ用文字列</param>
        /// <param name="isWavOut">音声ファイルのみ出力するか否か</param>
        /// <param name="voice">現在の声設定</param>
        /// <param name="output">現在の出力設定</param>
        /// <returns></returns>
        public async Task<string> CreateFileAsync(string subtitle, string speak, bool isWavOut, VoiceSetting voice, OutputSetting output)
        {
            string wavFile = CreateWavFilePath(subtitle, voice, output);
            double wavTime = await CreateWavAsync(speak, voice, wavFile);

            if (isWavOut)
                return wavFile;
            else
                return CreateExo(subtitle, wavFile, wavTime, voice, output);
        }

        private async Task<double> CreateWavAsync(string speak, VoiceSetting voice, string wavFile)
        {
            Process.Start(AquesTalkPlayerPath, $"/T \"{speak.Replace("\r\n", string.Empty)}\" /P \"{voice.Name}\" /W \"{wavFile}\"");

            FileStream? fs = null;
            for (int i = 0; i < 250; i++)
            {
                await Task.Delay(50);
                try
                {
                    fs = new FileStream(wavFile, FileMode.Open);
                    break;
                }
                catch { }
            }

            if (fs == null)
                throw new TimeoutException($"{wavFile} が見つかりませんでした");

            var wavStream = new WaveFileReader(fs);
            double wavTime = wavStream.TotalTime.TotalMilliseconds;

            fs.Dispose();
            wavStream.Dispose();

            return wavTime;
        }

        private string CreateWavFilePath(string subtitle, VoiceSetting voice, OutputSetting output)
        {
            return Path.Combine(Path.GetFullPath(output.AudioOut),
                                $"{DateTime.Now.ToString("yyyyMMddHHmmss")}-{voice.Name}-{subtitle.Replace("\r\n", string.Empty)}.wav");
        }

        private string CreateExo(string subtitle, string wavFile, double wavTime, VoiceSetting voice, OutputSetting output)
        {
            string rowCaptionText = BitConverter.ToString(Encoding.Unicode.GetBytes(subtitle)).Replace("-", string.Empty).ToLower().PadRight(4096, '0');

            int endFrame = (int)(Math.Floor((wavTime + voice.CorrectionMillisecond) / 100d) / 10 * output.AviUtlFps);

            if (endFrame < 1) endFrame = 1;

            string exoText = string.Format(voice.ExoTemplate, rowCaptionText, wavFile, endFrame);

            string exoPath = wavFile + ".exo";

            File.WriteAllText(exoPath, exoText, Encoding.GetEncoding("shift_jis"));

            return exoPath;
        }
    }
}

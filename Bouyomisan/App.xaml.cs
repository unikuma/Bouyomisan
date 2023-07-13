using System;
using System.IO;
using System.Windows;
using Livet;

namespace Bouyomisan
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DispatcherHelper.UIDispatcher = Dispatcher;
            AppDomain.CurrentDomain.UnhandledException += CatchUnhandledException;
        }

        private void CatchUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = (Exception)e.ExceptionObject;

                MessageBox.Show(
                    $"ハンドルされていない例外が発生しました。" + Environment.NewLine + exception.Message,
                    "Bouyomisan エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                File.AppendAllText(
                    "./Bouyomisan.log",
                    DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + exception.Message + Environment.NewLine);
            }
            finally
            {
                Environment.Exit(1);
            }
        }
    }
}

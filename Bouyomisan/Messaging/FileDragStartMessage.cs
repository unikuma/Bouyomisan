using System.Windows;
using Livet.Messaging;

namespace Bouyomisan.Messaging
{
    public class FileDragStartMessage : InteractionMessage
    {
        public FileDragStartMessage(string targetFile) : base()
        {
            TargetFile = targetFile;
        }

        public FileDragStartMessage(string targetFile, string messageKey) : base(messageKey)
        {
            TargetFile = targetFile;
        }

        public string TargetFile { get; init; }

        /// <summary>
        /// Please do not remove this method.
        /// </summary>
        /// <returns>A new instance of this.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new FileDragStartMessage(TargetFile, MessageKey);
        }
    }
}

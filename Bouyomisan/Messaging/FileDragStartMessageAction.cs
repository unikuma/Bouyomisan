using System.Windows;
using Livet.Behaviors.Messaging;
using Livet.Messaging;

namespace Bouyomisan.Messaging
{
    public class FileDragStartMessageAction : InteractionMessageAction<FrameworkElement>
    {
        protected override void InvokeAction(InteractionMessage m)
        {
            if (m is FileDragStartMessage message)
            {
                DragDrop.DoDragDrop(
                    AssociatedObject,
                    new DataObject(DataFormats.FileDrop, new string[] { message.TargetFile }),
                    DragDropEffects.Copy);
            }
        }
    }
}

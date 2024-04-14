using Desktoptale.Messages;
using Desktoptale.Messaging;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class InteractionManager
    {
        private MonitorManager monitorManager;
        private GameWindow mainWindow;
        
        private bool clickthrough;
        
        public InteractionManager(MonitorManager monitorManager, GameWindow mainWindow)
        {
            this.monitorManager = monitorManager;
            this.mainWindow = mainWindow;
            
            MessageBus.Subscribe<StartInteractionMessage>(OnInteractionStateChangedMessage);
            MessageBus.Subscribe<ClickThroughChangedMessage>(OnClickThroughChangedMessage);
        }

        private void OnInteractionStateChangedMessage(StartInteractionMessage message)
        {
            Point previousPosition = WindowsUtils.GetCursorPosition();
                
            if(!clickthrough) WindowsUtils.MakeClickthrough(mainWindow);

            Point hitboxCenter = message.Target.HitBox.Center;
            Point clickPosition = monitorManager
                .ToVirtualScreenCoordinates(new Vector2(hitboxCenter.X, MathHelper.Lerp(hitboxCenter.Y, message.Target.HitBox.Bottom, 0.9f)))
                .ToPoint();
            
            WindowsUtils.SetCursorPosition(clickPosition);
            WindowsUtils.SendLeftMouseButtonDown();
            WindowsUtils.SendLeftMouseButtonUp();
            WindowsUtils.SetCursorPosition(previousPosition);
                
            if(!clickthrough) WindowsUtils.MakeClickable(mainWindow);
                
            MessageBus.Send(new FocusCharacterMessage() { Character = message.Target });
        }

        private void OnClickThroughChangedMessage(ClickThroughChangedMessage message)
        {
            clickthrough = message.Enabled;
        }
    }
}
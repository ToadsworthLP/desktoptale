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
        private bool interactionEnabled;
        
        public InteractionManager(MonitorManager monitorManager, GameWindow mainWindow)
        {
            this.monitorManager = monitorManager;
            this.mainWindow = mainWindow;
            
            MessageBus.Subscribe<StartInteractionMessage>(OnStartInteractionMessage);
            MessageBus.Subscribe<ClickThroughChangedMessage>(OnClickThroughChangedMessage);
            MessageBus.Subscribe<InteractionButtonChangedMessage>(OnInteractionButtonChangedMessage);
        }

        private void OnStartInteractionMessage(StartInteractionMessage message)
        {
            if (!interactionEnabled) return;
            
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
        
        private void OnInteractionButtonChangedMessage(InteractionButtonChangedMessage message)
        {
            interactionEnabled = message.Enabled;
        }
    }
}
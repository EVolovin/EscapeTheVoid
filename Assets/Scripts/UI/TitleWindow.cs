using System;

namespace EscapeTheVoid.UI
{
    public class TitleWindow : UIWindow
    {
        // The Title menu window that is shown at the very beginning.
        
        protected override void Initialize()
        {
            base.Initialize();
            _actionButton.onClick.AddListener(_gameManager.StartGame);
        }
    }
}

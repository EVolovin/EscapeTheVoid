using System;

namespace EscapeTheVoid.UI
{
    public class ActionWindow : UIWindow
    {
        // The Win/Loss menu window.
        
        protected override void Initialize()
        {
            base.Initialize();
            _actionButton.onClick.AddListener(_gameManager.RestartGame);
        }
    }
}

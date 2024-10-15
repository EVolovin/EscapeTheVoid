using System;

namespace EscapeTheVoid.UI
{
    public class PauseWindow : UIWindow
    {
        // The Pause menu window.
        protected override void Initialize()
        {
            base.Initialize();
            _actionButton.onClick.AddListener(_gameManager.ResumeGame);
        }
    }
}

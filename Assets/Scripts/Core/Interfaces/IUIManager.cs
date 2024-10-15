using System;

namespace EscapeTheVoid.Core
{
    public interface IUIManager
    {
        bool WindowShown { get; }
        
        void SetKeysCollected(int collected, int total);

        void CloseTitleWindow();
        
        void ShowPauseWindow();
        void HidePauseWindow();
        void ShowActionWindow(string captionText);
        void HideActionWindow();
    }
}

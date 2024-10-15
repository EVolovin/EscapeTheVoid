using System;
using UnityEngine;
using TMPro;
using EscapeTheVoid.UI;

namespace EscapeTheVoid.Core
{
    public class UIManager : MonoBehaviour, IUIManager
    {
        // This class controls the UI windows behaviour.
        
        const string KEYS_FORMAT = "{0}|{1}";

        [SerializeField] TextMeshProUGUI _keysCollectedText;
        [SerializeField] UIWindow _pauseWindow;
        [SerializeField] UIWindow _actionWindow;
        [SerializeField] UIWindow _titleWindow;
        
        
        public bool WindowShown { get; private set; }
        
        void Awake()
        {
            // The Title window is shown at the start
            WindowShown = true;
        }
        
        public void SetKeysCollected(int collected, int total)
        {
            if (total < 0 || collected < 0 || collected > total)
                throw new ArgumentException(nameof(collected) + ": " + collected);
            
            _keysCollectedText.text = string.Format(KEYS_FORMAT, collected, total);
        }
        
        public void CloseTitleWindow()
        {
            _titleWindow.Hide();
            WindowShown = false;
        }

        public void ShowPauseWindow()
        {
            _pauseWindow.Show();
        }

        public void HidePauseWindow()
        {
            _pauseWindow.Hide();
        }

        public void ShowActionWindow(string captionText)
        {
            _actionWindow.SetCaptionText(captionText);
            _actionWindow.Show();
            WindowShown = true;
        }

        public void HideActionWindow()
        {
            _actionWindow.Hide();
            WindowShown = false;
        }
    }
}

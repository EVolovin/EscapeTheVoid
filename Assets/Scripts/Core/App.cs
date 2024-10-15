using System;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeTheVoid.Core
{
    public class App : MonoBehaviour
    {
        [SerializeField] CanvasGroup _mainCanvasGroup;
        [SerializeField] CanvasScaler _mainCanvasScaler;
        
        
        Vector2Int _currentResolution;
        
        void Awake()
        {
            // should unsubscribe first if Domain reloading is disabled
            Application.lowMemory += () => Resources.UnloadUnusedAssets();
            
            UpdateResolution();
        }
        
        void Update()
        {
            // this is the way to adjust the ui canvas for a given screen resolution at runtime
            if (Screen.width != _currentResolution.x || Screen.height != _currentResolution.y)
            {
                Debug.LogWarning("Changed resolution");
                UpdateResolution();
            }
        }
        
        void UpdateResolution()
        {
            _currentResolution.x = Screen.width;
            _currentResolution.y = Screen.height;
            
            // Adjust the reference resolution to the new screen resolution
            _mainCanvasScaler.referenceResolution = new Vector2(_currentResolution.x, _currentResolution.y);
        }
    }
}

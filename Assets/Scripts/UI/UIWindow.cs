using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using EscapeTheVoid.Core;

namespace EscapeTheVoid.UI
{
    public abstract class UIWindow : MonoBehaviour
    {
        // The base class for the menu windows that provides the common functionality.
        
        static readonly int HIDE_HASH = Animator.StringToHash("Hide");
        static readonly int SHOW_HASH = Animator.StringToHash("Show");
        
        [Inject] protected IGameManager _gameManager;
        
        [SerializeField] Animator _animator;
        [SerializeField] TextMeshProUGUI _captionText;
        [SerializeField] protected Button _actionButton;
        [SerializeField] protected Button _quitButton;
        
        
        void Awake()
        {
            Initialize();
        }
        
        public void SetCaptionText(string captionText)
        {
            _captionText.text = captionText;
        }
        
        public void Show()
        {
            _animator.SetTrigger(SHOW_HASH);
        }

        public void Hide()
        {
            _animator.SetTrigger(HIDE_HASH);
        }

        protected virtual void Initialize()
        {
            _quitButton.onClick.AddListener(_gameManager.QuitGame);
        }
    }
}

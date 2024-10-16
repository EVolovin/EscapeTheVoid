using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using EscapeTheVoid.World;
using EscapeTheVoid.World.Geometry;
using EscapeTheVoid.World.Player;

namespace EscapeTheVoid.Core
{
    public class GameManager : MonoBehaviour, IGameManager
    {
        const string CAPTION_TEXT_WIN  = "You have escaped the Void\nBarely...";
        const string CAPTION_TEXT_LOSS = "The Void\nhas consumed you...";
        
        [Inject] ISoundManager _soundManager;
        [Inject] IUIManager _uiManager;
        [Inject] IThreatManager _threatManager;
        [Inject] IPlayerController _playerController;
        [Inject] ITimer _timer;
        
        [SerializeField] PlayerTrigger _playerLeftStartingChamberTrigger;
        [SerializeField] PlayerTrigger _playerEnteredExitChamberTrigger;

        Chamber _startingChamber;
        Chamber _exitChamber;
        
        // Keys collecting tracking 
        List<Key> _keys;
        int _collectedKeys;
        
        bool _isPaused;
        bool _cutsceneShown;
        
        
        void Start()
        {
            // Find and store the Chambers
            var chambers = GameObject.FindGameObjectsWithTag(Strings.TAG_CHAMBER)
                .Select(x => x.GetComponent<Chamber>())
                .ToArray();
            
            _startingChamber = chambers.First(chamber => chamber.Type == ChamberType.Entrance);
            _exitChamber     = chambers.First(chamber => chamber.Type == ChamberType.Exit);
            
            // Find and store the Keys
            _keys = GameObject.FindGameObjectsWithTag(Strings.TAG_KEY)
                .Select(obj => obj.GetComponent<Key>())
                .ToList();
            
            foreach (var key in _keys)
                key.OnCollected += OnKeyCollected;
            
            _uiManager.SetKeysCollected(_collectedKeys, _keys.Count);
            _exitChamber.Shrink(0f, 0f);
            
            _playerController.OnFalling += OnPlayerFalling;
            _playerController.OnDead    += OnPlayerDead;
            
            _playerLeftStartingChamberTrigger.Triggered += OnPlayerLeftStartingChamber;
            _playerEnteredExitChamberTrigger.Triggered  += OnPlayerEnteredExitChamber;
            
            SetPaused(true);
        }
        
        
        void Update()
        {
            // Pause/Resume the game at the press of the ESC key
            if (!Input.GetKeyDown(KeyCode.Escape))
                return;
            
            if (_uiManager.WindowShown)
                return;
            
            if (!_isPaused)
                PauseGame();
            else
                ResumeGame();
            
            // =================================================================================================
            // NOTE: In Editor, the mouse cursor may not hide upon Resuming the game with pressing the ESC key.
            // The mouse cursor does hide in a build though.
            // This behaviour is considered as a bug by the Unity community which was "introduced" in Unity 2021.
            // This is also told to be a design choice by the Unity devs themselves. ¯\_(ツ)_/¯
            // =================================================================================================
        }
        
        
        void OnKeyCollected(Key key)
        {
            // Event handler for when the Player collects a Key
            
            _collectedKeys++;
            
            _soundManager.PlaySound(_soundManager.Sounds.KeyCollected);
            _uiManager.SetKeysCollected(_collectedKeys, _keys.Count);
            
            // Open the exit Chamber when all Keys have been collected
            if (_collectedKeys == _keys.Count)
                OpenExitChamberDoor();
        }
        
        void OnPlayerFalling()
        {
            // Play some heavy heart beating sounds when the Played is falling into the Void
            _soundManager.PlaySound(_soundManager.Sounds.HeartBeatRacingSound);
            _threatManager.Enabled = false;
        }
        
        void OnPlayerDead()
        {
            // Event handler for when the Player has fallen into the Void
            
            _playerController.SetControlsLocked(true);
            SetCursorState(true);
            
            _threatManager.Enabled = false;
            
            // Show the UI window
            _uiManager.ShowActionWindow(CAPTION_TEXT_LOSS);
            _timer.SetPaused(true);
            
            SetPaused(true);
        }
        
        void OnPlayerLeftStartingChamber()
        {
            // Look back at the disappearing starting Chamber
            StartCoroutine(PlayOnPlayerLeftStartingChamber());
        }
        
        void OnPlayerEnteredExitChamber()
        {
            if (_collectedKeys < _keys.Count)
                return;
            
            _playerController.SetControlsLocked(true);
            SetCursorState(true);
            
            _threatManager.Enabled = false;
            
            _uiManager.ShowActionWindow(CAPTION_TEXT_WIN);
            _timer.SetPaused(true);
        }
        
        void OpenExitChamberDoor()
        {
            _exitChamber.Expand();
            _exitChamber.OpenDoor(2f);
        }
        
        IEnumerator PlayOnPlayerLeftStartingChamber()
        {
            if (!_cutsceneShown)
            {
                _playerController.SetControlsLocked(true);
                
                // Look around
                const float duration = 0.5f;
                _playerController.SetViewDirection(0f, 0f, duration);
                yield return new WaitForSeconds(duration);
            }
            
            // Chamber door closing and disappearing
            _startingChamber.CloseDoor();
            _startingChamber.Shrink(1f);
            
            if (!_cutsceneShown)
            {
                yield return new WaitForSeconds(2f);
                _playerController.SetControlsLocked(false);
                
                _cutsceneShown = true;
            }
            
            _threatManager.Enabled = true;
        }
        
        void SetCursorState(bool visible)
        {
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }
        
        public void StartGame()
        {
            SetCursorState(false);
            _uiManager.CloseTitleWindow();
            
            // The Main Camera is disabled at the game start (so it doesn't render anything behind the Title screen), enable it
            GameObject.FindWithTag("MainCamera").GetComponent<Camera>().enabled = true;
            SetPaused(false);
            
            _timer.StartNewTimer();
            
            _playerController.SetViewDirection(180f, 0f);
            _startingChamber.OpenDoor();
        }
        
        public void RestartGame()
        {
            _uiManager.HideActionWindow();
            SetCursorState(false);
            
            // Reset everything in the scene to the initial state
            
            _startingChamber.Expand(0f, 0f);
            _startingChamber.CloseDoor(0f, 0f);
            _startingChamber.OpenDoor();
            
            _exitChamber.CloseDoor();
            _exitChamber.Shrink(0f, 0f);
            
            foreach (var key in _keys)
                key.ResetKey();
            
            // Reset the Player state and Position
            _playerController.ResetPlayer();
            
            _playerController.SetControlsLocked(false);
            _playerController.SetViewDirection(180f, 0f);
            
            // Resubscribe for the Player events
            _playerController.OnFalling += OnPlayerFalling;
            _playerController.OnDead += OnPlayerDead;
            
            _playerLeftStartingChamberTrigger.Triggered -= OnPlayerLeftStartingChamber;
            _playerEnteredExitChamberTrigger.Triggered  -= OnPlayerEnteredExitChamber;
            _playerLeftStartingChamberTrigger.Triggered += OnPlayerLeftStartingChamber;
            _playerEnteredExitChamberTrigger.Triggered  += OnPlayerEnteredExitChamber;
            
            // Reset the Keys
            _collectedKeys = 0;
            _uiManager.SetKeysCollected(_collectedKeys, _keys.Count);
            
            _timer.StartNewTimer();
            
            if (_isPaused)
                SetPaused(false);
        }
        
        public void PauseGame()
        {
            if (_isPaused)
                return;
            
            // Pause the timer and show the pause menu window
            _timer.SetPaused(true);
            _uiManager.ShowPauseWindow();
            
            _playerController.SetControlsLocked(true);
            SetCursorState(true);
            
            SetPaused(true);
        }
        
        public void ResumeGame()
        {
            if (!_isPaused)
                return;
            
            SetPaused(false);
            _timer.SetPaused(false);
            
            _uiManager.HidePauseWindow();
            _playerController.SetControlsLocked(false);
            SetCursorState(false);
        }
        
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void SetPaused(bool paused)
        {
            Time.timeScale = paused ? 0f : 1f;
            _isPaused = paused;
        }
    }
}

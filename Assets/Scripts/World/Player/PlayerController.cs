using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Zenject;
using EscapeTheVoid.Core;

namespace EscapeTheVoid.World.Player
{
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        // This class controls the inputs of the user
        
        const float GRAVITY = -9.81f;
        const float FALLING_HEIGHT = -10f;
        const float DEATH_HEIGHT   = -40f;
        
        [Inject] ISoundManager _soundManager;
        
        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] CharacterController _controller;
        [SerializeField] Transform _cameraTransform;
        
        [SerializeField] float _playerSpeed = 5f;
        [SerializeField] float _momentumDamping = 5f;
        [SerializeField] float _sensitivity = 1.5f;
        [SerializeField] float _smoothing = 1.5f;

        public event Action OnFalling;
        public event Action OnDead;
        
        bool _controlsLocked;
        bool _isTouchingFloorThisFrame;
        
        bool _isFootStepsSprinting;
        Coroutine _playFootstepsCoroutine;
        SoundHandler _footStepsSoundHandler;
        
        Vector3 _input;
        Vector3 _force;
        Vector3 _move;
        
        Vector2 _smoothedMousePos;
        Vector2 _currentView;
        Vector3 _initialPosition;
        
        public Vector3 Position => transform.position;
        
        
        void Start()
        {
            _initialPosition = transform.position;
            _currentView.x = transform.eulerAngles.y;
        }
        
        void Update()
        {
            if (_controlsLocked)
                return;
            
            ScanInput();
            MovePlayer();
            CheckFalling();
        }
        
        
        void ScanInput()
        {
            // Scan mouse movement
            var mousePos = new Vector2
            {
                x = Input.GetAxisRaw("Mouse X") * _sensitivity * _smoothing,
                y = Input.GetAxisRaw("Mouse Y") * _sensitivity * _smoothing
            };
            
            // Set and mouse movement vector and smooth it out a bit
            _smoothedMousePos.x = Mathf.Lerp(_smoothedMousePos.x, mousePos.x, 1f / _smoothing);
            _smoothedMousePos.y = Mathf.Lerp(_smoothedMousePos.y, mousePos.y, 1f / _smoothing);
            
            // Scan the WASD / Arrow keys input
            var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            input.Normalize();
            
            // If the user makes the input, move the Player accordingly
            // The movement has some momentum, so when the user stops the input, continue moving with said momentum and damp it over time
            
            _input = input != Vector3.zero ? 
                transform.TransformDirection(input) : 
                Vector3.Lerp(_input, Vector3.zero, _momentumDamping * Time.deltaTime);
            
            // Apply the force added by AddForce() method.
            ApplyForce();
            
            _move = _force + Vector3.up * GRAVITY;
            
            // Sprint if the user holds the left shift key.
            bool sprint = Input.GetKey(KeyCode.LeftShift);
            
            // user-input movement is only applied when the Player stands on the ground (i.e. not falling into the Void)
            if (_isTouchingFloorThisFrame)
            {
                _move += _input * (_playerSpeed * (sprint ? 2f : 1f));
                
                // play the footsteps if the Player is moving
                if (input != Vector3.zero)
                    PlayFootsteps(sprint);
                else
                    StopFootsteps();
            }
            else
            {
                StopFootsteps();
            }
        }
        
        void MovePlayer()
        {
            _currentView.x += _smoothedMousePos.x;
            _currentView.y += _smoothedMousePos.y;
            _currentView.y = Mathf.Clamp(_currentView.y, -90f, 90f);
            
            // rotate the Player and the camera according to the mouse input
            transform.localRotation        = Quaternion.AngleAxis(_currentView.x, transform.up);
            _cameraTransform.localRotation = Quaternion.AngleAxis(_currentView.y, Vector3.left);
            
            _isTouchingFloorThisFrame = false;
            // Move the Player with the movement vector
            _controller.Move(_move * Time.deltaTime);
        }

        void CheckFalling()
        {
            float height = transform.position.y;
            // if the height position is below certain number, invoke the corresponding event
            switch (height)
            {
                case < DEATH_HEIGHT:
                    OnDead?.Invoke();
                    OnDead = null;
                    break;
                case < FALLING_HEIGHT:
                    OnFalling?.Invoke();
                    OnFalling = null;
                    break;
            }
        }
        
        void PlayFootsteps(bool sprint)
        {
            if (_playFootstepsCoroutine != null)
            {
                if (_isFootStepsSprinting == sprint)
                    return;
                
                StopFootsteps();
            }
            
            _isFootStepsSprinting = sprint;
            _playFootstepsCoroutine = StartCoroutine(PlayFootstepsSound(sprint));
        }

        void StopFootsteps()
        {
            // stop the footsteps sound when the Player stops moving
            
            if (_playFootstepsCoroutine == null)
                return;
            
            StopCoroutine(_playFootstepsCoroutine);
            _playFootstepsCoroutine = null;
            
            _footStepsSoundHandler.Stop();
            _footStepsSoundHandler = null;
        }

        void ApplyForce()
        {
            if (_force == Vector3.zero)
                return;
            
            // weaken the force (added with the AddForce() method) over time
            _force = Vector3.Lerp(_force, Vector3.zero, _momentumDamping * 0.1f * Time.deltaTime);
            
            if (_force.sqrMagnitude < 0.09f)
                _force = Vector3.zero;
        }
        
        IEnumerator PlayFootstepsSound(bool sprint)
        {
            var clip = sprint ? _soundManager.Sounds.FootStepsSprintSound : _soundManager.Sounds.FootStepsSound;
            var wait = new WaitForSeconds(clip.length);
            
            while (true)
            {
                _footStepsSoundHandler = _soundManager.PlaySound(clip);
                yield return wait;
            }
        }
        
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _isTouchingFloorThisFrame = true;
        }
        
        public void AddForce(Vector3 direction)
        {
            // add the impulse-type force to the Player
            // i.e. when a meteor hits the Player
            _force += direction;
        }
        
        public void SetControlsLocked(bool locked)
        {
            // locks the user input
            _controlsLocked = locked;
            
            if (locked)
                StopFootsteps();
        }

        public void SetViewDirection(float horizontalAngle, float verticalAngle, float duration = 0f)
        {
            // sets the Player's view direction instantly or over specified time 
            
            if (duration == 0f)
            {
                transform.localRotation        = Quaternion.AngleAxis(horizontalAngle, transform.up);
                _cameraTransform.localRotation = Quaternion.AngleAxis(verticalAngle,   Vector3.left);
            }
            else
            {
                // rotate the Player over time using DOTween
                transform.       DOLocalRotateQuaternion(Quaternion.AngleAxis(horizontalAngle, transform.up), duration);
                _cameraTransform.DOLocalRotateQuaternion(Quaternion.AngleAxis(verticalAngle,   Vector3.left), duration);
            }
            
            _currentView.x = horizontalAngle;
            _currentView.y = verticalAngle;
        }

        public void ResetPlayer()
        {
            // Reset the Player's position
            // the CharacterController won't allow the transform.position change if it's enabled
            _controller.enabled = false;
            transform.position = _initialPosition;
            _controller.enabled = true;
            
            // remove the applied force
            _force = Vector3.zero;
        }
    }
}

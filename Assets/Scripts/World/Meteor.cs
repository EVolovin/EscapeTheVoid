using System;
using UnityEngine;
using DG.Tweening;
using EscapeTheVoid.Core;
using EscapeTheVoid.Utils;
using EscapeTheVoid.World.Player;
using Zenject;

namespace EscapeTheVoid.World
{
    public class Meteor : MonoBehaviour
    {
        // This is a meteor that is thrown at the Player to knock them off straight into the Void
        
        [Range(1f, 10f)]  [SerializeField] float _effectRadius = 3f;
        [Range(1f, 100f)] [SerializeField] float _forcePower = 10f;

        bool _collisionDone;
        IPlayerController _playerController;
        
        public event Action Collided;
        
        
        void Awake()
        {
            _playerController = GameObject.FindWithTag(Strings.TAG_PLAYER).GetComponent<IPlayerController>();
        }

        public void LaunchAtPlayer()
        {
            _collisionDone = false;
            
            // Launch the meteor with the parabolic trajectory using DOTween
            var path = MathUtils.CreateParabolicCurve(transform.position, _playerController.Position + Vector3.down);
            var tween = transform.DOPath(path, 4f);
            
            tween.onComplete += Collide;
        }
        
        void OnCollisionEnter(Collision other)
        {
            Collide();
        }

        void Collide()
        {
            if (_collisionDone)
                return;
            
            // the squared distance is cheaper to calculate than the regular distance since it doesn't use the sqrt function
            // in this particular case, it really would NOT affect performance in any way at all
            // but is used here to demonstrate my knowledge of it
            float sqrDistance = (_playerController.Position - transform.position).sqrMagnitude;
            float sqrRadius = _effectRadius * _effectRadius;
            
            if (sqrDistance <= sqrRadius)
            {
                // Apply the knocking-off force to the Player depending on the distance to them
                // i.e. the closer the Player is to the impact point, the stronger the force is
                
                var randomDirection = UnityEngine.Random.onUnitSphere;
                randomDirection.y = Mathf.Abs(randomDirection.y);
                randomDirection.y = Mathf.Clamp(randomDirection.y, 0f, 0.2f);
                
                _playerController.AddForce(randomDirection * _forcePower * (sqrRadius / sqrDistance));
            }
            
            _collisionDone = true;
            
            Collided?.Invoke();
        }
    }
}

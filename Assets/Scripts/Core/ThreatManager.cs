using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using EscapeTheVoid.World;
using EscapeTheVoid.World.Player;

namespace EscapeTheVoid.Core
{
    public class ThreatManager : MonoBehaviour, IThreatManager
    {
        // This class is responsible for launching the meteors at the Player
        
        [Inject] IPlayerController _playerController;
        [Inject] ISoundManager _soundManager;
        
        [Range(1f, float.PositiveInfinity)]
        [SerializeField] float _launchFrequency = 5f;
        
        [SerializeField] Transform _originPoint;
        [SerializeField] Transform _meteorsRoot;
        [SerializeField] Meteor _meteorPrefab;
        
        // Pool the meteors instead of destroying and instantiating them over and over again
        readonly Stack<Meteor> _meteorsPool = new Stack<Meteor>();
        
        bool _enabled;
        Coroutine _launchMeteorsCoroutine;
        
        
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;
                
                _enabled = value;
                
                if (_enabled)
                {
                    _launchMeteorsCoroutine = StartCoroutine(LaunchMeteors());
                }
                else
                {
                    StopCoroutine(_launchMeteorsCoroutine);
                    _launchMeteorsCoroutine = null;
                }
            }
        }
        
        IEnumerator LaunchMeteors()
        {
            while (true)
            {
                // Get the meteor and launch it at the Player with style
                var meteor = SpawnOrGetFromPool();
                meteor.LaunchAtPlayer();
            
                _soundManager.PlaySound(_soundManager.Sounds.MeteorFallingSound);
                
                // Wait for cooldown
                yield return new WaitForSeconds(_launchFrequency);
            }
        }
        
        Meteor SpawnOrGetFromPool()
        {
            // Get the random position in the "sky"
            var position = UnityEngine.Random.onUnitSphere * 40f;
            position.y = Mathf.Abs(position.y);
            position += _originPoint.position;
            
            // Get a meteor from pool or spawn a new one if the pool is empty
            if (_meteorsPool.TryPop(out var meteor))
            {
                meteor.transform.position = position;
                meteor.gameObject.SetActive(true);
            }
            else
            {
                meteor = Instantiate(_meteorPrefab, position, Quaternion.identity, _meteorsRoot);
                meteor.Collided += () =>
                {
                    // Upon impact, play the sound and pool the meteor.
                    _soundManager.PlaySound(_soundManager.Sounds.MeteorImpactSound);
                    Pool(meteor);
                };
            }
            
            return meteor;
        }
        
        void Pool(Meteor meteor)
        {
            meteor.gameObject.SetActive(false);
            _meteorsPool.Push(meteor);
        }
    }
}

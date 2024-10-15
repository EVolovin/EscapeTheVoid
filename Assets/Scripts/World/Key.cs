using System;
using UnityEngine;
using DG.Tweening;
using EscapeTheVoid.Utils;

namespace EscapeTheVoid.World
{
    public class Key : MonoBehaviour
    {
        // This class represents the Keys that the Player has to collect
        
        [SerializeField] bool _relocation; // a key can fly away into opposite part of the labyrinth when the Player approaches it
        [SerializeField] Vector3 _relocationPosition; // the position to which the key flies to
        
        bool _initialRelocation;
        Vector3 _initialPosition;
        
        public event Action<Key> OnCollected;
        
        
        void Awake()
        {
            _initialRelocation = _relocation;
            _initialPosition = transform.position;
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;
            
            OnApproached();
        }
        
        void OnApproached()
        {
            // When the Player approaches the key, make the key fly away or just get collected
            // depending on the "_relocation" variable set in the Editor.
            
            if (_relocation)
                Relocate();
            else
                GetCollected();
        }

        void Relocate()
        {
            var col = GetComponent<CapsuleCollider>();
            col.enabled = false;
            
            // using DOTween to move the key with the ballistic trajectory
            var path = MathUtils.CreateBallisticCurve(transform.position, _relocationPosition, 1.5f);
            var tween  = transform.DOPath(path, 3f);
            
            tween.onComplete += () =>
            {
                _relocation = false;
                col.enabled = true;
            };
        }
        
        void GetCollected()
        {
            OnCollected?.Invoke(this);
            
            gameObject.SetActive(false);
        }
        
        public void ResetKey()
        {
            // Reset the values when the game is restarted
            _relocation = _initialRelocation;
            transform.position = _initialPosition;
            
            gameObject.SetActive(true);
        }
    }
}

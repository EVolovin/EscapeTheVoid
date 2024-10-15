using System;
using UnityEngine;

namespace EscapeTheVoid.World.Geometry
{
    public class Block : MonoBehaviour
    {
        // This class represents a Block that pops in and out of existence based on the Player's proximity
        
        [SerializeField] MeshRenderer _renderer;
        
        void Awake()
        {
            _renderer.enabled = false;
        }
        
        void OnTriggerEnter(Collider other)
        {
            // The Player has a trigger Sphere collider that interacts with Blocks
            
            if (_renderer.enabled || !other.gameObject.CompareTag("Player"))
                return;
            
            // Enable the rendering of the Block
            _renderer.enabled = true;
        }
        
        void OnTriggerExit(Collider other)
        {
            // The Player has a trigger Sphere collider that interacts with Blocks
            
            if (!_renderer.enabled || !other.gameObject.CompareTag("Player"))
                return;
            
            // Disable the rendering of the Block
            _renderer.enabled = false;
        }
    }
}

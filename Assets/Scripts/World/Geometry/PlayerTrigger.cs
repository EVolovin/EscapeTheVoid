using System;
using UnityEngine;

namespace EscapeTheVoid.World.Geometry
{
    public class PlayerTrigger : MonoBehaviour
    {
        // This class triggers the event when the collider is triggered by the Player.
        
        public event Action Triggered;
        
        
        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;
            
            Triggered?.Invoke();
            Triggered = null;
        }
    }
}

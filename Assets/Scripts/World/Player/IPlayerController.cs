using System;
using UnityEngine;

namespace EscapeTheVoid.World.Player
{
    public interface IPlayerController
    {
        event Action OnFalling;
        event Action OnDead;
        
        Vector3 Position { get; }
        
        void AddForce(Vector3 direction);
        void SetControlsLocked(bool locked);
        void SetViewDirection(float horizontalAngle, float verticalAngle, float duration = 0f);
        void ResetPlayer();
    }
}

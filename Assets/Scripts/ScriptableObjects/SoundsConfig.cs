using System;
using UnityEngine;

namespace EscapeTheVoid.Configs
{
    [CreateAssetMenu(fileName = "Sounds Config", menuName = "Configs/SoundsConfig", order = 2)]
    public class SoundsConfig : ScriptableObject
    {
        // The config asset that contains the sound clips.
        
        [SerializeField] AudioClip _doorOpenClip;
        [SerializeField] AudioClip _doorCloseClip;
        [SerializeField] AudioClip _footStepsClip;
        [SerializeField] AudioClip _footStepsSprintClip;
        [SerializeField] AudioClip _keyCollectedClip;
        [SerializeField] AudioClip _heartBeatRacingClip;
        [SerializeField] AudioClip _meteorFallingClip;
        [SerializeField] AudioClip _meteorImpactClip;
        
        
        public AudioClip DoorOpen => _doorOpenClip;
        public AudioClip DoorClose => _doorOpenClip;
        public AudioClip FootStepsSound => _footStepsClip;
        public AudioClip FootStepsSprintSound => _footStepsSprintClip;
        public AudioClip KeyCollected => _keyCollectedClip;
        public AudioClip HeartBeatRacingSound => _heartBeatRacingClip;
        public AudioClip MeteorFallingSound => _meteorFallingClip;
        public AudioClip MeteorImpactSound => _meteorImpactClip;
    }
}

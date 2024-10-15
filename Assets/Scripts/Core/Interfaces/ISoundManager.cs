using System;
using UnityEngine;
using EscapeTheVoid.Configs;

namespace EscapeTheVoid.Core
{
    public interface ISoundManager
    {
        SoundHandler PlaySound(AudioClip clip);
        SoundHandler PlaySound(AudioClip clip, Vector3 position);
        
        public SoundsConfig Sounds { get; }
    }
}

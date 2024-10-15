using System;
using UnityEngine;

namespace EscapeTheVoid.Core
{
    /// <summary>
    /// A wrapper of a sound being played which provides some control over individual clip playback
    /// </summary>
    public class SoundHandler
    {
        // Everything in this class should be self-explanatory
        
        public SoundHandler(AudioSource audioSource)
        {
            _audioSource = audioSource;
        }
        
        readonly AudioSource _audioSource;
        
        public float Volume
        {
            get => _audioSource.volume;
            set => _audioSource.volume = value;
        }

        public void Play()
        {
            if (_audioSource != null)
                _audioSource.Play();
        }

        public void Stop()
        {
            if (_audioSource != null)
                _audioSource.Stop();
        }

        public void Pause()
        {
            if (_audioSource != null) 
                _audioSource.Pause();
        }

        public void UnPause()
        {
            if (_audioSource != null)
                _audioSource.UnPause();
        }
    }
}

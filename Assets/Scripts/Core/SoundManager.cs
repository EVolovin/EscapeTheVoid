using System;
using DG.Tweening;
using UnityEngine;
using EscapeTheVoid.Configs;

namespace EscapeTheVoid.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        [SerializeField] SoundsConfig _soundsConfig;
        [SerializeField] AudioSource _audioSource;
        
        public SoundsConfig Sounds => _soundsConfig;
        
        /// <summary>
        /// Plays a clip and returns a handler for its AudioSource. 
        /// </summary>
        public SoundHandler PlaySound(AudioClip clip)
        {
            if (clip == null)
                return null;
            
            return PlaySoundInternal(clip);
        }
        
        /// <summary>
        /// Plays a clip at given world position and returns a handler for its AudioSource. 
        /// </summary>
        public SoundHandler PlaySound(AudioClip clip, Vector3 position)
        {
            if (clip == null)
                return null;
            
            return PlaySoundInternal(clip, position);
        }
        

        SoundHandler PlaySoundInternal(AudioClip clip)
        {
            // For each sound playing we need its own AudioSource in order to have the flexibility of playback control
            // (i.e. we can play/pause/stop individual sounds instead of ALL the clips that may be being played by a single AudioSource simultaneously)
            // "MainAudioSource" GameObject is configured to play a sound regardless of world position, so we instantiate a copy of it and play the clip
            var audioSource = Instantiate(_audioSource, transform);
            audioSource.PlayOneShot(clip);
            
            // Destroy the AudioSource gameObject when the playback is finished (copied from Unity Source code - AudioSource.cs)
            Destroy(audioSource.gameObject, clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
            
            return new SoundHandler(audioSource);
        }

        SoundHandler PlaySoundInternal(AudioClip clip, Vector3 position)
        {
            // Create a gameObject with an AudioSource attached to it
            // This AudioSource plays sounds at a world position
            
            var gObject = new GameObject("Positional Audio Source");
            gObject.transform.SetParent(transform);
            gObject.transform.position = position;
            
            var audioSource = gObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.volume = 1f;
            audioSource.Play();
            
            // Destroy the AudioSource gameObject when the playback is finished (copied from Unity Source code - AudioSource.cs)
            Destroy(gObject, clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
            
            return new SoundHandler(audioSource);
        }
    }
}

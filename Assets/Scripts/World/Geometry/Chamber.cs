using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Zenject;
using EscapeTheVoid.Core;

namespace EscapeTheVoid.World.Geometry
{
    public enum ChamberType
    {
        Entrance,
        Exit
    }
    
    public class Chamber : MonoBehaviour
    {
        // This class represents the Starting and Exit Chambers
        
        [Inject] ISoundManager _soundManager;
        
        [SerializeField] ChamberType _type;
        [SerializeField] GameObject _door;


        public ChamberType Type => _type;
        
        
        public void OpenDoor(float delay = 0f, float duration = 1.5f)
        {
            StartCoroutine(PlayOpenDoor(delay, duration));
        }
        
        public void CloseDoor(float delay = 0f, float duration = 1.5f)
        {
            if (!gameObject.activeInHierarchy)
                return;
            
            StartCoroutine(PlayCloseDoor(delay, duration));
        }
        
        public void Shrink(float delay = 0f, float duration = 1f)
        {
            // Pops the Chamber out of existence using the Quantum entanglement principles (scaling the transform to 0)
            
            if (!gameObject.activeInHierarchy)
                return;
            
            StartCoroutine(PlayShrink(delay, duration));
        }
        
        public void Expand(float delay = 0f, float duration = 1f)
        {
            // Pops the Chamber back into existence using the Quantum entanglement principles (scaling the transform to 1)
            
            gameObject.SetActive(true);
            StartCoroutine(PlayExpand(delay, duration));
        }
        
        IEnumerator PlayOpenDoor(float delay = 0f, float duration = 1f)
        {
            if (delay != 0f)
                yield return new WaitForSeconds(delay);
            
            // Rotate the door and play the sound
            
            if (duration != 0f)
                _soundManager.PlaySound(_soundManager.Sounds.DoorOpen, transform.position);
            
            _door.transform.DORotateQuaternion(Quaternion.Euler(-90f, 0f, 90f), duration);
        }
        
        IEnumerator PlayCloseDoor(float delay = 0f, float duration = 1f)
        {
            if (delay != 0f)
                yield return new WaitForSeconds(delay);
            
            // Rotate the door and play the sound
            
            if (duration != 0f)
                _soundManager.PlaySound(_soundManager.Sounds.DoorClose, transform.position);
            
            _door.transform.DORotateQuaternion(Quaternion.Euler(-90f, 0f, 0f), duration);
        }
        
        
        IEnumerator PlayShrink(float delay = 0f, float duration = 1f)
        {
            if (delay != 0f)
                yield return new WaitForSeconds(delay);
            
            if (duration == 0f)
                transform.localScale = Vector3.zero;
            else
                transform.DOScale(Vector3.zero, duration);
            
            yield return new WaitForSeconds(duration);
            gameObject.SetActive(false);
        }
        
        IEnumerator PlayExpand(float delay = 0f, float duration = 1f)
        {
            if (delay != 0f)
                yield return new WaitForSeconds(delay);
            
            if (duration == 0f)
                transform.localScale = Vector3.one;
            else
                transform.DOScale(Vector3.one, duration);
        }
    }
}

using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace EscapeTheVoid.Core
{
    public class Timer : MonoBehaviour, ITimer
    {
        // This class represents a timer using the System.Diagnostics.Stopwatch class
        
        const string TIMER_FORMAT = @"mm\:ss";
        
        [SerializeField] bool _showMilliseconds;
        [SerializeField] TextMeshProUGUI _timerText;
        
        Stopwatch _timer;
        
        
        void Update()
        {
            if (_timer == null)
                return;
            
            _timerText.text = _timer.Elapsed.ToString(TIMER_FORMAT);
        }
        
        public void StartNewTimer()
        {
            _timer = new Stopwatch();
            _timer.Start();
        }
        
        public void SetPaused(bool pause)
        {
            if (pause)
                _timer.Stop();
            else
                _timer.Start();
        }
    }
}

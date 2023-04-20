using System;
using UnityEngine;

namespace CS.Scripts.Utils
{
    public class Timer
    {
        public Timer(float max)
        {
            _max = max;
        }

        public event Action OnTimerOver = () => { };

        private readonly float _max;

        private float _currentValue;

        public void Update(float delta)
        {
            _currentValue += delta;

            if (_currentValue >= _max)
            {
                OnTimerOver.Invoke();
            }
        }

        public void Reset()
        {
            _currentValue = 0;
        }
    }
}
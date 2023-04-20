using System;
using System.Collections.Generic;
using System.Linq;
using Birdhouse.Common.Singleton.Mono;
using CS.Scripts.Utils;
using UnityEngine;

namespace Utils
{
    public class SoundService : MonoSingleton<SoundService>
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private List<Clip> clips;

        public void Shoot(ESound sound)
        {
            var clip = clips
                .FirstOrDefault(value => value.Key == sound)
                .AudioClip;
            
            source.PlayOneShot(clip);
        }

        [Serializable]
        private struct Clip
        {
            [field: SerializeField]
            public ESound Key
            {
                get;
                private set;
            }

            [field: SerializeField]
            public AudioClip AudioClip
            {
                get;
                private set;
            }
        }
    }
}

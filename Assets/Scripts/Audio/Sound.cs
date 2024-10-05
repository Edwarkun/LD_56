using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [System.Serializable]
    public class Sound
    {
        [HideInInspector]
        public float modulationRange = 0.1f;

        public string name;
        public AudioClip clip;
        [Range(-1f, 1f)]
        public float volume = 1.0f;
        [Range(-1f, 1f)]
        public float pitch = 1.0f;
        public bool loop = false;

        [HideInInspector]
        public AudioSource source;
    }

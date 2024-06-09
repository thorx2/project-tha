using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ProjTha
{
    /// <summary>
    /// Simple audio manager class which would control SFX playback from various sources
    /// </summary>
    public class AudioManager : MonoBehaviourSingleton<AudioManager>
    {
        [SerializeField]
        private List<AudioSource> sfxSources;

        [SerializeField]
        private AudioSource bgmSource;

        [SerializeField]
        private AudioMixer gameAudioMixer;

        public void PlaySFX(AudioClip clip)
        {
            foreach (var src in sfxSources)
            {
                if (!src.isPlaying)
                {
                    src.clip = clip;
                    src.Play();
                    break;
                }
            }
        }
    }
}
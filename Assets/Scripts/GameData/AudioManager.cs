using System;
using UnityEngine;

namespace GameData
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [SerializeField] private AudioSource audio_player;

        private void Awake()
        {
            Instance = this;
        }

        public void PlaySFX(AudioClip in_sfx)
        {
            audio_player.PlayOneShot(in_sfx);
        }




    }
}

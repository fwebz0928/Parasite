using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameData;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public enum EPowerUpType
    {
        Health,
        Shotgun,
        Bot
    }

    [System.Serializable]
    public struct PowerUpInfo
    {
        public EPowerUpType powerup_type;
        public float spawn_chance;
        public Sprite power_up_icon;
        public AudioClip pickup_sfx;
    }

    public class PowerUp : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer icon;
       
        
        private PowerUpInfo current_power_up;

        private void Awake()
        {
            this.transform.localScale = Vector3.zero;
        }

        private void Update()
        {
            this.transform.Translate(Vector3.down * (1.5f * Time.deltaTime));
        }

        public void SpawnPowerUp(Vector3 spawn_pos, PowerUpInfo in_infp)
        {
            //Set this powerup to the chosen info
            current_power_up = in_infp;
            icon.sprite = current_power_up.power_up_icon;

            this.transform.position = spawn_pos;
            this.transform.DOScale(Vector3.one, .15f);

        }



        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") == false) return;

            if (!other.gameObject.TryGetComponent<PlayerController>(out var player)) return;

            switch (current_power_up.powerup_type)
            {
                case EPowerUpType.Health:
                    player.IncreaseHealth(1);
                    break;
                case EPowerUpType.Shotgun:
                    break;
                case EPowerUpType.Bot:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Play effects
            AudioManager.Instance.PlaySFX(current_power_up.pickup_sfx);

            //Release this back to the pool 
            GameManager.instance.PowerUpPool.Release(this);
        }




    }
}

using System;
using GameData;
using UnityEngine;

namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private float move_speed;
        [SerializeField] private float auto_destroy_time = 2;
        [SerializeField] private TrailRenderer trail;

        private bool is_releasing = false;
        private PlayerController player_ref;

        public void InitProjectile(Vector3 target_position, PlayerController in_player_ref)
        {
            player_ref = in_player_ref;
            rb2d.velocity = target_position * move_speed;
            is_releasing = false;

            //Toggle the trial to be active
            trail.emitting = true;
            trail.gameObject.SetActive(true);

            //Start the timer for auto destroying projectiles that go off screen
            Invoke(nameof(ResetProjectile), auto_destroy_time);
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.OnTakeDamage(1);
                //is_releasing = true;
                //ResetProjectile();
            }
        }

        protected virtual void OnHitDamageable() { }

        private void ResetProjectile()
        {
            if (is_releasing == false)
            {
                CancelInvoke();
                player_ref.ProjectilePool.Release(this);
                trail.emitting = false;
                trail.Clear();
                trail.gameObject.SetActive(false);
            }


        }




    }
}

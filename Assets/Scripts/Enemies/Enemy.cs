using System;
using System.Collections;
using DG.Tweening;
using GameData;
using Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyFireType
{
    Down,
    Direct,
    RadialBurst,
    Radial,
}

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int health = 1;
    [SerializeField] private int score = 1;
    [SerializeField] private EnemyFireType fire_type;
    [SerializeField] private float min_attack_rate;
    [SerializeField] private float max_attack_rate;
    [SerializeField] private float fire_loops = 1;
    [SerializeField] private float bullet_speed = 5;
    [SerializeField] private Projectile projectile_prefab;
    [SerializeField] private Transform spawn_point;

    public float BulletSpeed => bullet_speed;


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Fire());
    }



    private IEnumerator Fire()
    {
        while (true)
        {
            for (var i = 0; i < fire_loops; i++)
            {

                yield return this.transform.DOScale(new Vector3(1.2f, 1.2f, 1.0f), .15f).SetLoops(2, LoopType.Yoyo);
                switch (fire_type)
                {
                    case EnemyFireType.Down:
                        var spawned_bullet = Instantiate(projectile_prefab, spawn_point.position, Quaternion.identity);
                        var target = Vector3.down * 4.0f;
                        spawned_bullet.InitEnemyProjectile(target, this);
                        break;

                    case EnemyFireType.Direct: //Fires towards the players position
                        break;

                    case EnemyFireType.RadialBurst:
                        var burst_amount = 8; // Number of projectiles in the burst
                        var burst_angle_step = 360f / burst_amount;
                        for (var j = 0; j < burst_amount; j++)
                        {
                            var angle = j * burst_angle_step;
                            var (bullet, direction_burst) = SpawnRadialProjectile(angle);
                            bullet.InitEnemyProjectile(direction_burst.normalized, this);
                        } // fires in a burst around the enemy
                        break;

                    case EnemyFireType.Radial: // fires in a radial spiral around the enemy
                        // Fires projectiles steadily in a radial pattern
                        var radial_amount = 30; // Number of projectiles for the radial pattern
                        var radial_angle_step = 360f / radial_amount;
                        for (int j = 0; j < radial_amount; j++)
                        {
                            var angle = j * radial_angle_step;
                            var (radial_bullet, direction_radial) = SpawnRadialProjectile(angle);
                            radial_bullet.InitEnemyProjectile(direction_radial.normalized, this);
                            yield return new WaitForSeconds(0.15f);
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            yield return new WaitForSeconds(Random.Range(min_attack_rate, max_attack_rate));
        }
        yield return null;

        (Projectile spawned_projectile, Vector3 direction) SpawnRadialProjectile(float angle)
        {
            var directionRadial = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            var radialBullet = Instantiate(projectile_prefab, this.transform.position, Quaternion.identity);
            return (radialBullet, directionRadial);
        }



    }





    public void OnTakeDamage(int damage_amount)
    {
        health -= damage_amount;
        if (health <= 0)
        {
            GameManager.instance.UpdateScore(score, this.transform.position);
            Destroy(this.gameObject);
        }
    }
}

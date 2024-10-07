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

public enum EnemyMovementType
{
    Down,
    ZigZag,
    Circle,
    SinWave
}

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int health = 1;
    [SerializeField] private int score = 1;
    [SerializeField] private AudioClip hit_sfx;
    [SerializeField] private AudioClip death_sfx;
    [Header("Movement")]
    [SerializeField] private EnemyMovementType movement_type;
    [SerializeField] private float move_speed;

    [Header("Sin")]
    [SerializeField] private float sine_freq = 5;
    [SerializeField] private float sine_amplitude = 5;

    [Header("Zig")]
    [SerializeField] private float trace_distance = 10.0f;
    [SerializeField] private LayerMask trace_layer;

    [Header("Firing")]
    [SerializeField] private EnemyFireType fire_type;
    [SerializeField] private float min_attack_rate;
    [SerializeField] private float max_attack_rate;
    [SerializeField] private float fire_loops = 1;
    [SerializeField] private float bullet_speed = 5;
    [SerializeField] private Projectile projectile_prefab;
    [SerializeField] private Transform spawn_point;

    private bool is_moving_right = true;
    private Vector2 zig_dir = Vector2.right;


    public float BulletSpeed => bullet_speed;


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Fire());

        if (movement_type == EnemyMovementType.ZigZag)
        {
            is_moving_right = Random.Range(0, 2) == 0;
            zig_dir = is_moving_right ? Vector2.right : Vector2.left;
        }

    }

    private void Update() { }

    private void FixedUpdate()
    {
        Movement();
        if (movement_type == EnemyMovementType.ZigZag)
        {
            //Check if hitting wall
            zig_dir.Normalize();
            var ray = Physics2D.Raycast(transform.position, zig_dir, trace_distance, trace_layer);
            Debug.DrawLine(transform.position, transform.position + (Vector3)zig_dir * trace_distance, Color.blue, 1);
            if (ray.collider && ray.collider.CompareTag("Wall"))
            {
                is_moving_right = !is_moving_right;
                zig_dir = is_moving_right ? Vector2.right : Vector2.left;
            }
        }
    }


    private void Movement()
    {
        switch (movement_type)
        {

            case EnemyMovementType.Down:
                transform.Translate(Vector3.down * (move_speed * Time.deltaTime));
                break;
            case EnemyMovementType.ZigZag:

                var move_dir = new Vector3(zig_dir.x * move_speed, -move_speed, 0.0f);
                transform.Translate(move_dir * Time.deltaTime);

                break;
            case EnemyMovementType.Circle:
                break;
            case EnemyMovementType.SinWave:
                var sin_movement = Mathf.Sin(Time.deltaTime * sine_freq) * sine_amplitude;
                transform.Translate(new Vector3(sin_movement, -move_speed, 0) * Time.deltaTime);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
                        var burst_amount = 5; // Number of projectiles in the burst
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
                        var radial_amount = 10; // Number of projectiles for the radial pattern
                        var radial_angle_step = 360f / radial_amount;
                        for (var j = 0; j < radial_amount; j++)
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
            GameManager.instance.CreatePowerUp(this.transform.position);
            GameManager.instance.UpdateScore(score, this.transform.position);
            AudioManager.Instance.PlaySFX(death_sfx);
            Destroy(this.gameObject);
        }
        AudioManager.Instance.PlaySFX(hit_sfx);
    }
}

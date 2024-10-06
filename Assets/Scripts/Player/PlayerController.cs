using System;
using GameData;
using Projectiles;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private float move_speed = 10.0f;
    [SerializeField] private float smoothing = .1f;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private int base_health = 3;
    [SerializeField] private int lives = 1;
    [SerializeField] private Projectile projectile_prefab;
    [SerializeField] private Transform fire_spawn_point;


    private Vector2 move_dir;
    private Vector2 target_position;
    private int current_health;
    private ObjectPool<Projectile> projectile_pool;

    public ObjectPool<Projectile> ProjectilePool => projectile_pool;
    public Action<int> OnHealthUpdated;
    public Action<int, int> OnMaxHealthUpdated;
    public Action<int> OnLivesUpdated;
    public int MaxHealth => base_health;
    public int CurrentHealth => current_health;
    public int Lives => lives;

    private void Awake()
    {
        projectile_pool = new ObjectPool<Projectile>
        (
            OnCreateNewProjectile,
            OnGetProjectile,
            OnProjectileRelease,
            OnProjectileDestroyed,
            true,
            100,
            1000
        );

        current_health = MaxHealth;
    }


    private void Update()
    {
        // Get the raw input axes for snappy input
        var moveX = Input.GetAxisRaw("Horizontal");
        var moveY = Input.GetAxisRaw("Vertical");

        // Set the movement direction based on input
        move_dir = new Vector2(moveX, moveY).normalized;

        Fire();
    }

    private void FixedUpdate()
    {
        var targetVelocity = move_dir * move_speed;
        rb2d.velocity = targetVelocity;
    }

    private void Fire()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            print("Firing...");
            var bullet = projectile_pool.Get();
            if (bullet)
            {
                print("Bullet Valid...");
                bullet.transform.position = fire_spawn_point.position;
                var target = Vector3.up * 4.0f;
                bullet.InitProjectile(target, this);
            }
        }
    }


    public void OnTakeDamage(int damage_amount)
    {
        current_health = Mathf.Clamp(current_health - 1, 0, base_health);
        OnHealthUpdated?.Invoke(current_health);
        if (current_health <= 0)
        {
            //Check how many lives player has and reset if possible
            if (lives > 0)
            {
                lives -= 1;
                current_health = MaxHealth;
                OnHealthUpdated?.Invoke(current_health);
                OnLivesUpdated?.Invoke(lives);
            }
            else // No Lives available so display the game over menu
            { }
        }
    }


    private Projectile OnCreateNewProjectile()
    {
        var new_projectile = Instantiate(projectile_prefab);
        new_projectile.gameObject.SetActive(false);
        return new_projectile;
    }
    private void OnGetProjectile(Projectile obj)
    {
        obj.gameObject.SetActive(true);
    }
    private void OnProjectileDestroyed(Projectile obj) { }
    private void OnProjectileRelease(Projectile obj)
    {
        obj.gameObject.SetActive(false);
        obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        obj.transform.position = new Vector3(1000.0f, 10000.0f, 0.0f);
    }



}

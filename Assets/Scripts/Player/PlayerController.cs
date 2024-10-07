using System;
using GameData;
using Projectiles;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour, IDamageable
{
    public static PlayerController Instance;


    [SerializeField] private float move_speed = 10.0f;
    [SerializeField] private float smoothing = .1f;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private int base_health = 3;
    [SerializeField] private int lives = 1;
    [SerializeField] private float fire_rate = 1.5f;
    [SerializeField] private Projectile projectile_prefab;
    [SerializeField] private Transform fire_spawn_point;
    [SerializeField] private AudioClip fire_sfx;
    [SerializeField] private AudioClip hit_sfx;


    private Vector2 move_dir;
    private Vector2 target_position;
    private int current_health;
    private ObjectPool<Projectile> projectile_pool;
    private float next_fire_time;


    public ObjectPool<Projectile> ProjectilePool => projectile_pool;
    public Action<int> OnHealthUpdated;
    public Action<int, int> OnMaxHealthUpdated;
    public Action<int> OnLivesUpdated;
    public int MaxHealth => base_health;
    public int CurrentHealth => current_health;
    public int Lives => lives;

    public bool in_play_mode = false;

    private void Awake()
    {
        Instance = this;

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
        if (in_play_mode == false) return;


        // Get the raw input axes for snappy input
        var moveX = Input.GetAxisRaw("Horizontal");
        var moveY = Input.GetAxisRaw("Vertical");

        // Set the movement direction based on input
        move_dir = new Vector2(moveX, moveY).normalized;

        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && Time.time >= next_fire_time)
            Fire();
    }

    private void FixedUpdate()
    {
        var targetVelocity = move_dir * move_speed;
        rb2d.velocity = targetVelocity;
    }

    private void Fire()
    {
        // Update the next time the player can fire
        next_fire_time = Time.time + fire_rate;

        // Get the projectile from the pool and initialize it
        var bullet = projectile_pool.Get();
        if (bullet)
        {
            bullet.transform.position = fire_spawn_point.position;
            var target = Vector3.up * 4.0f; // Adjust this as needed for your game
            bullet.InitProjectile(target, this);

            // Play the fire sound
            AudioManager.Instance.PlaySFX(fire_sfx);
        }
    }


    public void OnTakeDamage(int damage_amount)
    {
        current_health = Mathf.Clamp(current_health - 1, 0, base_health);
        OnHealthUpdated?.Invoke(current_health);
        AudioManager.Instance.PlaySFX(hit_sfx);
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
            {
                GameManager.instance.StopGame();
                in_play_mode = false;
            }
        }
    }
    public void LivesUpdated(int new_amount)
    {
        lives += new_amount;
        OnLivesUpdated?.Invoke(lives);
    }
    public void IncreaseHealth(int amount)
    {
        current_health += amount;
        OnHealthUpdated?.Invoke(current_health);
    }
    public void ResetStats()
    {
        current_health = MaxHealth;
        lives = 1;

        OnHealthUpdated?.Invoke(current_health);
        OnLivesUpdated?.Invoke(lives);
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
    private void OnProjectileDestroyed(Projectile obj)
    {
        Destroy(obj.gameObject);
    }
    private void OnProjectileRelease(Projectile obj)
    {
        obj.gameObject.SetActive(false);
        obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        obj.transform.position = new Vector3(1000.0f, 10000.0f, 0.0f);
    }



}

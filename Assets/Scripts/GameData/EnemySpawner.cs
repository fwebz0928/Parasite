using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace GameData
{
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance;
        [SerializeField] private BoxCollider2D spawn_area;
        [SerializeField] private float min_spawn_rate;
        [SerializeField] private float max_spawn_rate;

        private LevelData current_level_data;
        public bool b_spawn_enemies = true;
        private float elapsed_time;
        private void Awake()
        {
            Instance = this;
        }



        public void InitEnemyManager(LevelData in_level_data)
        {
            current_level_data = in_level_data;
            b_spawn_enemies = true;
            StartCoroutine(SpawnEnemies());
        }


        public IEnumerator SpawnEnemies()
        {
            while (b_spawn_enemies)
            {
                SpawnRandomEnemy();
                yield return new WaitForSeconds(Random.Range(min_spawn_rate, max_spawn_rate));
            }
            yield return null;
        }


        public void SpawnRandomEnemy()
        {
            var total_chance = current_level_data.spawnable_enemies.Sum(enemy_data => enemy_data.SpawnChance);

            var random_value = Random.Range(0, total_chance);
            var cumulativeChance = 0f;

            foreach (var enemy_info in current_level_data.spawnable_enemies)
            {
                cumulativeChance += enemy_info.SpawnChance;
                if (!(random_value <= cumulativeChance)) continue;

                Instantiate(enemy_info.EnemyPrefab, GetRandomSpawnPoint(), Quaternion.identity, this.transform);
                break;
            }

        }

        public void SpawnBoss() { }

        public void StopSpawning()
        {
            b_spawn_enemies = false;

            foreach (Transform child_enemy in this.transform)
                Destroy(child_enemy.gameObject);

        }


        public void DestroyALlEnemies() { }


        private Vector3 GetRandomSpawnPoint()
        {
            var bounds = spawn_area.bounds;

            var bound_x = Random.Range(bounds.min.x, bounds.max.x);
            var bound_y = Random.Range(bounds.min.y, bounds.max.y);
            return new Vector3(bound_x, bound_y, 0.0f);
        }



    }
}

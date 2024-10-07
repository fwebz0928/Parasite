using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UI;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace GameData
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        [SerializeField] private PlayerController player_ref;
        [SerializeField] private ScoreText score_prefab;
        [SerializeField] private PowerUp power_up_prefab;
        [SerializeField] private Transform player_start_pos;
        [SerializeField] private List<LevelData> current_levels = new List<LevelData>();
        [SerializeField] private List<PowerUpInfo> PowerUps = new List<PowerUpInfo>();
        public float level_timer = 100;

        private int current_level_index;
        private bool b_boss_active = false;
        private bool b_boss_stop_level = false;
        public float elapsed_timer { get; private set; } = 0.0f;

        public int CurrentScore { get; private set; }
        public ObjectPool<ScoreText> ScoreTextPool { get; private set; }
        public ObjectPool<PowerUp> PowerUpPool { get; private set; }

        public int BestScore = 0;



        public Action<int> OnScoreUpdated;
        public Action<float> OnTimerUpdated;


        private Coroutine initGameCoroutine;
        private Coroutine startGameCoroutine;
        private Coroutine spawnEnemiesCoroutine;


        private void Awake()
        {
            instance = this;

            ScoreTextPool = new ObjectPool<ScoreText>
            (
                OnTextCreated,
                OnTextGet,
                OnTextReleased,
                OnTextDestroyed
            );

            PowerUpPool = new ObjectPool<PowerUp>
            (
                OnPowerUpCreated,
                OnPowerUpGet,
                OnPowerUpReleased,
                OnPowerUpDestroyed
            );




        }

        public void PlayGame()
        {
            StartCoroutine(InitGame());
        }

        public void StopGame()
        {
            if (startGameCoroutine != null)
                StopCoroutine(StartGame());


            foreach (Transform child in this.transform)
                Destroy(child.gameObject);

            b_boss_active = true;
            b_boss_stop_level = true;

            EnemySpawner.Instance.StopSpawning();
            EnemySpawner.Instance.DestroyALlEnemies();
            DeathMenu.Instance.DisplayDeathMenu();
        }


        private IEnumerator InitGame()
        {
            player_ref.transform.DOMove(player_start_pos.position, .55f).OnComplete(() => { player_ref.in_play_mode = true; });

            b_boss_active = false;
            b_boss_stop_level = false;

            yield return new WaitForSeconds(.6f);
            yield return MainUI.Instance.DisplayLevelTitle();
            yield return new WaitForSeconds(0.45f);
            EnemySpawner.Instance.InitEnemyManager(current_levels[current_level_index]);
            startGameCoroutine = StartCoroutine(StartGame());
        }



        private IEnumerator StartGame()
        {
            while (b_boss_active == false && b_boss_stop_level == false)
            {
                //if (elapsed_timer >= level_timer)
                //{
                //    EnemySpawner.Instance.SpawnBoss();
                //    b_boss_active = true;
                //    break;
                //}
                //
                //elapsed_timer += Time.deltaTime;
                ////print($"Timer: {elapsed_timer}");
                //OnTimerUpdated?.Invoke(elapsed_timer);

                yield return null;
            }
            yield return null;
        }






        public void UpdateScore(int in_score, Vector3 spawn_point)
        {
            CurrentScore += in_score;
            OnScoreUpdated?.Invoke(CurrentScore);

            //Display the score text
            var score_text = ScoreTextPool.Get();
            score_text.transform.position = spawn_point;
            score_text.InitScoreText(in_score);

            if (in_score % 1000 == 0)
                player_ref.LivesUpdated(1);
        }
        private ScoreText OnTextCreated()
        {
            var new_text = Instantiate(score_prefab);
            new_text.gameObject.SetActive(false);
            return new_text;
        }
        private void OnTextGet(ScoreText obj)
        {
            obj.gameObject.SetActive(true);
        }
        private void OnTextReleased(ScoreText obj)
        {
            obj.gameObject.SetActive(false);
        }
        private void OnTextDestroyed(ScoreText obj) { }

        public void CreatePowerUp(Vector3 spawn_pos)
        {
            foreach (var power_ups in PowerUps)
            {
                var random_amount = Random.Range(0, 150.0f);
                if (random_amount <= power_ups.spawn_chance)
                {
                    var spawned_powerup = PowerUpPool.Get();
                    spawned_powerup.SpawnPowerUp(spawn_pos, power_ups);
                    break;
                }
            }

        }

        private PowerUp OnPowerUpCreated()
        {
            var new_powerup = Instantiate(power_up_prefab, this.transform);
            new_powerup.gameObject.SetActive(false);
            return new_powerup;
        }
        private void OnPowerUpGet(PowerUp obj)
        {
            obj.gameObject.SetActive(true);
        }
        private void OnPowerUpReleased(PowerUp obj)
        {
            obj.transform.localScale = Vector3.zero;
            obj.transform.position = new Vector3(1000.0f, 1000.0f, 0.0f);
            obj.gameObject.SetActive(false);
        }
        private void OnPowerUpDestroyed(PowerUp obj)
        {
            Destroy(obj.gameObject);
        }

        public (bool new_highscore, int highscore) CheckHighScore()
        {
            if (BestScore >= CurrentScore)
                return (false, BestScore);

            BestScore = CurrentScore;
            CurrentScore = 0;
            OnScoreUpdated?.Invoke(CurrentScore);
            return (true, BestScore);
        }


    }
}

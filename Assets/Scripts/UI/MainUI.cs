using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameData;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MainUI : MonoBehaviour
    {
        [SerializeField] private PlayerController player_ref;
        [SerializeField] private GameManager game_manager;


        [Header("Health")]
        [SerializeField] private HeartContainer heart_prefab;
        [SerializeField] private Transform heart_parent;
        [SerializeField] private TextMeshProUGUI lives_text;

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI score_text;

        [Header("Level")]
        [SerializeField] private CanvasGroup level_group;
        [SerializeField] private TextMeshProUGUI level_name;


        private List<HeartContainer> hearts = new List<HeartContainer>();

        private void Start()
        {
            player_ref.OnHealthUpdated += HealthUpdated;
            player_ref.OnMaxHealthUpdated += MaxHealthUpdated;
            player_ref.OnLivesUpdated += LivesUpdated;
            game_manager.OnScoreUpdated += ScoreUpdated;

            //Clean up any hearts from the editor
            foreach (Transform child_transform in heart_parent)
                Destroy(child_transform.gameObject);


            //Initialize health with the base health
            for (var i = 0; i < player_ref.MaxHealth; i++)
                CreateNewHeart();

            ScoreUpdated(game_manager.CurrentScore);
            LivesUpdated(player_ref.Lives);

            //Fade in then fade out the level Title
            StartCoroutine(DisplayLevelTitle());
        }
        private void LivesUpdated(int current_lives)
        {
            lives_text.text = $"{current_lives}";
        }
        private void ScoreUpdated(int new_score)
        {
            score_text.text = $"{new_score}";
        }
        private void MaxHealthUpdated(int added_health, int current_health)
        {
            for (var i = 0; i < added_health; i++)
                CreateNewHeart();

            HealthUpdated(current_health);
        }
        private void HealthUpdated(int current_health)
        {
            print($"Updating Current Health: {current_health}");
            for (var i = 0; i < hearts.Count; i++)
            {
                var b_filled = i < current_health;
                hearts[i].ToggleHeart(b_filled);
            }

        }


        private void CreateNewHeart()
        {
            var new_heart = Instantiate(heart_prefab, heart_parent);
            new_heart.ToggleHeart(true);
            hearts.Add(new_heart);

        }

        private IEnumerator DisplayLevelTitle()
        {
            yield return level_group.DOFade(1.0f, .25f);
            yield return new WaitForSeconds(1.0f);
            yield return level_group.DOFade(0.0f, .25f);
        }


    }
}

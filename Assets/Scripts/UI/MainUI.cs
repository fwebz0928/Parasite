using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class MainUI : MonoBehaviour
    {
        [SerializeField] private PlayerController player_ref;
        [SerializeField] private HeartContainer heart_prefab;
        [SerializeField] private Transform heart_parent;

        private List<HeartContainer> hearts = new List<HeartContainer>();

        private void Start()
        {
            player_ref.OnHealthUpdated += HealthUpdated;
            player_ref.OnMaxHealthUpdated += MaxHealthUpdated;

            //Clean up any hearts from the editor
            foreach (Transform child_transform in heart_parent)
                Destroy(child_transform.gameObject);


            //Initialize health with the base health
            for (var i = 0; i < player_ref.MaxHealth; i++)
                CreateNewHeart();
        }
        private void MaxHealthUpdated(int added_health, int current_health)
        {
            for (var i = 0; i < added_health; i++)
                CreateNewHeart();

            HealthUpdated(current_health);
        }
        private void HealthUpdated(int current_health)
        {
            for (var i = 0; i < current_health; i++)
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


    }
}

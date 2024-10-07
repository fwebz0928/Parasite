using System;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DeathMenu : MonoBehaviour
    {
        public static DeathMenu Instance;

        [SerializeField] private Button RestartButton;
        [SerializeField] private Button QuitButton;
        [SerializeField] private TextMeshProUGUI HighScoreText;




        private void Awake()
        {
            Instance = this;
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            RestartButton.onClick.AddListener(RestartGame);
        }
        private void OnDisable()
        {
            RestartButton.onClick.RemoveListener(RestartGame);
        }


        public void DisplayDeathMenu()
        {
            this.gameObject.SetActive(true);

            //Update highscore
            var (b_new_high_score, new_high_score) = GameManager.instance.CheckHighScore();
            if (b_new_high_score)
            {
                HighScoreText.text = $"{new_high_score}";
            }
        }



        private void RestartGame()
        {
            PlayerController.Instance.ResetStats();
            GameManager.instance.PlayGame();
            this.gameObject.SetActive(false);
        }


    }
}

using System;
using UnityEngine;
using UnityEngine.Pool;

namespace GameData
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        [SerializeField] private ScoreText score_prefab;

        public int CurrentScore { get; private set; }
        public ObjectPool<ScoreText> ScoreTextPool { get; private set; }

        public Action<int> OnScoreUpdated;


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


        }

        public void UpdateScore(int in_score, Vector3 spawn_point)
        {
            CurrentScore += in_score;
            OnScoreUpdated?.Invoke(CurrentScore);

            //Display the score text
            var score_text = ScoreTextPool.Get();
            score_text.transform.position = spawn_point;
            score_text.InitScoreText(in_score);
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

        private void OnTextReleased(ScoreText obj) { }
        private void OnTextDestroyed(ScoreText obj) { }






    }
}

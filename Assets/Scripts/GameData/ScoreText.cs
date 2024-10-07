using System.Collections;
using TMPro;
using UnityEngine;

namespace GameData
{
    public class ScoreText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI score_text;
        [SerializeField] private float duration = 1.0f;
        [SerializeField] private float move_speed = 4.0f;
        [SerializeField] private float fade_duration = 4.0f;

        private float elapsed_time;
        private float fade_out_time;

        public void InitScoreText(int score)
        {
            score_text.text = $"+{score}";
            score_text.color = Color.white;
            elapsed_time = 0.0f;

            StartCoroutine(AnimateNumbers());
        }


        private IEnumerator AnimateNumbers()
        {
            var text_color = score_text.color;
            while (elapsed_time < duration)
            {
                //Launch up into the air 
                elapsed_time += Time.deltaTime;
                transform.position += new Vector3(0, move_speed) * Time.deltaTime;

                //Make it slowly fade out over time
                fade_out_time -= Time.deltaTime;
                if (elapsed_time > fade_out_time)
                {
                    text_color.a -= fade_duration * Time.deltaTime;
                    score_text.color = text_color;
                }
                yield return null;
            }

            GameManager.instance.ScoreTextPool.Release(this);

        }


    }
}

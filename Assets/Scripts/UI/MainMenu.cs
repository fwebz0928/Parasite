using DG.Tweening;
using GameData;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button start_button;
        [SerializeField] private Button quit_button;
        [SerializeField] private CanvasGroup main_menu_scalar;
        [SerializeField] private CanvasGroup main_ui_scalar;

        private void Awake()
        {
            start_button.onClick.AddListener(StartGame);
        }
        private void StartGame()
        {
            main_menu_scalar.DOFade(0.0f, .15f).OnComplete(() => {
                main_ui_scalar.DOFade(1.0f, .15f).OnComplete(() => {
                    GameManager.instance.PlayGame();
                    this.gameObject.SetActive(false);
                });
            });
        }




    }
}

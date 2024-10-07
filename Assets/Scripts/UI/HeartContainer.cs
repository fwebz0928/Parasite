using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeartContainer : MonoBehaviour
    {
        [SerializeField] private Image heart_image;
        [SerializeField] private Sprite[] heart_icons;


        public void ToggleHeart(bool b_filled)
        {
            heart_image.sprite = b_filled ? heart_icons[0] : heart_icons[1];
            if (b_filled == false)
                this.transform.DOScale(new Vector3(1.2f, 1.2f, 1.0f), .15f).SetLoops(2, LoopType.Yoyo);
        }



    }
}

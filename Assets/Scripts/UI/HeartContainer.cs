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
        }



    }
}

using UnityEngine;
using UnityEngine.UI;

namespace YsoCorp {

    [RequireComponent(typeof(Image))]
    public class SpritesAnimator : MonoBehaviour {

        [SerializeField] private Sprite[] sprites;
        [SerializeField] private float rate;

        private Image image;
        private int index = -1;

        private void OnValidate() {
            GetComponent<Image>().sprite = sprites?.Length > 0 ? sprites[0] : null;
        }

        private void Start() {
            image = GetComponent<Image>();
            InvokeRepeating(nameof(NextSprite), 0, rate);
        }

        private void NextSprite() {
            index += 1;
            if (index >= sprites.Length) {
                index = 0;
            }

            image.sprite = sprites[index];
        }

    }

}
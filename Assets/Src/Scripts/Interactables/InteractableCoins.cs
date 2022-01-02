using DG.Tweening;
using UnityEngine;

namespace YsoCorp {

    public class InteractableCoins : Interactable {

        [SerializeField] private int quantity;
        [SerializeField] private float animRotationDuration = 0.5f;

        private void Start() {
            transform
                .DOLocalRotate(new Vector3(0, 360, 0), animRotationDuration)
                .SetEase(Ease.Linear)
                .SetRelative(true)
                .SetLoops(-1);
        }

        protected override void OnDestroyNotQuitting() {
            base.OnDestroyNotQuitting();

            transform.DOKill();
        }

        public override void Interact() {
            this.game.menuGame.AddCoins(this.cam.ycCamera.WorldToScreenPoint(transform.position), quantity);
            Destroy(gameObject);
        }

    }

}
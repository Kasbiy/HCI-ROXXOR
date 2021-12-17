using DG.Tweening;
using UnityEngine;

namespace YsoCorp {

    public class InteractableCoins : Interactable {

        [SerializeField] private int quantity;
        [SerializeField] private RectTransform prefabCoin2D;
        [SerializeField] private float animRotationDuration = 0.5f;
        [SerializeField] private float animCoin2DDuration = 0.5f;

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
            this.dataManager.AddCoins(quantity);
            Destroy(gameObject);

            return;
            RectTransform coin2D = Instantiate(prefabCoin2D, this.cam.ycCamera.WorldToScreenPoint(transform.position), Quaternion.identity, this.game.menuGame.transform);
            coin2D.DOAnchorPos(Vector2.zero, animCoin2DDuration);
        }

    }

}
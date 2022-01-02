using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YsoCorp {

    public class MenuGame : AMenu {
        private const float COIN_SCALE_ANIMATION = 0.25f;
        private const float COIN_PATH_ANIMATION = 0.5f;
        private const float COIN_ANIMATION_DELAY = 0.25f;

        public Button bBack;
        public Joystick joystick;
        public TMP_Text coin;
        public RectTransform coinIcon;
        public RectTransform prefabAnimatedCoin;

        void Start() {

            this.bBack.onClick.AddListener(() => {
                this.ycManager.adsManager.ShowInterstitial(() => {
                    this.game.state = Game.States.Home;
                });
            });

        }

        public override void Display() {

            base.Display();

            this.coin.text = this.dataManager.GetCoins().ToString();

        }

        public void AddCoins(Vector3 screenPosition, int quantity) {

            this.dataManager.AddCoins(quantity);
            for (int i = 0; i < quantity; i += 1) {
                RectTransform coinAnimated = Instantiate(this.prefabAnimatedCoin, screenPosition, Quaternion.identity, transform);
                coinAnimated.SetParent(coinIcon, true);

                DOTween.Sequence()
                    .Append(coinAnimated.DOScale(2, COIN_SCALE_ANIMATION))
                    .Append(coinAnimated.DOAnchorPos(Vector2.zero, COIN_PATH_ANIMATION))
                    .OnComplete(() => {
                        this.coin.text = (int.Parse(this.coin.text) + 1).ToString();
                        this.coinIcon.DOPunchScale(Vector3.one * 1.1f, 0.1f);
                        Destroy(coinAnimated.gameObject);
                    })
                    .SetDelay(i * COIN_ANIMATION_DELAY);
            }

        }

    }

}

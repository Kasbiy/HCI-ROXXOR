using UnityEngine.UI;
using DG.Tweening;

namespace YsoCorp {

    public class MenuHome : AMenu {

        public Button bPlay;
        public Button bSelect;
        public Button bBuy;
        public Button bWatchAdd;
        public Button bPrev;
        public Button bNext;
        public Button bSetting;
        public Button bRemoveAds;

        public DOTweenAnimation[] cTween;
        public Text levelIndex;

        private UnlockableCharacter[] unlockableCharacters;
        private int selectedUnlockableCharacterIndex;
        private int currentUnlockableCharacterIndex;
        private Player currentPlayer;

        void Start() {
            unlockableCharacters = this.unlockableResourcesManager.GetUnlockableItems<UnlockableCharacter>();
            selectedUnlockableCharacterIndex = this.unlockableResourcesManager.GetSelectedNum<UnlockableCharacter>();
            currentUnlockableCharacterIndex = selectedUnlockableCharacterIndex;

            this.bPlay.onClick.AddListener(() => {
                this.ycManager.adsManager.ShowInterstitial(() => {
                    this.game.state = Game.States.Playing;
                });
            });
            this.bSelect.onClick.AddListener(() => {
                selectedUnlockableCharacterIndex = currentUnlockableCharacterIndex;
                this.unlockableResourcesManager.Select(unlockableCharacters[selectedUnlockableCharacterIndex]);
                ResetPlayer();
            });
            this.bPrev.onClick.AddListener(() => {
                SetModelIndex(currentUnlockableCharacterIndex - 1);
            });
            this.bNext.onClick.AddListener(() => {
                SetModelIndex(currentUnlockableCharacterIndex + 1);
            });
            this.bSetting.onClick.AddListener(() => {
                this.ycManager.settingManager.Show();
            });
            this.bRemoveAds.onClick.AddListener(() => {
                this.ycManager.inAppManager.BuyProductIDAdsRemove();
            });
        }

        public void Update() {
            this.bRemoveAds.gameObject.SetActive(this.ycManager.ycConfig.InAppRemoveAds != "" && this.ycManager.dataManager.GetAdsShow());
        }

        void SetModelIndex(int newUnlockableCharacterIndex) {
            currentUnlockableCharacterIndex = newUnlockableCharacterIndex;

            if (currentPlayer != null) {
                Destroy(currentPlayer.gameObject);
                currentPlayer = null;
            } else {
                this.player.gameObject.SetActive(false);
            }

            if (currentUnlockableCharacterIndex == selectedUnlockableCharacterIndex) {
                this.player.gameObject.SetActive(true);
                this.player.Reset();
            } else {
                currentPlayer = Instantiate(unlockableCharacters[newUnlockableCharacterIndex].player);
                currentPlayer.Reset();
            }
        }

        public override void Display() {
            base.Display();

            foreach (DOTweenAnimation cmove in cTween) { cmove.DOPlay(); }
        }
        public override void Hide() {
            base.Hide();

            foreach (DOTweenAnimation cmove in cTween) { cmove.DOPlayBackwards(); }
        }

    }

}

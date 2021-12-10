using UnityEngine.UI;

namespace YsoCorp {

    public  class MenuHome : AMenu {

        public Button bPlay;
        public Button bSelect;
        public Button bPrev;
        public Button bNext;
        public Button bSetting;
        public Button bRemoveAds;

        public Text levelIndex;

        private int modelIndex;

        void Start() {
            this.bPlay.onClick.AddListener(() => {
                this.ycManager.adsManager.ShowInterstitial(() => {
                    this.game.state = Game.States.Playing;
                });
            });
            this.bSelect.onClick.AddListener(() => {
            });
            this.bPrev.onClick.AddListener(() => {
            });
            this.bNext.onClick.AddListener(() => {
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

        void SetModelIndex(int newModelIndex) {
        }

    }

}

using UnityEngine.UI;

namespace YsoCorp {

    public class MenuHome : AMenu {

        public Button bPlay;
        public Button bSelect;
        public Button bBuy;
        public Button bWatchAds;
        public Button bPrev;
        public Button bNext;
        public Button bSetting;
        public Button bRemoveAds;

        public Text levelIndex;
        public Text unlockDescription;
        public Text price;

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
                this.ResetPlayer();
            });
            this.bPrev.onClick.AddListener(() => {
                this.SetModelIndex(currentUnlockableCharacterIndex - 1);
                LookPrevNextButton();
            });
            this.bNext.onClick.AddListener(() => {
                this.SetModelIndex(currentUnlockableCharacterIndex + 1);
                LookPrevNextButton();
            });
            this.bSetting.onClick.AddListener(() => {
                this.ycManager.settingManager.Show();
            });
            this.bRemoveAds.onClick.AddListener(() => {
                this.ycManager.inAppManager.BuyProductIDAdsRemove();
            });

            LookPrevNextButton();
        }

        public void Update() {
            this.bRemoveAds.gameObject.SetActive(this.ycManager.ycConfig.InAppRemoveAds != "" && this.ycManager.dataManager.GetAdsShow());
        }

        void LookPrevNextButton() {
            this.bPrev.interactable = currentUnlockableCharacterIndex > 0;
            this.bNext.interactable = currentUnlockableCharacterIndex < unlockableCharacters.Length - 1;
        }

        void SetModelIndex(int newUnlockableCharacterIndex) {
            currentUnlockableCharacterIndex = newUnlockableCharacterIndex;

            if (currentPlayer != null) {
                Destroy(currentPlayer.gameObject);
                currentPlayer = null;
            } else {
                this.player.gameObject.SetActive(false);
            }

            UnlockableCharacter currentCharacter = unlockableCharacters[currentUnlockableCharacterIndex];
            bool isUnlocked = this.unlockableResourcesManager.IsUnlocked(currentCharacter);
            bool isSelected = currentUnlockableCharacterIndex == selectedUnlockableCharacterIndex;

            this.bPlay.gameObject.SetActive(isSelected);
            if (isSelected) {
                this.bSelect.gameObject.SetActive(false);
                this.bBuy.gameObject.SetActive(false);
                this.bWatchAds.gameObject.SetActive(false);
                this.unlockDescription.gameObject.SetActive(false);

                this.player.gameObject.SetActive(true);
                this.player.Reset();
            } else {
                this.bSelect.gameObject.SetActive(isUnlocked);
                if (!isUnlocked) {
                    int mapNumber = this.resourcesManager.GetMapNumber();
                    int interstitialsNumber = this.ycManager.dataManager.GetInterstitialsNb();

                    if (currentCharacter.levelForUnlock != -1 && mapNumber < currentCharacter.levelForUnlock) {
                        this.bBuy.gameObject.SetActive(false);
                        this.bWatchAds.gameObject.SetActive(false);
                        this.unlockDescription.gameObject.SetActive(true);

                        unlockDescription.text = "Reach level " + currentCharacter.levelForUnlock;
                    } else if (currentCharacter.adsForUnlock != -1 && interstitialsNumber < currentCharacter.adsForUnlock) {
                        this.bBuy.gameObject.SetActive(false);
                        this.bWatchAds.gameObject.SetActive(false);
                        this.unlockDescription.gameObject.SetActive(true);

                        unlockDescription.text = interstitialsNumber + "/" + currentCharacter.levelForUnlock + " ads watched";
                    } else {
                        this.bBuy.gameObject.SetActive(true);
                        this.bWatchAds.gameObject.SetActive(currentCharacter.watchAdsForUnlock);
                        this.unlockDescription.gameObject.SetActive(false);

                        this.bBuy.interactable = this.dataManager.GetCoins() >= currentCharacter.priceForUnlock;
                        price.text = currentCharacter.priceForUnlock.ToString();
                    }
                }

                currentPlayer = Instantiate(currentCharacter.player);
                currentPlayer.Reset();
            }
        }

    }

}

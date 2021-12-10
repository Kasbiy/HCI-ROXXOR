using System;
using UnityEngine;

namespace YsoCorp {

    [DefaultExecutionOrder(-1)]
    public class CharactersManager : AResourcesManager {

        public UnlockableCharacter forceCharacter;

        public UnlockableCharacter[] Characters { private set; get; }

        protected override void Awake() {
            base.Awake();
            this.Characters = this.LoadIterator<UnlockableCharacter>("Characters/Character");

            if (this.dataManager.GetNumCharacter() == -1) {
                this.dataManager.UnlockNumCharacter(0);
            }
        }

        public UnlockableCharacter GetCharacter() {
#if UNITY_EDITOR
            if (this.forceCharacter != null) {
                return this.forceCharacter;
            }
#endif
            return this.Characters[this.dataManager.GetNumCharacter()];
        }

        public bool IsUnlocked(UnlockableCharacter character) {
            return this.dataManager.IsUnlockNumCharacter(Array.IndexOf(Characters, character));
        }

        public bool TryUnlockByAds(UnlockableCharacter character) {
            bool isUnlocked = true;

            if (isUnlocked) {
                this.dataManager.UnlockNumCharacter(Array.IndexOf(Characters, character));
            }
            return isUnlocked;
        }
    }

}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace YsoCorp {

    [DefaultExecutionOrder(-1)]
    public class UnlockableResourcesManager : AResourcesManager {
        private readonly Dictionary<Type, UnlockableItem[]> _unlockableItems = new Dictionary<Type, UnlockableItem[]>();

        protected override void Awake() {
            base.Awake();

            Type[] types = new Type[0];
            for (int i = 0; i < types.Length; i += 1) {

                _unlockableItems[types[i]] = this.LoadIterator<UnlockableCharacter>("Characters/Character");
            }

            if (this.dataManager.GetNumCharacter() == -1) {
                this.dataManager.UnlockNumCharacter(0);
            }
        }

        private UnlockableItem[] GetUnlockableItems<T>() where T : UnlockableItem {
            return _unlockableItems[typeof(T)];
        }

        private int GetIndexOf<T>(T unlockableItem) where T : UnlockableItem {
            return Array.IndexOf(GetUnlockableItems<T>(), unlockableItem);
        }

        public T GetSelected<T>() where T : UnlockableItem {
            return null;
        }

        public bool IsUnlocked<T>(T unlockableItem) where T : UnlockableItem {
            return this.dataManager.IsUnlockNumCharacter(GetIndexOf(unlockableItem));
        }

        public bool TryUnlockByAds<T>(T unlockableItem) where T : UnlockableItem {
            bool isUnlocked = true;

            if (isUnlocked) {
                this.dataManager.UnlockNumCharacter(GetIndexOf(unlockableItem));
            }
            return isUnlocked;
        }
    }

}

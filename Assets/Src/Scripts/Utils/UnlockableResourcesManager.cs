using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace YsoCorp {

    [DefaultExecutionOrder(-1)]
    public class UnlockableResourcesManager : AResourcesManager {

        private readonly Dictionary<Type, UnlockableItem[]> _unlockableItems = new Dictionary<Type, UnlockableItem[]>();

        protected override void Awake() {
            base.Awake();

            Type unlockableItemType = typeof(UnlockableItem);
            Type[] types = unlockableItemType.Assembly
                .GetTypes()
                .Where(type => type.IsSubclassOf(unlockableItemType))
                .ToArray();

            for (int i = 0; i < types.Length; i += 1) {
                Type type = types[i];
                string typePath = (string)type.GetField("PATH").GetValue(null);

                _unlockableItems[type] = this.LoadIterator<UnlockableCharacter>(typePath);
            }

            if (this.dataManager.GetSelected<UnlockableCharacter>() == -1) {
                this.dataManager.SetSelected<UnlockableCharacter>(0);
            }
        }

        private UnlockableItem[] GetUnlockableItems(Type type) {
            return _unlockableItems[type];
        }

        private int GetIndexOf<T>(T unlockableItem) where T : UnlockableItem {
            return Array.IndexOf(GetUnlockableItems<T>(), unlockableItem);
        }

        public T[] GetUnlockableItems<T>() where T : UnlockableItem {
            return GetUnlockableItems(typeof(T)).Select(unlockableItem => unlockableItem as T).ToArray();
        }

        public int GetSelectedNum<T>() where T : UnlockableItem {
            return this.dataManager.GetSelected<T>();
        }

        public T GetSelected<T>() where T : UnlockableItem {
            Debug.Log(GetSelectedNum<T>());
            Debug.Log(GetUnlockableItems(typeof(T)));
            Debug.Log(GetUnlockableItems(typeof(T)).Length);
            return GetUnlockableItems(typeof(T))[GetSelectedNum<T>()] as T;
        }

        public void Select<T>(T unlockableItem) where T : UnlockableItem {
            this.dataManager.SetSelected<T>(GetIndexOf(unlockableItem));
        }

        public bool IsUnlocked<T>(T unlockableItem) where T : UnlockableItem {
            return this.dataManager.IsUnlocked<T>(GetIndexOf(unlockableItem));
        }

        public bool TryUnlockByAds<T>(T unlockableItem) where T : UnlockableItem {
            bool isUnlocked = true;

            if (isUnlocked) {
                this.dataManager.SetUnlocked<T>(GetIndexOf(unlockableItem));
            }
            return isUnlocked;
        }

    }

}

using UnityEngine;

namespace YsoCorp {

    public abstract class UnlockableItem : ScriptableObject {

        public const string PATH = "";
        public const string ID = "";

        public int levelForUnlock = -1;
        public int adsForUnlock = -1;

        public int priceForUnlock = -1;
        public bool watchAdsForUnlock = false;

    }

}

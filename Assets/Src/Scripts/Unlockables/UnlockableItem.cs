using UnityEngine;

namespace YsoCorp {

    public abstract class UnlockableItem : ScriptableObject {

        public const string PATH = "";
        public const string ID = "";

        public int levelForUnlock;
        public int adsForUnlock;

    }

}

using UnityEngine;

namespace YsoCorp {

    public abstract class UnlockableItem : ScriptableObject {
        public abstract string ID { get; }

        public int levelForUnlock;
        public int adsForUnlock;
    }

}

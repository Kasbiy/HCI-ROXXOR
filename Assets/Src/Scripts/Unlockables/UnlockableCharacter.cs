using UnityEngine;

namespace YsoCorp {

    [CreateAssetMenu(menuName = "Unlockables/Character")]
    public class UnlockableCharacter : UnlockableItem {
        public override string ID => "character";

        public Player player;
    }

}
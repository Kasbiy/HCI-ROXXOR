using UnityEngine;

namespace YsoCorp {

    [CreateAssetMenu(menuName = "Unlockables/Character")]
    public class UnlockableCharacter : UnlockableItem {

        public new const string PATH = "Characters/character";
        public new const string ID = "character";

        public Player player;

    }

}
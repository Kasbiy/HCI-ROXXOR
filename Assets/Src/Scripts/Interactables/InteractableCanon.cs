using UnityEngine;
using DG.Tweening;

namespace YsoCorp {

    public class InteractableCanon : Interactable {
        [SerializeField] private DOTweenPath path;

        public override void Interact() {
            if (!path.tween.IsPlaying()) {
                this.player.FollowPath(path);
            }
        }

    }

}
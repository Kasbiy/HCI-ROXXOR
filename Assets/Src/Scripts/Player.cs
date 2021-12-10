using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace YsoCorp {

    public class Player : Movable {

        private static float SPEED_ROTATION = 25f;
        private static float SPEED_ACCELERATION = 0.5f;
        private static float SPEED = 4f;

        private static float SWIPE_MIN_DISTANCE = 50f;

        private float _swipeDistance;
        private bool _isMoving;
        private Animator _animator;
        private Quaternion _rotation;
        private Rigidbody _rigidbody;
        private RagdollBehaviour _ragdollBehviour;

        public bool isAlive { get; protected set; }
        public float speed { get; private set; }

        protected override void Awake() {
            this._rigidbody = this.GetComponent<Rigidbody>();
            this._animator = this.GetComponentInChildren<Animator>();
            this._ragdollBehviour = this.GetComponent<RagdollBehaviour>();
            this.isAlive = true;

            this.game.onStateChanged += this.Launch;
        }

        private void Launch(Game.States states) {
            if (states == Game.States.Playing) {
                this._isMoving = true;
                this._animator?.SetBool("Moving", true);
            } else if (states == Game.States.Win) {
                this._isMoving = false;
                this._animator?.SetBool("Moving", false);
                this._animator?.SetTrigger("Win");
            } else if (states == Game.States.Lose) {
                this._isMoving = false;
            }
        }

        public void Reset() {
            this.isAlive = true;
            this._isMoving = false;
            if (this._animator != null) {
                this._animator.enabled = true;
                this._animator.SetBool("Moving", false);
            }

            Transform spot = this.game.map.GetStartingPos();
            this.transform.position = spot.position;
            this.transform.rotation = spot.rotation;
            this._rigidbody.velocity = Vector3.zero;
            this._ragdollBehviour?.Reset();
            this.cam.Follow(this.transform);

            this._rotation = Quaternion.Euler(0f, this.transform.rotation.eulerAngles.y, 0);
        }

        public void Die(Transform killer) {
            this.isAlive = false;

            if (this._ragdollBehviour != null) {
                this._ragdollBehviour.EnableRagdoll(killer);
                this.cam.Follow(this._ragdollBehviour.hips);
            }
        }

        private void FixedUpdate() {
            if (this.game.state != Game.States.Playing || this.isAlive == false) {
                return;
            }

            if (this._isMoving == true) {
                this.speed += SPEED_ACCELERATION;
            } else {
                this.speed -= SPEED_ACCELERATION * 3f;
            }

            this.speed = Mathf.Clamp(this.speed, 0, SPEED);
            if (this.speed != 0) {
                this._rigidbody.MovePosition(this._rigidbody.position + this.transform.forward * this.speed * Time.fixedDeltaTime);
                this._rigidbody.MoveRotation(Quaternion.RotateTowards(this._rigidbody.rotation, this._rotation, SPEED_ROTATION));
            }
        }

        private void SwipeUp() {
            this._animator.SetTrigger("Jump");
        }

        private void SwipeDown() {
            this._animator.SetTrigger("Stomp");
        }

        public override void GesturePanDown() {
            if (this.game.state != Game.States.Playing || this.isAlive == false) {
                return;
            }

            _swipeDistance = 0;
        }

        public override void GesturePanDeltaY(float deltaY) {
            if (this.game.state != Game.States.Playing || this.isAlive == false) {
                return;
            }

            if ((_swipeDistance < 0 && deltaY > 0) || (_swipeDistance > 0 && deltaY < 0)) {
                _swipeDistance = 0;
            }
            _swipeDistance += deltaY;
            if (_swipeDistance < 0 && -_swipeDistance >= SWIPE_MIN_DISTANCE / this.ScreenScaleH()) {
                SwipeDown();
            } else if (_swipeDistance > 0 && _swipeDistance >= SWIPE_MIN_DISTANCE / this.ScreenScaleH()) {
                SwipeUp();
            }
        }

        public void FollowPath(DOTweenPath path) {
            Transform oldParent = transform.parent;

            void OnPathStarted() {
                transform.SetParent(path.transform);
                _rigidbody.isKinematic = true;
                this.speed = 0;
                this._isMoving = false;
                this._animator?.SetBool("Moving", false);
            }

            void OnPathCompleted() {
                transform.SetParent(oldParent);
                _rigidbody.isKinematic = false;
                this._isMoving = true;
                this._animator?.SetBool("Moving", true);
                path.onPlay.RemoveListener(OnPathStarted);
                path.onComplete.RemoveListener(OnPathCompleted);
            }

            if (!path.hasOnPlay) {
                path.hasOnPlay = true;
                path.onPlay = new UnityEvent();
            }
            path.onPlay.AddListener(OnPathStarted);

            if (!path.hasOnComplete) {
                path.hasOnComplete = true;
                path.onComplete = new UnityEvent();
            }
            path.onComplete.AddListener(OnPathCompleted);

            path.DOPlay();
        }
    }
}

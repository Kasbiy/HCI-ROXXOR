using UnityEngine;

namespace YsoCorp {

    public class Player : Movable {

        private const float SPEED_ACCELERATION = 0.1f;
        private const float SPEED = 5f;
        private const float MOVE_SENSITIVITY = 0.005f;
        private const float JUMP_FORCE = 500f;
        private const float SPECIAL_JUMP_FORCE = 1000f;

        private const float SWIPE_MIN_DISTANCE = 50f;

        private float _swipeDistance;
        private bool _isMoving;
        private Vector3 _slideMove;
        private Animator _animator;
        private Rigidbody _rigidbody;
        private RagdollBehaviour _ragdollBehviour;
        private bool _swipeUp;
        private bool _swipeDown;

        public bool isAlive { get; protected set; }
        public float speed { get; private set; }

        protected override void Awake() {
            this._rigidbody = this.GetComponent<Rigidbody>();
            this._animator = this.GetComponentInChildren<Animator>();
            this._ragdollBehviour = this.GetComponent<RagdollBehaviour>();
            this.isAlive = true;

            this.game.onStateChanged += this.Launch;
        }

        protected void Start() {
            this.game.GetComponent<PanController>().AddMovable(this);
        }

        protected override void OnDestroyNotQuitting() {
            base.OnDestroyNotQuitting();

            this.game.onStateChanged -= this.Launch;
            this.game.GetComponent<PanController>().RemoveMovable(this);
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
                this._rigidbody.MovePosition(this._rigidbody.position + this.transform.forward * this.speed * Time.fixedDeltaTime + this._slideMove);
                this._slideMove = Vector3.zero;
            }

            if (Physics.Raycast(transform.position + new Vector3(0, 0.01f, 0), Vector3.down, 0.01f, LayerMask.GetMask("Ground"))) {
                if (_swipeUp) {
                    _swipeUp = false;
                    this._rigidbody.AddForce(Vector3.up * SPECIAL_JUMP_FORCE);
                } else {
                    this._rigidbody.AddForce(Vector3.up * JUMP_FORCE);
                }

                if (_swipeDown) {
                    _swipeDown = false;
                    Physics.gravity /= 25;
                }
            }
        }

        private void SwipeUp() {
            if (_swipeUp) return;

            _swipeUp = true;
        }

        private void SwipeDown() {
            if (_swipeDown) return;

            _swipeDown = true;
            Physics.gravity *= 25;
        }

        public override void GesturePanDown() {
            if (this.game.state != Game.States.Playing || this.isAlive == false) {
                return;
            }

            _swipeDistance = 0;
        }

        public override void GesturePanDeltaX(float deltaX) {
            if (this.game.state != Game.States.Playing || this.isAlive == false) {
                return;
            }

            this._slideMove += new Vector3(deltaX * MOVE_SENSITIVITY, 0, 0);
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
    }
}

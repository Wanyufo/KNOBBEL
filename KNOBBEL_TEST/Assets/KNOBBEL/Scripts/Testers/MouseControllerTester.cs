using System.Linq;
using Gm;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KNOBBEL.Scripts.Testers
{
    public class MouseControllerTester : MonoBehaviour
    {
        // Ideas: 
        // down dash/slam
        // faster falling / maybe even higher gravity generally
        // slo mo ability  -  give user time to adjust
        [SerializeField] private float groundSpeed = 10f;
        [SerializeField] private float airSpeed = 10f;
        [SerializeField] private float jumpSpeed = 10f;
        [SerializeField] private float climbSpeed = 10f;
        [SerializeField] private float slamSpeed = 20f;
        [SerializeField] private float gravity = 10f;
        [SerializeField] private float fallMultiplier = 1.75f;
        private Rigidbody2D _rigidbody2D;
        private Collider2D _groundCheckCollider;
        private Collider2D _climbCheckCollider;
        private int _groundLayerMask;
        private int _climbableLayerMask;
        private Vector2 _speed;

        private Vector3 _resetPosition;

        public void SetResetPosition(Vector3 resetPosition)
        {
            _resetPosition = resetPosition;
        }

        private void Death()
        {
            transform.position = _resetPosition;
            _speed = Vector2.zero;
            _rigidbody2D.velocity = Vector2.zero;
        }

        // handle collision with DeathZone
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<MiscTagger>(out MiscTagger tagger))
            {
                if (tagger.GetTag().Equals(TagManager.MiscTags.DeathZone))
                {
                    Death();
                }
            }
        }

        public bool IsOnGround { get; protected set; }
        public bool IsClimbing { get; protected set; }

        public bool IsInAir => !IsOnGround && !IsClimbing;

        // Set to false once executed
        private bool _jump = false;
        private bool _slam = false;
        private Vector2 _move = Vector2.zero;


        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            // we do our own gravity because that gives us more control
            // _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody2D.gravityScale = 0;

            SetResetPosition(transform.position);

            // TODO rewrite this, it's ugly as hell
            _groundCheckCollider = GM.I.TagManager.GetObjectsWithTag(TagManager.MiscTags.PlayerGroundCheck).Select(x =>
            {
                MiscTagger tagger = (MiscTagger) x;
                // check that the tagger is a child of this object
                if (tagger.transform.IsChildOf(this.transform))
                {
                    return tagger.GetComponent<Collider2D>();
                }
                else
                {
                    throw new System.Exception("PlayerGroundCheck is not a child of the Player");
                }
            }).First();
            _climbCheckCollider = GM.I.TagManager.GetObjectsWithTag(TagManager.MiscTags.PlayerClimbCheck).Select(x =>
            {
                MiscTagger tagger = (MiscTagger) x;
                // check that the tagger is a child of this object
                if (tagger.transform.IsChildOf(this.transform))
                {
                    return tagger.GetComponent<Collider2D>();
                }
                else
                {
                    throw new System.Exception("PlayerClimbCheck is not a child of the Player");
                }
            }).First();
            _groundLayerMask = LayerMask.GetMask("Ground");
            _climbableLayerMask = LayerMask.GetMask("Climbable");
        }

        public void OnMove(InputValue inputValue)
        {
            Vector2 inputDir = inputValue.Get<Vector2>();
            _move = new Vector2(inputDir.x > 0 ? 1 : inputDir.x < 0 ? -1 : 0,
                inputDir.y > 0 ? 1 : inputDir.y < 0 ? -1 : 0);
            if (_move.y < 0)
            {
                _slam = true;
            }
            // we set the input to be left or right full. and to be not up but down full if down is pressed
            // float x = inputDir.x switch
            // {
            //     > 0 => 1,
            //     < 0 => -1,
            //     _ => 0
            // };
            //
            // float y = inputDir.y switch
            // {
            //     > 0 => 0,
            //     < 0 => -1,
            //     _ => 0
            // };
            // // Vector2 nonUpInputDir = new Vector2(inputDir.x > 0 ? 1 : inputDir.x < 0 ? -1 : 0,
            // //     inputDir.y > 0 ? 0 : inputDir.y < 0 ? -1 : 0);
            // _moveGroundAir = new Vector2(x, y);
            //    Debug.Log("_move = " + "(" + _move.x + "," + _move.y + ")");
        }

        public void OnJump(InputValue inputValue)
        {
            _jump = inputValue.isPressed;
            // Debug.Log("Jump: " + inputValue.isPressed);
        }

        private void FixedUpdate()
        {
            IsOnGround = _groundCheckCollider.IsTouchingLayers(_groundLayerMask);
            IsClimbing = _climbCheckCollider.IsTouchingLayers(_climbableLayerMask);

            _speed = _rigidbody2D.velocity;
            // horizontal movement
            if (IsOnGround)
            {
                _speed.x = _move.x * groundSpeed;
            }
            else
            {
                _speed.x = _move.x * airSpeed;
            }


            // vertical movement

            // if we can and we want, we jump
            // when we are on ground, we ignore all downwards movement, both physical and input
            // this also means that jumping is a setting of speed, not an addition to it
            if (IsOnGround)
            {
                if (_jump)
                {
                    _speed.y = jumpSpeed;
                }
                // climbing also means we dont apply gravity and we dont slam    
            }

            if (IsClimbing)
            {
                _speed.y = _move.y * climbSpeed;
            }

            // we are in the air. We apply gravity, which is stronger if we are falling. Finally we apply the slam speed if we want to slam
            if (IsInAir)
            {
                // apply gravity
                _speed.y -= gravity * Time.deltaTime * (_speed.y < 0 ? fallMultiplier : 1f);
                // slam
                if (_slam)
                {
                    _speed.y -= slamSpeed;
                }
            }

            // apply speed and reset jump flag
            _rigidbody2D.velocity = _speed;
            _jump = false;
            // TODO reset slam only if touching floor if this is a desired behaviour. Like this a slam can be cancelled
            _slam = false;
        }
    }
}
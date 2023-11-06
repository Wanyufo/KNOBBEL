using System;
using System.Collections.Generic;
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
        [Header("Components")] [SerializeField]
        private GameObject visuals;

        [SerializeField] private Collider2D groundCheckCollider;
        [SerializeField] private Collider2D climbCheckCollider;


        [Header("Movement Mechanics")] [SerializeField]
        private float groundSpeed = 10f;

        [SerializeField] private float airSpeed = 10f;
        [SerializeField] private float jumpSpeed = 10f;
        [SerializeField] private float climbSpeed = 10f;
        [SerializeField] private float slamSpeed = 20f;
        [SerializeField] private float gravity = 10f;
        [SerializeField] private float fallMultiplier = 1.75f;
        [Space] [Header("Visuals")] private Rigidbody2D _rigidbody2D;
        private Collider2D _groundCheckCollider;
        private Collider2D _climbCheckCollider;
        private GameObject _visuals;


        private int _groundLayerMask;
        private int _climbableLayerMask;
        private Vector2 _speed;

        private Vector3 _resetPosition;

        private List<Vector3> _climbingSurfaceRotations = new List<Vector3>();

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

        // move is a who's values are eiter -1, 0 or 1
        private Vector2 _move = Vector2.zero;


        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            // we do our own gravity because that gives us more control
            _rigidbody2D.gravityScale = 0;
            SetResetPosition(transform.position);
            _groundCheckCollider = groundCheckCollider;
            _climbCheckCollider = climbCheckCollider;
            _visuals = visuals;
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
        }

        private void Update()
        {
            if (_move.x != 0)
            {
                SetVisualDirection(_move.x);
            }

            if (IsClimbing && !IsOnGround)
            {
                // TODO implement rotation to match climbing surface
            }
            else
            {
                // TODO reset rotation
            }
        }

        private void SetVisualDirection(float direction)
        {
            Vector3 scale = _visuals.transform.localScale;
            scale.x = direction;
            _visuals.transform.localScale = scale;
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
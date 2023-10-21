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
        // switch to velocity control instead of force. is snappier
        [SerializeField] float groundSpeed = 10f;
        [SerializeField] float jumpForce = 10f;
        private Rigidbody2D _rigidbody2D;
        private Collider2D _groundCheckCollider;
        private int _groundLayerMask;

        public bool IsOnGround { get; protected set; }

        // Set to false once executed
        private bool _jump = false;
        private Vector2 _move = Vector2.zero;


        // TODO Calculate all the gravity etc manually
        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
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
            Debug.Log("_groundCheckCollider = " + _groundCheckCollider.name);
            _groundLayerMask = Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Ground"));
        }

        public void OnMove(InputValue inputValue)
        {
            Vector2 inputDir = inputValue.Get<Vector2>();
            Vector2 nonUpInputDir = new Vector2(inputDir.x > 0 ? 1 : inputDir.x < 0 ? -1 : 0,
                inputDir.y > 0 ? 0 : inputDir.y);
            _move = nonUpInputDir;
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
            // Debug.Log("Ground: " + IsOnGround);
            Vector2 speed = Vector2.zero;

            speed += _move * groundSpeed;

            if (_jump)
            {
                if (IsOnGround)
                {
                    // apply jump force to rigidbody
                    // Debug.Log("Jumping");
                    speed.y += jumpForce;
                }

                _jump = false;
            }

            // Debug.Log("Speed: " + speed);
            // ToDO currently a very quick multi jump (eg when touching a wall) is possibe.
            _rigidbody2D.velocity = new Vector2(speed.x, _rigidbody2D.velocity.y + speed.y);
            // Debug.Log("Velocity: " + _rigidbody2D.velocity);
        }
    }
}
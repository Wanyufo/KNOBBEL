using Gm;
using UnityEngine;

namespace KNOBBEL.Scripts.Testers
{
    public class CameraFollowTester : MonoBehaviour
    {
        [SerializeField] private float smoothTime = 0.05f;
        private MouseControllerTester _mouse;
        [SerializeField] public float height = 0f;

        private float _z = -10f;

        // Start is called before the first frame update
        void Start()
        {
            _mouse = GM.I.Mouse;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Vector2 currentPos = new Vector2(this.transform.position.x, this.transform.position.y);
            Vector2 targetPos = new Vector2(_mouse.transform.position.x, height);
            // maybe mult. Smooth time by Deltatime to make it framerate independent if smoothdamp doesnt do that already
            Vector2 lerpPos = Vector2.Lerp(currentPos, targetPos, smoothTime);
            this.transform.position = new Vector3(lerpPos.x, lerpPos.y, _z);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KNOBBEL.Scripts.Testers
{
    public class ParallaxTester : MonoBehaviour
    {
        private List<ParallaxObject> _parallaxObjList;
        [SerializeField] private float parallaxFactor = 0.5f;

        private Camera _camera;
        private float _previousFrameCameraXPos = 0;

        // Start is called before the first frame update
        void Start()
        {
            _camera = Camera.main;
            _parallaxObjList = FindObjectsOfType<ParallaxObject>().ToList();
        }

        // Update is called once per frame
        void Update()
        {
            float cameraMovement = _camera.transform.position.x - _previousFrameCameraXPos;
            foreach (ParallaxObject parallaxObject in _parallaxObjList)
            {
                // DO THE PARALLAX EFFECT FOR THIS
                // problem: like this, objects that are in BG and are far away from the center cant be placed nicely....
                // ofc. it could move them out by this much on start. 
                // so they all start at their "logical" place, so their place that they'd have if the cam was right in front of their Privot


                // implement Parallax effect, using the parallaxObject's z position as factor
                Vector3 parallaxMovement = Vector3.zero;
                parallaxMovement.x = cameraMovement * parallaxObject.transform.position.z * parallaxFactor;
                parallaxObject.transform.position += parallaxMovement;
            }

            _previousFrameCameraXPos = _camera.transform.position.x;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KNOBBEL.Scripts.Testers
{
    public class ParallaxTester : MonoBehaviour
    {
        [SerializeField] private bool useDeltaParallax = true;
        private List<(ParallaxObject obj, float xOffset)> _parallaxObjList;
        [SerializeField] private float parallaxFactor = 0.5f;

        private Camera _camera;
        private float _previousFrameCameraXPos = 0;
        private float _initialCameraXPos = 0;

        // Start is called before the first frame update
        void Start()
        {
            _camera = Camera.main;
            _parallaxObjList = new List<(ParallaxObject obj, float xOffset)>();
            var objList = FindObjectsOfType<ParallaxObject>().ToList();
            _parallaxObjList = objList.Zip(objList.Select(obj => obj.transform.position.x), (obj, x) => (obj, x))
                .ToList();
            _initialCameraXPos = _camera.transform.position.x;
            
            // TODO move all the parallax objects
        }

        // Update is called once per frame
        void Update()
        {
            if (useDeltaParallax)
            {
                DeltaParallax();
            }
            else
            {
                AbsoluteParallax();
            }
            _previousFrameCameraXPos = _camera.transform.position.x;
        }

        private void AbsoluteParallax()
        {
            float cameraMovementSinceStart = _camera.transform.position.x - _initialCameraXPos;

            foreach ((ParallaxObject obj, float xOffset) tuple in _parallaxObjList)
            {
                Vector3 parallaxPos = tuple.obj.transform.position;
                parallaxPos.x = cameraMovementSinceStart * tuple.obj.transform.position.z * parallaxFactor + tuple.xOffset;
                tuple.obj.transform.position = parallaxPos;
            }
        }

        private void DeltaParallax()
        {
            float cameraMovement = _camera.transform.position.x - _previousFrameCameraXPos;
            foreach ((ParallaxObject obj, float xOffset) tuple in _parallaxObjList){
                
                Vector3 parallaxMovement = Vector3.zero;
                parallaxMovement.x = cameraMovement * tuple.obj.transform.position.z * parallaxFactor;
                tuple.obj.transform.position += parallaxMovement;
            }

        }
    }
}
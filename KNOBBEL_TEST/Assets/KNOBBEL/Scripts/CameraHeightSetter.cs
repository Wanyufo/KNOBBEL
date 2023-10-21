using System;
using Gm;
using KNOBBEL.Scripts.Testers;
using UnityEngine;

namespace KNOBBEL.Scripts
{
    public class CameraHeightSetter : MonoBehaviour
    {
        [SerializeField] private float height;
        private CameraFollowTester _cameraFollowTester;
        private BoxCollider2D _collider;

        private void Start()
        {
            _collider = GetComponent<BoxCollider2D>();

            CameraFollowTester[] cameraFollowTesters = FindObjectsByType<CameraFollowTester>(FindObjectsSortMode.None);
            _cameraFollowTester = cameraFollowTesters[0];
            if (cameraFollowTesters.Length > 1)
            {
                Debug.LogError("Multiple CameraFollowTesters found, using first one");
            }

            if (_collider == null)
            {
                Debug.LogError("No BoxCollider2D found on this object");
                enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            // check for Player Tag
            if (col.attachedRigidbody.TryGetComponent<MiscTagger>(out MiscTagger tagger))
            {
                if (tagger.GetTag().Equals(TagManager.MiscTags.Player))
                {
                    _cameraFollowTester.height = height;
                }
            }
        }
    }
}
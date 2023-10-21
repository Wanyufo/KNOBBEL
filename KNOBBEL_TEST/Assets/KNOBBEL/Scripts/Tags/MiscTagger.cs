using System;
using UnityEditor;
using UnityEngine;

namespace Gm
{
    public class MiscTagger : MonoBehaviour, ITag
    {
        [SerializeField] public TagManager.MiscTags smartTag;

        private void OnEnable()
        {
            GM.I.TagManager.RegisterTag(smartTag, this);
        }

        private void OnDestroy()
        {
            GM.I.TagManager.DeRegisterTag(smartTag, this);
        }

        public Enum GetTag()
        {
            return smartTag;
        }
    }
}
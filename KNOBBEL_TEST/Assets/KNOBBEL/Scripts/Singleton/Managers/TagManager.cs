using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gm
{
    public class TagManager : MonoBehaviour, IManager
    {
        /// <summary>
        /// Dictionary of all List for each TagType
        /// </summary>
        private Dictionary<string, List<ITag>> Tags { get; } = new Dictionary<string, List<ITag>>();

        public void RegisterTag<TEnum>(TEnum smartTag, ITag iTag) where TEnum : Enum
        {
            string tagGroup = typeof(TEnum).ToString();
            if (!Tags.ContainsKey(tagGroup))
            {
                Tags.Add(tagGroup, new List<ITag>());
            }

            Tags[tagGroup].Add(iTag);
        }

        public void DeRegisterTag<TEnum>(TEnum smartTag, ITag iTag) where TEnum : Enum
        {
            Tags[typeof(TEnum).ToString()].Remove(iTag);
        }

        // ##### Start of Tag Edit Area #####
        // For Instructions on how to add new Tags or TagGroups, please see the TagInstructions.txt


        /// <summary>
        /// Must Contain all the Names of TagGroups
        /// Must be Updated Manually
        /// </summary>
        // public enum TagGroup
        // {
        //     PlaceableObjectTag,
        //     ShaderTag
        // }

        // ### Tag Group Enums ###
        public enum MiscTags
        {
            Player,
            Ground,
            Enemy,
            PlayerGroundCheck,
            DeathZone,
            PlayerClimbCheck
        }

  

        // ##### End of Tag Edit Area #####

        // ##### Tag Related Methods #####


        // returns an empty list if none are registered

        /// <summary>
        /// Get All References to objects that are tagged with a Tag of the same Type as the given smartTag
        /// </summary>
        /// <param name="smartTag">A Tag that is defined as an Enum</param>
        /// <typeparam name="TEnum">Type of the Tag</typeparam>
        /// <returns>A list of References to Tagged objects</returns>
        public List<ITag> GetObjectsWithTagType<TEnum>(TEnum smartTag) where TEnum : Enum
        {
            string tagGroup = typeof(TEnum).ToString();
            return Tags.ContainsKey(tagGroup) ? Tags[tagGroup] : new List<ITag>() { };
        }

        /// <summary>
        /// Get All References to objects that are tagged with the given smartTag
        /// </summary>
        /// <param name="smartTag">A Tag that is defined as an Enum</param>
        /// <typeparam name="TEnum">Type of the Tag</typeparam>
        /// <returns>A list of References to Tagged objects</returns>
        public List<ITag> GetObjectsWithTag<TEnum>(TEnum smartTag) where TEnum : Enum
        {
            return GetObjectsWithTagType(smartTag).FindAll(iTag => iTag.GetTag().Equals(smartTag));
        }

        /// <summary>
        /// Get a single Reference to an object that is tagged with the given smartTag
        /// </summary>
        /// <param name="smartTag">A Tag that is defined as an Enum</param>
        /// <typeparam name="TEnum">Type of the Tag</typeparam>
        /// <returns>One Reference of a tagged Object</returns>
        public ITag GetObjectWithTag<TEnum>(TEnum smartTag) where TEnum : Enum
        {
            return GetObjectsWithTagType(smartTag).Find(iTag => iTag.GetTag().Equals(smartTag));
        }

        // do i need a compareTag? it's just comparing enums...


        private void Start()
        {
        }


        // /// <summary>
        // /// Adds a List to the Dictionary for each TagGroup in enum: TagGroups
        // /// </summary>
        // private void Awake()
        // {
        //     string foundTagGroups = "";
        //
        //     foreach (TagGroup tagType in Enum.GetValues(typeof(TagGroup)))
        //     {
        //         Tags[tagType] = new List<ITag>();
        //         foundTagGroups += tagType + "\n";
        //     }
        //
        //     Debug.Log("Found the following TagGroups\n" + foundTagGroups);
        // }
    }
}
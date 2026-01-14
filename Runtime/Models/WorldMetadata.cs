using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class WorldMetadata
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string createdAt;

        [SerializeField]
        public string name;

        [SerializeField]
        public string description;

        [SerializeField]
        public string updatedAt;

        [SerializeField]
        public string userId;
    }
}

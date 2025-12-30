

using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class StructureData
    {
        public string id;
        public Vector3 size;
        public Vector3 rotation;
        public Vector3 offset;
        public Vector3 position;
        public string objectName;
        public StructureData(string id, string objectName, Vector3 size, Vector3 rotation, Vector3 offset, Vector3 position)
        {
            this.id = id;
            this.objectName = objectName;
            this.size = size;
            this.rotation = rotation;
            this.offset = offset;
            this.position = position;
        }
    }
}
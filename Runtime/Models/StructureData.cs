

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
        public string structureName;

        [NonSerialized]
        public string structureFilepath;
        public StructureData(string id, string structureName, Vector3 size, Vector3 rotation, Vector3 offset, Vector3 position)
        {
            this.id = id;
            this.structureName = structureName;
            this.size = size;
            this.rotation = rotation;
            this.offset = offset;
            this.position = position;
        }

        public override string ToString()
        {
            return $"StructureData: {structureName} (ID: {id}) at Position: {position}, Size: {size}, Rotation: {rotation}, Offset: {offset}, Filepath: {structureFilepath}";
        }
    }
}
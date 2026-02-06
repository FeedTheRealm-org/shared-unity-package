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

        public bool isShop = false;

        public Vector3 colliderSize;
        public Vector3 colliderCenter;

        [NonSerialized]
        public string structureFilepath;

        public StructureData(
            string id,
            string structureName,
            Vector3 size,
            Vector3 rotation,
            Vector3 offset,
            Vector3 position,
            bool isShop = false,
            Vector3 colliderSize = default,
            Vector3 colliderCenter = default
        )
        {
            this.id = id;
            this.structureName = structureName;
            this.size = size;
            this.rotation = rotation;
            this.offset = offset;
            this.position = position;
            this.isShop = isShop;
            this.colliderSize = colliderSize;
            this.colliderCenter = colliderCenter;
        }

        public override string ToString()
        {
            return $"StructureData: {structureName} (ID: {id}) at Position: {position}, Size: {size}, Rotation: {rotation}, Offset: {offset}, ColliderSize: {colliderSize}, ColliderCenter: {colliderCenter}, Filepath: {structureFilepath}, IsShop: {isShop}";
        }
    }
}

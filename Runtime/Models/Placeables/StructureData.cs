using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class StructureData
    {
        public string id;
        public Vector3 size = Vector3.one;
        public Vector3 rotation = Vector3.zero;
        public Vector3 position = Vector3.zero;
        public string fileName = null;
        public string structureName = "New Structure";
        public bool isShop = false;
        public string shopId = null;
        public Vector3 colliderSize = Vector3.zero;
        public Vector3 colliderCenter = Vector3.zero;
        public bool hasCustomCollider = false;

        [NonSerialized]
        public string structureFilepath;

        public StructureData() { }

        public StructureData(
            string id,
            string structureName,
            Vector3 size,
            Vector3 rotation,
            string fileName = null
        )
        {
            this.id = id;
            this.structureName = structureName;
            this.size = size;
            this.rotation = rotation;
            this.fileName = fileName;
            position = Vector3.zero;
            isShop = false;
            colliderSize = Vector3.zero;
            colliderCenter = Vector3.zero;
            shopId = null;

            if (colliderSize == default)
            {
                colliderSize = Vector3.zero;
                hasCustomCollider = false;
            }
            else
            {
                hasCustomCollider = true;
            }
        }

        public StructureData Clone()
        {
            return new StructureData
            {
                id = id,
                size = size,
                rotation = rotation,
                position = position,
                fileName = fileName,
                structureName = structureName,
                isShop = isShop,
                shopId = shopId,
                colliderSize = colliderSize,
                colliderCenter = colliderCenter,
                hasCustomCollider = hasCustomCollider,
            };
        }

        public override string ToString()
        {
            return $"StructureData: {structureName} (ID: {id}) at Position: {position}, Size: {size}, Rotation: {rotation}, IsShop: {isShop}, ShopId: {shopId}, ColliderSize: {colliderSize}, ColliderCenter: {colliderCenter}, HasCustomCollider: {hasCustomCollider}";
        }
    }
}

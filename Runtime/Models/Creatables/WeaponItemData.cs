using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public enum WeaponType
    {
        Ranged,
        Melee,
        None,
    }

    [Serializable]
    public class WeaponItemData : ItemData
    {
        public WeaponType weaponType = WeaponType.None;
        public int damage = 0;
        public float attackSpeed = 0f;
        public float range = 0f;
        public int ammo = 0;

        public WeaponItemData(
            ItemData itemData,
            WeaponType weaponType,
            int damage,
            float attackSpeed,
            float range,
            int ammo
        )
            : base(itemData.id, itemData.name, itemData.description, itemData.spriteFilePath)
        {
            this.weaponType = weaponType;
            this.damage = damage;
            this.attackSpeed = attackSpeed;
            this.range = range;
            this.ammo = ammo;
        }
    }
}

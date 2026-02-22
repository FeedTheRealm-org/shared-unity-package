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
        [SerializeField]
        public WeaponType weaponType = WeaponType.None;

        [SerializeField]
        public int damage = 0;

        [SerializeField]
        public float attackSpeed = 0f;

        [SerializeField]
        public float range = 0f;

        [SerializeField]
        public int ammo = 0;

        public WeaponItemData(
            ItemData itemData,
            WeaponType weaponType,
            int damage,
            float attackSpeed,
            float range,
            int ammo
        )
            : base(itemData.id, itemData.name, itemData.description, itemData.spriteFilepath)
        {
            this.weaponType = weaponType;
            this.damage = damage;
            this.attackSpeed = attackSpeed;
            this.range = range;
            this.ammo = ammo;
        }
    }
}

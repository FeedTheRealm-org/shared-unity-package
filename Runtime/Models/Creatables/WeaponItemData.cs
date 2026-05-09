using System;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public enum WeaponType
    {
        Ranged,
        Melee,
    }

    [Serializable]
    public enum SubWeaponType
    {
        Bow,
        HandHeld,
    }

    public enum ValidSubRangedWeaponType
    {
        Bow,
        HandHeld,
    }

    public enum ValidSubMeleeWeaponType
    {
        HandHeld,
    }

    [Serializable]
    public class WeaponItemData : ItemData
    {
        public WeaponType weaponType = WeaponType.Melee;
        public SubWeaponType subWeaponType = SubWeaponType.HandHeld;
        public int damage = 0;
        public float attackSpeed = 0f;
        public float range = 0f;
        public int ammo = 0;
        public float reloadSpeed = 0f;

        public WeaponItemData(
            ItemData itemData,
            WeaponType weaponType,
            SubWeaponType subWeaponType,
            int damage,
            float attackSpeed,
            float range,
            int ammo,
            float reloadSpeed
        )
            : base(itemData.id, itemData.name, itemData.description, itemData.spriteFilePath)
        {
            this.weaponType = weaponType;
            this.subWeaponType = subWeaponType;
            this.damage = damage;
            this.attackSpeed = attackSpeed;
            this.range = range;
            this.ammo = ammo;
            this.reloadSpeed = reloadSpeed;
        }
    }
}

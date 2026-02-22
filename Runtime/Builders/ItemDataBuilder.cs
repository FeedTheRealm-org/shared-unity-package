using System;
using UnityEngine;

namespace Builders
{
    public class ItemDataBuilder
    {
        private FTRShared.Runtime.Models.ItemData itemData;

        public ItemDataBuilder SetItemData(
            string id,
            string name,
            string description,
            string spriteFilepath
        )
        {
            itemData = new FTRShared.Runtime.Models.ItemData(id, name, description, spriteFilepath);

            return this;
        }

        public FTRShared.Runtime.Models.ConsumableItemData BuildConsumableItem(
            FTRShared.Runtime.Models.EffectType effectType,
            int value,
            float duration,
            float cooldown,
            int maxStack
        )
        {
            return new FTRShared.Runtime.Models.ConsumableItemData(
                itemData,
                effectType,
                value,
                duration,
                cooldown,
                maxStack
            );
        }

        public FTRShared.Runtime.Models.WeaponItemData BuildWeaponItem(
            FTRShared.Runtime.Models.WeaponType weaponType,
            int damage,
            float attackSpeed,
            float range,
            int ammo
        )
        {
            return new FTRShared.Runtime.Models.WeaponItemData(
                itemData,
                weaponType,
                damage,
                attackSpeed,
                range,
                ammo
            );
        }
    }
}

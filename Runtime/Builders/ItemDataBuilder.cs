using System;
using UnityEngine;

namespace Builders
{
    public class ItemDataBuilder
    {
        private Models.ItemData itemData;

        public ItemDataBuilder SetItemData(
            string id,
            string name,
            string description,
            string spriteFilepath
        )
        {
            itemData = new Models.ItemData(id, name, description, spriteFilepath);

            return this;
        }

        public Models.ConsumableItemData BuildConsumableItem(
            Models.EffectType effectType,
            int value,
            float duration,
            float cooldown,
            int maxStack
        )
        {
            return new Models.ConsumableItemData(
                itemData,
                effectType,
                value,
                duration,
                cooldown,
                maxStack
            );
        }

        public Models.WeaponItemData BuildWeaponItem(
            Models.WeaponType weaponType,
            int damage,
            float attackSpeed,
            float range,
            int ammo
        )
        {
            return new Models.WeaponItemData(
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

using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public enum EffectType
    {
        Heal,
        Damage,
        Buff,
        Debuff,
        RestoreMana,
        DrainMana,
        None,
    }

    [Serializable]
    public class ConsumableItemData : ItemData
    {
        [SerializeField]
        public EffectType effectType = EffectType.None;

        [SerializeField]
        public int value = 0;

        [SerializeField]
        public float duration = 0f;

        [SerializeField]
        public float cooldown = 0f;

        [SerializeField]
        public int maxStack = 0;

        public ConsumableItemData(
            ItemData itemData,
            EffectType effectType,
            int value,
            float duration,
            float cooldown,
            int maxStack
        )
            : base(itemData.id, itemData.name, itemData.description, itemData.spriteFilePath)
        {
            this.effectType = effectType;
            this.value = value;
            this.duration = duration;
            this.cooldown = cooldown;
            this.maxStack = maxStack;
        }
    }
}

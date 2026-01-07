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
    public class ConsumableItemData
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string name = "Consumable Item";

        [SerializeField]
        public string description = "";

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

        [SerializeField]
        public string spriteFilepath = "None";

        public ConsumableItemData(
            string id,
            string name,
            string description,
            EffectType effectType,
            int value,
            float duration,
            float cooldown,
            int maxStack,
            string spriteFilepath
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.effectType = effectType;
            this.value = value;
            this.duration = duration;
            this.cooldown = cooldown;
            this.maxStack = maxStack;
            this.spriteFilepath = spriteFilepath;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class ConsumableItem
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public string description;

        [SerializeField]
        public string effectType;

        [SerializeField]
        public int value;

        [SerializeField]
        public float duration;

        [SerializeField]
        public float cooldown;

        [SerializeField]
        public int maxStack;

        [SerializeField]
        public string spriteId;

        public ConsumableItem(
            string name,
            string description,
            string effectType,
            int value,
            float duration,
            float cooldown,
            int maxStack,
            string spriteId
        )
        {
            this.name = name;
            this.description = description;
            this.effectType = effectType;
            this.value = value;
            this.duration = duration;
            this.cooldown = cooldown;
            this.maxStack = maxStack;
            this.spriteId = spriteId;
        }
    }
}

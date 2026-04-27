using System;
using System.Collections.Generic;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public struct StringFloatDictionaryEntry
    {
        public string key;
        public float value;

        public StringFloatDictionaryEntry(string key, float value)
        {
            this.key = key;
            this.value = value;
        }
    }

    internal static class StringFloatDictionarySerialization
    {
        public static List<StringFloatDictionaryEntry> ToEntries(Dictionary<string, float> source)
        {
            var entries = new List<StringFloatDictionaryEntry>();
            if (source == null)
                return entries;

            foreach (var pair in source)
            {
                entries.Add(new StringFloatDictionaryEntry(pair.Key, pair.Value));
            }

            return entries;
        }

        public static Dictionary<string, float> ToDictionary(
            List<StringFloatDictionaryEntry> entries
        )
        {
            var map = new Dictionary<string, float>();
            if (entries == null)
                return map;

            foreach (var entry in entries)
            {
                if (string.IsNullOrEmpty(entry.key))
                    continue;

                map[entry.key] = entry.value;
            }

            return map;
        }
    }
}

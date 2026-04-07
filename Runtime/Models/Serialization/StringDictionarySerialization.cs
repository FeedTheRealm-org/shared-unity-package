using System;
using System.Collections.Generic;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public struct StringDictionaryEntry
    {
        public string key;
        public string value;

        public StringDictionaryEntry(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    internal static class StringDictionarySerialization
    {
        public static List<StringDictionaryEntry> ToEntries(Dictionary<string, string> source)
        {
            var entries = new List<StringDictionaryEntry>();
            if (source == null)
                return entries;

            foreach (var pair in source)
            {
                entries.Add(new StringDictionaryEntry(pair.Key, pair.Value));
            }

            return entries;
        }

        public static Dictionary<string, string> ToDictionary(List<StringDictionaryEntry> entries)
        {
            var map = new Dictionary<string, string>();
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

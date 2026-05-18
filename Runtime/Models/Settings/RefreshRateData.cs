using System;
using UnityEngine;

namespace FTRShared.Runtime.Models.Settings
{
    [Serializable]
    public struct RefreshRateData
    {
        public uint numerator;
        public uint denominator;

        public RefreshRateData(uint num, uint den)
        {
            numerator = num;
            denominator = den;
        }

        public static RefreshRateData FromRefreshRate(RefreshRate rr) =>
            new RefreshRateData(rr.numerator, rr.denominator);

        public RefreshRate ToRefreshRate() =>
            new RefreshRate { numerator = numerator, denominator = denominator };

        public double ToDouble() => denominator == 0 ? 0 : (double)numerator / denominator;
    }
}

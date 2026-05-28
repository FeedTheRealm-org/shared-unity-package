namespace FTRShared.Runtime.Models.Settings
{
    [System.Serializable]
    public class SettingsData
    {
        public float globalVolume = 1f;
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
        public bool isMuted = false;

        public int resolutionWidth = 0;
        public int resolutionHeight = 0;
        public RefreshRateData refreshRate = new RefreshRateData(0, 0);
        public bool isFullscreen = true;

        public bool enableCaching = true;
    }
}

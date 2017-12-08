using System;

namespace Define
{
    public static class SoundParam
    {
        public enum BgmType
        {
            None = 0,
        }
        public enum SeType
        {
            None = 0,
        }
    }

    public static class SoundFunc
    {
        public static string GetBgmFilePath(SoundParam.BgmType type)
        {
            var filePath = "Sounds/";
            return filePath;
        }

        public static string GetEraseSphereSeFileName(int comboCount)
        {
            var fileIndex = Math.Min(comboCount + 1, 6);
            return "se_combo" + fileIndex;
        }
    }
} //Sound

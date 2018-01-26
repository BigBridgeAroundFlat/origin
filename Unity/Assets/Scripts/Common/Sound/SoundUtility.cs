namespace Common.Sound
{
    public static class SoundUtility
    {
        #region param

        public enum BgmType
        {
            None = 0,
        }

        public enum SeType
        {
            None = 0,
        }

        #endregion

        #region func
        
        // BGMファイルパス取得
        public static string GetBgmFilePath(BgmType type)
        {
            var filePath = "Sounds/";

            switch (type)
            {
                case BgmType.None: break;
            }

            return filePath;
        }

        #endregion
    }
}

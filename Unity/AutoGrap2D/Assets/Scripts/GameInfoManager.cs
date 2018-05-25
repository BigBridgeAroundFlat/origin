public static class GameInfoManager
{
    public static bool IsPlayerAiMode = true;  //プレイヤーキャラクターもAIで動作
    public static bool IsSpeedUp = false;  //速度アップ

    #region character type

    public enum CharacterType
    {
        None = 0,

        Kohaku,
        Toko,
    }
    public static CharacterType PlayerSelectCharacterType = CharacterType.Kohaku;
    public static CharacterType EnemySelectCharacterType = CharacterType.Toko;

    public static string CalcCharacterAnimationControllerFilePath(CharacterType type)
    {
        var filePath = "AnimationController/";

        switch (type)
        {
            case CharacterType.Kohaku: filePath += "Animator_Kohaku"; break;
            case CharacterType.Toko: filePath += "Animator_Toko"; break;
        }
        return filePath;
    }

    #endregion

    #region novel

    public class NovelInfo
    {
        public enum NovelType
        {
            None = 0,

            Normal,
            Special,
        }
        public NovelType Type = NovelType.Normal;
        public int No = 1;
        public bool IsSceneView;
    }
    private static NovelInfo _currentNovelInfo = new NovelInfo();
    public static void SetCurrentNovelInfo(NovelInfo info)
    {
        _currentNovelInfo = info;
    }
    public static NovelInfo GetCurrentNovelInfo()
    {
        return _currentNovelInfo;
    }

    #endregion
}
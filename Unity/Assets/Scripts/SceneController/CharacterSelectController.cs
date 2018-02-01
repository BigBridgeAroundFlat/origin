using Common.Scene;
using System;
using System.Linq;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SceneController
{
    public class CharacterSelectController : MonoBehaviour
    {
        [SerializeField] private GameObject _characterButtonParent;

        // Use this for initialization
        void Start ()
        {
            var buttonList = _characterButtonParent.Descendants().OfComponent<Button>().ToList();
            foreach (var button in buttonList)
            {
                var objName = button.gameObject.name;
                button.OnClickEtension(() =>
                {
                    // select character type
                    {
                        var selectCharacterType = (GameInfoManager.CharacterType)Enum.Parse(typeof(GameInfoManager.CharacterType), objName);
                        GameInfoManager.PlayerSelectCharacterType = selectCharacterType;
                        GameInfoManager.EnemySelectCharacterType = selectCharacterType == GameInfoManager.CharacterType.Toko ? GameInfoManager.CharacterType.Kohaku : GameInfoManager.CharacterType.Toko;
                    }

                    // set novel info
                    {
                        var novelInfo = new GameInfoManager.NovelInfo();
                        {
                            novelInfo.Type = GameInfoManager.NovelInfo.NovelType.Normal;
                            novelInfo.No = 1;
                            novelInfo.IsSceneView = false;
                        }
                        GameInfoManager.SetCurrentNovelInfo(novelInfo);
                    }

                    // change scene
                    TransitionSceneManager.Instance.TransitionScene("Novel");
                });
            }
        }

    }
}

using Common.Other;
using Common.Scene;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterSelect
{
    public class CharacterSelectButton : MonoBehaviour
    {
        [SerializeField] private GameInfoManager.CharacterType _characterType = GameInfoManager.CharacterType.None;
        private void Start ()
        {
            GetComponent<Button>().OnClickEtension(() =>
            {
                GameInfoManager.PlayerSelectCharacterType = _characterType;

                if (_characterType == GameInfoManager.CharacterType.Kohaku)
                {
                    GameInfoManager.EnemySelectCharacterType = GameInfoManager.CharacterType.Toko;
                }
                else
                {
                    GameInfoManager.EnemySelectCharacterType = GameInfoManager.CharacterType.Kohaku;
                }

                TransitionSceneManager.Instance.TransitionScene("Battle");
            });
        }
    }
}

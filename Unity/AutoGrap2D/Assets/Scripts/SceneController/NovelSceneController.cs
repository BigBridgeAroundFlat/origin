using System.Collections.Generic;
using Common.Scene;
using System.Linq;
using Battle;
using Unity.Linq;
using UnityEngine;

namespace Novel
{
    public class NovelSceneController : MonoBehaviour
    {
        [SerializeField] private GameObject _flowChartParent;

        // special scene
        [SerializeField] private GameObject _rightBg;
        [SerializeField] private GameObject _animationObjectParent;

        // cash
        private List<ViewerCharacter> _viewerCharacterList;
        private GameInfoManager.NovelInfo _targetNovelInfo;

        private void Start()
        {
            _targetNovelInfo = GameInfoManager.GetCurrentNovelInfo();

            // animation obj
            {
                // 対象取得：最初にParent以下のObject取得
                _viewerCharacterList = _animationObjectParent.Descendants().OfComponent<ViewerCharacter>().ToList();

                if (_targetNovelInfo.Type == GameInfoManager.NovelInfo.NovelType.Special)
                {
                    _rightBg.SetActive(true);
                }
                else
                {
                    _rightBg.SetActive(false);

                    foreach (var viewerCharacter in _viewerCharacterList)
                    {
                        viewerCharacter.gameObject.SetActive(false);
                    }
                }
            }

            // flow chart
            {
                var flowChartName = CalcFlowchartName();
                var targetObj = _flowChartParent.Descendants().FirstOrDefault(x => x.name == flowChartName);
                if (targetObj != null)
                {
                    targetObj.SetActive(true);
                }
            }
        }
        private string CalcFlowchartName()
        {
            var baseName = string.Empty;

            switch (_targetNovelInfo.Type)
            {
                case GameInfoManager.NovelInfo.NovelType.Normal: baseName = "NormalScene"; break;
                case GameInfoManager.NovelInfo.NovelType.Special: baseName = "SpecialScene"; break;
            }

            return baseName + _targetNovelInfo.No;
        }

        public void ChangeBattleScene()
        {
            var sceneName = _targetNovelInfo.IsSceneView ? "SceneSelect" : "Battle";
            TransitionSceneManager.Instance.TransitionScene(sceneName);
        }
        public void ChangeTitleScene()
        {
            var sceneName = _targetNovelInfo.IsSceneView ? "SceneSelect" : "Title";
            TransitionSceneManager.Instance.TransitionScene(sceneName);
        }
    }
}

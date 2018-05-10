using Battle;
using Common.Scene;
using UnityEngine;
using UnityEngine.UI;

namespace SceneController
{
    public class TopController : MonoBehaviour
    {
        [SerializeField] private Button _buttonStage1;
        [SerializeField] private Button _buttonStage2;
        [SerializeField] private Button _buttonStage3;

        private void Start()
        {
            _buttonStage1.OnClickEtension(() =>{ PushStageButton(1); });
            _buttonStage2.OnClickEtension(() => { PushStageButton(2); });
            _buttonStage3.OnClickEtension(() => { PushStageButton(3); });
        }

        private void PushStageButton(int stageId)
        {
            BattleParam.CurrentStageId = stageId;
            TransitionSceneManager.Instance.TransitionScene("Battle");
        }
    }
}

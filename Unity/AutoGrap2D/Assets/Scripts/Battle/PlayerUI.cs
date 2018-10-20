using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class PlayerUI : MonoBehaviour
    {
        private const int SUMMON_POINT_MAX = 1000;
        private int _currentSummonPoint;
        [SerializeField] private GameObject _monsterPrefab;
        [SerializeField] private GameObject _monsterParent;
        [SerializeField] private List<Button> _summonButtonList = new List<Button>();
        public void Awake()
        {
            _currentSummonPoint = SUMMON_POINT_MAX;
            UpdateSummonPointGauge();

            var index = 0;
            foreach (var button in _summonButtonList)
            {
                index++;
                button.OnClickEtension(() =>
                {
                    CreateMonster(index);
                });
            }
        }

        [SerializeField] private List<HpFace> _hpFaceList = new List<HpFace>();
        private HpFace GetHpFaceEmpty()
        {
            foreach (var hpface in _hpFaceList)
            {
                if (hpface.IsUse == false)
                {
                    return hpface;
                }
            }

            return null;
        }

        private List<PlayerMonster> _playerScriptList = new List<PlayerMonster>();
        public List<PlayerMonster> GetPlayerScriptList() { return _playerScriptList; }
        private void CreateMonster(int id)
        {
            if(_currentSummonPoint == 0)
            {
                return;
            }

            var hpFace = GetHpFaceEmpty();
            if (hpFace == null)
            {
                return;
            }
            hpFace.Init();

            var obj = Instantiate(_monsterPrefab);
            {
                obj.transform.SetParent(_monsterParent.transform);
                obj.transform.localPosition = new Vector3(0, 0, 0);
                obj.GetComponent<PlayerMonster>().Init(hpFace);
            }
            _playerScriptList.Add(obj.GetComponent<PlayerMonster>());
            ReduceSummonPoint(300);
        }

        [SerializeField] private Slider _hpSlider;
        private void ReduceSummonPoint(int value)
        {
            _currentSummonPoint = Mathf.Max(_currentSummonPoint - value, 0);
            _hpSlider.value = (float)_currentSummonPoint / SUMMON_POINT_MAX;
        }
        private void UpdateSummonPointGauge()
        {
            _hpSlider.value = (float)_currentSummonPoint / SUMMON_POINT_MAX;
        }

        public void NotifyDeathMonster(GameObject obj)
        {
            var playerScript = obj.GetComponent<PlayerMonster>();
            _playerScriptList.Remove(playerScript);

            var hpFace = playerScript.GetHpFace();
            hpFace.UnenableHpFace();

            Destroy(obj);
        }

        public bool CheckGameOver()
        { 
            return _playerScriptList.IsEmpty() && _currentSummonPoint == 0;
        }
    }
}
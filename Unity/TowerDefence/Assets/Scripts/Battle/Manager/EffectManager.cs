using Common.FrameWork.Singleton;
using UnityEngine;

namespace Battle.Manager
{
    public class EffectManager : OnlyOneBehavior<EffectManager>
    {
        // prefab
        [SerializeField] private GameObject _explodePrefab;
        [SerializeField] private GameObject _kiraPrefab;

        #region create

        // 作成
        private GameObject CalcEffectPrefab(BattleParam.EffectType type)
        {
            GameObject prefab = null;

            switch (type)
            {
                case BattleParam.EffectType.Explode: prefab = _explodePrefab; break;
                case BattleParam.EffectType.Kira: prefab = _kiraPrefab; break;
            }

            return prefab;
        }
        public void CreateEffect(BattleParam.EffectType type, Vector3 pos, float scaleRate = 1.0f)
        {
            var prefab = CalcEffectPrefab(type);
            var obj = Instantiate(prefab, pos, Quaternion.identity);
            {
                obj.SetActive(true);
                obj.transform.SetParent(transform);
                obj.transform.localScale *= scaleRate;
            }
        }

        #endregion

    }
}

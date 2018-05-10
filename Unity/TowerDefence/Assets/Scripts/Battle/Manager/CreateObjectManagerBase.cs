using System.Collections.Generic;
using Battle.CreateObject;
using Common.FrameWork.Singleton;
using UnityEngine;

namespace Battle.Manager
{
    public class CreateObjectManagerBase<T> : OnlyOneBehavior<CreateObjectManagerBase<T>>
    {
        // prefab
        [SerializeField] protected GameObject Prefab;

        // 一覧
        protected readonly List<T> PlayerScriptList = new List<T>();
        protected readonly List<T> EnemyScriptList = new List<T>();
        public List<T> GetScriptList(BattleParam.Affiliation affiliation)
        {
            return affiliation == BattleParam.Affiliation.Player ? PlayerScriptList : EnemyScriptList;
        }

        public virtual void UpdateScriptList()
        {}

        #region create

        // 作成
        public T CreateObject(BattleData.CreateObjectInfo info)
        {
            var obj = InstantiatePrefab(info);
            {
                obj.SetActive(true);
                obj.transform.SetParent(transform);
                obj.GetComponent<ObjectBase>().Init(info);
            }

            // Script追加
            var script = obj.GetComponent<T>();
            AddScriptList(script, info.Affiliation);

            return script;
        }
        protected virtual GameObject InstantiatePrefab(BattleData.CreateObjectInfo info)
        {
            return Instantiate(Prefab, info.Pos, Quaternion.identity);
        }
        private void AddScriptList(T script, BattleParam.Affiliation affiliation)
        {
            var targetScriptList = GetScriptList(affiliation);
            targetScriptList.Add(script);
        }

        // 外部から呼ばれるのでPublic : オブジェクト破棄時にリストから除外
        public void RemoveScriptList(T script, BattleParam.Affiliation affiliation)
        {
            var targetScriptList = GetScriptList(affiliation);
            targetScriptList.Remove(script);
        }

        #endregion

    }
}

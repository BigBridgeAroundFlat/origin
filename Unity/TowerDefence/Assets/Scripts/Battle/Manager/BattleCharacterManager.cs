using Battle.CreateObject;
using UnityEngine;

namespace Battle.Manager
{
    public class BattleCharacterManager : CreateObjectManagerBase<BattleCharacterObject>
    {
        [SerializeField] private GameObject _prefabKohaku;
        [SerializeField] private GameObject _prefabToko;

        protected override GameObject InstantiatePrefab(BattleData.CreateObjectInfo info)
        {
            var battleCharacterType = (BattleParam.BattleCharacterType) info.TypeId;
            switch (battleCharacterType)
            {
                case BattleParam.BattleCharacterType.Kohaku: return Instantiate(_prefabKohaku, info.Pos, Quaternion.identity);
                case BattleParam.BattleCharacterType.Toko: return Instantiate(_prefabToko, info.Pos, Quaternion.identity);
            }

            return base.InstantiatePrefab(info);
        }

        public override void UpdateScriptList()
        {
            foreach (var script in PlayerScriptList)
            {
                script.UpdateAi();
                script.UpdateAbnormalStatus();
            }

            foreach (var script in EnemyScriptList)
            {
                script.UpdateAi();
                script.UpdateAbnormalStatus();
            }
        }
    }
}

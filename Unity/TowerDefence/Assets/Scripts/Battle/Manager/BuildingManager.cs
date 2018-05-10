using Battle.CreateObject;

namespace Battle.Manager
{
    public class BuildingManager : CreateObjectManagerBase<BuildingObject>
    {
        public override void UpdateScriptList()
        {
            foreach (var script in PlayerScriptList)
            {
                script.UpdateAutoAction();
            }

            foreach (var script in EnemyScriptList)
            {
                script.UpdateAutoAction();
            }
        }
    }
}

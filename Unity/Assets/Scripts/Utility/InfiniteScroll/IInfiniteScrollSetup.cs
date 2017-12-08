using UnityEngine;

namespace Utility
{
    public interface IInfiniteScrollSetup
    {
        void OnPostSetupItems();
        void OnUpdateItem(int itemCount, GameObject obj);
    }
}

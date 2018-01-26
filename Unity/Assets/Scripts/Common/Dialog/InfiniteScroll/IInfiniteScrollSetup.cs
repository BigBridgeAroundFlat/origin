using UnityEngine;

namespace Common.Dialog.InfiniteScroll
{
    public interface IInfiniteScrollSetup
    {
        void OnPostSetupItems();
        void OnUpdateItem(int itemCount, GameObject obj);
    }
}

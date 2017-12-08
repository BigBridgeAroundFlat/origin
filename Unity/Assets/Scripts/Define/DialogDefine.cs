
namespace Define
{
    public static class DialogParam
    {
        public enum DialogType
        {
            None = 0,

            OkDialog,
            OkCancelDialog,
        }
    }

    public static class DialogData
    {
        public class DialogInfo
        {
            #region constracta

            public DialogInfo()
            {}

            public DialogInfo(DialogParam.DialogType type, CommonData.VoidCallback okCallback = null)
            {
                DialogType = type;
                OkCallback = okCallback;
            }

            #endregion

            // common
            public DialogParam.DialogType DialogType;
            public CommonData.VoidCallback OkCallback;

            // text
            public string Title;
            public string Message;

            // ok cancel
            public bool UnenableCancelButton;
            public CommonData.VoidCallback CancelCallback;
        }
    }

    public static class DialogFunc
    {
    }
}

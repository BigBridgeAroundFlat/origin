using UnityEngine.UI;

namespace Common.Dialog
{
    public class OkCancelDialog : DialogBase
    {
        // ref
        public Text TitleText;
        public Text MessageText;
        public Button OkButton;
        public Button CancelButton;

        private void Start()
        {
            OkButton.OnClickEtension(PushOkButton);
            CancelButton.OnClickEtension(PushCancelButton);
        }
        public override void PushBackKey()
        {
            PushCancelButton();
        }
        protected override void InitCore()
        {
            UpdateText();
            UpdateButton();
        }
        private void UpdateText()
        {
            if (string.IsNullOrEmpty(DialogInfo.Title))
            {
                TitleText.gameObject.SetActive(false);
            }
            else
            {
                TitleText.text = DialogInfo.Title;
            }

            MessageText.text = DialogInfo.Message;
        }
        private void UpdateButton()
        {
            if (DialogInfo.UnenableCancelButton)
            {
                CancelButton.gameObject.SetActive(false);
            }
        }

        private void PushOkButton()
        {
            if (IsEnableButton() == false)
            {
                return;
            }

            if (DialogInfo.OkCancelButtonCallback != null)
            {
                DialogInfo.OkCancelButtonCallback(true);
            }

            FrameOut();
        }
        private void PushCancelButton()
        {
            if (IsEnableButton() == false)
            {
                return;
            }

            if (DialogInfo.OkCancelButtonCallback != null)
            {
                DialogInfo.OkCancelButtonCallback(false);
            }

            FrameOut();
        }
    }
}

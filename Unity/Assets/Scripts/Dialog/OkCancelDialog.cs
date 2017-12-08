using UnityEngine.UI;

namespace Dialog
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
            if (CancelButton)
            {
                CancelButton.OnClickEtension(PushCancelButton);
            }
        }
        public override void PushBackKey()
        {
        }
        protected override void InitCore()
        {
            UpdateText();
            UpdateButton();
        }
        private void UpdateText()
        {
            if(TitleText)
            {
                if (string.IsNullOrEmpty(DialogInfo.Title))
                {
                    TitleText.gameObject.SetActive(false);
                }
                else
                {
                    TitleText.text = DialogInfo.Title;
                }
            }

            MessageText.text = DialogInfo.Message;
        }
        private void UpdateButton()
        {
/*
            if (DialogInfo.UnenableCancelButton)
            {
                CancelButton.gameObject.SetActive(false);
            }
*/
        }

        private void PushOkButton()
        {
            if (IsEnableButton() == false)
            {
                return;
            }

            if (DialogInfo.OkCallback != null)
            {
                DialogInfo.OkCallback();
            }

            FrameOut();
        }
        private void PushCancelButton()
        {
            if (IsEnableButton() == false)
            {
                return;
            }

            if (DialogInfo.CancelCallback != null)
            {
                DialogInfo.CancelCallback();
            }

            FrameOut();
        }
    }
}

using System;
using Common.FrameWork.Singleton;

namespace Common.FrameWork
{
    public class MessageManager : Singleton<MessageManager>
    {
        public enum MessageType
        {
            None = 0,   
        }

        public string GetMessage(MessageType type)
        {
            var message = String.Empty;
            return message;
        }
    }
}

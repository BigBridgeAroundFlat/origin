namespace Common.FrameWork.Singleton
{
    public class Singleton<T> where T : class, new()
    {
        protected Singleton()
        {
            UnityEngine.Debug.Assert(null == _instance);
        }

        private static readonly T _instance = new T();

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }
    }

}

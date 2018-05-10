namespace Common.Other
{
    public static class SimpleUniqueId
    {
        private static int _uniqueId = 1;

        public static int Id()
        {
            _uniqueId++;
            if (_uniqueId > 10000000)
            {
                _uniqueId = 1;
            }
            return _uniqueId;
        }

    }
}

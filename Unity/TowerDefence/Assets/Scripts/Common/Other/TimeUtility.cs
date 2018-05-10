using System;


namespace Common.Utility
{
	public class TimeUtility
	{
		//----------------------------
		static TimeUtility _instance;
		public static TimeUtility Instance{
			get{
				if(_instance == null){
					_instance = new TimeUtility();
				}
				return _instance;
			}
		}

		//----------------------------

		private readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/*===========================================================================*/
		/**
		 * 現在時刻からUnixTimeを計算する.
		 *
		 * @return UnixTime.
		 */
		public int GetNow()
		{
			return (int)(ToUnixTime(DateTime.UtcNow));
		}
	    public int GetPassUnixTime(int preUnixTime)
	    {
	        return GetNow() - preUnixTime;
	    }

        /*===========================================================================*/
        /**
		 * UnixTimeからDateTimeに変換.
		 *
		 * @param [in] unixTime 変換したいUnixTime.
		 * @return 引数時間のDateTime.
		 */
        public DateTime ToDateTime(int unixTime)
		{
			return UnixEpoch.AddSeconds(unixTime).ToLocalTime();
		}

		/*===========================================================================*/
		/**
		 * 指定時間をUnixTimeに変換する.
		 *
		 * @param [in] dateTime DateTimeオブジェクト.
		 * @return UnixTime.
		 */
		public long ToUnixTime(DateTime dateTime)
		{
			double nowTicks = (dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
			return (int)nowTicks;
		}
	}

}

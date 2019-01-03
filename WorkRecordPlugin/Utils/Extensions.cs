using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Utils
{
	static class Extensions
	{
		public static Guid IntToGuid(this int value)
		{
			byte[] bytes = new byte[16];
			BitConverter.GetBytes(value).CopyTo(bytes, 0);
			return new Guid(bytes);
		}

		public static int GuidToInt(this Guid value)
		{
			byte[] b = value.ToByteArray();
			int bint = BitConverter.ToInt32(b, 0);
			return bint;
		}

		public static Guid LongToGuid(this long value)
		{
			byte[] bytes = new byte[16];
			BitConverter.GetBytes(value).CopyTo(bytes, 0);
			return new Guid(bytes);
		}

		public static Int64 GuidToLong(this Guid value)
		{
			byte[] b = value.ToByteArray();
			long bint = BitConverter.ToInt64(b, 0);
			return bint;
		}
	}
}

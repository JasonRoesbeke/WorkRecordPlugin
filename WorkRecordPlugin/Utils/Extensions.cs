/*******************************************************************************
  * Copyright (C) 2019 AgGateway and ADAPT Contributors
  * Copyright (C) 2019 CNH Industrial N.V.
  * All rights reserved. This program and the accompanying materials
  * are made available under the terms of the Eclipse Public License v1.0
  * which accompanies this distribution, and is available at
  * http://www.eclipse.org/legal/epl-v20.html
  *
  * Contributors:
  *    Jason Roesbeke - Initial version.
  *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;

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

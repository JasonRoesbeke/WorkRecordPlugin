using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.DATA.DTOs
{
	public class BaseDto
	{
		public const string EntityId = "@Id";

		public BaseDto(string ParentPropertyName, params string[] otherDetailProperties)
		{
			JsonDetailProperties = new List<string>();
			JsonDetailProperties.Add(ParentPropertyName);
			if (otherDetailProperties != null)
			{
				JsonDetailProperties.AddRange(otherDetailProperties);
			}
		}

		[JsonIgnore]
		public List<string> JsonDetailProperties { get; }
	}
}

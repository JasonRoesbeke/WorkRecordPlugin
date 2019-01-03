using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs
{
	public class BaseDto
	{
		public const string EntityId = "@Id";

		public BaseDto(string ParentPropertyName = null, params string[] otherDetailProperties)
		{
			JsonDetailProperties = new List<string>();
			if (ParentPropertyName != null || ParentPropertyName != "")
			{
				JsonDetailProperties.Add(ParentPropertyName);
			}
			if (otherDetailProperties != null)
			{
				JsonDetailProperties.AddRange(otherDetailProperties);
			}
		}

		[JsonIgnore]
		public List<string> JsonDetailProperties { get; }
	}
}

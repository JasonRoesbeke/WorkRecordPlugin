using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData
{
	public class LoggedDataDto : BaseDto
	{
		const string Parent = "FieldWorkRecordId";
		public LoggedDataDto() : base(Parent)
		{
			OperationDatas = new List<OperationDataDto>();
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(PropertyName = Parent)]
		[JsonIgnore]
		public Guid FieldWorkRecordId { get; set; }

		public List<OperationDataDto> OperationDatas { get; set; }
	}
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Documents
{
	public class FieldWorkRecordDto : BaseDto
	{
		const string Parent = "Documents";

		public FieldWorkRecordDto() : base(Parent)
		{
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		public string Description { get; set; }
		// ToDo: public FieldLoggedDataDto LoggedData { get; set; }
		public FieldSummaryDto Summary { get; set; }
	}
}

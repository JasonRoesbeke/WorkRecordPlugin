using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class EquipmentConfigurationDto : BaseDto
	{
		const string Parent = "OperationDataId";

		public EquipmentConfigurationDto() : base(Parent)
		{

		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid OperationDataGuid { get; set; }

		public ConnectorDto Connector1 { get; set; }

		public ConnectorDto Connector2 { get; set; }


	}
}

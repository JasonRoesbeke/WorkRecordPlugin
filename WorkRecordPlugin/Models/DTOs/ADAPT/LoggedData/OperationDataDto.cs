using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData
{
	public class OperationDataDto : BaseDto
	{
		const string Parent = "FieldSummaryDtoId";

		public OperationDataDto() : base(Parent)
		{
			EquipmentConfigurations = new List<EquipmentConfigurationDto>();
			WorkingDatas = new Dictionary<int, List<WorkingDataDto>>();
			SpatialRecords = new Dictionary<int, DataTable>();
		}

		[JsonProperty(PropertyName = Parent)]
		[JsonIgnore]
		public Guid FieldSummaryDtoId { get; set; }

		public string Description { get; set; }
		public string OperationType { get; set; }
		public string Product { get; set; }

		public Dictionary<int, DataTable> SpatialRecords { get; set; }
		//public List<DeviceElementUseDto> DeviceElementUses { get; set; }
		public List<EquipmentConfigurationDto> EquipmentConfigurations { get; set; }
		public Dictionary<int, List<WorkingDataDto>> WorkingDatas { get; set; }

	}
}
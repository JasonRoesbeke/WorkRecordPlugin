using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
using WorkRecordPlugin.Models.DTOs.ADAPT.FieldBoundaries;
using WorkRecordPlugin.Models.DTOs.ADAPT.Logistics;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Documents
{
	public class SummaryDto : BaseDto
	{
		const string Parent = "FieldWorkRecordId";

		public SummaryDto() : base(Parent)
		{
			SummaryData = new List<StampedMeteredValuesDto>();
			OperationSummaries = new List<OperationSummaryDto>();
			Users = new List<UserDto>();
			DeviceElements = new List<DeviceElementDto>();
			Notes = new List<string>();
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(PropertyName = Parent)]
		[JsonIgnore]
		public Guid FieldWorkRecordId { get; set; }

		// Company
		public CompanyDto Company { get; set; }
		// Grower/Farm/Field/FieldBoundary
		public GrowerDto Grower { get; set; }

		// ToDo: create UserRoleDto
		[JsonProperty(PropertyName = "Persons")]
		public List<UserDto> Users { get; set; }

		[JsonProperty(PropertyName = "DeviceElements")]
		public List<DeviceElementDto> DeviceElements { get; set; }

		public DateTime? EventDate { get; set; }
		public DateTime? EventEndDate { get; set; }

		public List<string> Notes { get; set; }
		// ToDo: create ProductDto

		public List<StampedMeteredValuesDto> SummaryData { get; set; }
		public List<OperationSummaryDto> OperationSummaries { get; set; }
	}
}

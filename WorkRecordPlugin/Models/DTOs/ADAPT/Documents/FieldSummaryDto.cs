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
	public class FieldSummaryDto : BaseDto
	{
		const string Parent = "FieldWorkRecordId";

		public FieldSummaryDto() : base(Parent)
		{
			SummaryData = new List<StampedMeteredValuesDto>();
			OperationSummaries = new List<OperationSummaryDto>();
			Users = new List<UserDto>();
			Vehicles = new List<VehicleDto>();
			Notes = new List<string>();
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(PropertyName = Parent)]
		[JsonIgnore]
		public Guid FieldWorkRecordId { get; set; }

		// Company/Grower/Farm/Field/FieldBoundary
		public CompanyDto Company { get; set; }
		public GrowerDto Grower { get; set; }
		//public FarmDto Farm { get; set; }
		//public FieldDto Field { get; set; }
		// ToDo: Is a CropZoneDto needed?

		[JsonProperty(PropertyName = "Operators")]
		public List<UserDto> Users { get; set; }

		[JsonProperty(PropertyName = "Machines")]
		public List<VehicleDto> Vehicles { get; set; }

		public DateTime? EventDate { get; set; }
		public DateTime? EventEndDate { get; set; }

		public List<string> Notes { get; set; }
		// ToDo: create ProductDto

		public List<StampedMeteredValuesDto> SummaryData { get; set; }
		public List<OperationSummaryDto> OperationSummaries { get; set; }
	}
}

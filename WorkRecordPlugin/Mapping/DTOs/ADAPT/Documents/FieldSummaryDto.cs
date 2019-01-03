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
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid FieldWorkRecordId { get; set; }

		// Company/Grower/Farm/Field/FieldBoundary
		public CompanyDto Company { get; set; }
		public GrowerDto Grower { get; set; }
		public FarmDto Farm { get; set; }
		public FieldDto Field { get; set; }
		// ToDo: Is a CropZoneDto needed?
		public FieldBoundaryDto FieldBoundary { get; set; }
		// ToDo: create a FieldBoundaryDtoWithGeoJson or a geometry property in FieldBoundaryDto

		[JsonProperty(PropertyName = "Operator")]
		public UserDto User { get; set; }

		[JsonProperty(PropertyName = "Machines")]
		public List<VehicleDto> Vehicle { get; set; }

		public DateTime EventDate { get; set; }
		public DateTime EventEndDate { get; set; }

		public string Notes { get; set; }
		public string OperationType { get; set; }
		// ToDo: create ProductDto
		public string Product { get; set; }

		public List<StampedMeteredValuesDto> SummaryData { get; set; }
	}
}

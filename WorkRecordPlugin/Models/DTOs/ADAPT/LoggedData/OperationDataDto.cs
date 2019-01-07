using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData
{
	public class OperationDataDto : BaseDto
	{
		const string Parent = "FieldSummaryDtoId";

		public OperationDataDto() : base(Parent)
		{
			//SpatialRecords = new List<SpatialRecordDto>();
			SpatialRecords = new DataTable();
		}

		[JsonProperty(PropertyName = Parent)]
		[JsonIgnore]
		public Guid FieldSummaryDtoId { get; set; }

		public string Description { get; set; }
		public string OperationType { get; set; }
		public string Product { get; set; }

		// ToDo: setting the order of fields so that RepresentationsHeader is always the first
		//public RepresentationsHeaderDto RepresentationsHeader { get; set;}
		//public List<SpatialRecordDto> SpatialRecords { get; set; }

		public DataTable SpatialRecords { get; set; }
	}
}
using System.Collections.Generic;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Documents
{
	public class OperationSummaryDto
	{
		public List<StampedMeteredValuesDto> Data { get; set; }
		public string OperationType { get; set; }
		public string Product { get; set; }
	}
}
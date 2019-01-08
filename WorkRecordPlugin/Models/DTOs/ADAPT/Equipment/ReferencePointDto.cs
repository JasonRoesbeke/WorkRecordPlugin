using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class ReferencePointDto
	{
		public NumericRepresentationValueDto XOffset { get; set; }

		public NumericRepresentationValueDto YOffset { get; set; }

		public NumericRepresentationValueDto ZOffset { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Text;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class ImplementConfigurationDto : DeviceElementConfigurationDto
	{
		public NumericRepresentationValueDto Width { get; set; }

		public NumericRepresentationValueDto TrackSpacing { get; set; }

		public NumericRepresentationValueDto PhysicalWidth { get; set; }

		public NumericRepresentationValueDto InGroundTurnRadius { get; set; }

		public NumericRepresentationValueDto ImplementLength { get; set; }

		public NumericRepresentationValueDto VerticalCuttingEdgeZOffset { get; set; }

		public NumericRepresentationValueDto GPSReceiverZOffset { get; set; }

		public NumericRepresentationValueDto YOffset { get; set; }

		public ReferencePointDto ControlPoint { get; set; }

	}
}

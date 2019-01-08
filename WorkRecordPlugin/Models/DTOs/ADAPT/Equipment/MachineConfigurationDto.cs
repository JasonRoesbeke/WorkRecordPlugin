using System;
using System.Collections.Generic;
using System.Text;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class MachineConfigurationDto
	{
		public NumericRepresentationValueDto GpsReceiverXOffset { get; set; }

		public NumericRepresentationValueDto GpsReceiverYOffset { get; set; }

		public NumericRepresentationValueDto GpsReceiverZOffset { get; set; }

		public string OriginAxleLocation { get; set; }

	}
}

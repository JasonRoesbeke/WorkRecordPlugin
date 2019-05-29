using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class EndgunConfigurationDto : DeviceElementConfigurationDto
	{
		public EndgunTableEntryDto NominalValues { get; set; }
		public EndgunTableDto TabularValues { get; set; }
	}
}

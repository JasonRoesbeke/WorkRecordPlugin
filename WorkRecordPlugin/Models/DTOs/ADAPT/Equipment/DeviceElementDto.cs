using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class DeviceElementDto : BaseDto
	{
		const string Parent = "CompanyId";

		// ToDo: invstigate add jsonResolver.IgnoreProperty(typeof(Person), "Title");
		public DeviceElementDto() : base(Parent, "Brand", "Series")
		{
			ChilderenDeviceElements = new List<DeviceElementDto>();
			DeviceElementConfigurations = new List<DeviceElementConfigurationDto>();
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid CompanyGuid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Type { get; set; }

		public string Brand { get; set; }

		public string Series { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Guid? ParentDeviceElementGuid { get; set; }

		public List<DeviceElementDto> ChilderenDeviceElements { get; set; }

		public List<DeviceElementConfigurationDto> DeviceElementConfigurations { get; set; }

		[JsonIgnore]
		public int ReferenceId { get; set; }
	}
}

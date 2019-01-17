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

		public string Manufacturer { get; set; }

		public string Brand { get; set; }

		// ToDo: Voyager2 Plugin is not ADAPT 2.0 so DeviceSeries is not mapped!
		[JsonIgnore]
		public string Series { get; set; }

		[JsonIgnore]
		public Guid? ParentDeviceElementGuid { get; set; }

		public List<DeviceElementDto> ChilderenDeviceElements { get; set; }

		public List<DeviceElementConfigurationDto> DeviceElementConfigurations { get; set; }

		// ToDo: check if ReferenceId prop needs to be moved to BaseDto
		[JsonIgnore]
		public int ReferenceId { get; set; }

		[JsonIgnore]
		public int ParentReferenceId { get; set; }

		[JsonIgnore]
		public bool IsParent { get; set; }
	}
}

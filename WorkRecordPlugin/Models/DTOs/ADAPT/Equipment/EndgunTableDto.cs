using System.Collections.Generic;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class EndgunTableDto
	{
		public EndgunTableDto()
		{
			TableEntries = new List<EndgunTableEntryDto>();
		}
		public List<EndgunTableEntryDto> TableEntries { get; set; }
	}
}
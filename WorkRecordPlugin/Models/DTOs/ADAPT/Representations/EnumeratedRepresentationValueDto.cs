using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Representations
{
	public class EnumeratedRepresentationValueDto : RepresentationValueDto
	{
		public EnumeratedRepresentationDto Representation { get; set; }

		public EnumerationMemberDto Value { get; set; }
	}
}

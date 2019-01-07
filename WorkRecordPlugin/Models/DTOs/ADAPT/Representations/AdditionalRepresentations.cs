using AgGateway.ADAPT.ApplicationDataModel.Representations;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Representations
{
	public static class AdditionalRepresentations
	{
		public static readonly NumericRepresentation vrTimeStamp = new NumericRepresentation()
		{
			Code = "vrTimeStamp",
			CodeSource = RepresentationCodeSourceEnum.User_Defined,
			Description = "TimeStamp"
		};
	}
}

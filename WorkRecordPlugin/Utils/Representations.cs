using AgGateway.ADAPT.ApplicationDataModel.Representations;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Utils
{
	public static class Representations
	{
		public static readonly StringRepresentation srTimeStamp = new StringRepresentation() { Code = "srTimeStamp", CodeSource = RepresentationCodeSourceEnum.User_Defined, Description = "TimeStamp", LongDescription = "TimeStamp" };
	}
}

using AgGateway.ADAPT.ApplicationDataModel.Representations;

namespace WorkRecordPlugin.Utils
{
	public static class Representations
	{
		public static readonly StringRepresentation SrTimeStamp = new StringRepresentation() { Code = "srTimeStamp", CodeSource = RepresentationCodeSourceEnum.User_Defined, Description = "TimeStamp", LongDescription = "TimeStamp" };
	}
}

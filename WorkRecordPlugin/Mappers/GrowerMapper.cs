using AgGateway.ADAPT.ApplicationDataModel.ADM;

namespace WorkRecordPlugin.Mappers
{
	internal class GrowerMapper
	{
		private ApplicationDataModel DataModel;
		private readonly PluginProperties Properties;

		public GrowerMapper(ApplicationDataModel dataModel, PluginProperties properties)
		{
			DataModel = dataModel;
			Properties = properties;
		}


		#region import
		//public Grower ImportOrFind(GrowerDto growerDto)
		//{
		//	Grower grower = Find(growerDto);
		//}

		//private Grower Find(GrowerDto growerDto)
		//{
		//	var uniqueId = UniqueIdMapper.GetUniqueId(growerDto.Guid, Properties.InfoFile);

		//}
		#endregion
	}
}
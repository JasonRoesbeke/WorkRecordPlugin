/*******************************************************************************
  * Copyright (C) 2019 AgGateway and ADAPT Contributors
  * Copyright (C) 2019 CNH Industrial N.V.
  * All rights reserved. This program and the accompanying materials
  * are made available under the terms of the Eclipse Public License v1.0
  * which accompanies this distribution, and is available at
  * http://www.eclipse.org/legal/epl-v20.html
  *
  * Contributors:
  *    Jason Roesbeke - Initial version.
  *******************************************************************************/
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
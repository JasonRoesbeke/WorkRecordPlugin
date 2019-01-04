using System;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Products;

namespace WorkRecordPlugin.Mappers
{
	public class ProductMapper
	{
		private readonly ApplicationDataModel DataModel;

		public ProductMapper(ApplicationDataModel dataModel)
		{
			DataModel = dataModel;
		}

		public string Map(Product product)
		{
			// ToDo: create Full ProductDto!!
			return product.Description;
		}
	}
}
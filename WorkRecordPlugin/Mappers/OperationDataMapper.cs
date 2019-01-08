using System;
using System.Collections.Generic;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Mappers
{
	public class OperationDataMapper
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;

		public OperationDataMapper(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<FieldSummaryProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}

		public OperationDataDto Map(OperationData operationData)
		{
			OperationDataDto operationDataDto = new OperationDataDto();
			operationDataDto.OperationType = operationData.OperationType.ToString();

			var product = DataModel.Catalog.Products.Find(p => p.Id.ReferenceId == operationData.ProductId);
			if (product != null)
			{
				ProductMapper productMapper = new ProductMapper(DataModel);
				operationDataDto.Product = productMapper.Map(product);
			}

			OperationDataProcessor operationDataProcessor = new OperationDataProcessor(DataModel);
			operationDataDto.SpatialRecords = operationDataProcessor.ProcessOperationData(operationData);

			return operationDataDto;
		}
	}
}
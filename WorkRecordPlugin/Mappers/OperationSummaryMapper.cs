using System;
using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;

namespace WorkRecordPlugin.Mappers
{
	internal class OperationSummaryMapper
	{
		private readonly ApplicationDataModel DataModel;

		public OperationSummaryMapper(ApplicationDataModel dataModel)
		{
			DataModel = dataModel;
		}

		public IEnumerable<OperationSummaryDto> Map(Summary summary)
		{
			List<OperationSummaryDto> operationSummaryDtos = new List<OperationSummaryDto>();
			foreach (var operationSummary in summary.OperationSummaries)
			{
				var operationSummaryDto = MapOperationSummary(operationSummary);
				if (operationSummaryDto != null)
				{
					operationSummaryDtos.Add(operationSummaryDto);
				}
			}
			return operationSummaryDtos;
		}

		private OperationSummaryDto MapOperationSummary(OperationSummary operationSummary)
		{
			OperationSummaryDto operationSummaryDto = new OperationSummaryDto();
			operationSummaryDto.OperationType = operationSummary.OperationType.ToString();

			var product = DataModel.Catalog.Products.Find(p => p.Id.ReferenceId == operationSummary.ProductId);
			if (product != null)
			{
				ProductMapper productMapper = new ProductMapper(DataModel);
				operationSummaryDto.Product = productMapper.Map(product);
			}

			if (operationSummary.Data.Count == 0)
			{
				return null;
			}
			StampedMeteredValuesMapper stampedMeteredValuesMapper = new StampedMeteredValuesMapper(DataModel);
			operationSummaryDto.Data = stampedMeteredValuesMapper.Map(operationSummary.Data);

			
			return operationSummaryDto;
		}
	}
}
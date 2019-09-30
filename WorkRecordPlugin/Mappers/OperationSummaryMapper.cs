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
using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using ADAPT.DTOs.Documents;

namespace WorkRecordPlugin.Mappers
{
	internal class OperationSummaryMapper
	{
		private readonly ApplicationDataModel _dataModel;

		public OperationSummaryMapper(ApplicationDataModel dataModel)
		{
			_dataModel = dataModel;
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

			var product = _dataModel.Catalog.Products.Find(p => p.Id.ReferenceId == operationSummary.ProductId);
			if (product != null)
			{
				ProductMapper productMapper = new ProductMapper(_dataModel);
				operationSummaryDto.Product = productMapper.Map(product);
			}

			if (operationSummary.Data.Count == 0)
			{
				return null;
			}
			StampedMeteredValuesMapper stampedMeteredValuesMapper = new StampedMeteredValuesMapper();
			operationSummaryDto.Data = stampedMeteredValuesMapper.Map(operationSummary.Data);

			
			return operationSummaryDto;
		}
	}
}
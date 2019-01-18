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
using System;
using System.Collections.Generic;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
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
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}

		public OperationDataDto Map(OperationData operationData, SummaryDto summaryDto)
		{
			OperationDataDto operationDataDto = new OperationDataDto();
			operationDataDto.OperationType = operationData.OperationType.ToString();

			// Product
			var product = DataModel.Catalog.Products.Find(p => p.Id.ReferenceId == operationData.ProductId);
			if (product != null)
			{
				ProductMapper productMapper = new ProductMapper(DataModel);
				operationDataDto.Product = productMapper.Map(product);
			}

			// SpatialRecords & WorkingDatas
			OperationDataProcessor operationDataProcessor = new OperationDataProcessor(DataModel);
			// ToDo: only process the values of a spatialRecords till the requested maximum depth, this value should be given as a property when using the plugin, '-1' is no limit
			operationDataProcessor.ProcessOperationData(operationData, summaryDto, operationDataDto);

			// EquipmentConfigurations
			var equipmentConfigurationDtos = new List<EquipmentConfigurationDto>();
			foreach (var equipmentConfigId in operationData.EquipmentConfigurationIds)
			{
				var equipmentConfiguration = DataModel.Catalog.EquipmentConfigurations.FirstOrDefault(ec => ec.Id.ReferenceId == equipmentConfigId);
				if (equipmentConfiguration == null)
				{
					// ToDo: when an equipmentConfig is not found in DataModel
					throw new NullReferenceException();
				}

				// Map EquipmentConfig
				EquipmentConfigurationMapper equipmentConfigurationMapper = new EquipmentConfigurationMapper(DataModel);
				operationDataDto.EquipmentConfigurations.Add(equipmentConfigurationMapper.Map(equipmentConfiguration, summaryDto));

			}

			return operationDataDto;
		}
	}
}
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
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;

namespace WorkRecordPlugin.Mappers
{
	public class OperationDataMapper
	{
		private readonly ApplicationDataModel _dataModel;
		private readonly PluginProperties _exportProperties;
		private readonly Utils.SpatialRecordUtils _spatialRecordUtil;

		public OperationDataMapper(ApplicationDataModel dataModel, PluginProperties exportProperties, Utils.SpatialRecordUtils spatialRecordUtil)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			config.CreateMapper();
			_dataModel = dataModel;
			_exportProperties = exportProperties;
			_spatialRecordUtil = spatialRecordUtil;
		}


		public OperationDataDto Map(OperationData operationData, SummaryDto summaryDto)
		{
			OperationDataDto operationDataDto = new OperationDataDto();
			operationDataDto.OperationType = operationData.OperationType.ToString();

			// Products
			ProductMapper productMapper = new ProductMapper(_dataModel);
			foreach (var productId in operationData.ProductIds)
			{
				var product = _dataModel.Catalog.Products.Find(p => p.Id.ReferenceId == productId);
				if (product != null)
				{
					operationDataDto.Product = productMapper.Map(product);
				}
			}

			// SpatialRecords & WorkingDatas
			OperationDataProcessor operationDataProcessor = new OperationDataProcessor(_dataModel, _exportProperties, _spatialRecordUtil);
			// ToDo: only process the values of a spatialRecords till the requested maximum depth, this value should be given as a property when using the plugin, '-1' is no limit
			operationDataProcessor.ProcessOperationData(operationData, summaryDto, operationDataDto);

			// EquipmentConfigurations
			foreach (var equipmentConfigId in operationData.EquipmentConfigurationIds)
			{
				var equipmentConfiguration = _dataModel.Catalog.EquipmentConfigurations.FirstOrDefault(ec => ec.Id.ReferenceId == equipmentConfigId);
				if (equipmentConfiguration == null)
				{
					// ToDo: when an equipmentConfig is not found in DataModel
					throw new NullReferenceException();
				}

				// Map EquipmentConfig
				EquipmentConfigurationMapper equipmentConfigurationMapper = new EquipmentConfigurationMapper(_dataModel, _exportProperties);
				operationDataDto.EquipmentConfigurations.Add(equipmentConfigurationMapper.Map(equipmentConfiguration, summaryDto));

			}

			return operationDataDto;
		}
	}
}
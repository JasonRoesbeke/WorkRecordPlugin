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
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers
{
	public class LoggedDataMapper
	{
		private readonly ApplicationDataModel DataModel;
		private readonly PluginProperties ExportProperties;
		private readonly SpatialRecordUtils SpatialRecordUtil;

		public LoggedDataMapper(ApplicationDataModel dataModel, PluginProperties exportProperties)
		{
			DataModel = dataModel;
			ExportProperties = exportProperties;
			SpatialRecordUtil = new SpatialRecordUtils();

		}

		public LoggedDataDto Map(WorkRecord workRecord, SummaryDto summaryDto)
		{
			LoggedDataDto fieldLoggedDataDto = new LoggedDataDto();
			var LoggedDatas = DataModel.Documents.LoggedData.Where(ld => ld.WorkRecordId == workRecord.Id.ReferenceId);
			if (LoggedDatas.Any())
			{
				foreach (var loggedData in LoggedDatas)
				{
					IEnumerable<OperationDataDto> operationDatas = Map(loggedData, summaryDto);
					if (operationDatas != null || operationDatas.Any())
					{
						fieldLoggedDataDto.OperationDatas.AddRange(operationDatas);
					}
				}
			}
			else // [AgGateway] Needed for ISOXML plugin (v2.0.0, ADAPT 1.2.0)
			{
				foreach (var loggedDataId in workRecord.LoggedDataIds)
				{
					var loggedData = DataModel.Documents.LoggedData.Where(ld => ld.Id.ReferenceId == loggedDataId).FirstOrDefault();
					if (loggedData != null)
					{
						IEnumerable<OperationDataDto> operationDatas = Map(loggedData, summaryDto);
						if (operationDatas != null || operationDatas.Any())
						{
							fieldLoggedDataDto.OperationDatas.AddRange(operationDatas);
						}
					}
				}
			}

			return fieldLoggedDataDto;
		}

		private IEnumerable<OperationDataDto> Map(LoggedData loggedData, SummaryDto summaryDto)
		{
			List<OperationDataDto> operationDataDtos = new List<OperationDataDto>();
			OperationDataMapper operationDataMapper = new OperationDataMapper(DataModel, ExportProperties, SpatialRecordUtil);
			foreach (var operationData in loggedData.OperationData)
			{
				OperationDataDto operationDataDto = operationDataMapper.Map(operationData, summaryDto);
				if (operationDataDto != null)
				{
					operationDataDtos.Add(operationDataDto);
				}
			}
			return operationDataDtos;
		}
	}
}
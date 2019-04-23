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
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers
{
	public class LoggedDataMapper
	{
		private readonly ApplicationDataModel _dataModel;
		private readonly PluginProperties _exportProperties;
		private readonly SpatialRecordUtils _spatialRecordUtil;

		public LoggedDataMapper(ApplicationDataModel dataModel, PluginProperties exportProperties)
		{
			_dataModel = dataModel;
			_exportProperties = exportProperties;
			_spatialRecordUtil = new SpatialRecordUtils();

		}

		public LoggedDataDto Map(WorkRecord workRecord, SummaryDto summaryDto)
		{
			LoggedDataDto fieldLoggedDataDto = new LoggedDataDto();
			var loggedDatas = _dataModel.Documents.LoggedData.Where(ld => ld.WorkRecordId == workRecord.Id.ReferenceId).ToList();
			if (loggedDatas.Any())
			{
				foreach (var loggedData in loggedDatas)
				{
					var operationDatas = Map(loggedData, summaryDto);
					if (operationDatas.Count > 0)
					{
						fieldLoggedDataDto.OperationDatas.AddRange(operationDatas);
					}
				}
			}
			else // [AgGateway] Needed for ISOXML plugin (v2.0.0, ADAPT 1.2.0)
			{
				foreach (var loggedDataId in workRecord.LoggedDataIds)
				{
					var loggedData = _dataModel.Documents.LoggedData.FirstOrDefault(ld => ld.Id.ReferenceId == loggedDataId);
					if (loggedData != null)
					{
						var operationDatas = Map(loggedData, summaryDto);
						if (operationDatas.Count > 0)
						{
							fieldLoggedDataDto.OperationDatas.AddRange(operationDatas);
						}
					}
				}
			}

			return fieldLoggedDataDto;
		}

		private List<OperationDataDto> Map(LoggedData loggedData, SummaryDto summaryDto)
		{
			List<OperationDataDto> operationDataDtos = new List<OperationDataDto>();
			OperationDataMapper operationDataMapper = new OperationDataMapper(_dataModel, _exportProperties, _spatialRecordUtil);
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
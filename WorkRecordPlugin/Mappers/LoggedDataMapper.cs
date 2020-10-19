using System.Collections.Generic;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using ADAPT.DTOs.Documents;
using ADAPT.DTOs.LoggedData;
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
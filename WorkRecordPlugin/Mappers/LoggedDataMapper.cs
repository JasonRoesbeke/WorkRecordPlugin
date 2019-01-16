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

namespace WorkRecordPlugin.Mappers
{
	public class LoggedDataMapper
	{
		public ApplicationDataModel DataModel { get; }

		public LoggedDataMapper(ApplicationDataModel dataModel)
		{
			DataModel = dataModel;
		}

		public LoggedDataDto Map(WorkRecord workRecord, SummaryDto summaryDto)
		{
			LoggedDataDto fieldLoggedDataDto = new LoggedDataDto();
			var LoggedDatas = DataModel.Documents.LoggedData.Where(ld => ld.WorkRecordId == workRecord.Id.ReferenceId);
			foreach (var loggedData in LoggedDatas)
			{
				IEnumerable<OperationDataDto> operationDatas = Map(loggedData, summaryDto);
				if (operationDatas != null || operationDatas.Any())
				{
					fieldLoggedDataDto.OperationDatas.AddRange(operationDatas);
				}
			}

			return fieldLoggedDataDto;
		}

		private IEnumerable<OperationDataDto> Map(LoggedData loggedData, SummaryDto summaryDto)
		{
			List<OperationDataDto> operationDataDtos = new List<OperationDataDto>();
			OperationDataMapper operationDataMapper = new OperationDataMapper(DataModel);
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
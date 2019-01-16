using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;

namespace WorkRecordPlugin.Mappers
{
	class WorkRecordMapper
	{
		public ApplicationDataModel DataModel { get; }

		public WorkRecordMapper(ApplicationDataModel dataModel)
		{
			DataModel = dataModel;
		}

		public WorkRecordDto Map(WorkRecord workRecord)
		{
			WorkRecordDto fieldWorkRecordDto = new WorkRecordDto();

			Guid? recordId = UniqueIdMapper.GetUniqueId(workRecord.Id);
			if (recordId == null)
			{
				return null;
			}

			fieldWorkRecordDto.Guid = (Guid)recordId;

			SummaryMapper fieldSummaryMapper = new SummaryMapper(DataModel);
			var summaryDto = fieldSummaryMapper.Map(workRecord);
			if (summaryDto == null)
			{
				// Do not map further because summary is required
				return null;
			}

			fieldWorkRecordDto.Summary = summaryDto;

			LoggedDataMapper fieldOperationDataMapper = new LoggedDataMapper(DataModel);
			fieldWorkRecordDto.LoggedData = fieldOperationDataMapper.Map(workRecord, summaryDto);

			return fieldWorkRecordDto;
		}

		
	}
}

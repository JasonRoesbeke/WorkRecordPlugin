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
	class FieldWorkRecordMapper
	{
		public ApplicationDataModel DataModel { get; }

		public FieldWorkRecordMapper(ApplicationDataModel dataModel)
		{
			DataModel = dataModel;
		}

		public FieldWorkRecordDto Map(WorkRecord workRecord)
		{
			FieldWorkRecordDto fieldWorkRecordDto = new FieldWorkRecordDto();

			Guid? recordId = UniqueIdMapper.GetUniqueId(workRecord.Id);
			if (recordId == null)
			{
				return null;
			}

			fieldWorkRecordDto.Guid = (Guid)recordId;

			FieldSummaryMapper fieldSummaryMapper = new FieldSummaryMapper(DataModel);
			var summary = fieldSummaryMapper.Map(workRecord);
			if (summary == null)
			{
				// Do not map further because summary is required
				return null;
			}

			fieldWorkRecordDto.Summary = summary;

			return fieldWorkRecordDto;
		}

		
	}
}

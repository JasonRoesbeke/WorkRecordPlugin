using System;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Logistics;

namespace WorkRecordPlugin.Mappers
{
	class FieldSummaryMapper
	{
		private readonly IMapper mapper;

		public FieldSummaryMapper(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<FieldSummaryProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}

		public ApplicationDataModel DataModel { get; }

		public FieldSummaryDto Map(WorkRecord workRecord)
		{
			FieldSummaryDto fieldSummaryDto = SetGFF(workRecord);

			if (fieldSummaryDto == null)
			{
				return null;
			}

			return fieldSummaryDto;
		}

		private FieldSummaryDto SetGFF(WorkRecord workRecord)
		{
			FieldSummaryDto fieldSummaryDto = new FieldSummaryDto();

			if (workRecord.FieldIds.Count != 0)
			{
				return null;
			}

			Field field = DataModel.Catalog.Fields.Find(f => f.Id.ReferenceId == workRecord.FieldIds.First());
			if (field == null)
			{
				return null;
			}
			FieldDto fieldDto = mapper.Map<FieldDto>(field);

			return fieldSummaryDto;
		}
	}
}
using System;
using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Mappers
{
	public class NumericRepresentationValueMapper
	{
		private readonly IMapper mapper;

		// ToDo: check if this class can be made static, no need then for a constructor!
		private readonly ApplicationDataModel DataModel;

		public NumericRepresentationValueMapper(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}

		public List<NumericRepresentationValueDto> Map(List<MeteredValue> values)
		{
			// ToDo: AutoMapper of this List!
			List<NumericRepresentationValueDto> numericRepresentationValueDtos = new List<NumericRepresentationValueDto>();
			foreach (var item in values)
			{
				if (item.Value is NumericRepresentationValue)
				{
					numericRepresentationValueDtos.Add(mapper.Map<NumericRepresentationValue, NumericRepresentationValueDto>((NumericRepresentationValue)item.Value));
				}
			}
			return numericRepresentationValueDtos;
		}
	}
}
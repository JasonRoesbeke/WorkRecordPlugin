using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AutoMapper;
using ADAPT.DTOs.AutoMapperProfiles;
using ADAPT.DTOs.Representations;

namespace WorkRecordPlugin.Mappers
{
	public class NumericRepresentationValueMapper
	{
		private readonly IMapper _mapper;

		public NumericRepresentationValueMapper()
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			_mapper = config.CreateMapper();
		}

		public List<NumericRepresentationValueDto> Map(List<MeteredValue> values)
		{
			// ToDo: AutoMapper of this List!
			List<NumericRepresentationValueDto> numericRepresentationValueDtos = new List<NumericRepresentationValueDto>();
			foreach (var item in values)
			{
				if (item.Value is NumericRepresentationValue)
				{
					numericRepresentationValueDtos.Add(_mapper.Map<NumericRepresentationValue, NumericRepresentationValueDto>((NumericRepresentationValue)item.Value));
				}
			}
			return numericRepresentationValueDtos;
		}
	}
}
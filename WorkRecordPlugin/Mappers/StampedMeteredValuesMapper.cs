using System;
using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;

namespace WorkRecordPlugin.Mappers
{
	internal class StampedMeteredValuesMapper
	{
		private readonly ApplicationDataModel DataModel;

		public StampedMeteredValuesMapper(ApplicationDataModel dataModel)
		{
			DataModel = dataModel;
		}

		public List<StampedMeteredValuesDto> Map(List<StampedMeteredValues> data)
		{
			List<StampedMeteredValuesDto> stampedMeteredValuesDtos = new List<StampedMeteredValuesDto>();

			foreach (var stampedMeterdValue in data)
			{
				var stampedMeteredValuesDto = MapStampedMeterValue(stampedMeterdValue);
				if (stampedMeteredValuesDto != null)
				{
					stampedMeteredValuesDtos.Add(stampedMeteredValuesDto);
				}
			}

			return stampedMeteredValuesDtos;
		}

		private StampedMeteredValuesDto MapStampedMeterValue(StampedMeteredValues stampedMeteredValues)
		{
			StampedMeteredValuesDto stampedMeteredValuesDto = new StampedMeteredValuesDto();

			if (stampedMeteredValues.Values == null)
			{
				return null;
			}

			if (stampedMeteredValues.Values.Count == 0)
			{
				return null;
			}

			if (stampedMeteredValues.Stamp != null)
			{
				if (stampedMeteredValues.Stamp.TimeStamp1 != null)
				{
					stampedMeteredValuesDto.TimeStamp = (DateTime)stampedMeteredValues.Stamp.TimeStamp1;
				}
			}
			

			NumericRepresentationValueMapper numericRepresentationValueMapper = new NumericRepresentationValueMapper(DataModel);
			stampedMeteredValuesDto.Values = numericRepresentationValueMapper.Map(stampedMeteredValues.Values);
			if (stampedMeteredValuesDto.Values == null)
			{
				return null;
			}
			return stampedMeteredValuesDto;
		}
	}
}
using System;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Mappers
{
	public static class NumericValueMapper
	{
		public static NumericValueDto Map(NumericValue value)
		{
			NumericValueDto numericValueDto = new NumericValueDto();
			numericValueDto.UnitOfMeasure = UnitOfMeasureMapper.Map(value.UnitOfMeasure);
			numericValueDto.Value = value.Value;
			return numericValueDto;
		}
	}
}
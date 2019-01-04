using System;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using WorkRecordPlugin.Models.DTOs.ADAPT.Common;

namespace WorkRecordPlugin.Mappers
{
	internal class UnitOfMeasureMapper
	{
		internal static UnitOfMeasureDto Map(UnitOfMeasure unitOfMeasure)
		{
			UnitOfMeasureDto unitOfMeasureDto = new UnitOfMeasureDto();
			unitOfMeasureDto.Code = unitOfMeasure.Code;
			unitOfMeasureDto.Dimension = unitOfMeasure.Dimension.ToString();

			return unitOfMeasureDto;
		}
	}
}
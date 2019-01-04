using System;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Mappers
{
	static class NumericRepresentationMapper
	{
		public static NumericRepresentationDto Map(NumericRepresentation representation)
		{
			// ToDo: use AutoMapper for NumericRepresentationDto!!
			NumericRepresentationDto numericRepresentationDto = new NumericRepresentationDto();
			numericRepresentationDto.Code = representation.Code;
			numericRepresentationDto.CodeSource = representation.CodeSource.ToString();
			numericRepresentationDto.Description = representation.Description;
			return numericRepresentationDto;
		}
	}
}
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.FieldBoundaries;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkRecordPlugin.Models.DTOs.ADAPT.Common;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.FieldBoundaries;
using WorkRecordPlugin.Models.DTOs.ADAPT.Logistics;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles
{
	public class FieldSummaryProfile : Profile
	{
		public FieldSummaryProfile()
		{
			// CompanyAdapt -> Dto
			CreateMap<Company, CompanyDto>()
				;

			// GrowerAdapt -> Dto
			CreateMap<Grower, GrowerDto>()
				;

			// FarmAdapt -> Dto
			CreateMap<Farm, FarmDto>()
				;

			// FieldAdapt -> Dto
			CreateMap<Field, FieldDto>()
				;

			// PersonAdapt -> Dto
			CreateMap<Person, UserDto>()
				;

			// WorkRecordAdapt -> Dto
			CreateMap<WorkRecord, FieldSummaryDto>()
				;

			// OperationSummary -> Dto
			CreateMap<OperationSummary, OperationSummaryDto>()
				.ForMember(dest => dest.OperationType, opt => opt.MapFrom(src => src.OperationType.ToString()))
				;

			// NumericRepresentationValue -> Dto
			CreateMap<NumericRepresentationValue, NumericRepresentationValueDto>()
				;

			// NumericRepresentation -> Dto
			CreateMap<NumericRepresentation, NumericRepresentationDto>()
				.ForMember(dest => dest.CodeSource, opt => opt.MapFrom(src => src.CodeSource.ToString()))
				;

			// NumericValue -> Dto
			CreateMap<NumericValue, NumericValueDto>()
				;

			// UnitOfMeasure -> Dto
			CreateMap<UnitOfMeasure, UnitOfMeasureDto>()
				.ForMember(dest => dest.Dimension, opt => opt.MapFrom(src => src.Dimension.ToString()))
				;

		}
	}
}

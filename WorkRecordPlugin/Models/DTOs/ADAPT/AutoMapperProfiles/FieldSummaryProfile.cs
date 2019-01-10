using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
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
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
using WorkRecordPlugin.Models.DTOs.ADAPT.FieldBoundaries;
using WorkRecordPlugin.Models.DTOs.ADAPT.Logistics;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles
{
	public class FieldSummaryProfile : Profile
	{
		public FieldSummaryProfile()
		{
			// test MapFrom.src.enum.ToString() not needed https://stackoverflow.com/questions/45478928/how-to-give-default-mapping-of-enum-to-string-using-automapper 
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
			CreateMap<WorkRecord, SummaryDto>()
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

			// DeviceElement -> Dto
			CreateMap<DeviceElement, DeviceElementDto>()
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.DeviceElementType.ToString()))
				.ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.Id.ReferenceId))
				;

			// DeviceElementUse -> Dto
			CreateMap<DeviceElementUse, DeviceElementUseDto>()
				;

			// DeviceElementConfiguration -> Dto
			CreateMap<DeviceElementConfiguration, DeviceElementConfigurationDto>()
				;

			// ImplementConfiguration -> Dto
			CreateMap<ImplementConfiguration, ImplementConfigurationDto>()
				;

			// ReferencePoint -> Dto
			CreateMap<ReferencePoint, ReferencePointDto>()
				;

			// MachineConfiguration -> Dto
			CreateMap<MachineConfiguration, MachineConfigurationDto>()
				.ForMember(dest => dest.OriginAxleLocation, opt => opt.MapFrom(src => src.OriginAxleLocation.ToString()))
				;

			// SectionConfiguration -> Dto
			CreateMap<SectionConfiguration, SectionConfigurationDto>()
				;
		}
	}
}

/*******************************************************************************
  * Copyright (C) 2019 AgGateway and ADAPT Contributors
  * Copyright (C) 2019 CNH Industrial N.V.
  * All rights reserved. This program and the accompanying materials
  * are made available under the terms of the Eclipse Public License v1.0
  * which accompanies this distribution, and is available at
  * http://www.eclipse.org/legal/epl-v20.html
  *
  * Contributors:
  *    Jason Roesbeke - Initial version.
  *******************************************************************************/
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.Common;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;
using WorkRecordPlugin.Models.DTOs.ADAPT.Logistics;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles
{
	public class WorkRecordDtoProfile : Profile
	{
		public WorkRecordDtoProfile()
		{
			// test MapFrom.src.enum.ToString() not needed https://stackoverflow.com/questions/45478928/how-to-give-default-mapping-of-enum-to-string-using-automapper 

			// UniqueId -> Dto
			CreateMap<UniqueId, UniqueIdDto>()
				;

			// Company -> Dto
			CreateMap<Company, CompanyDto>()
				;

			// Grower -> Dto
			CreateMap<Grower, GrowerDto>()
				.ReverseMap()
				;

			// Farm -> Dto
			CreateMap<Farm, FarmDto>()
				.ReverseMap()
				;

			// Field -> Dto
			CreateMap<Field, FieldDto>()
				.ReverseMap()
				;

			// Person -> Dto
			CreateMap<Person, UserDto>()
				;

			// WorkRecord -> Dto
			CreateMap<WorkRecord, WorkRecordDto>()
				;

			// Summary -> Dto
			CreateMap<Summary, SummaryDto>()
				;

			// OperationSummary -> Dto
			CreateMap<OperationSummary, OperationSummaryDto>()
				.ForMember(dest => dest.OperationType, opt => opt.MapFrom(src => src.OperationType.ToString()))
				;

			// Representation -> Dto
			CreateMap<Representation, RepresentationDto>()
				.ForMember(dest => dest.CodeSource, opt => opt.MapFrom(src => src.CodeSource.ToString()))
				;

			// NumericRepresentation -> Dto
			CreateMap<NumericRepresentation, NumericRepresentationDto>()
				.IncludeBase<Representation, RepresentationDto>()
				;

			// EnumeratedRepresentation -> Dto
			CreateMap<EnumeratedRepresentation, EnumeratedRepresentationDto>()
				.IncludeBase<Representation, RepresentationDto>()
				;

			// StringRepresentation -> Dto
			CreateMap<StringRepresentation, StringRepresentationDto>()
				.IncludeBase<Representation, RepresentationDto>()
				;

			// RepresentationValue -> Dto
			CreateMap<RepresentationValue, RepresentationValueDto>()
				;

			// NumericRepresentationValue -> Dto
			CreateMap<NumericRepresentationValue, NumericRepresentationValueDto>()
				.IncludeBase<RepresentationValue, RepresentationValueDto>()
				;

			// NumericValue -> Dto
			CreateMap<NumericValue, NumericValueDto>()
				;

			// EnumeratedRepresentationValue -> Dto
			CreateMap<EnumeratedValue, EnumeratedRepresentationValueDto>()
				.IncludeBase<RepresentationValue, RepresentationValueDto>()
				;

			// EnumerationMember -> Dto
			CreateMap<EnumerationMember, EnumerationMemberDto>()
				;

			// UnitOfMeasure -> Dto
			CreateMap<UnitOfMeasure, UnitOfMeasureDto>()
				.ForMember(dest => dest.Dimension, opt => opt.MapFrom(src => src.Dimension.ToString()))
				;

			// WorkingData -> Dto
			CreateMap<WorkingData, WorkingDataDto>()
				.ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.Id.ReferenceId))
				;

			// NumericWorkingData -> Dto
			CreateMap<NumericWorkingData, NumericWorkingDataDto>()
				.IncludeBase<WorkingData, WorkingDataDto>()
				;

			// EnumeratedWorkingData -> Dto
			CreateMap<EnumeratedWorkingData, EnumeratedWorkingDataDto>()
				.IncludeBase<WorkingData, WorkingDataDto>()
				;

			// DeviceModel -> Dto
			CreateMap<DeviceModel, DeviceModelDto>()
				.ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.Id.ReferenceId))
				;

			// DeviceElement -> Dto
			CreateMap<DeviceElement, DeviceElementDto>()
				.ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.Id.ReferenceId))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.DeviceElementType.ToString()))
				;

			// DeviceElementUse -> Dto
			CreateMap<DeviceElementUse, DeviceElementUseDto>()
				;

			// DeviceElementConfiguration -> Dto
			CreateMap<DeviceElementConfiguration, DeviceElementConfigurationDto>()
				.ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.Id.ReferenceId))
				.ForMember(dest => dest.DeviceElementReferenceId, opt => opt.MapFrom(src => src.DeviceElementId))
				;

			// ImplementConfiguration -> Dto
			CreateMap<ImplementConfiguration, ImplementConfigurationDto>()
				.IncludeBase<DeviceElementConfiguration, DeviceElementConfigurationDto>()
				;

			// ReferencePoint -> Dto
			CreateMap<ReferencePoint, ReferencePointDto>()
				;

			// MachineConfiguration -> Dto
			CreateMap<MachineConfiguration, MachineConfigurationDto>()
				.ForMember(dest => dest.OriginAxleLocation, opt => opt.MapFrom(src => src.OriginAxleLocation.ToString()))
				.IncludeBase<DeviceElementConfiguration, DeviceElementConfigurationDto>()
				;

			// SectionConfiguration -> Dto
			CreateMap<SectionConfiguration, SectionConfigurationDto>()
				.IncludeBase<DeviceElementConfiguration, DeviceElementConfigurationDto>()
				;

			// EquipmentConfiguration -> Dto
			CreateMap<EquipmentConfiguration, EquipmentConfigurationDto>()
				.ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.Id.ReferenceId))
				;

			// Connector -> Dto
			CreateMap<Connector, ConnectorDto>()
				;

			// HitchPoint -> Dto
			CreateMap<HitchPoint, HitchPointDto>()
				;

		}
	}
}

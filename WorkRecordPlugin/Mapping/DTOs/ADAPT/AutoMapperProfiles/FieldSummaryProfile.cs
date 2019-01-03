using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.FieldBoundaries;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkRecordPlugin.Models.DTOs.ADAPT.FieldBoundaries;
using WorkRecordPlugin.Models.DTOs.ADAPT.Logistics;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles
{
	public class FieldSummaryProfile : Profile
	{
		public FieldSummaryProfile()
		{
			// CompanyAdapt -> Dto
			CreateMap<Company, CompanyDto>()
				.ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Id.UniqueIds.FirstOrDefault()))
				;

			// GrowerAdapt -> Dto
			CreateMap<Grower, GrowerDto>()
				.ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Id.UniqueIds.FirstOrDefault()))
				;

			// FarmAdapt -> Dto
			CreateMap<Farm, FarmDto>()
				.ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Id.UniqueIds.FirstOrDefault()))
				;

			// FieldAdapt -> Dto
			CreateMap<Field, FieldDto>()
				.ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Id.UniqueIds.FirstOrDefault()))
				;

			// FieldboundaryAdapt -> Dto
			CreateMap<FieldBoundary, FieldBoundaryDto>()
				.ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Id.UniqueIds.FirstOrDefault()))
				;

			// PersonAdapt -> Dto
			CreateMap<Person, UserDto>()
				.ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Id.UniqueIds.FirstOrDefault()))
				;
		}
	}
}

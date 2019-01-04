using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.FieldBoundaries;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
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
		}
	}
}

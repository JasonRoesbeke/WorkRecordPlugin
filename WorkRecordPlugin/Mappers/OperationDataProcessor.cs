using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using AgGateway.ADAPT.Representation.UnitSystem;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Mappers
{
	public class OperationDataProcessor
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;
		private DataTable _dataTable;
		private Dictionary<int, DataTable> _dataTablesPerDepth;

		public OperationDataProcessor(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}

		public void ProcessOperationData(OperationData operationData, SummaryDto summaryDto, OperationDataDto operationDataDto, int maximumDepth = -1)
		{
			// ToDo: [AgGateway] change to public Func<int,IEnumerable<SpatialRecord>> GetSpatialRecords { get; set; } where int is depth
			var spatialRecords = operationData.GetSpatialRecords();

			if (spatialRecords.Any())
			{
				

				// Requested depth of mapping, default is the maximum
				if (maximumDepth <= -1 || maximumDepth > operationData.MaxDepth)
				{
					maximumDepth = operationData.MaxDepth;
				}

				
				// WorkingData per value of depth
				var metersPerDepth = GetMetersPerDepth(operationData, maximumDepth, summaryDto);
				var metersDtoPerDepth = GetWorkingDataDtoPerDepth(metersPerDepth);
				operationDataDto.WorkingDatas = metersDtoPerDepth;

				SpatialRecordMapper spatialRecordMapper = new SpatialRecordMapper(DataModel);
				operationDataDto.SpatialRecords = spatialRecordMapper.Map(spatialRecords, metersPerDepth, maximumDepth, summaryDto, out Dictionary<int, List<WorkingDataDto>> meterDtosPerDepth);
			}
		}

		private Dictionary<int, List<WorkingDataDto>> GetWorkingDataDtoPerDepth(Dictionary<int, List<WorkingData>> metersPerDepth)
		{
			Dictionary<int, List<WorkingDataDto>> metersDtoPerDepth = new Dictionary<int, List<WorkingDataDto>>();
			foreach (var keyValuePair in metersPerDepth)
			{
				var key = keyValuePair.Key;
				var meters = keyValuePair.Value;

				var meterDtos = new List<WorkingDataDto>();

				foreach (var workingData in meters)
				{
					meterDtos.Add(mapper.Map<WorkingData, WorkingDataDto>(workingData));
				}

				metersDtoPerDepth.Add(key, meterDtos);
			}
			return metersDtoPerDepth;
		}

		private Dictionary<int, List<WorkingData>> GetMetersPerDepth(OperationData operationData, int depth, SummaryDto summaryDto)
		{
			Dictionary<int, List<WorkingData>> workingDataWithDepth = new Dictionary<int, List<WorkingData>>();

			for (int i = 0; i <= depth; i++)
			{
				IEnumerable<DeviceElementUse> deviceElementUses = operationData.GetDeviceElementUses(i);

				// Create the list of meters to fill the dataSet.Column
				var allMeters = new List<WorkingData>();

				workingDataWithDepth.Add(i, allMeters);

				// DeviceElements & DeviceElementConfigurations
				foreach (var deviceElementUse in deviceElementUses)
				{
					// ToDo: Check when you use IEnumerable instead of List, provides this then less or more performance when serializing to JSON?
					foreach (var workingData in deviceElementUse.GetWorkingDatas())
					{
						allMeters.Add(workingData);
						var deviceElementConfigurationDto = MapDeviceElementConfiguration(deviceElementUse, summaryDto);
						if (deviceElementConfigurationDto == null)
						{
							// ToDo: handle deviceElementConfigurationDto == null
							throw new NullReferenceException();
						}

						var workdingDataDto = mapper.Map<WorkingData, WorkingDataDto>(workingData);
						workdingDataDto.DeviceElementConfigurationId = deviceElementConfigurationDto.Guid;
						//ToDo: this needs to be given with the SpatialRecord!

					}

					allMeters.AddRange(deviceElementUse.GetWorkingDatas().Where(x => x.Representation != null).ToList());
				}

			}
			return workingDataWithDepth;
		}

		public DeviceElementConfigurationDto MapDeviceElementConfiguration(DeviceElementUse deviceElementUse, SummaryDto summaryDto)
		{
			DeviceElementConfiguration config = DataModel.Catalog.DeviceElementConfigurations.FirstOrDefault(c => c.Id.ReferenceId == deviceElementUse.DeviceConfigurationId);
			if (config == null)
			{
				// ToDo: when DeviceElementConfigurations could not be found in Catalog
				throw new NullReferenceException();
			}

			DeviceElement deviceElement = DataModel.Catalog.DeviceElements.FirstOrDefault(de => de.Id.ReferenceId == config.DeviceElementId);
			if (deviceElement == null)
			{
				// ToDo: when deviceElement could not be found in Catalog
				throw new NullReferenceException();
			}

			// Add Or Find already-mapped DeviceElementDto
			DeviceElementMapper deviceElementMapper = new DeviceElementMapper(DataModel);
			DeviceElementDto deviceElementDto = deviceElementMapper.FindOrMap(deviceElement, summaryDto);
			if (deviceElementDto == null)
			{
				// ToDo: when deviceElementDto could not be found or mapped
				throw new NullReferenceException();
			}

			DeviceElementConfigurationDto deviceElementConfigurationDto = MapDeviceElementConfiguration(config);
			if (deviceElementConfigurationDto == null)
			{
				// ToDo: when deviceElementConfigurationDto could not be found or mapped
				throw new NullReferenceException();
			}

			// Add DeviceElementConfigurationDto
			deviceElementConfigurationDto.DeviceElementGuid = deviceElementDto.Guid;
			deviceElementDto.DeviceElementConfigurations.Add(deviceElementConfigurationDto);

			return deviceElementConfigurationDto;
		}

		private DeviceElementConfigurationDto MapDeviceElementConfiguration(DeviceElementConfiguration config)
		{
			if (config is ImplementConfiguration)
			{
				return mapper.Map<ImplementConfiguration, ImplementConfigurationDto>((ImplementConfiguration)config);
			}
			else if (config is SectionConfiguration)
			{
				return mapper.Map<SectionConfiguration, SectionConfigurationDto>((SectionConfiguration)config);
			}
			else if (config is MachineConfiguration)
			{
				return mapper.Map<MachineConfiguration, MachineConfigurationDto>((MachineConfiguration)config);
			}
			return null;
		}
	}
}
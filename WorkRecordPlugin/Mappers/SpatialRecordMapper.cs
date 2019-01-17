using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using AgGateway.ADAPT.Representation.UnitSystem;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Mappers
{
	public class SpatialRecordMapper
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;
		private Dictionary<int, DataTable> _dataTablesPerDepth;

		public SpatialRecordMapper(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}

		public Dictionary<int, DataTable> Map(IEnumerable<SpatialRecord> spatialRecords, Dictionary<int, List<KeyValuePair<WorkingData, WorkingDataDto>>> metersPerDepth, int maximumDepth, SummaryDto summaryDto)
		{
			Dictionary<int, List<WorkingDataDto>> _meterDtosPerDepth = new Dictionary<int, List<WorkingDataDto>>();
			_dataTablesPerDepth = new Dictionary<int, DataTable>();
			for (int i = 0; i <= maximumDepth; i++)
			{
				DataTable dataTable = new DataTable();

				//Add base columns: time, lat, long & elev
				dataTable.Columns.Add(new DataColumn(GetColumnName(AdditionalRepresentations.vrTimeStamp, i))); //time
				dataTable.Columns.Add(new DataColumn(GetColumnName(RepresentationInstanceList.vrLatitude.ToModelRepresentation(), i, UnitSystemManager.GetUnitOfMeasure("arcdeg")))); //Y
				dataTable.Columns.Add(new DataColumn(GetColumnName(RepresentationInstanceList.vrLongitude.ToModelRepresentation(), i, UnitSystemManager.GetUnitOfMeasure("arcdeg")))); //X
				dataTable.Columns.Add(new DataColumn(GetColumnName(RepresentationInstanceList.vrElevation.ToModelRepresentation(), i, UnitSystemManager.GetUnitOfMeasure("m")))); //Z

				CreateColumns(metersPerDepth[i], dataTable, summaryDto);

				foreach (var spatialRecord in spatialRecords)
				{
					CreateRow(metersPerDepth[i], spatialRecord, i, dataTable);
				}
				_dataTablesPerDepth.Add(i, dataTable);

			}
			return _dataTablesPerDepth;
		}

		private List<WorkingDataDto> CreateColumns(List<KeyValuePair<WorkingData, WorkingDataDto>> workingDatas, DataTable dataTable, SummaryDto summaryDto)
		{
			List<WorkingDataDto> meterDtos = new List<WorkingDataDto>();
			foreach (var workingData in workingDatas)
			{
				try
				{
					dataTable.Columns.Add(workingData.Value.Guid.ToString());
				}
				catch (DuplicateNameException e)
				{
					// ToDo: Handling DuplicateNameException for vrElevation-ADAPT
					;
				}
			}
			return meterDtos;
		}

		private void CreateRow(List<KeyValuePair<WorkingData, WorkingDataDto>> workingDatas, SpatialRecord spatialRecord, int depth, DataTable dataTable)
		{
			var dataRow = dataTable.NewRow();

			// Value
			foreach (var workingData in workingDatas)
			{

				if (workingData.Key as EnumeratedWorkingData != null)
				{
					CreateEnumeratedMeterCell(spatialRecord, workingData.Key, workingData.Value.Guid, dataRow);
				}

				if (workingData.Key as NumericWorkingData != null)
				{
					CreateNumericMeterCell(spatialRecord, workingData.Key, workingData.Value.Guid, dataRow);
				}
			}

			// Location
			if (spatialRecord.Geometry != null)
			{
				// ToDo: [AgGateway] add vrLatitude|vrLongitude|vrElevation to the Visualizer on public github
				// ToDo: [AgGateway] deferentiate between GPS values from gps Device on Parent/MotherDevice and GPS values from a gps device on the deviceElementUse itself
				//Fill in the other cells
				if (spatialRecord.Geometry is Point)
				{
					dataRow[GetColumnName(RepresentationInstanceList.vrLatitude.ToModelRepresentation(), depth, UnitSystemManager.GetUnitOfMeasure("arcdeg"))] = (spatialRecord.Geometry as Point).Y.ToString(CultureInfo.InvariantCulture); //Y
					dataRow[GetColumnName(RepresentationInstanceList.vrLongitude.ToModelRepresentation(), depth, UnitSystemManager.GetUnitOfMeasure("arcdeg"))] = (spatialRecord.Geometry as Point).X.ToString(CultureInfo.InvariantCulture); //X
					if ((spatialRecord.Geometry as Point).Z != null)
					{
						dataRow[GetColumnName(RepresentationInstanceList.vrElevation.ToModelRepresentation(), depth, UnitSystemManager.GetUnitOfMeasure("m"))] = ((double)(spatialRecord.Geometry as Point).Z).ToString(CultureInfo.InvariantCulture); //Z
					}
				}
			}

			// TimeStamp
			if (spatialRecord.Timestamp != null)
			{
				// ToDo: change way how TimeStamp is represented
				dataRow[GetColumnName(AdditionalRepresentations.vrTimeStamp, depth)] = spatialRecord.Timestamp.ToUniversalTime().ToString();
			}

			dataTable.Rows.Add(dataRow);
		}

		private static void CreateEnumeratedMeterCell(SpatialRecord spatialRecord, WorkingData workingData, Guid workingDataDtoGuid, DataRow dataRow)
		{
			var enumeratedValue = spatialRecord.GetMeterValue(workingData) as EnumeratedValue;
			var value = enumeratedValue != null && enumeratedValue.Value != null
				? enumeratedValue.Value.Value
				: "";

			dataRow[workingDataDtoGuid.ToString()] = value;
		}

		private static void CreateNumericMeterCell(SpatialRecord spatialRecord, WorkingData workingData, Guid workingDataDtoGuid, DataRow dataRow)
		{
			var numericRepresentationValue = spatialRecord.GetMeterValue(workingData) as NumericRepresentationValue;
			var value = numericRepresentationValue != null
				? numericRepresentationValue.Value.Value.ToString(CultureInfo.InvariantCulture)
				: "";

			dataRow[workingDataDtoGuid.ToString()] = value;
		}


		private static string GetColumnName(AgGateway.ADAPT.ApplicationDataModel.Representations.Representation representation, int depth, AgGateway.ADAPT.ApplicationDataModel.Common.UnitOfMeasure uom = null)
		{
			if (uom != null)
			{
				return $"{representation.Code};{representation.CodeSource.ToString()};{depth};{uom.Code}";
			}
			return $"{representation.Code};{representation.CodeSource.ToString()};{depth}";
		}
	}
}
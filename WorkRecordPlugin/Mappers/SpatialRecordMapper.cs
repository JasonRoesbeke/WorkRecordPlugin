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
using CoordinateSharp;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Common;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers
{
	public class SpatialRecordMapper
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;
		private readonly ExportProperties ExportProperties;
		private readonly SpatialRecordUtils SpatialRecordUtil;
		private Dictionary<int, DataTable> _dataTablesPerDepth;

		public SpatialRecordMapper(ApplicationDataModel dataModel, ExportProperties exportProperties, SpatialRecordUtils spatialRecordUtil)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
			ExportProperties = exportProperties;
			SpatialRecordUtil = spatialRecordUtil;

			// Initialise the representations of Base values of a Spatial Record (TimeStamp, Lat, Long, Elev)

		}

		public Dictionary<int, DataTable> Map(IEnumerable<SpatialRecord> spatialRecords, Dictionary<int, List<KeyValuePair<WorkingData, WorkingDataDto>>> metersPerDepth, int maximumDepth, SummaryDto summaryDto)
		{
			Dictionary<int, List<WorkingDataDto>> _meterDtosPerDepth = new Dictionary<int, List<WorkingDataDto>>();
			_dataTablesPerDepth = new Dictionary<int, DataTable>();

			for (int i = 0; i <= maximumDepth; i++)
			{
				DataTable dataTable = new DataTable();

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
					CreateEnumeratedMeterCell(spatialRecord, workingData.Key, workingData.Value, dataRow);
				}

				if (workingData.Key as NumericWorkingData != null)
				{
					CreateNumericMeterCell(spatialRecord, workingData.Key, workingData.Value, dataRow);
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
					var point = (Point)spatialRecord.Geometry;
					var latitude = point.Y;
					var longitude = point.X;
					var elevation = point.Z;

					if (ExportProperties.Anonymized)
					{
						// Anonymize spatial records by moving the lat/long coordinates
						var movedPoint = AnonymizeUtils.MovePoint(point, ExportProperties.RandomDistance, ExportProperties.RandomBearing);
						latitude = movedPoint.Y;
						longitude = movedPoint.X;
					}

					dataRow[SpatialRecordUtil.Latitude.Value.Guid.ToString()] = latitude.ToString(CultureInfo.InvariantCulture); //Y
					dataRow[SpatialRecordUtil.Longitude.Value.Guid.ToString()] = longitude.ToString(CultureInfo.InvariantCulture); //X
					if (elevation != null)
					{
						dataRow[SpatialRecordUtil.Elevation.Value.Guid.ToString()] = ((double)elevation).ToString(CultureInfo.InvariantCulture); //Z
					}
				}
			}

			// TimeStamp
			if (spatialRecord.Timestamp != null)
			{
				// ToDo: change way how TimeStamp is represented
				dataRow[SpatialRecordUtil.TimeStamp.Value.Guid.ToString()] = spatialRecord.Timestamp.ToUniversalTime().ToString();
			}

			dataTable.Rows.Add(dataRow);
		}

		private void CreateEnumeratedMeterCell(SpatialRecord spatialRecord, WorkingData workingData, WorkingDataDto workingDataDto, DataRow dataRow)
		{
			var enumeratedValue = spatialRecord.GetMeterValue(workingData) as EnumeratedValue;
			var value = enumeratedValue != null && enumeratedValue.Value != null
				? enumeratedValue.Value.Value
				: "";


			//if (workingDataDto.Representation != null)
			//{
			//	if (workingDataDto.Representation is EnumeratedRepresentationDto)
			//	{
			//	}
			//}

			dataRow[workingDataDto.Guid.ToString()] = value;
		}

		private void CreateNumericMeterCell(SpatialRecord spatialRecord, WorkingData workingData, WorkingDataDto workingDataDto, DataRow dataRow)
		{
			var numericRepresentationValue = spatialRecord.GetMeterValue(workingData) as NumericRepresentationValue;
			var value = numericRepresentationValue != null
				? numericRepresentationValue.Value.Value.ToString(CultureInfo.InvariantCulture)
				: "";

			//if (workingDataDto.Representation != null)
			//{
			//	if (workingDataDto.Representation is NumericRepresentationDto)
			//	{
			//		//if(((NumericRepresentationDto)workingDataDto.Representation).UnitOfMeasure == null)
			//		{
			//			//((NumericRepresentationDto)workingDataDto.Representation).UnitOfMeasure = mapper.Map<AgGateway.ADAPT.ApplicationDataModel.Common.UnitOfMeasure, UnitOfMeasureDto>(numericRepresentationValue.Value.UnitOfMeasure);
			//		}
			//	}
			//}
			dataRow[workingDataDto.Guid.ToString()] = value;
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
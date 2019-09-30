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
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using AutoMapper;
using NetTopologySuite.Geometries;
using ADAPT.DTOs.AutoMapperProfiles;
using ADAPT.DTOs.Documents;
using ADAPT.DTOs.LoggedData;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers
{
	public class SpatialRecordMapper
	{
		private readonly PluginProperties _exportProperties;
		private readonly SpatialRecordUtils _spatialRecordUtil;
		private Dictionary<int, DataTable> _dataTablesPerDepth;

		public SpatialRecordMapper(PluginProperties exportProperties, SpatialRecordUtils spatialRecordUtil)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			config.CreateMapper();
			_exportProperties = exportProperties;
			_spatialRecordUtil = spatialRecordUtil;

			// Initialise the representations of Base values of a Spatial Record (TimeStamp, Lat, Long, Elev)

		}

		public Dictionary<int, DataTable> Map(List<SpatialRecord> spatialRecords, Dictionary<int, List<KeyValuePair<WorkingData, WorkingDataDto>>> metersPerDepth, int maximumDepth, SummaryDto summaryDto)
		{
			_dataTablesPerDepth = new Dictionary<int, DataTable>();

			for (int i = 0; i <= maximumDepth; i++)
			{
				DataTable dataTable = new DataTable();

				CreateColumns(metersPerDepth[i], dataTable);

				foreach (var spatialRecord in spatialRecords)
				{
					CreateRow(metersPerDepth[i], spatialRecord, dataTable);
				}
				_dataTablesPerDepth.Add(i, dataTable);

			}
			return _dataTablesPerDepth;
		}

		private void CreateColumns(List<KeyValuePair<WorkingData, WorkingDataDto>> workingDatas, DataTable dataTable)
		{
			foreach (var workingData in workingDatas)
			{
				try
				{
					dataTable.Columns.Add(workingData.Value.Guid.ToString());
				}
				catch (DuplicateNameException)
				{
					// ToDo: Handling DuplicateNameException for vrElevation-ADAPT
				}
			}
		}

		private void CreateRow(List<KeyValuePair<WorkingData, WorkingDataDto>> workingDatas, SpatialRecord spatialRecord, DataTable dataTable)
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
				if (spatialRecord.Geometry is AgGateway.ADAPT.ApplicationDataModel.Shapes.Point)
				{
					var point = (AgGateway.ADAPT.ApplicationDataModel.Shapes.Point)spatialRecord.Geometry;
					var latitude = point.Y;
					var longitude = point.X;
					var elevation = point.Z;

					if (_exportProperties.Anonymise)
					{
						// Anonymize spatial records by moving the lat/long coordinates
						Coordinate movedPoint = new Coordinate(); ;
						_exportProperties.AffineTransformation.Transform(new Coordinate(point.X, point.Y), movedPoint);
						latitude = movedPoint.Y;
						longitude = movedPoint.X;
					}

					dataRow[_spatialRecordUtil.Latitude.Value.Guid.ToString()] = latitude.ToString(CultureInfo.InvariantCulture); //Y
					dataRow[_spatialRecordUtil.Longitude.Value.Guid.ToString()] = longitude.ToString(CultureInfo.InvariantCulture); //X
					if (elevation != null)
					{
						dataRow[_spatialRecordUtil.Elevation.Value.Guid.ToString()] = ((double)elevation).ToString(CultureInfo.InvariantCulture); //Z
					}
				}
			}

			// TimeStamp
			// ToDo: change way how TimeStamp is represented
			dataRow[_spatialRecordUtil.TimeStamp.Value.Guid.ToString()] = spatialRecord.Timestamp.ToString("O", CultureInfo.InvariantCulture);

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
	}
}
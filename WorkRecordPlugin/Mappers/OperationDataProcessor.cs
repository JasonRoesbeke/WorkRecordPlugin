using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using AgGateway.ADAPT.Representation.UnitSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Mappers
{
	public class OperationDataProcessor
	{
		private readonly ApplicationDataModel DataModel;
		private DataTable _dataTable;

		public OperationDataProcessor(ApplicationDataModel dataModel)
		{
			DataModel = dataModel;
		}

		public DataTable ProcessOperationData(OperationData operationData)
		{
			_dataTable = new DataTable();

			//Add extra columns
			_dataTable.Columns.Add(new DataColumn(GetColumnName(AdditionalRepresentations.vrTimeStamp, 0))); //time
			_dataTable.Columns.Add(new DataColumn(GetColumnName(RepresentationInstanceList.vrLatitude.ToModelRepresentation(), 0, UnitSystemManager.GetUnitOfMeasure("arcdeg")))); //Y
			_dataTable.Columns.Add(new DataColumn(GetColumnName(RepresentationInstanceList.vrLongitude.ToModelRepresentation(), 0, UnitSystemManager.GetUnitOfMeasure("arcdeg")))); //X
			_dataTable.Columns.Add(new DataColumn(GetColumnName(RepresentationInstanceList.vrElevation.ToModelRepresentation(), 0, UnitSystemManager.GetUnitOfMeasure("m")))); //Z

			var spatialRecords = operationData.GetSpatialRecords();

			if (spatialRecords.Any())
			{
				var meters = GetWorkingData(operationData);

				CreateColumns(meters);

				foreach (var spatialRecord in spatialRecords)
				{
					CreateRow(meters, spatialRecord);
				}

				UpdateColumnNamesWithUom(meters, spatialRecords);
			}

			return _dataTable;
		}

		private static Dictionary<int, IEnumerable<WorkingData>> GetWorkingData(OperationData operationData)
		{
			var workingDataWithDepth = new Dictionary<int, IEnumerable<WorkingData>>();

			for (var i = 0; i <= operationData.MaxDepth; i++)
			{
				var meters = operationData.GetDeviceElementUses(i).SelectMany(x => x.GetWorkingDatas()).Where(x => x.Representation != null);

				workingDataWithDepth.Add(i, meters);
			}
			return workingDataWithDepth;
		}
		
		private void CreateColumns(Dictionary<int, IEnumerable<WorkingData>> workingDataDictionary)
		{
			foreach (var kvp in workingDataDictionary)
			{
				foreach (var workingData in kvp.Value)
				{
					try
					{
						_dataTable.Columns.Add(GetColumnName(workingData, kvp.Key));
					}
					catch (DuplicateNameException e)
					{
						// ToDo: Handling DuplicateNameException for vrElevation-ADAPT
						;
					}
				}
			}
		}

		private void CreateRow(Dictionary<int, IEnumerable<WorkingData>> workingDataDictionary, SpatialRecord spatialRecord)
		{
			var dataRow = _dataTable.NewRow();

			foreach (var key in workingDataDictionary.Keys)
			{
				var depth = key;

				foreach (var workingData in workingDataDictionary[key])
				{
					if (workingData as NumericWorkingData != null)
						CreateNumericMeterCell(spatialRecord, workingData, depth, dataRow);

					if (workingData as EnumeratedWorkingData != null)
						CreateEnumeratedMeterCell(spatialRecord, workingData, depth, dataRow);
				}
			}

			if (spatialRecord.Geometry != null)
			{
				// ToDo: [AgGateway] add vrLatitude|vrLongitude|vrElevation to the Visualizer on public github
				//Fill in the other cells
				if (spatialRecord.Geometry is Point)
				{
					dataRow[GetColumnName(RepresentationInstanceList.vrLatitude.ToModelRepresentation(), 0, UnitSystemManager.GetUnitOfMeasure("arcdeg"))] = (spatialRecord.Geometry as Point).Y.ToString(); //Y
					dataRow[GetColumnName(RepresentationInstanceList.vrLongitude.ToModelRepresentation(), 0, UnitSystemManager.GetUnitOfMeasure("arcdeg"))] = (spatialRecord.Geometry as Point).X.ToString(); //X
					dataRow[GetColumnName(RepresentationInstanceList.vrElevation.ToModelRepresentation(), 0, UnitSystemManager.GetUnitOfMeasure("m"))] = (spatialRecord.Geometry as Point).Z.ToString(); //Z
				}
			}
			if (spatialRecord.Timestamp != null)
			{
				dataRow[GetColumnName(AdditionalRepresentations.vrTimeStamp, 0)] = spatialRecord.Timestamp.ToString();
			}

			_dataTable.Rows.Add(dataRow);
		}

		private static void CreateEnumeratedMeterCell(SpatialRecord spatialRecord, WorkingData workingData, int depth, DataRow dataRow)
		{
			var enumeratedValue = spatialRecord.GetMeterValue(workingData) as EnumeratedValue;

			dataRow[GetColumnName(workingData, depth)] = enumeratedValue != null && enumeratedValue.Value != null ? enumeratedValue.Value.Value : "";
		}

		private static void CreateNumericMeterCell(SpatialRecord spatialRecord, WorkingData workingData, int depth, DataRow dataRow)
		{
			var numericRepresentationValue = spatialRecord.GetMeterValue(workingData) as NumericRepresentationValue;
			var value = numericRepresentationValue != null
				? numericRepresentationValue.Value.Value.ToString(CultureInfo.InvariantCulture)
				: "";

			dataRow[GetColumnName(workingData, depth)] = value;
		}

		private void UpdateColumnNamesWithUom(Dictionary<int, IEnumerable<WorkingData>> workingDatas, IEnumerable<SpatialRecord> spatialRecords)
		{
			foreach (var kvp in workingDatas)
			{
				foreach (var data in kvp.Value)
				{
					var data1 = data;
					var workingDataValues = spatialRecords.Select(x => x.GetMeterValue(data1) as NumericRepresentationValue);
					var numericRepresentationValues = workingDataValues.Where(x => x != null);
					var uoms = numericRepresentationValues.Select(x => x.Value.UnitOfMeasure).ToList();

					if (uoms.Any())
					{
						try
						{
							_dataTable.Columns[GetColumnName(data, kvp.Key)].ColumnName += ";" + uoms.First().Code;
						}
						catch (DuplicateNameException e)
						{
							// ToDo: Handling DuplicateNameException for vrElevation;ADAPT;m
							;
						}
					}
				}
			}
		}

		private static string GetColumnName(WorkingData workingData, int depth, AgGateway.ADAPT.ApplicationDataModel.Common.UnitOfMeasure uom = null)
		{
			return GetColumnName(workingData.Representation, depth, uom);
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
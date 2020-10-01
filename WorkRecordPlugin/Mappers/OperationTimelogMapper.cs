/*******************************************************************************
  * Copyright (C) 2019 AgGateway and ADAPT Contributors
  * Copyright (C) 2019 CNH Industrial N.V.
  * All rights reserved. This program and the accompanying materials
  * are made available under the terms of the Eclipse Public License v1.0
  * which accompanies this distribution, and is available at
  * http://www.eclipse.org/legal/epl-v20.html
  *
  * Contributors:
  *    Inge La Riviere - Initial version, based on LoggedDataMapper.
  *******************************************************************************/
using System.Collections.Generic;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using GeoJSON.Net.Feature;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using WorkRecordPlugin.Mappers.GeoJson;
using AgGateway.ADAPT.ApplicationDataModel.Representations;

namespace WorkRecordPlugin.Mappers
{
    public class OperationTimelogMapper
	{
		private readonly ApplicationDataModel _dataModel;
		private readonly PluginProperties _properties;

		public OperationTimelogMapper(PluginProperties properties, ApplicationDataModel dataModel = null)
		{
			_dataModel = dataModel;
			_properties = properties;
		}

		private List<DeviceElementUse> GetAllSections(OperationData operationData)
		{
			var deviceElementUses = new List<DeviceElementUse>();

			if (operationData.GetDeviceElementUses != null)
			{
				int maximumDepth = 0;	// @ToDo ILaR: operationData.MaxDepth;
				
				if (_properties.MaximumMappingDepth != null)
				{
					if (_properties.MaximumMappingDepth >= -1 || _properties.MaximumMappingDepth <= operationData.MaxDepth)
						maximumDepth = (int)_properties.MaximumMappingDepth;
				}

				for (var i = 0; i <= maximumDepth; i++)
				{
					var dvcElemUses = operationData.GetDeviceElementUses(i);
					if (dvcElemUses != null)
						deviceElementUses.AddRange(dvcElemUses);
				}
			}

			return deviceElementUses;
		}

	public List<Feature> MapMultiple(OperationData operation, IEnumerable<SpatialRecord> spatialRecords)
		{
			List<DeviceElementUse> deviceElementUses = GetAllSections(operation);
			List<WorkingData> workingDatas = deviceElementUses.SelectMany(x => x.GetWorkingDatas()).ToList();   // meters

			// inspired by ISOv4Plugin/Mappers/TimeLogMapper
			List<Feature> features = new List<Feature>();

			foreach (SpatialRecord spatialRecord in spatialRecords)
			{
				if (spatialRecord.Geometry != null && spatialRecord.Geometry as Point != null)
				{
					Dictionary<string, object> properties = new Dictionary<string, object>();
						
					Point location = spatialRecord.Geometry as Point;
					if (location.X == 0 || location.Y == 0)
						continue;

					// altitude
					if (location.Z != null)
					{
						properties.Add("Elevation", location.Z);
					}

					// timeStamp
					properties.Add("Timestamp", spatialRecord.Timestamp.ToString());
						
					// meter values
					var workingDatasWithValues = workingDatas.Where(x => spatialRecord.GetMeterValue(x) != null);
					foreach (WorkingData workingData in workingDatasWithValues)		//.Where(d => _dlvOrdersByWorkingDataID.ContainsKey(d.Id.ReferenceId)))
					{
						string key = workingData.Representation.Code;
						// DDI: hex2int
						if (workingData.Representation.CodeSource == RepresentationCodeSourceEnum.ISO11783_DDI)
                        {
							key = "DDI_" + int.Parse(key, System.Globalization.NumberStyles.HexNumber).ToString();
						}
						object value = null;
						string uom = null;

						if (workingData is EnumeratedWorkingData)
						{
							EnumeratedWorkingData enumeratedMeter = workingData as EnumeratedWorkingData;
							if (enumeratedMeter != null && spatialRecord.GetMeterValue(enumeratedMeter) != null)
							{
								EnumeratedValue enumValue = (spatialRecord.GetMeterValue(enumeratedMeter) as EnumeratedValue);
								value = enumValue.Value.Value.ToString();
							}
						}
						else if (workingData is NumericWorkingData)
						{
							NumericWorkingData numericMeter = workingData as NumericWorkingData;
							if (numericMeter != null && spatialRecord.GetMeterValue(numericMeter) != null)
							{
								NumericRepresentationValue numValue = spatialRecord.GetMeterValue(numericMeter) as NumericRepresentationValue;
								value = numValue.Value.Value;
								uom = numValue.Value.UnitOfMeasure.Code;
							}
						}
                        else // needed ?
                        {
							value = spatialRecord.GetMeterValue(workingData);
						}

						if (value != null)
						{
							properties.Add(key, value);

							if (uom != null)
								properties.Add(key + "_Uom", uom);
						}
					}
					// add to FC
					features.Add(new Feature(PointMapper.MapPoint2Point(spatialRecord.Geometry as Point, _properties.AffineTransformation), properties));
				}

			}
			
			return features;
		}
		internal static string GetPrefix()
		{
			return "OperationTimelog";
		}
	}
}
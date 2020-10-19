using System.Collections.Generic;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using GeoJSON.Net.Feature;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using WorkRecordPlugin.Mappers.GeoJson;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AgGateway.ADAPT.Representation.RepresentationSystem;

namespace WorkRecordPlugin.Mappers
{
    public class OperationTimelogMapper
	{
		private readonly ApplicationDataModel _dataModel;
		private readonly PluginProperties _properties;
		//ILaR: temporary fix, should come from ddiExport.txt
		private Dictionary<int, string> _missingDDI = new Dictionary<int, string>   { {67, "Actual Working Width" }
																					, {72, "Actual Volume Content" }
																					, {75, "Actual Mass Content" }
																					, {390, "Actual Revolutions Per Time" }
																					};

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

								// better key for DDI (hex2int)
								if (workingData.Representation.CodeSource == RepresentationCodeSourceEnum.ISO11783_DDI)
								{
									if (numValue.Designator != null && numValue.Designator != "")
										key = numValue.Designator;
									else if (workingData.Representation.Description != null && workingData.Representation.Description != "")
										key = workingData.Representation.Description;
                                    else
                                    {
										// ILaR cause: key missing in representation system
										int intKey = int.Parse(key, System.Globalization.NumberStyles.HexNumber);
										if (_missingDDI.ContainsKey(intKey))
											key = _missingDDI[intKey];
										else
											key = "DDI_" + intKey.ToString();
									}
								}
							}
						}
                        else // needed ?
                        {
							value = spatialRecord.GetMeterValue(workingData);
						}

						if (value != null && key != null)
						{
							properties.Add(key, value);

							if (uom != null)
							{
								properties.Add(key + "_Uom", uom);
							}							
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

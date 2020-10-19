using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.FieldBoundaries;
using AutoMapper;
using GeoJSON.Net.Feature;
using ADAPT.DTOs.AutoMapperProfiles;
using ADAPT.DTOs.Logistics;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using WorkRecordPlugin.Mappers.GeoJson;

namespace WorkRecordPlugin.Mappers
{
	internal class FieldBoundaryMapper
	{
		private static readonly string UniqueIdSourceCNH = "http://www.cnhindustrial.com";
		private readonly PluginProperties _properties;
		private readonly ApplicationDataModel _dataModel;

		public FieldBoundaryMapper(PluginProperties properties, ApplicationDataModel dataModel = null)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			config.CreateMapper();
			_properties = properties;
			_dataModel = dataModel;
		}

		#region Export_old
		public List<Feature> Map(IEnumerable<FieldBoundary> fieldBoundaries, FieldDto fieldDto)
		{
			List<Feature> fieldBoundaryDtos = new List<Feature>();
			foreach (var fieldBoundary in fieldBoundaries)
			{
				Feature fieldBoundaryDto = Map(fieldBoundary);
				if (fieldBoundaryDto != null)
				{
					fieldBoundaryDtos.Add(fieldBoundaryDto);
				}
			}
			return fieldBoundaryDtos;
		}
		#endregion

		#region Export
		private Feature Map(FieldBoundary fieldBoundary)
		{
			GeoJSON.Net.Geometry.MultiPolygon multiPolygon = MultiPolygonMapper.Map(fieldBoundary.SpatialData, _properties.AffineTransformation);
			if (multiPolygon == null)
			{
				return null;
			}

			Dictionary<string, object> properties = new Dictionary<string, object>();
			properties.Add("Guid", UniqueIdMapper.GetUniqueGuid(fieldBoundary.Id, UniqueIdSourceCNH));

			if (_properties.Anonymise)
			{
				properties.Add("Description", "Field boundary " + fieldBoundary.Id.ReferenceId);
			}
			else
			{
				properties.Add("Description", fieldBoundary.Description);
			}

			// GpsSource
			var gpsSource = fieldBoundary.GpsSource;
			properties.Add("GpsSource", null);
			if (gpsSource != null)
			{
				properties["GpsSource"] = fieldBoundary.GpsSource.ToString();
			}

			// Created time
			var creationTime = fieldBoundary.TimeScopes.Where(ts => ts.DateContext == DateContextEnum.Creation).FirstOrDefault();
			properties.Add("CreationTime", null);
			if (creationTime != null)
			{
				if (creationTime.TimeStamp1 != null)
				{
					properties["CreationTime"] = ((DateTime)creationTime.TimeStamp1).ToString("O", CultureInfo.InvariantCulture);
				}
			}

			// Modified time
			var modifiedTime = fieldBoundary.TimeScopes.Where(ts => ts.DateContext == DateContextEnum.Modification).FirstOrDefault();
			properties.Add("ModifiedTime", null);
			if (modifiedTime != null)
			{
				if (modifiedTime.TimeStamp1 != null)
				{
					properties["ModifiedTime"] = ((DateTime)modifiedTime.TimeStamp1).ToString("O", CultureInfo.InvariantCulture);
				}
			}

			Feature fieldBoundaryDto = new Feature(multiPolygon, properties);

			return fieldBoundaryDto;
		}

		public Feature MapAsSingleFeature(FieldBoundary fieldBoundary)
		{
			if (_dataModel == null)
			{
				return null;
			}

			Feature fieldBoundaryFeature = Map(fieldBoundary);

			if (fieldBoundaryFeature == null)
			{
				return null;
			}

			Field adaptField = _dataModel.Catalog.Fields.Where(f => f.Id.ReferenceId == fieldBoundary.FieldId).FirstOrDefault();

			if (adaptField != null)
			{
				fieldBoundaryFeature.Properties.Add("Field", adaptField.Description);

				//Guid fieldguid = UniqueIdMapper.GetUniqueGuid(adaptField.Id, UniqueIdSourceCNH);
				//fieldBoundaryFeature.Properties.Add("FieldId", fieldguid);
				fieldBoundaryFeature.Properties.Add("FieldId", adaptField.Id.ReferenceId);

				Farm adaptFarm = _dataModel.Catalog.Farms.Where(f => f.Id.ReferenceId == adaptField.FarmId).FirstOrDefault();

				if (adaptFarm != null)
				{
					fieldBoundaryFeature.Properties.Add("Farm", adaptFarm.Description);

					Grower adaptGrower = _dataModel.Catalog.Growers.Where(f => f.Id.ReferenceId == adaptFarm.GrowerId).FirstOrDefault();

					if (adaptGrower != null)
					{
						fieldBoundaryFeature.Properties.Add("Grower", adaptGrower.Name);

					}
				}

				if (adaptField.GrowerId != null && !fieldBoundaryFeature.Properties.ContainsKey("Grower"))
				{
					Grower adaptGrower = _dataModel.Catalog.Growers.Where(f => f.Id.ReferenceId == adaptFarm.GrowerId).FirstOrDefault();

					if (adaptGrower != null)
					{
						fieldBoundaryFeature.Properties.Add("Grower", adaptGrower.Name);

					}
				}

			}

			return fieldBoundaryFeature;
		}

		internal static string GetFieldBoundaryPrefix()
		{
			return "FieldBoundary";
		}
		#endregion


		#region Import
		public FieldBoundary Map(Feature fieldBoundaryGeoJson)
		{
			FieldBoundary fieldBoundary = new FieldBoundary();
			var adaptShape = FeatureMapper.Map(fieldBoundaryGeoJson);
			if (adaptShape.GetType() == typeof(AgGateway.ADAPT.ApplicationDataModel.Shapes.MultiPolygon))
			{
				fieldBoundary.SpatialData = (AgGateway.ADAPT.ApplicationDataModel.Shapes.MultiPolygon)adaptShape;
			}

			// ToDo: map properties of a fieldBoundary in GeoJson

			return fieldBoundary;
		}
		#endregion
	}
}
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
using System.Globalization;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.FieldBoundaries;
using AutoMapper;
using GeoJSON.Net.Feature;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Logistics;

namespace WorkRecordPlugin.Mappers
{
	internal class FieldBoundaryMapper
	{
		private static readonly string UniqueIdSourceCNH = "http://www.cnhindustrial.com";
		private readonly PluginProperties _properties;

		public FieldBoundaryMapper(PluginProperties properties)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			config.CreateMapper();
			_properties = properties;
		}

		public List<Feature> Map(IEnumerable<FieldBoundary> fieldBoundaries, FieldDto fieldDto)
		{
			List<Feature> fieldBoundaryDtos = new List<Feature>();
			foreach (var fieldBoundary in fieldBoundaries)
			{
				Feature fieldBoundaryDto = Map(fieldBoundary, fieldDto);
				if (fieldBoundaryDto != null)
				{
					fieldBoundaryDtos.Add(fieldBoundaryDto);
				}
			}
			return fieldBoundaryDtos;
		}

		private Feature Map(FieldBoundary fieldBoundary, FieldDto fieldDto)
		{
			MultiPolygonMapper multiPolygonMapper = new MultiPolygonMapper(_properties);
			GeoJSON.Net.Geometry.MultiPolygon multiPolygon = multiPolygonMapper.Map(fieldBoundary.SpatialData);
			if (multiPolygon == null)
			{
				return null;
			}

			Dictionary<string, object> properties = new Dictionary<string, object>();
			properties.Add("Id", UniqueIdMapper.GetUniqueGuid(fieldBoundary.Id, UniqueIdSourceCNH));

			if (_properties.Anonymise)
			{
				properties.Add("Description", "Field boundary " + fieldBoundary.Id.ReferenceId);
			}
			else
			{
				properties.Add("Description", fieldBoundary.Description);
			}
			//properties.Add("FieldId", fieldDto.Guid);

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

		public FieldBoundary Map(Feature fieldBoundaryGeoJson)
		{
			FieldBoundary fieldBoundary = new FieldBoundary();

			MultiPolygonMapper multiPolygonMapper = new MultiPolygonMapper(_properties);
			fieldBoundary.SpatialData = multiPolygonMapper.Map(fieldBoundaryGeoJson);

			// ToDo: map properties of a fieldBoundary in GeoJson

			return fieldBoundary;
		}
	}
}
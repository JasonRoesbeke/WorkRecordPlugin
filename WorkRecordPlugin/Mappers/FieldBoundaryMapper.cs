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
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.FieldBoundaries;
using AutoMapper;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Logistics;

namespace WorkRecordPlugin.Mappers
{
	internal class FieldBoundaryMapper
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;

		public FieldBoundaryMapper(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
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
			Dictionary<string, object> properties = new Dictionary<string, object>();
			properties.Add("Id", UniqueIdMapper.GetUniqueId(fieldBoundary.Id));
			properties.Add("Description", fieldBoundary.Description);
			properties.Add("FieldId", fieldDto.Guid);

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
				properties["CreationTime"] = (creationTime.TimeStamp1);
			}

			// Modified time
			var modifiedTime = fieldBoundary.TimeScopes.Where(ts => ts.DateContext == DateContextEnum.Modification).FirstOrDefault();
			properties.Add("ModifiedTime", null);
			if (modifiedTime != null)
			{
				properties["ModifiedTime"] = (modifiedTime.TimeStamp1);
			}

			MultiPolygonMapper multiPolygonMapper = new MultiPolygonMapper();
			GeoJSON.Net.Geometry.MultiPolygon multiPolygon = multiPolygonMapper.Map(fieldBoundary.SpatialData);

			Feature fieldBoundaryDto = new Feature(multiPolygon, properties);

			return fieldBoundaryDto;
		}
	}
}
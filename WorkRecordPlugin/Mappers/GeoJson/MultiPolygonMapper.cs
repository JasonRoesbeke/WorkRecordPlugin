﻿/*******************************************************************************
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
using NetTopologySuite.Geometries.Utilities;

namespace WorkRecordPlugin.Mappers.GeoJson
{
    public class MultiPolygonMapper
	{
		#region Export
		public static GeoJSON.Net.Geometry.MultiPolygon Map(AgGateway.ADAPT.ApplicationDataModel.Shapes.MultiPolygon multiPolygon, AffineTransformation affineTransformation = null)
		{
			var polygons = new List<GeoJSON.Net.Geometry.Polygon>();
			foreach (var adaptPolygon in multiPolygon.Polygons)
			{
				GeoJSON.Net.Geometry.Polygon polygon = PolygonMapper.MapPolygon(adaptPolygon, affineTransformation);
				if (polygon != null)
				{
					polygons.Add(polygon);
				}
			}

			if (polygons.Count == 0)
			{
				return null;
			}

			return new GeoJSON.Net.Geometry.MultiPolygon(polygons);
		}
		#endregion

		#region Import
		public static AgGateway.ADAPT.ApplicationDataModel.Shapes.MultiPolygon MapMultiPolygon(GeoJSON.Net.Geometry.MultiPolygon multiPolygonGeoJson, AffineTransformation affineTransformation = null)
		{
			var multiPolygon = new AgGateway.ADAPT.ApplicationDataModel.Shapes.MultiPolygon();
			foreach (var polygonGeoJson in multiPolygonGeoJson.Coordinates)
			{
				var polygon = PolygonMapper.MapPolygon(polygonGeoJson, affineTransformation);
				multiPolygon.Polygons.Add(polygon);
			}

			return multiPolygon;
		}
		#endregion
	}
}
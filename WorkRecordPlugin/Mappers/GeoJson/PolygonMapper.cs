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
using GeoJSON.Net.Geometry;
using NetTopologySuite.Geometries.Utilities;
using System.Collections.Generic;

namespace WorkRecordPlugin.Mappers.GeoJson
{
	public class PolygonMapper
	{
		#region Export
		public static GeoJSON.Net.Geometry.Polygon MapPolygon(AgGateway.ADAPT.ApplicationDataModel.Shapes.Polygon adaptPolygon, AffineTransformation affineTransformation = null)
		{
			var lineStrings = new List<LineString>();
			// First LineString is ExteriorRing
			var lineString = LineStringMapper.MapLinearRing(adaptPolygon.ExteriorRing, affineTransformation);
			if (lineString == null)
			{
				// Stopping here because ExteriorRing is needed, see https://tools.ietf.org/html/rfc7946#section-3.1.6
				return null;
			}
			lineStrings.Add(lineString);

			foreach (var adaptInteriorLinearRing in adaptPolygon.InteriorRings)
			{
				var interiorLineString = LineStringMapper.MapLinearRing(adaptInteriorLinearRing, affineTransformation);
				if (interiorLineString != null)
				{
					lineStrings.Add(interiorLineString);
				}
			}

			return new GeoJSON.Net.Geometry.Polygon(lineStrings);
		}

		public static GeoJSON.Net.Geometry.Polygon MapBoundingBox(AgGateway.ADAPT.ApplicationDataModel.Shapes.BoundingBox adaptBBox, AffineTransformation affineTransformation = null)
		{
			var points = new List<AgGateway.ADAPT.ApplicationDataModel.Shapes.Point>();
			points.Add(new AgGateway.ADAPT.ApplicationDataModel.Shapes.Point() { X = adaptBBox.MinX.Value.Value, Y = adaptBBox.MinY.Value.Value });
			points.Add(new AgGateway.ADAPT.ApplicationDataModel.Shapes.Point() { X = adaptBBox.MinX.Value.Value, Y = adaptBBox.MaxY.Value.Value });
			points.Add(new AgGateway.ADAPT.ApplicationDataModel.Shapes.Point() { X = adaptBBox.MaxX.Value.Value, Y = adaptBBox.MaxY.Value.Value });
			points.Add(new AgGateway.ADAPT.ApplicationDataModel.Shapes.Point() { X = adaptBBox.MaxX.Value.Value, Y = adaptBBox.MinY.Value.Value });

			var linearRing = new AgGateway.ADAPT.ApplicationDataModel.Shapes.LinearRing();
			linearRing.Points = points;

			var lineString = LineStringMapper.MapLinearRing(linearRing, affineTransformation);
			if (lineString == null)
			{
				// Stopping here because ExteriorRing is needed, see https://tools.ietf.org/html/rfc7946#section-3.1.6
				return null;
			}

			return new GeoJSON.Net.Geometry.Polygon(new List<LineString>() { lineString });
		}
		#endregion

		#region Import
		public static AgGateway.ADAPT.ApplicationDataModel.Shapes.Polygon MapPolygon(GeoJSON.Net.Geometry.Polygon polygonGeoJson, AffineTransformation affineTransformation = null)
		{
			var polygon = new AgGateway.ADAPT.ApplicationDataModel.Shapes.Polygon();

			// First LineString is ExteriorRing, see https://tools.ietf.org/html/rfc7946#section-3.1.6
			for (int i = 0; i < polygonGeoJson.Coordinates.Count; i++)
			{
				if (i == 0)
				{
					polygon.ExteriorRing = LineStringMapper.MapLineString(polygonGeoJson.Coordinates[i], affineTransformation);
				}
				else
				{
					polygon.InteriorRings.Add(LineStringMapper.MapLineString(polygonGeoJson.Coordinates[i], affineTransformation));
				}
			}

			return polygon;
		}
		#endregion
	}
}
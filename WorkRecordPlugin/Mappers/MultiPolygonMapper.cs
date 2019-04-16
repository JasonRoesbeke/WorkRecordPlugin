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
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using CoordinateSharp;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using WorkRecordPlugin.Utils;
using LineString = GeoJSON.Net.Geometry.LineString;

namespace WorkRecordPlugin.Mappers
{
	internal class MultiPolygonMapper
	{
		private readonly PluginProperties Properties;

		public MultiPolygonMapper(PluginProperties properties)
		{
			Properties = properties;
		}

		#region Export
		public GeoJSON.Net.Geometry.MultiPolygon Map(AgGateway.ADAPT.ApplicationDataModel.Shapes.MultiPolygon spatialData)
		{
			var polygons = new List<GeoJSON.Net.Geometry.Polygon>();
			// ToDo: Create PolygonMapper
			foreach (var ADAPTpolygon in spatialData.Polygons)
			{
				GeoJSON.Net.Geometry.Polygon polygon = MapPolygon(ADAPTpolygon);
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

		private GeoJSON.Net.Geometry.Polygon MapPolygon(AgGateway.ADAPT.ApplicationDataModel.Shapes.Polygon ADAPTpolygon)
		{
			var lineStrings = new List<LineString>();
			// ToDo: Create LinearRingMapper

			// First LineString is ExteriorRing
			var linearRing = MapLinearRing(ADAPTpolygon.ExteriorRing);
			if (linearRing == null)
			{
				// Stopping here because ExteriorRing is needed
				return null;
			}
			lineStrings.Add(linearRing);
			foreach (var ADAPTInteriorLinearRing in ADAPTpolygon.InteriorRings)
			{
				var interiorLinearRing = MapLinearRing(ADAPTInteriorLinearRing);
				if (interiorLinearRing != null)
				{
					lineStrings.Add(interiorLinearRing);
				}
			}

			return new GeoJSON.Net.Geometry.Polygon(lineStrings);
		}

		private LineString MapLinearRing(LinearRing ADAPTlinearRing)
		{
			var lineString = MapLineString(ADAPTlinearRing);

			// [Check] for https://tools.ietf.org/html/rfc7946#section-3.1.6 to ensure no ArgumentException from GeoJSON.Net when adding the lineString to a Polygon
			if (!lineString.IsLinearRing())
			{
				lineString = MakeClosedLinearRing(lineString);

				if (lineString.Coordinates.Count < 4)
				{
					return null;
				}
			}

			return lineString;
		}
		private LineString MapLineString(LinearRing ADAPTlinearRing)
		{
			var positions = new List<Position>();
			// ToDo: Create PointMapper
			foreach (var point in ADAPTlinearRing.Points)
			{
				if (Properties.Anonymized)
				{
					var movedPoint = AnonymizeUtils.MovePoint(point, Properties.RandomDistance, Properties.RandomBearing);
					positions.Add(new Position(movedPoint.Y, movedPoint.X, movedPoint.Z));
				}
				else
				{
					positions.Add(new Position(point.Y, point.X, point.Z));
				}
			}

			return new LineString(positions);
		}

		private static LineString MakeClosedLinearRing(LineString lineString)
		{
			if (!lineString.IsClosed())
			{
				var positions = new List<Position>();
				foreach (var coordinate in lineString.Coordinates)
				{
					positions.Add((Position)coordinate);
				}
				// Add first position also as last position
				positions.Add((Position)lineString.Coordinates[0]);
				lineString = new LineString(positions);
			}

			return lineString;
		}
		#endregion

		#region Import

		public AgGateway.ADAPT.ApplicationDataModel.Shapes.MultiPolygon Map(Feature fieldBoundaryGeoJson)
		{
			var multiPolygon = new AgGateway.ADAPT.ApplicationDataModel.Shapes.MultiPolygon();

			var multiPolygonGeoJson = (GeoJSON.Net.Geometry.MultiPolygon)fieldBoundaryGeoJson.Geometry;

			foreach (var polygonGeoJson in multiPolygonGeoJson.Coordinates)
			{
				var polygon = MapPolygon(polygonGeoJson);
				multiPolygon.Polygons.Add(polygon);
			}

			return multiPolygon;
		}

		private AgGateway.ADAPT.ApplicationDataModel.Shapes.Polygon MapPolygon(GeoJSON.Net.Geometry.Polygon polygonGeoJson)
		{
			var polygon = new AgGateway.ADAPT.ApplicationDataModel.Shapes.Polygon();

			// First LineString is ExteriorRing
			for (int i = 0; i < polygonGeoJson.Coordinates.Count; i++)
			{
				if (i == 0)
				{
					polygon.ExteriorRing = MapLinearRing(polygonGeoJson.Coordinates[i]);
				}
				else
				{
					polygon.InteriorRings.Add(MapLinearRing(polygonGeoJson.Coordinates[i]));
				}
			}

			return polygon;
		}

		private LinearRing MapLinearRing(GeoJSON.Net.Geometry.LineString lineString)
		{
			LinearRing linearRing = new LinearRing();

			foreach (var position in lineString.Coordinates)
			{
				AgGateway.ADAPT.ApplicationDataModel.Shapes.Point point = new AgGateway.ADAPT.ApplicationDataModel.Shapes.Point();
				point.Y = position.Latitude;
				point.X = position.Longitude;
				point.Z = position.Altitude;
				linearRing.Points.Add(point);
			}

			return linearRing;
		}
		#endregion
	}
}
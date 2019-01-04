using System;
using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using GeoJSON.Net.Geometry;

namespace WorkRecordPlugin.Mappers
{
	internal class MultiPolygonMapper
	{
		public MultiPolygonMapper()
		{
		}

		public GeoJSON.Net.Geometry.MultiPolygon Map(AgGateway.ADAPT.ApplicationDataModel.Shapes.MultiPolygon spatialData)
		{
			List<GeoJSON.Net.Geometry.Polygon> polygons = new List<GeoJSON.Net.Geometry.Polygon>();
			// ToDo: Create PolygonMapper
			foreach (var ADAPTpolygon in spatialData.Polygons)
			{
				GeoJSON.Net.Geometry.Polygon polygon = MapPolygon(ADAPTpolygon);
				polygons.Add(polygon);
			}

			return new GeoJSON.Net.Geometry.MultiPolygon(polygons);
		}

		private GeoJSON.Net.Geometry.Polygon MapPolygon(AgGateway.ADAPT.ApplicationDataModel.Shapes.Polygon ADAPTpolygon)
		{
			List<GeoJSON.Net.Geometry.LineString> lineStrings = new List<GeoJSON.Net.Geometry.LineString>();
			// ToDo: Create LinearRingMapper
			lineStrings.Add(MapLinearRing(ADAPTpolygon.ExteriorRing));
			foreach (var ADAPTInteriorLinearRing in ADAPTpolygon.InteriorRings)
			{
				lineStrings.Add(MapLinearRing(ADAPTInteriorLinearRing));
			}

			return new GeoJSON.Net.Geometry.Polygon(lineStrings);
		}

		private GeoJSON.Net.Geometry.LineString MapLinearRing(LinearRing ADAPTlinearRing)
		{
			List<Position> positions = new List<Position>();
			// ToDo: Create PointMapper
			foreach (var point in ADAPTlinearRing.Points)
			{
				positions.Add(new Position(point.Y, point.X, point.Z));
			}

			return new GeoJSON.Net.Geometry.LineString(positions);
		}
	}
}
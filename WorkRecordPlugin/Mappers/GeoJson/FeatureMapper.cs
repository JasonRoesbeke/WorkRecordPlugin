using GeoJSON.Net.Feature;
using NetTopologySuite.Geometries.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Mappers.GeoJson
{
	class FeatureMapper
	{
		#region Export
		// Needed?
		#endregion

		#region Import
		public static AgGateway.ADAPT.ApplicationDataModel.Shapes.Shape Map(Feature feature, AffineTransformation affineTransformation = null)
		{
			// ToDo: importing different GeoJSONObjectTypes
			switch (feature.Type)
			{
				case GeoJSON.Net.GeoJSONObjectType.Point:
					return PointMapper.MapPosition(((GeoJSON.Net.Geometry.Point)feature.Geometry).Coordinates, affineTransformation);
				case GeoJSON.Net.GeoJSONObjectType.MultiPoint:
					break;
				case GeoJSON.Net.GeoJSONObjectType.LineString:
					return LineStringMapper.MapLineString((GeoJSON.Net.Geometry.LineString)feature.Geometry, affineTransformation);
				case GeoJSON.Net.GeoJSONObjectType.MultiLineString:
					break;
				case GeoJSON.Net.GeoJSONObjectType.Polygon:
					return PolygonMapper.MapPolygon((GeoJSON.Net.Geometry.Polygon)feature.Geometry, affineTransformation);
				case GeoJSON.Net.GeoJSONObjectType.MultiPolygon:
					return MultiPolygonMapper.MapMultiPolygon((GeoJSON.Net.Geometry.MultiPolygon)feature.Geometry, affineTransformation);
				case GeoJSON.Net.GeoJSONObjectType.GeometryCollection:
					break;
				case GeoJSON.Net.GeoJSONObjectType.Feature:
					break;
				case GeoJSON.Net.GeoJSONObjectType.FeatureCollection:
					break;
			}
			return null;
		}
		#endregion
	}
}

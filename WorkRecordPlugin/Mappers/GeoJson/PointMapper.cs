using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using GeoJSON.Net.Geometry;
using NetTopologySuite.Geometries.Utilities;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers.GeoJson
{
	public class PointMapper
	{
		#region Export
		public static Position MapPoint(AgGateway.ADAPT.ApplicationDataModel.Shapes.Point point, AffineTransformation affineTransformation = null)
		{
			if (affineTransformation != null)
			{
				var movedPoint = AnonymizeUtils.MovePoint(point, affineTransformation);
				return new Position(movedPoint.Y, movedPoint.X, point.Z);
			}
			else
			{
				return new Position(point.Y, point.X, point.Z);
			}
		}
		public static GeoJSON.Net.Geometry.Point MapPoint2Point(AgGateway.ADAPT.ApplicationDataModel.Shapes.Point point, AffineTransformation affineTransformation = null)
		{
			Position position = null;
			if (affineTransformation != null)
			{
				var movedPoint = AnonymizeUtils.MovePoint(point, affineTransformation);
				position = new Position(movedPoint.Y, movedPoint.X, point.Z);
			}
			else
			{
				position = new Position(point.Y, point.X, point.Z);
			}
			return new GeoJSON.Net.Geometry.Point(position);
		}
		#endregion

		#region Import
		public static AgGateway.ADAPT.ApplicationDataModel.Shapes.Point MapPosition(IPosition position, AffineTransformation affineTransformation = null)
		{
			if (affineTransformation != null)
			{
				var movedPosition = AnonymizeUtils.MovePosition(position, affineTransformation);
				return new AgGateway.ADAPT.ApplicationDataModel.Shapes.Point
					{
						Y = movedPosition.Latitude,
						X = movedPosition.Longitude,
						Z = position.Altitude
					};
			}
			else
			{
				return new AgGateway.ADAPT.ApplicationDataModel.Shapes.Point
				{
					Y = position.Latitude,
					X = position.Longitude,
					Z = position.Altitude
				};
			}
		}
		#endregion
	}
}
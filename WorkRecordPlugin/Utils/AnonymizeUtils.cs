using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using CoordinateSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Utils
{
	public static class AnonymizeUtils
	{
		public static Point MovePoint(Point point, int distance, int bearing)
		{
			Coordinate coordinate = new Coordinate(point.Y, point.X);
			coordinate.Move(distance, bearing, CoordinateSharp.Shape.Ellipsoid);
			point.Y = coordinate.Latitude.ToDouble();
			point.X = coordinate.Longitude.ToDouble();
			return point;
		}
	}
}

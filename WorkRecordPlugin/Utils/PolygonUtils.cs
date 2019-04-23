using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;

namespace WorkRecordPlugin.Utils
{
	static class PolygonUtils
	{
		#region CalculateAreaMethod1
		/// <summary>
		/// https://www.mathsisfun.com/geometry/area-irregular-polygons.html
		/// </summary>
		/// <param name="positions"></param>
		/// <returns></returns>
		public static double CalculateAreaMethod1(List<Position> positions)
		{
			int i, j;
			double area = 0;

			for (i = 0; i < positions.Count; i++)
			{
				j = (i + 1) % positions.Count;

				area += positions[i].Latitude * positions[j].Longitude;
				area -= positions[i].Longitude * positions[j].Latitude;
			}

			area /= 2;
			return (area < 0 ? -area : area);
		}
		#endregion

		#region CalculateAreaMethod2
		/// <summary>
		/// SphericalUtil.ComputeSignedArea method from Google's Android Maps Utils.
		/// </summary>
		/// <param name="positions"></param>
		/// <returns></returns>
		public static double CalculateAreaMethod2(List<Position> positions)
		{
			return ComputeSignedArea(positions);
		}

		const double EarthRadius = 6371009; //Mean radius as defined by IUGG

		static double ToRadians(double input)
		{
			return input / 180.0 * Math.PI;
		}

		public static double ComputeSignedArea(IList<Position> path)
		{
			return ComputeSignedArea(path, EarthRadius);
		}

		static double ComputeSignedArea(IList<Position> path, double radius)
		{
			int size = path.Count;
			if (size < 3) { return 0; }
			double total = 0;
			var prev = path[size - 1];
			double prevTanLat = Math.Tan((Math.PI / 2 - ToRadians(prev.Latitude)) / 2);
			double prevLng = ToRadians(prev.Longitude);

			foreach (var point in path)
			{
				double tanLat = Math.Tan((Math.PI / 2 - ToRadians(point.Latitude)) / 2);
				double lng = ToRadians(point.Longitude);
				total += PolarTriangleArea(tanLat, lng, prevTanLat, prevLng);
				prevTanLat = tanLat;
				prevLng = lng;
			}
			return total * (radius * radius);
		}

		static double PolarTriangleArea(double tan1, double lng1, double tan2, double lng2)
		{
			double deltaLng = lng1 - lng2;
			double t = tan1 * tan2;
			return 2 * Math.Atan2(t * Math.Sin(deltaLng), 1 + t * Math.Cos(deltaLng));
		}
		#endregion
	}
}
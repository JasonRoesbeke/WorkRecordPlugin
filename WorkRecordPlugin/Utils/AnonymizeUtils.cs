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
using System.CodeDom.Compiler;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;

namespace WorkRecordPlugin.Utils
{
	public static class AnonymizeUtils
	{
		// ToDo: [IoF2020-WP6] Is a distance between 30 & 80 km enough to be anonymized?
		// ToDo: tip: move it to the sea
		private const int MinimumDistance = 30000;
		private const int MaximumDistance = 80000;
		private const int MinimumBearing = 0;
		private const int MaximumBearing = 360;
		private const int EarthRadiusAvg = 6371000;

		public static Coordinate GetEndCoordinate(Coordinate startCoordinate, double distance, double bearing)
		{
			double lat1 = startCoordinate.Y * (Math.PI / 180);
			double lon1 = startCoordinate.X * (Math.PI / 180);
			double brng = bearing * (Math.PI / 180);
			double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(distance / EarthRadiusAvg) + Math.Cos(lat1) * Math.Sin(distance / EarthRadiusAvg) * Math.Cos(brng));
			double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(distance / EarthRadiusAvg) * Math.Cos(lat1), Math.Cos(distance / EarthRadiusAvg) - Math.Sin(lat1) * Math.Sin(lat2));
			return new Coordinate(lon2 * (180 / Math.PI), (lat2 * (180 / Math.PI)));
		}

		public static void GenerateRandomAffineTransformation(PluginProperties pluginProperties)
		{
			Random rnd = new Random();
			var randomDistance = rnd.Next(MinimumDistance, MaximumDistance);
			var randomBearing = rnd.Next(MinimumBearing, MaximumBearing);
			var startCoordinate = new Coordinate();
			var endCoordinate = GetEndCoordinate(startCoordinate, randomDistance, randomBearing);
			pluginProperties.AffineTransformation = AffineTransformationFactory.CreateFromControlVectors(startCoordinate, endCoordinate);
		}
	}
}

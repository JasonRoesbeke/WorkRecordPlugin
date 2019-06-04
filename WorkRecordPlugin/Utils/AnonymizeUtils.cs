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
using CoordinateSharp;

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

		public static Point MovePoint(Point point, int distance, int bearing)
		{
			Coordinate coordinate = new Coordinate(point.Y, point.X);
			coordinate.Move(distance, bearing, CoordinateSharp.Shape.Ellipsoid);
			point.Y = coordinate.Latitude.ToDouble();
			point.X = coordinate.Longitude.ToDouble();
			return point;
		}

		public static void GenerateRandomValues(PluginProperties pluginProperties)
		{
			Random rnd = new Random();
			pluginProperties.RandomDistance = rnd.Next(MinimumDistance, MaximumDistance);
			pluginProperties.RandomBearing = rnd.Next(MinimumBearing, MaximumBearing);
		}
	}
}

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

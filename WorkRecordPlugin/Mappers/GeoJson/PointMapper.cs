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
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
using GeoJSON.Net.Geometry;
using NetTopologySuite.Geometries.Utilities;
using System.Collections.Generic;

namespace WorkRecordPlugin.Mappers.GeoJson
{
	public class LineStringMapper
	{
		#region Export
		public static LineString MapLinearRing(AgGateway.ADAPT.ApplicationDataModel.Shapes.LinearRing adaptLinearRing, AffineTransformation affineTransformation = null)
		{
			var lineString = MapLineString(adaptLinearRing, affineTransformation);

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

		public static LineString MapLineString(AgGateway.ADAPT.ApplicationDataModel.Shapes.LinearRing adaptLinearRing, AffineTransformation affineTransformation = null)
		{
			var positions = new List<Position>();
			foreach (var point in adaptLinearRing.Points)
			{
				var position = PointMapper.MapPoint(point, affineTransformation);
				positions.Add(position);
			}

			return new LineString(positions);
		}

		public static LineString MakeClosedLinearRing(LineString lineString)
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
		public static AgGateway.ADAPT.ApplicationDataModel.Shapes.LinearRing MapLineString(LineString lineString, AffineTransformation affineTransformation = null)
		{
			// ToDo: [Check] if the LineString is actually a LinearRing
			var linearRing = new AgGateway.ADAPT.ApplicationDataModel.Shapes.LinearRing();

			foreach (var position in lineString.Coordinates)
			{
				linearRing.Points.Add(PointMapper.MapPosition(position, affineTransformation));
			}

			return linearRing;
		}
		#endregion
	}
}
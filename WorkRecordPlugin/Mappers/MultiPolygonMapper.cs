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
using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using CoordinateSharp;
using GeoJSON.Net.Geometry;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers
{
	internal class MultiPolygonMapper
	{
		private readonly ExportProperties exportProperties;

		public MultiPolygonMapper(ExportProperties exportProperties)
		{
			this.exportProperties = exportProperties;
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
				if (exportProperties.Anonymized)
				{
					var movedPoint = AnonymizeUtils.MovePoint(point, exportProperties.RandomDistance, exportProperties.RandomBearing);
					positions.Add(new Position(movedPoint.Y, movedPoint.X, movedPoint.Z));
				}
				else
				{
					positions.Add(new Position(point.Y, point.X, point.Z));
				}
			}

			return new GeoJSON.Net.Geometry.LineString(positions);
		}
	}
}
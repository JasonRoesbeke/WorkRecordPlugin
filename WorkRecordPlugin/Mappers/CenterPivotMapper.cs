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
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Guidance;
using AgGateway.ADAPT.Representation.UnitSystem.ExtensionMethods;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkRecordPlugin.Mappers.GeoJson;

namespace WorkRecordPlugin.Mappers
{
    internal class CenterPivotMapper
    {
        private PluginProperties _properties;
        private ApplicationDataModel _dataModel;
        private Dictionary<string, object> _featProps;

        public CenterPivotMapper(PluginProperties properties, ApplicationDataModel dataModel, Dictionary<string, object> featProps)
        {
            this._properties = properties;
            this._dataModel = dataModel;
            this._featProps = featProps;
        }

        public List<Feature> MapAsMultipleFeatures(PivotGuidancePattern guidancePatternAdapt)
        {
            _featProps.Add("DefinitionMethod", guidancePatternAdapt.DefinitionMethod.ToString());
            List<Feature> centerPivotFeatures = new List<Feature>();
            const string labelPT = "PointType";
            const string labelCP = "Center";
            const string labelRa = "Radius (m)";
            List<Position> positions = new List<Position>();
            var oneDegree = Math.PI / 180.0;

            switch (guidancePatternAdapt.DefinitionMethod)
            {
                case PivotGuidanceDefinitionEnum.PivotGuidancePatternStartEndCenter:
                case PivotGuidanceDefinitionEnum.PivotGuidancePatternThreePoints:
                    while (_featProps.ContainsKey(labelPT))
                        _featProps.Remove(labelPT);
                    Dictionary<string, object> featProps2 = new Dictionary<string, object>(_featProps);
                    Dictionary<string, object> featProps3 = new Dictionary<string, object>(_featProps);
                    var distanceEllipsoidal = GetDistanceEllipsoidal(guidancePatternAdapt.Center.X, guidancePatternAdapt.Center.Y, guidancePatternAdapt.StartPoint.X, guidancePatternAdapt.StartPoint.Y);

                    _featProps.Add(labelPT, "StartPoint");
                    centerPivotFeatures.Add(new Feature(PointMapper.MapPoint2Point(guidancePatternAdapt.StartPoint, _properties.AffineTransformation), _featProps));
                    featProps2.Add(labelPT, labelCP);
                    featProps2.Add(labelRa, distanceEllipsoidal);
                    centerPivotFeatures.Add(new Feature(PointMapper.MapPoint2Point(guidancePatternAdapt.Center, _properties.AffineTransformation), featProps2));
                    featProps3.Add(labelPT, "EndPoint");
                    centerPivotFeatures.Add(new Feature(PointMapper.MapPoint2Point(guidancePatternAdapt.EndPoint, _properties.AffineTransformation), featProps3));

                    // extra "visually nice" Features
                    var distanceCartesian = GetDistanceCartesian(guidancePatternAdapt.Center.X, guidancePatternAdapt.Center.Y, guidancePatternAdapt.StartPoint.X, guidancePatternAdapt.StartPoint.Y);
                    for (int i = 0; i <= 360; i += 2)
                    {
                        // guidancePatternAdapt.GuidancePatternOptions ([1]Clockwise [2]Couter-clockwise [3]Full Circle) not yet available
                        positions.Add(new Position((distanceCartesian * Math.Cos(i * oneDegree)) + guidancePatternAdapt.Center.Y,
                            (distanceCartesian * Math.Sin(i * oneDegree)) + guidancePatternAdapt.Center.X));
                    }
                    centerPivotFeatures.Add(new Feature(new GeoJSON.Net.Geometry.LineString(positions), new Dictionary<string, object>() { { labelRa, distanceEllipsoidal } }));
                    centerPivotFeatures.Add(new Feature(LineStringMapper.MapLineString(guidancePatternAdapt.Center, guidancePatternAdapt.StartPoint, null), null));
                    centerPivotFeatures.Add(new Feature(LineStringMapper.MapLineString(guidancePatternAdapt.Center, guidancePatternAdapt.EndPoint, null), null));
                    break;
                case PivotGuidanceDefinitionEnum.PivotGuidancePatternCenterRadius:
                    if (guidancePatternAdapt.Radius == null)
                    {
                        Console.WriteLine("Error if (guidancePatternAdapt.Radius == null)");
                        break;
                    }
                    while (_featProps.ContainsKey(labelPT))
                        _featProps.Remove(labelPT);

                    _featProps.Add(labelPT, labelCP);
                    _featProps.Add(labelRa, guidancePatternAdapt.Radius.Multiply(1000.0));
                    centerPivotFeatures.Add(new Feature(PointMapper.MapPoint2Point(guidancePatternAdapt.Center, _properties.AffineTransformation), _featProps));

                    // extra "visually nice" Features
                    for (int i = 0; i <= 360; i += 2)
                    {
                        positions.Add(new Position((0.00036 * Math.Cos(i * oneDegree)) + guidancePatternAdapt.Center.Y,
                            (0.00036 * Math.Sin(i * oneDegree)) + guidancePatternAdapt.Center.X));
                    }
                    centerPivotFeatures.Add(new Feature(new GeoJSON.Net.Geometry.LineString(positions), new Dictionary<string, object>() { { labelRa, guidancePatternAdapt.Radius.Multiply(1000.0) } }));
                    break;
                default:
                    break;
            }

            return centerPivotFeatures;
        }

        private static double GetDistanceEllipsoidal(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            var oneDegree = Math.PI / 180.0;
            var d1 = latitude * oneDegree;
            var num1 = longitude * oneDegree;
            var d2 = otherLatitude * oneDegree;
            var num2 = otherLongitude * oneDegree - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        private static double GetDistanceCartesian(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
    }
}
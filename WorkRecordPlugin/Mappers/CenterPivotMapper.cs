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
using GeoJSON.Net.Feature;
using System;
using System.Collections.Generic;
using WorkRecordPlugin.Mappers.GeoJson;

namespace WorkRecordPlugin.Mappers
{
    internal class CenterPivotMapper
    {
        private PluginProperties _properties;
        private ApplicationDataModel _dataModel;

        public CenterPivotMapper(PluginProperties properties, ApplicationDataModel dataModel)
        {
            _properties = properties;
            _dataModel = dataModel;
        }

        public Feature MapAsSingleFeature(PivotGuidancePattern guidancePatternAdapt)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            if (_properties.Anonymise)
            {
                properties.Add("Description", "Guidance Pattern " + guidancePatternAdapt.Id.ReferenceId);
            }
            else
            {
                properties.Add("Description", guidancePatternAdapt.Description);
            }

            properties.Add("GuidancePatternType", guidancePatternAdapt.GuidancePatternType.ToString());
            properties.Add("DefinitionMethod", guidancePatternAdapt.DefinitionMethod.ToString());
            //properties.Add("Radius", guidancePatternAdapt.Radius.ToString());

            GeoJSON.Net.Geometry.Point startPoint = PointMapper.MapPoint2Point(guidancePatternAdapt.StartPoint, _properties.AffineTransformation);
            GeoJSON.Net.Geometry.Point centerPoint = PointMapper.MapPoint2Point(guidancePatternAdapt.Center, _properties.AffineTransformation);
            GeoJSON.Net.Geometry.Point endPoint = PointMapper.MapPoint2Point(guidancePatternAdapt.EndPoint, _properties.AffineTransformation);
            return new Feature(new GeoJSON.Net.Geometry.MultiPoint(new List<GeoJSON.Net.Geometry.Point>
            {
                startPoint,
                centerPoint,
                endPoint
            }), properties);

            // Version 2
            //return new Feature(MultiLineStringMapper.MapMultiLineString(new List<GeoJSON.Net.Geometry.LineString>
            //{
            //    LineStringMapper.MapLineString(guidancePatternAdapt.StartPoint, guidancePatternAdapt.Center, _properties.AffineTransformation),
            //    LineStringMapper.MapLineString(guidancePatternAdapt.EndPoint, guidancePatternAdapt.Center, _properties.AffineTransformation)
            //}), properties);

            // Version 1
            //var lineStrings = new List<GeoJSON.Net.Geometry.LineString>();
            //GeoJSON.Net.Geometry.LineString lineStringA = LineStringMapper.MapLineString(guidancePatternAdapt.StartPoint, guidancePatternAdapt.Center, _properties.AffineTransformation);
            //GeoJSON.Net.Geometry.LineString lineStringB = LineStringMapper.MapLineString(guidancePatternAdapt.EndPoint, guidancePatternAdapt.Center, _properties.AffineTransformation);
            //lineStrings.Add(lineStringA);
            //lineStrings.Add(lineStringB);
            //return new Feature(new GeoJSON.Net.Geometry.MultiLineString(lineStrings), properties);
            //return new Feature(MultiLineStringMapper.MapMultiLineString(lineStrings), properties);
        }
    }
}
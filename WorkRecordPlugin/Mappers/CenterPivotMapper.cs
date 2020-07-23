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
            //properties.Add("DefinitionMethod", guidancePatternAdapt.DefinitionMethod.ToString());
            //properties.Add("Radius", guidancePatternAdapt.Radius.ToString());
            
            _featProps.Add("DefinitionMethod", guidancePatternAdapt.DefinitionMethod.ToString());

            List<Feature> centerPivotFeatures = new List<Feature>();
            const string labelPT = "PointType";
            while (_featProps.ContainsKey(labelPT))
            {
                _featProps.Remove(labelPT);
            }
            Dictionary<string, object> featProps2 = new Dictionary<string, object>(_featProps);
            Dictionary<string, object> featProps3 = new Dictionary<string, object>(_featProps);
            switch (guidancePatternAdapt.DefinitionMethod)
            {
                case PivotGuidanceDefinitionEnum.PivotGuidancePatternStartEndCenter:
                    break;
                case PivotGuidanceDefinitionEnum.PivotGuidancePatternCenterRadius:
                    break;
                case PivotGuidanceDefinitionEnum.PivotGuidancePatternThreePoints:
                    break;
                default:
                    break;
            }
            _featProps.Add(labelPT, "StartPoint");
            centerPivotFeatures.Add(new Feature(PointMapper.MapPoint2Point(guidancePatternAdapt.StartPoint, _properties.AffineTransformation), _featProps));
            featProps2.Add(labelPT, "Center");
            centerPivotFeatures.Add(new Feature(PointMapper.MapPoint2Point(guidancePatternAdapt.Center, _properties.AffineTransformation), featProps2));
            featProps3.Add(labelPT, "EndPoint");
            centerPivotFeatures.Add(new Feature(PointMapper.MapPoint2Point(guidancePatternAdapt.EndPoint, _properties.AffineTransformation), featProps3));

            return centerPivotFeatures;

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
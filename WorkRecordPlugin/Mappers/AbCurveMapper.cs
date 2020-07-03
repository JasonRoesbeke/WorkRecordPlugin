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
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using WorkRecordPlugin.Mappers.GeoJson;

namespace WorkRecordPlugin.Mappers
{
    internal class AbCurveMapper
    {
        private PluginProperties _properties;
        private ApplicationDataModel _dataModel;

        public AbCurveMapper(PluginProperties properties, ApplicationDataModel dataModel)
        {
            _properties = properties;
            _dataModel = dataModel;
        }

        public Feature MapAsSingleFeature(AbCurve guidancePatternAdapt)
        {
            //if (guidancePatternAdapt.Shape.Count != 1)
            //{
            //    Console.WriteLine("[Check] why guidancePatternAdapt.Shape contains {0} LineString's", guidancePatternAdapt.Shape.Count);
            //    return null;
            //}

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

            //return new Feature(LineStringMapper.MapLineString(guidancePatternAdapt.Shape[0], _properties.AffineTransformation), properties);

            var lineStrings = new List<GeoJSON.Net.Geometry.LineString>();
            foreach (var adaptLineString in guidancePatternAdapt.Shape)
            {
                lineStrings.Add(LineStringMapper.MapLineString(adaptLineString, _properties.AffineTransformation));
            }
            return new Feature(MultiLineStringMapper.MapMultiLineString(lineStrings), properties);
        }
    }
}
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
    internal class SpiralMapper
    {
        private PluginProperties _properties;
        private ApplicationDataModel _dataModel;

        public SpiralMapper(PluginProperties properties, ApplicationDataModel dataModel)
        {
            _properties = properties;
            _dataModel = dataModel;
        }

        public Feature MapAsSingleFeature(Spiral guidancePatternAdapt)
        {
            GeoJSON.Net.Geometry.LineString lineString = LineStringMapper.MapLineString(guidancePatternAdapt.Shape, _properties.AffineTransformation);

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

            return new Feature(lineString, properties);
        }
    }
}
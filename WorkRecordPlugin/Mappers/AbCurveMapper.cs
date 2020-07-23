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
using System.Collections.Generic;
using WorkRecordPlugin.Mappers.GeoJson;

namespace WorkRecordPlugin.Mappers
{
    internal class AbCurveMapper
    {
        private PluginProperties _properties;
        private ApplicationDataModel _dataModel;
        private Dictionary<string, object> _featProps;

        public AbCurveMapper(PluginProperties properties, ApplicationDataModel dataModel, Dictionary<string, object> featProps)
        {
            this._properties = properties;
            this._dataModel = dataModel;
            this._featProps = featProps;
        }

        public Feature MapAsSingleFeature(AbCurve guidancePatternAdapt)
        {
            var lineStrings = new List<GeoJSON.Net.Geometry.LineString>();
            foreach (var adaptLineString in guidancePatternAdapt.Shape)
            {
                lineStrings.Add(LineStringMapper.MapLineString(adaptLineString, _properties.AffineTransformation));
            }
            return new Feature(MultiLineStringMapper.MapMultiLineString(lineStrings), _featProps);
        }
    }
}
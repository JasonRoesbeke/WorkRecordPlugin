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
using System.Linq;

namespace WorkRecordPlugin
{
    internal class GuidanceGroupMapper
    {
        private PluginProperties _properties;
        private ApplicationDataModel _dataModel;

        public GuidanceGroupMapper(PluginProperties properties, ApplicationDataModel dataModel)
        {
            _properties = properties;
            _dataModel = dataModel;
        }

        public List<Feature> MapAsMultipleFeatures(GuidanceGroup guidanceGroup)
        {
            //guidanceGroup.BoundingPolygon
            //guidanceGroup.GuidancePatternIds

            List<Feature> featureCollection = new List<Feature>();

            foreach (var guidancePatternId in guidanceGroup.GuidancePatternIds)
            {
                var guidancePatternAdapt = _dataModel.Catalog.GuidancePatterns.Where(gp => gp.Id.ReferenceId == guidancePatternId).FirstOrDefault();
                if (guidancePatternAdapt == null)
                {
                    continue;
                }
                switch (guidancePatternAdapt.GuidancePatternType)
                {
                    case GuidancePatternTypeEnum.APlus:
                        Console.WriteLine("Dealing with a case of \"APlus:\"");
                        break;
                    case GuidancePatternTypeEnum.AbLine:
                        if (guidancePatternAdapt.GetType() != typeof(AbLine))
                        {
                            Console.WriteLine("Error if (guidancePatternAdapt.GetType() != typeof(AbLine))");
                            break;
                        }
                        AbLineMapper abLineMapper = new AbLineMapper(_properties, _dataModel);
                        Feature abLineFeature = abLineMapper.MapAsSingleFeature((AbLine)guidancePatternAdapt);
                        if (abLineFeature != null)
                        {
                            featureCollection.Add(abLineFeature);
                        }
                        break;
                    case GuidancePatternTypeEnum.AbCurve:
                        // Todo: AbCurve is in fact a List<ADAPT...LineString>, so somehow this should be a MapAsMultipleFeatures instead of a MapAsSingleFeature, even though the List has only 1 item
                        if (guidancePatternAdapt.GetType() != typeof(AbCurve))
                        {
                            Console.WriteLine("Error if (guidancePatternAdapt.GetType() != typeof(AbCurve))");
                            break;
                        }
                        AbCurveMapper abCurveMapper = new AbCurveMapper(_properties, _dataModel);
                        Feature abCurveFeature = abCurveMapper.MapAsSingleFeature((AbCurve)guidancePatternAdapt);
                        if (abCurveFeature != null)
                        {
                            featureCollection.Add(abCurveFeature);
                        }
                        break;
                    case GuidancePatternTypeEnum.CenterPivot:
                        Console.WriteLine("Dealing with a case of \"CenterPivot:\"");
                        break;
                    case GuidancePatternTypeEnum.Spiral:
                        Console.WriteLine("Dealing with a case of \"Spiral:\"");
                        break;
                    default:
                        break;
                }
            }
            return featureCollection;
        }

        internal static string GetPrefix()
        {
            return "GuidanceGroup"; // Used to be "guidance-group-test"
        }
    }
}
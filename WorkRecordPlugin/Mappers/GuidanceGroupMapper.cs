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
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.Guidance;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using GeoJSON.Net.Feature;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkRecordPlugin.Mappers
{
    internal class GuidanceGroupMapper
    {
        private PluginProperties _properties;
        private ApplicationDataModel _dataModel;

        public GuidanceGroupMapper(PluginProperties properties, ApplicationDataModel dataModel)
        {
            this._properties = properties;
            this._dataModel = dataModel;
        }

        public List<Feature> MapAsMultipleFeatures(GuidanceGroup guidanceGroup)
        {
            List<Feature> featureCollection = new List<Feature>();

            foreach (var guidancePatternId in guidanceGroup.GuidancePatternIds)
            {
                var guidancePatternAdapt = _dataModel.Catalog.GuidancePatterns.Where(gp => gp.Id.ReferenceId == guidancePatternId).FirstOrDefault();
                if (guidancePatternAdapt == null)
                {
                    continue;
                }

                // Properties
                Dictionary<string, object> properties = new Dictionary<string, object>();
                Field adaptField = null;
                bool found = false;
                int i1 = 0;
                while (i1 < _dataModel.Catalog.Fields.Count && !found)
                {
                    // find the first Field that has a .GuidanceGroupIds[x] == our current guidanceGroup 's ID
                    int i2 = 0;
                    while (i2 < _dataModel.Catalog.Fields[i1].GuidanceGroupIds.Count && !found)
                    {
                        if (guidanceGroup.Id.ReferenceId == _dataModel.Catalog.Fields[i1].GuidanceGroupIds[i2])
                        {
                            adaptField = _dataModel.Catalog.Fields[i1];
                            found = true;
                        }
                        i2++;
                    }
                    i1++;
                }
                if (found)
                {
                    properties.Add("FieldId", adaptField.Id.ReferenceId);
                    properties.Add("FieldDescripton", (_properties.Anonymise || adaptField.Description == null) ? $"Field {adaptField.Id.ReferenceId}" : adaptField.Description);
                }
                properties.Add("GuidanceGroupId", guidanceGroup.Id.ReferenceId);
                properties.Add("GuidanceGroupDescription", (_properties.Anonymise || guidanceGroup.Description == null) ? $"Guidance group {guidanceGroup.Id.ReferenceId}" : guidanceGroup.Description);
                properties.Add("GuidancePatternType", guidancePatternAdapt.GuidancePatternType.ToString());

                switch (guidancePatternAdapt.GuidancePatternType)
                {
                    case GuidancePatternTypeEnum.APlus:
                        if (guidancePatternAdapt.GetType() != typeof(APlus))
                        {
                            Console.WriteLine("Error if (guidancePatternAdapt.GetType() != typeof(APlus))");
                            break;
                        }
                        APlusMapper aPlusMapper = new APlusMapper(_properties, _dataModel, properties);
                        Feature aPlusFeature = aPlusMapper.MapAsSingleFeature((APlus)guidancePatternAdapt);
                        if (aPlusFeature != null) featureCollection.Add(aPlusFeature);
                        break;
                    case GuidancePatternTypeEnum.AbLine:
                        if (guidancePatternAdapt.GetType() != typeof(AbLine))
                        {
                            Console.WriteLine("Error if (guidancePatternAdapt.GetType() != typeof(AbLine))");
                            break;
                        }
                        AbLineMapper abLineMapper = new AbLineMapper(_properties, _dataModel, properties);
                        Feature abLineFeature = abLineMapper.MapAsSingleFeature((AbLine)guidancePatternAdapt);
                        if (abLineFeature != null) featureCollection.Add(abLineFeature);
                        break;
                    case GuidancePatternTypeEnum.AbCurve:
                        // Note: AbCurve is in fact a List<ADAPT...LineString>, so somehow this should be a MapAsMultipleFeatures
                        //       instead of a MapAsSingleFeature, even though the List has only 1 item.
                        //       For now, the List<LineString> has been mapped as a MultiLineString single Feature.
                        if (guidancePatternAdapt.GetType() != typeof(AbCurve))
                        {
                            Console.WriteLine("Error if (guidancePatternAdapt.GetType() != typeof(AbCurve))");
                            break;
                        }
                        AbCurveMapper abCurveMapper = new AbCurveMapper(_properties, _dataModel, properties);
                        Feature abCurveFeature = abCurveMapper.MapAsSingleFeature((AbCurve)guidancePatternAdapt);
                        if (abCurveFeature != null) featureCollection.Add(abCurveFeature);
                        break;
                    case GuidancePatternTypeEnum.CenterPivot:
                        if (guidancePatternAdapt.GetType() != typeof(PivotGuidancePattern))
                        {
                            Console.WriteLine("Error if (guidancePatternAdapt.GetType() != typeof(CenterPivot))");
                            break;
                        }
                        CenterPivotMapper centerPivotMapper = new CenterPivotMapper(_properties, _dataModel, properties);
                        List<Feature> centerPivotFeatures = centerPivotMapper.MapAsMultipleFeatures((PivotGuidancePattern)guidancePatternAdapt);
                        foreach (var item in centerPivotFeatures)
                        {
                            featureCollection.Add(item);
                        }
                        break;
                    case GuidancePatternTypeEnum.Spiral:
                        if (guidancePatternAdapt.GetType() != typeof(Spiral))
                        {
                            Console.WriteLine("Error if (guidancePatternAdapt.GetType() != typeof(Spiral))");
                            break;
                        }
                        SpiralMapper spiralMapper = new SpiralMapper(_properties, _dataModel, properties);
                        Feature spiralFeature = spiralMapper.MapAsSingleFeature((Spiral)guidancePatternAdapt);
                        if (spiralFeature != null) featureCollection.Add(spiralFeature);
                        break;
                    default:
                        break;
                }
            }
            return featureCollection;
        }

        internal static string GetPrefix()
        {
            return "GuidancePattern"; // Used to be "guidance-group-test"
        }
    }
}
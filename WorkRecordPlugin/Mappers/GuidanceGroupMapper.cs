﻿/*******************************************************************************
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
        private static readonly string UniqueIdSourceCNH = "http://www.cnhindustrial.com";
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
                        if (guidancePatternAdapt.GetType() != typeof(APlus))
                        {
                            Console.WriteLine("Error if (guidancePatternAdapt.GetType() != typeof(APlus))");
                            break;
                        }
                        APlusMapper aPlusMapper = new APlusMapper(_properties, _dataModel);
                        Feature aPlusFeature = aPlusMapper.MapAsSingleFeature((APlus)guidancePatternAdapt);
                        if (aPlusFeature != null)
                        {
                            featureCollection.Add(aPlusFeature);
                        }
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
                        // Note: AbCurve is in fact a List<ADAPT...LineString>, so somehow this should be a MapAsMultipleFeatures
                        //       instead of a MapAsSingleFeature, even though the List has only 1 item.
                        //       For now, the List<LineString> has been mapped as a MultiLineString single Feature.
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
                        if (guidancePatternAdapt.GetType() != typeof(PivotGuidancePattern))
                        {
                            Console.WriteLine("Error if (guidancePatternAdapt.GetType() != typeof(CenterPivot))");
                            break;
                        }
                        CenterPivotMapper centerPivotMapper = new CenterPivotMapper(_properties, _dataModel);
                        Feature centerPivotFeature = centerPivotMapper.MapAsSingleFeature((PivotGuidancePattern)guidancePatternAdapt);
                        if (centerPivotFeature != null)
                        {
                            featureCollection.Add(centerPivotFeature);
                        }
                        break;
                    case GuidancePatternTypeEnum.Spiral:
                        if (guidancePatternAdapt.GetType() != typeof(Spiral))
                        {
                            Console.WriteLine("Error if (guidancePatternAdapt.GetType() != typeof(Spiral))");
                            break;
                        }
                        SpiralMapper spiralMapper = new SpiralMapper(_properties, _dataModel);
                        Feature spiralFeature = spiralMapper.MapAsSingleFeature((Spiral)guidancePatternAdapt);
                        if (spiralFeature != null)
                        {
                            featureCollection.Add(spiralFeature);
                        }
                        break;
                    default:
                        break;
                }
                //Field adaptField = _dataModel.Catalog.Fields.Where(f => f.Id.ReferenceId == fieldBoundary.FieldId).FirstOrDefault();
                // Note: I didn't get to the corresponding Field with a "Where" like in the example code above.
                Field adaptField = null;
                GuidanceGroup adaptGuidanceGroup = null;
                var continueLooking = true;
                int i1 = 0;
                while (i1 < _dataModel.Catalog.GuidanceGroups.Count && continueLooking)
                {
                    int i2 = 0;
                    while (i2 < _dataModel.Catalog.GuidanceGroups[i1].GuidancePatternIds.Count && continueLooking)
                    {
                        if (_dataModel.Catalog.GuidanceGroups[i1].GuidancePatternIds[i2] == guidancePatternAdapt.Id.ReferenceId)
                        {
                            int i3 = 0;
                            while (i3 < _dataModel.Catalog.Fields.Count && continueLooking)
                            {
                                int i4 = 0;
                                while (i4 < _dataModel.Catalog.Fields[i3].GuidanceGroupIds.Count && continueLooking)
                                {
                                    if (_dataModel.Catalog.GuidanceGroups[i1].Id.ReferenceId == _dataModel.Catalog.Fields[i3].GuidanceGroupIds[i4])
                                    {
                                        adaptField = _dataModel.Catalog.Fields[i3];
                                        adaptGuidanceGroup = _dataModel.Catalog.GuidanceGroups[i1];
                                        continueLooking = false;
                                    }
                                    i4++;
                                }
                                i3++;
                            }
                        }
                        i2++;
                    }
                    i1++;
                }
                if (!continueLooking && featureCollection[0] != null)
                {
                    // Note: The fieldguid is not consistent. Each GuidancePattern file lists a different FieldId, even though the object contains the same one.
                    Guid fieldguid = UniqueIdMapper.GetUniqueGuid(adaptField.Id, UniqueIdSourceCNH);
                    featureCollection[0].Properties.Add("FieldId", fieldguid);
                    featureCollection[0].Properties.Add("FieldDescripton", adaptField.Description);
                    Guid guidancegroupguid = UniqueIdMapper.GetUniqueGuid(adaptGuidanceGroup.Id, UniqueIdSourceCNH);
                    featureCollection[0].Properties.Add("GuidanceGroupId", guidancegroupguid);
                    if (adaptGuidanceGroup.Description != null) { featureCollection[0].Properties.Add("GuidanceGroupDescription", adaptGuidanceGroup.Description); }
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
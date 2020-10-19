using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.FieldBoundaries;
using GeoJSON.Net.Feature;
using System;
using System.Collections.Generic;
using WorkRecordPlugin.Mappers.GeoJson;

namespace WorkRecordPlugin.Mappers
{
    internal class DrivenHeadlandMapper
    {
        private PluginProperties _properties;
        private ApplicationDataModel _dataModel;

        public DrivenHeadlandMapper(PluginProperties properties, ApplicationDataModel dataModel)
        {
            this._properties = properties;
            this._dataModel = dataModel;
        }

        public Feature MapAsSingleFeature(DrivenHeadland drivenHeadlandAdapt, Feature fieldBoundary)
        {
            GeoJSON.Net.Geometry.MultiPolygon multiPolygon = MultiPolygonMapper.Map(drivenHeadlandAdapt.SpatialData, _properties.AffineTransformation);
            if (multiPolygon == null)
                return null;

            Dictionary<string, object> properties = new Dictionary<string, object>();
            if (!String.IsNullOrEmpty(drivenHeadlandAdapt.Description))
                properties.Add("HeadlandDescription", drivenHeadlandAdapt.Description);
            foreach (var property in fieldBoundary.Properties)
                properties.Add(property.Key, property.Value);

            return new Feature(multiPolygon, properties);
        }

        internal static string GetPrefix() { return "DrivenHeadland"; }
    }
}

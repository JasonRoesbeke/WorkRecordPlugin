using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Guidance;
using GeoJSON.Net.Feature;
using System.Collections.Generic;
using WorkRecordPlugin.Mappers.GeoJson;

namespace WorkRecordPlugin.Mappers
{
    internal class AbLineMapper
    {
        private ApplicationDataModel _dataModel;
        private PluginProperties _properties;
        private Dictionary<string, object> _featProps;

        public AbLineMapper(PluginProperties properties, ApplicationDataModel dataModel, Dictionary<string, object> featProps)
        {
            this._properties = properties;
            this._dataModel = dataModel;
            this._featProps = featProps;
        }

        public Feature MapAsSingleFeature(AbLine guidancePatternAdapt)
        {
            GeoJSON.Net.Geometry.LineString lineString = LineStringMapper.MapLineString(guidancePatternAdapt.A, guidancePatternAdapt.B, _properties.AffineTransformation);
            return new Feature(lineString, _featProps);
        }
    }
}
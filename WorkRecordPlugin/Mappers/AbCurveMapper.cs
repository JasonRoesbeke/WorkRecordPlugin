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
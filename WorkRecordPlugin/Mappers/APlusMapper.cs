using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Guidance;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using WorkRecordPlugin.Mappers.GeoJson;

namespace WorkRecordPlugin.Mappers
{
    internal class APlusMapper
    {
        private PluginProperties _properties;
        private ApplicationDataModel _dataModel;
        private Dictionary<string, object> _featProps;

        public APlusMapper(PluginProperties properties, ApplicationDataModel dataModel, Dictionary<string, object> featProps)
        {
            this._properties = properties;
            this._dataModel = dataModel;
            this._featProps = featProps;
        }

        public Feature MapAsSingleFeature(APlus guidancePatternAdapt)
        {
            Point point = PointMapper.MapPoint2Point(guidancePatternAdapt.Point, _properties.AffineTransformation);
            _featProps.Add("Heading", guidancePatternAdapt.Heading.ToString());
            return new Feature(point, _featProps);
        }
    }
}
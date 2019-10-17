using GeoJSON.Net.Converters;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Utils
{
	public class GeoJsonContractResolver : DefaultContractResolver
	{
		public new static readonly GeoJsonContractResolver Instance = new GeoJsonContractResolver();

		protected override JsonContract CreateContract(Type objectType)
		{
			JsonContract contract = base.CreateContract(objectType);

			// this will only be called once and then cached
			if (objectType == typeof(Feature))
			{
				;
			}

			return contract;
		}
	}
}

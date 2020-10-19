using AgGateway.ADAPT.ApplicationDataModel.Common;
using NetTopologySuite.Geometries.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WorkRecordPlugin
{
	public class PluginProperties
	{
		public PluginProperties()
		{
			WorkRecordsToBeExported = new List<int>();
			FieldIdsWithWorkRecordsToBeExported = new List<int>();
			WorkRecordsToBeImported = new List<Guid>();
		}

		// What to Export
		public int? MaximumMappingDepth { get; set; }
		[JsonIgnore]
		public List<int> WorkRecordsToBeExported { get; set; }
		[JsonIgnore]
		public List<int> FieldIdsWithWorkRecordsToBeExported { get; set; }
		public OperationTypeEnum OperationTypeToBeExported { get; set; }

		// Other structures
		public bool Simplified { get; set; }
		public bool OperationDataInCsv { get; set; }

		// Anonymise data
		public bool Anonymise { get; set; }
		[JsonIgnore]
		public ApplyingAnonymiseValuesEnum ApplyingAnonymiseValuesPer { get; set; }
		[JsonIgnore]
		public int RandomDistance { get; set; }
		[JsonIgnore]
		public int RandomBearing { get; set; }
		[JsonIgnore]
		public AffineTransformation AffineTransformation { get; set; }

		public enum ApplyingAnonymiseValuesEnum
		{
			PerWorkRecord = 0,
			PerField = 1,
			PerParentDeviceElement = 2, // ToDo: add WorkRecordImporter.MapPerParentDeviceElement() method
			PerAdm = 3
		}

		// What to Import
		[JsonIgnore]
		public List<Guid> WorkRecordsToBeImported { get; set; }

		public InfoFile InfoFile { get; set; }

		// Format
		public CompressionEnum Compression { get; set; }

		public enum CompressionEnum
		{
			None,
			ZipUtil,
			Protobuf
		}
	}
}
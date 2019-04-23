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
using AgGateway.ADAPT.ApplicationDataModel.Common;
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
			WorkRecordsToBeImported = new List<Guid>();
		}

		// What to Export
		public int? MaximumMappingDepth { get; set; }
		[JsonIgnore]
		public List<int> WorkRecordsToBeExported { get; set; }
		public OperationTypeEnum OperationTypeToBeExported { get; set; }

		// Other structures
		public bool Simplified { get; set; }
		public bool OperationDataInCsv { get; set; }

		// Anonymized data
		public bool Anonymized { get; set; }
		[JsonIgnore]
		public int RandomDistance { get; set; }
		[JsonIgnore]
		public int RandomBearing { get; set; }

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
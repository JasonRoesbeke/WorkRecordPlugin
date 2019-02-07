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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WorkRecordPlugin
{
	public class InternalJsonSerializer
	{
		private readonly JsonSerializer _jsonSerializer;

		public InternalJsonSerializer() : this(new JsonSerializer
		{
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			TypeNameHandling = TypeNameHandling.Auto,
			//ContractResolver = new AdaptContractResolver(),
			//SerializationBinder = new InternalSerializationBinder()
		})
		{
		}

		public InternalJsonSerializer(JsonSerializer jsonSerializer)
		{
			_jsonSerializer = jsonSerializer;
		}

		public void Serialize<T>(T dataModel, string file)
		{
			using (var fileStream = File.Open(file, FileMode.Create, FileAccess.ReadWrite))
			using (var streamWriter = new StreamWriter(fileStream))
			using (var textWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented })
			{
				_jsonSerializer.Serialize(textWriter, dataModel);
			}
		}

		public T Deserialize<T>(string file)
		{
			using (var fileStream = File.Open(file, FileMode.Open))
			using (var streamReader = new StreamReader(fileStream))
			//using (var textReader = new InternalJsonTextReader(streamReader))
			using (var textReader = new JsonTextReader(streamReader))
			{
				return _jsonSerializer.Deserialize<T>(textReader);
			}
		}
	}
}

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

namespace WorkRecordPlugin.Utils
{
	public class InfoFileReader
	{
		public InfoFile ReadVersionInfoModel(string filename)
		{
			if (!File.Exists(filename))
				return null;

			var fileString = File.ReadAllText(filename);

			var model = JsonConvert.DeserializeObject<InfoFile>(fileString);
			return model;
		}
	}
}

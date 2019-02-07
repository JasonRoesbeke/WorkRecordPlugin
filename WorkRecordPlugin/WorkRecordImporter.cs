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
using System;
using System.Collections.Generic;
using System.Text;
using AgGateway.ADAPT.ApplicationDataModel.ADM;

namespace WorkRecordPlugin
{
	public class WorkRecordImporter
	{
		private readonly PluginProperties ImportProperties;

		public WorkRecordImporter(PluginProperties importProperties)
		{
			ImportProperties = importProperties;
		}

		public List<ApplicationDataModel> Import(string folder)
		{
			throw new NotImplementedException();
		}
	}
}

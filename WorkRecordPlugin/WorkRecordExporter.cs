/*******************************************************************************
  * Copyright (c) 2019 CNH Industrial N.V.
  * All rights reserved. This program and the accompanying materials
  * are made available under the terms of the Eclipse Public License v1.0
  * which accompanies this distribution, and is available at
  * http://www.eclipse.org/legal/epl-v20.html
  *
  * Contributors:
  *    Jason Roesbeke - Initial version.
  *******************************************************************************/
using System;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;

namespace WorkRecordPlugin
{
	public class WorkRecordExporter
	{
		private InternalJsonSerializer _internalJsonSerializer;

		public WorkRecordExporter(InternalJsonSerializer internalJsonSerializer)
		{
			_internalJsonSerializer = internalJsonSerializer;
		}

		public void ExportWorkRecords(string exportPath, ApplicationDataModel dataModel)
		{
			// ToDo: better null checking?
			if (dataModel == null)
			{
				return;
			}
			if (dataModel.Documents == null)
			{
				return;
			}
			if (dataModel.Documents.WorkRecords == null)
			{
				// ToDo: is this check Needed?
				throw new NullReferenceException();
				return;
			}

			foreach (WorkRecord workRecord in dataModel.Documents.WorkRecords)
			{
				// Currenlty only export if workRecord contains only reference to one field
			}

		}
	}
}
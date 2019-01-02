using System;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;

namespace WorkRecordToJSONPlugin
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

			}

		}
	}
}
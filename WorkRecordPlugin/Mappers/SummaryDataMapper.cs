using System;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;

namespace WorkRecordPlugin.Mappers
{
	internal class SummaryDataMapper
	{
		private readonly ApplicationDataModel DataModel;

		public SummaryDataMapper(ApplicationDataModel dataModel)
		{
			DataModel = dataModel;
		}

		public StampedMeteredValuesDto Map(Summary summary)
		{
			if (summary.SummaryData.Count < 0)
			{
				return null;
			}
			StampedMeteredValuesDto stampedMeteredValuesDto = new StampedMeteredValuesDto();
			var endTime = summary.TimeScopes.Find(ts => ts.DateContext == AgGateway.ADAPT.ApplicationDataModel.Common.DateContextEnum.ActualEnd);
			if (endTime != null)
			{
				if (endTime.TimeStamp1 != null)
				{
					stampedMeteredValuesDto.TimeStamp = (DateTime)endTime.TimeStamp1;
				}
			}

			return stampedMeteredValuesDto;
		}
	}
}
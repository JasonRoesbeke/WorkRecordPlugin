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
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;

namespace WorkRecordPlugin.Mappers
{
	internal class SummaryDataMapper
	{
		private readonly ApplicationDataModel _dataModel;

		public SummaryDataMapper(ApplicationDataModel dataModel)
		{
			_dataModel = dataModel;
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
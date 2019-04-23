﻿/*******************************************************************************
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
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;

namespace WorkRecordPlugin.Mappers
{
	public class StampedMeteredValuesMapper
	{
		public List<StampedMeteredValuesDto> Map(List<StampedMeteredValues> data)
		{
			List<StampedMeteredValuesDto> stampedMeteredValuesDtos = new List<StampedMeteredValuesDto>();

			foreach (var stampedMeterdValue in data)
			{
				var stampedMeteredValuesDto = MapStampedMeterValue(stampedMeterdValue);
				if (stampedMeteredValuesDto != null)
				{
					stampedMeteredValuesDtos.Add(stampedMeteredValuesDto);
				}
			}

			return stampedMeteredValuesDtos;
		}

		private StampedMeteredValuesDto MapStampedMeterValue(StampedMeteredValues stampedMeteredValues)
		{
			StampedMeteredValuesDto stampedMeteredValuesDto = new StampedMeteredValuesDto();

			if (stampedMeteredValues.Values == null)
			{
				return null;
			}

			if (stampedMeteredValues.Values.Count == 0)
			{
				return null;
			}

			if (stampedMeteredValues.Stamp != null)
			{
				if (stampedMeteredValues.Stamp.TimeStamp1 != null)
				{
					stampedMeteredValuesDto.TimeStamp = (DateTime)stampedMeteredValues.Stamp.TimeStamp1;
				}
			}
			

			NumericRepresentationValueMapper numericRepresentationValueMapper = new NumericRepresentationValueMapper();
			stampedMeteredValuesDto.Values = numericRepresentationValueMapper.Map(stampedMeteredValues.Values);
			if (stampedMeteredValuesDto.Values == null)
			{
				return null;
			}
			return stampedMeteredValuesDto;
		}
	}
}
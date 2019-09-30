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
using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AutoMapper;
using ADAPT.DTOs.AutoMapperProfiles;
using ADAPT.DTOs.Representations;

namespace WorkRecordPlugin.Mappers
{
	public class NumericRepresentationValueMapper
	{
		private readonly IMapper _mapper;

		public NumericRepresentationValueMapper()
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			_mapper = config.CreateMapper();
		}

		public List<NumericRepresentationValueDto> Map(List<MeteredValue> values)
		{
			// ToDo: AutoMapper of this List!
			List<NumericRepresentationValueDto> numericRepresentationValueDtos = new List<NumericRepresentationValueDto>();
			foreach (var item in values)
			{
				if (item.Value is NumericRepresentationValue)
				{
					numericRepresentationValueDtos.Add(_mapper.Map<NumericRepresentationValue, NumericRepresentationValueDto>((NumericRepresentationValue)item.Value));
				}
			}
			return numericRepresentationValueDtos;
		}
	}
}
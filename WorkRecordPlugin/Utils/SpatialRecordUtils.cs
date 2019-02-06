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
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using AgGateway.ADAPT.Representation.UnitSystem;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;

namespace WorkRecordPlugin.Utils
{
	public class SpatialRecordUtils
	{
		private readonly IMapper mapper;

		public KeyValuePair<WorkingData, WorkingDataDto> TimeStamp { get; set; }
		public KeyValuePair<WorkingData, WorkingDataDto> Latitude { get; set; }
		public KeyValuePair<WorkingData, WorkingDataDto> Longitude { get; set; }
		public KeyValuePair<WorkingData, WorkingDataDto> Elevation { get; set; }

		public SpatialRecordUtils()
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();

			// Initalise properties
			Initialise();
		}

		private void Initialise()
		{
			TimeStamp = GetStringKvp(Representations.srTimeStamp);
			Latitude = GetNumericKvp(RepresentationInstanceList.vrLatitude.ToModelRepresentation(), "arcdeg");
			Longitude = GetNumericKvp(RepresentationInstanceList.vrLongitude.ToModelRepresentation(), "arcdeg");
			Elevation = GetNumericKvp(RepresentationInstanceList.vrElevation.ToModelRepresentation(), "m");
		}

		private KeyValuePair<WorkingData, WorkingDataDto> GetStringKvp(StringRepresentation representation)
		{
			EnumeratedWorkingData enumeratedWorkingData = GetEnumeratedWorkingData(representation);
			EnumeratedWorkingDataDto dto = mapper.Map<EnumeratedWorkingData, EnumeratedWorkingDataDto>(enumeratedWorkingData);
			dto.Guid = Guid.NewGuid();
			return new KeyValuePair<WorkingData, WorkingDataDto>(enumeratedWorkingData, dto);
		}

		private KeyValuePair<WorkingData, WorkingDataDto> GetEnumeratedKvp(AgGateway.ADAPT.ApplicationDataModel.Representations.EnumeratedRepresentation representation)
		{
			EnumeratedWorkingData enumeratedWorkingData = GetEnumeratedWorkingData(representation);
			EnumeratedWorkingDataDto dto = mapper.Map<EnumeratedWorkingData, EnumeratedWorkingDataDto>(enumeratedWorkingData);
			dto.Guid = Guid.NewGuid();
			return new KeyValuePair<WorkingData, WorkingDataDto>(enumeratedWorkingData, dto);
		}

		private EnumeratedWorkingData GetEnumeratedWorkingData(AgGateway.ADAPT.ApplicationDataModel.Representations.Representation representation)
		{
			EnumeratedWorkingData meter = new EnumeratedWorkingData();
			meter.Representation = representation;
			return meter;
		}

		private KeyValuePair<WorkingData, WorkingDataDto> GetNumericKvp(AgGateway.ADAPT.ApplicationDataModel.Representations.NumericRepresentation numericRepresentation, string uomAbbr)
		{
			NumericWorkingData numericWorkingData = GetNumericWorkingData(numericRepresentation, uomAbbr);
			NumericWorkingDataDto dto = mapper.Map<NumericWorkingData, NumericWorkingDataDto>(numericWorkingData);
			dto.Guid = Guid.NewGuid();
			return new KeyValuePair<WorkingData, WorkingDataDto>(numericWorkingData, dto);
		}

		private NumericWorkingData GetNumericWorkingData(AgGateway.ADAPT.Representation.RepresentationSystem.NumericRepresentation representation, string uomAbbr)
		{
			NumericWorkingData meter = GetNumericWorkingData(representation.ToModelRepresentation(), uomAbbr);
			return meter;
		}

		private NumericWorkingData GetNumericWorkingData(AgGateway.ADAPT.ApplicationDataModel.Representations.NumericRepresentation representation, string uomAbbr)
		{
			NumericWorkingData meter = new NumericWorkingData();
			AgGateway.ADAPT.ApplicationDataModel.Common.UnitOfMeasure uom = UnitSystemManager.GetUnitOfMeasure(uomAbbr);

			// ToDo: [AgGateway] problems with representations (ToModelRepresentation)!
			representation.Dimension = uom.Dimension;

			meter.Representation = representation;
			meter.UnitOfMeasure = uom;

			return meter;
		}

		public List<KeyValuePair<WorkingData, WorkingDataDto>> GetKvps()
		{
			return new List<KeyValuePair<WorkingData, WorkingDataDto>>
			{
				TimeStamp,
				Latitude,
				Longitude,
				Elevation
			};
		}
	}


}

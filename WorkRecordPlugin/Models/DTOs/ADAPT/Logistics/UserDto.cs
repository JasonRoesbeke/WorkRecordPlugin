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
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Logistics
{
	public class UserDto : BaseDto
	{
		public UserDto() : base("", "FirstName", "LastName")
		{
		}

		public Guid Guid { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }

		public string Email { get; set; }

		public bool IsAdmin { get; set; }

		public List<CompanyDto> Companies { get; set; }
	}
}

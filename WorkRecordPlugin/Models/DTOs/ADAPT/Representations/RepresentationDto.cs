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

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Representations
{
	public abstract class RepresentationDto : BaseDto
	{
		public RepresentationDto() : base(null, "Description")
		{

		}
		public string CodeSource { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }

		[JsonIgnore]
		public string LongDescription { get; set; }
	}
}

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
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Utils
{
	public static class Representations
	{
		public static readonly StringRepresentation srTimeStamp = new StringRepresentation() { Code = "srTimeStamp", CodeSource = RepresentationCodeSourceEnum.User_Defined, Description = "TimeStamp", LongDescription = "TimeStamp" };
	}
}

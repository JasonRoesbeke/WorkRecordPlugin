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
using AgGateway.ADAPT.Representation.UnitSystem;
using AgGateway.ADAPT.Representation.UnitSystem.ExtensionMethods;

namespace WorkRecordPlugin.Utils
{
	public static class UnitMapper
	{
		public static AgGateway.ADAPT.ApplicationDataModel.Common.UnitOfMeasure GetModelUom(string unit)
		{
			AgGateway.ADAPT.ApplicationDataModel.Common.UnitOfMeasure uom = UnitSystemManager.GetUnitOfMeasure(unit);

			if (uom == null)
			{
				UnitOfMeasure incompatibleUnit = new CompositeUnitOfMeasure(unit);
				uom = incompatibleUnit.ToModelUom();
			}

			return uom;
		}
	}
}

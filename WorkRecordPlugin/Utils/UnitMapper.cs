using AgGateway.ADAPT.Representation.UnitSystem;
using AgGateway.ADAPT.Representation.UnitSystem.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Utils
{
	public static class UnitMapper
	{
		public static AgGateway.ADAPT.ApplicationDataModel.Common.UnitOfMeasure GetModelUOM(string unit)
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

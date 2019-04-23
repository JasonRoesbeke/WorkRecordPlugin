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
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Products;

namespace WorkRecordPlugin.Mappers
{
	public class ProductMapper
	{
		private readonly ApplicationDataModel _dataModel;

		public ProductMapper(ApplicationDataModel dataModel)
		{
			_dataModel = dataModel;
		}

		public string Map(Product product)
		{
			// ToDo: create Full ProductDto!!
			return product.Description;
		}
	}
}
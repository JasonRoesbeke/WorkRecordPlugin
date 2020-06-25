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
using GeoJSON.Net.Geometry;
using System.Collections.Generic;

namespace WorkRecordPlugin.Mappers.GeoJson
{
    public class MultiLineStringMapper
    {
        #region Export
        public static GeoJSON.Net.Geometry.MultiLineString MapMultiLineString(List<LineString> lineStrings)
        {
            return new GeoJSON.Net.Geometry.MultiLineString(lineStrings);
        }
        #endregion
    }
}
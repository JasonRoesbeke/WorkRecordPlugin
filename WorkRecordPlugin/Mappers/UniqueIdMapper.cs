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
using AgGateway.ADAPT.ApplicationDataModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers
{
	static class UniqueIdMapper
	{
		// ToDo: Create a LinkList file for each workRecord!
		/* LinkList idea:
		 * For each GUID: all the uniqueIds + as 'checksum' the ADAPT ReferenceId
		 * In WebAPI could be an URL to save bytes in file
		 */
		public static Guid GetUniqueGuid(CompoundIdentifier id, string preferredSource = null)
		{
			if (id.UniqueIds.Count == 0)
			{
				// ToDo: log that there are no UniqueId for that ADAPT entity and that there has been created a new one
				return Guid.NewGuid();
			}

			Guid guid;

			// 1st: preferredSource
			if (preferredSource != null)
			{
				if (GetUniqueIdFromSource(id, preferredSource, out guid))
				{
					return guid;
				}
			}

			// 2nd: if idType is UUID
			if (GetUniqueId(id.UniqueIds.Where(ui => ui.IdType == IdTypeEnum.UUID), out guid))
			{
				return guid;
			}

			// 3rd: if idType is String
			if (GetUniqueId(id.UniqueIds.Where(ui => ui.IdType == IdTypeEnum.String), out guid))
			{
				return guid;
			}

			// 4rd: if idType is LongInt
			// ToDo: [AgGateway] Ask AgGateway if IdTypeEnum.LongInt is int32
			if (GetUniqueGuidFromLong(id.UniqueIds, out guid))
			{
				return guid;
			}
			// 5th: try LongInt as string
			if (GetUniqueId(id.UniqueIds.Where(ui => ui.IdType == IdTypeEnum.LongInt), out guid))
			{
				return guid;
			}

			// 5th: if idType is URI
			if (GetUniqueId(id.UniqueIds.Where(ui => ui.IdType == IdTypeEnum.URI), out guid))
			{
				return guid;
			}

			// not succesfull, generate new GUID
			// ToDo: need to save all UniqueIds in the DTO
			return Guid.NewGuid();
		}

		private static bool GetUniqueIdFromSource(CompoundIdentifier id, string preferredSource, out Guid guid)
		{
			var preferredUniqueIds = id.UniqueIds.Where(ud => ud.Source == preferredSource).ToList();
			if (preferredUniqueIds.Any())
			{
				foreach (var uniqueId in preferredUniqueIds)
				{
					if (Guid.TryParse(uniqueId.Id, out guid))
					{
						return true;
					}
				}
			}
			guid = Guid.Empty;
			return false;
		}

		private static bool GetUniqueId(IEnumerable<UniqueId> guidUniqueIds, out Guid guid)
		{
			var uniqueIds = guidUniqueIds.ToList();
			if (uniqueIds.Count() > 0)
			{
				foreach (var guidUniqueId in uniqueIds)
				{
					if (Guid.TryParse(guidUniqueId.Id, out guid))
					{
						return true;
					}
				}
			}
			guid = Guid.Empty;
			return false;
		}

		private static bool GetUniqueGuidFromLong(List<UniqueId> uniqueIds, out Guid guid)
		{
			var list = uniqueIds.Where(ui => ui.IdType == IdTypeEnum.LongInt).ToList();

			if (list.Any())
			{
				foreach (var uniqueId in list)
				{
					if (long.TryParse(uniqueId.Id, out long longInt))
					{
						guid = longInt.LongToGuid();
						return true;
					}
				}
			}
			guid = Guid.Empty;
			return false;
		}

		public static UniqueId GetUniqueId(Guid guid, InfoFile infoFile)
		{
			return new UniqueId
			{
				Id = guid.ToString(),
				IdType = IdTypeEnum.UUID,
				SourceType = IdSourceTypeEnum.URI,
				Source = infoFile.NamePlugin + " " + infoFile.VersionPlugin
			};
		}
	}
}

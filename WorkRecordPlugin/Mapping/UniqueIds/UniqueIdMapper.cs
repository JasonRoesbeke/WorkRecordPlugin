using AgGateway.ADAPT.ApplicationDataModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mapping.UniqueIds
{
	static class UniqueIdMapper
	{
		static public Guid? GetUniqueId(CompoundIdentifier id, string preferredSource = null)
		{
			if (id.UniqueIds.Count == 0)
			{
				return null;
			}

			Guid? guid;

			// 1st: preferredSource
			if (preferredSource != null)
			{
				guid = GetUniqueIdFromSource(id, preferredSource);
				if (guid != null)
				{
					return guid;
				}
			}

			// 2nd: if idType is UUID
			guid = GetUniqueId(id.UniqueIds.Where(ui => ui.IdType == IdTypeEnum.UUID));
			if (guid != null)
			{
				return guid;
			}

			// 3rd: if idType is String
			guid = GetUniqueId(id.UniqueIds.Where(ui => ui.IdType == IdTypeEnum.String));
			if (guid != null)
			{
				return guid;
			}

			// 4rd: if idType is LongInt
			// ToDo: Ask AgGateway if IdTypeEnum.LongInt is int32
			guid = GetUniqueIdFromLong(id.UniqueIds);
			if (guid != null)
			{
				return guid;
			}
			// 5th: try LongInt as string
			guid = GetUniqueId(id.UniqueIds.Where(ui => ui.IdType == IdTypeEnum.LongInt));
			if (guid != null)
			{
				return guid;
			}

			// 5th: if idType is URI
			guid = GetUniqueId(id.UniqueIds.Where(ui => ui.IdType == IdTypeEnum.URI));
			if (guid != null)
			{
				return guid;
			}

			// not succesfull, return null!
			return null;
		}

		static private Guid? GetUniqueIdFromSource(CompoundIdentifier id, string preferredSource)
		{
			var preferredUniqueIds = id.UniqueIds.Where(ud => ud.Source == preferredSource);
			if (preferredUniqueIds.Any())
			{
				foreach (var uniqueId in preferredUniqueIds)
				{
					if (Guid.TryParse(uniqueId.Id, out Guid guid))
					{
						return guid;
					}
				}
			}
			return null;
		}

		static private Guid? GetUniqueId(IEnumerable<UniqueId> GuidUniqueIds)
		{
			if (GuidUniqueIds.Count() > 0)
			{
				foreach (var guidUniqueId in GuidUniqueIds)
				{
					if (Guid.TryParse(guidUniqueId.Id, out Guid guid))
					{
						return guid;
					}
				}
			}
			return null;
		}

		static private Guid? GetUniqueIdFromLong(List<UniqueId> uniqueIds)
		{
			var list = uniqueIds.Where(ui => ui.IdType == IdTypeEnum.LongInt);

			if (list.Any())
			{
				foreach (var uniqueId in list)
				{
					if (long.TryParse(uniqueId.Id, out long longInt))
					{
						return longInt.LongToGuid();
					}
				}
			}
			return null;
		}		
	}
}

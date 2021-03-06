﻿using AgGateway.ADAPT.ApplicationDataModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers
{
	static class UniqueIdMapper
	{
		private static readonly string UniqueIdSourceCNH = "http://www.cnhindustrial.com";
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

				if (preferredSource == UniqueIdSourceCNH)
				{
					if (GetUniqueIdFromCNHId(id, out guid))
					{
						return guid;
					}
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

		private static bool GetUniqueIdFromCNHId(CompoundIdentifier id, out Guid guid)
		{
			guid = new Guid();
			var CnhId = id.UniqueIds.FirstOrDefault(ui => ui.Source == UniqueIdSourceCNH);
			if (CnhId == null)
			{
				return false;
			}
			if (CnhId.Id.Length != 8 )
			{
				return false;
			}

			using (MD5 md5 = MD5.Create())
			{
				byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(CnhId.Id));
				guid = new Guid(hash);
				return true;
			}
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

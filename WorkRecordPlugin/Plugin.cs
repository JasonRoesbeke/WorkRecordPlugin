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
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WorkRecordPlugin.Mappers;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Utils;
using static WorkRecordPlugin.PluginProperties;

namespace WorkRecordPlugin
{
	public class Plugin : IPlugin
	{
		private readonly InternalJsonSerializer _internalJsonSerializer;
		private readonly WorkRecordExporter _workRecordExporter;
		private readonly InfoFileReader _infoFileReader;

		// ToDo: "context": "url...",
		// ToDo: "version plugin": "x.x.x-pre-alpha"
		// ToDo: "Modified": "DateTime"

		public Plugin() : this(new InternalJsonSerializer())
		{
			Errors = new List<IError>();
			//ToDo: using ProtoBuf
			//ToDo: Memory optimisation
		}

		public Plugin(InternalJsonSerializer internalJsonSerializer)
		{
			_internalJsonSerializer = internalJsonSerializer;
			// ToDo _workRecordImporter = new WorkRecordImporter(_internalJsonSerializer);
			_workRecordExporter = new WorkRecordExporter(_internalJsonSerializer);
			CustomProperties = new PluginProperties();
			_infoFileReader = new InfoFileReader(AssemblyVersion);
		}

		public string Name { get { return "WorkRecord Plugin - IoF2020"; } }

		public string Version
		{
			get
			{
				return AssemblyVersion.ToString();
			}
		}

		public Version AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version;
			}
		}

		public string Owner { get { return "CNH Industrial"; } }

		public PluginProperties CustomProperties { get; private set; }

		public IList<IError> Errors { get; set; }

		public Properties GetProperties(string dataPath)
		{
			// ToDo GetProperties of a file
			return new Properties();
		}

		public void Initialize(string args = null)
		{
		}

		public bool IsDataCardSupported(string path, AgGateway.ADAPT.ApplicationDataModel.ADM.Properties properties = null)
		{
			List<string> workRecordFolders = GetListOfWorkRecordFolders(path);
			return workRecordFolders.Any();
		}

		private List<string> GetListOfWorkRecordFolders(string path)
		{
			List<string> workRecordFolders = new List<string>();
			// [Check] if dataPath is the rootfolder of one workRecord folder
			if (_infoFileReader.ValidateMinimalFolderStructure(path))
			{
				workRecordFolders.Add(path);
				return workRecordFolders;
			}
			// If not, [check] if any subdirectories is a workRecord folder
			else
			{
				/* dataPath may contains multiple workRecord subfolders
                 * Subfolders must have a name starting with "InfoFileConstants.PluginFolder" & containing correct "InfoFileWorkRecordExport.json" with same PluginAssemblyVersion
				 * SubSubfolder are not checked
                 */
				string[] subfolders = Directory.GetDirectories(path, InfoFileConstants.PluginFolderPrefix + "*", SearchOption.TopDirectoryOnly);
				foreach (string folder in subfolders)
				{
					if (_infoFileReader.ValidateMinimalFolderStructure(folder))
					{
						workRecordFolders.Add(folder);
					}
				}
				return workRecordFolders;
			}
		}

		public IList<IError> ValidateDataOnCard(string dataPath, AgGateway.ADAPT.ApplicationDataModel.ADM.Properties properties = null)
		{
			// ToDo: When mapping to ADAPT, log errors
			return new List<IError>();
		}

		public IList<ApplicationDataModel> Import(string dataPath, AgGateway.ADAPT.ApplicationDataModel.ADM.Properties properties = null)
		{
			List<string> workRecordFolders = GetListOfWorkRecordFolders(dataPath);
			if (!workRecordFolders.Any())
			{
				return null;
			}

			


			List<ApplicationDataModel> adms = new List<ApplicationDataModel>();

			foreach (string folder in workRecordFolders)
			{
				var infoFile = _infoFileReader.ReadVersionInfoModel(folder);
				if (infoFile == null)
				{
					continue;
				}
				ParseImportProperties(properties, infoFile);

				WorkRecordImporter workRecordImporter = new WorkRecordImporter(CustomProperties);
				//Deserialize each workRecord.json
				List<WorkRecordDto> workRecordDtos = WorkRecordImporter.ReadFolder(folder);
				//Map each WorkRecordDto to a seperated adm
				var dataModels = workRecordImporter.Import(workRecordDtos);
				if (dataModels != null)
				{
					adms.AddRange(dataModels);
				}
			}

			return adms;
		}

		

		public void Export(ApplicationDataModel dataModel, string exportPath, AgGateway.ADAPT.ApplicationDataModel.ADM.Properties properties = null)
		{
			ParseExportProperties(properties);

			if (!Directory.Exists(exportPath))
				Directory.CreateDirectory(exportPath);

			// Path of exportfolder: "PluginFolder - [Name of Catalog]"
			var newPath = Path.Combine(exportPath, InfoFileConstants.PluginFolderPrefix + "-" + ZipUtils.GetSafeName(dataModel.Catalog.Description));

			WorkRecordMapper _workRecordsMapper = new WorkRecordMapper(dataModel, CustomProperties);
			_workRecordExporter.WriteInfoFile(newPath, Name, Version, dataModel.Catalog.Description, CustomProperties);
			// ToDo: check if dataModel contains workrecords
			foreach (var workRecord in dataModel.Documents.WorkRecords)
			{
				WorkRecordDto fieldWorkRecordDto = null;
				if (CustomProperties.WorkRecordsToBeExported.Any())
				{
					// Export only the requested WorkRecords
					if (CustomProperties.WorkRecordsToBeExported.Contains(workRecord.Id.ReferenceId))
					{
						fieldWorkRecordDto = _workRecordsMapper.Map(workRecord);
					}
				}
				else // Export all WorkRecords
				{
					fieldWorkRecordDto = _workRecordsMapper.Map(workRecord);
				}

				if (fieldWorkRecordDto != null)
				{
					bool success = _workRecordExporter.Write(newPath, fieldWorkRecordDto);
				}
				fieldWorkRecordDto = null; // Memory optimisation?
			}
		}

		private void ParseExportProperties(AgGateway.ADAPT.ApplicationDataModel.ADM.Properties properties)
		{
			// MaximumMappingDepth
			var prop = properties.GetProperty(GetPropertyName(() => CustomProperties.MaximumMappingDepth));
			if (prop != null && int.TryParse(prop, out int depth))
			{
				CustomProperties.MaximumMappingDepth = depth;
			}
			// WorkRecordsToBeExported
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.WorkRecordsToBeExported));
			if (prop != null)
			{
				ParseReferenceIdArray(prop);
			}
			// OperationTypeToBeExported
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.OperationTypeToBeExported));
			if (prop != null && Enum.TryParse(prop, out OperationTypeEnum operationTypeEnum))
			{
				CustomProperties.OperationTypeToBeExported = operationTypeEnum;
			}

			// Simplified
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.Simplified));
			if (prop != null && bool.TryParse(prop, out bool simplified))
			{
				CustomProperties.Simplified = simplified;
			}
			// Anonymized
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.Anonymized));
			if (prop != null && bool.TryParse(prop, out bool anonymized))
			{
				CustomProperties.Anonymized = anonymized;
			}
			// CompressionEnum
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.Compression));
			if (prop != null && Enum.TryParse(prop, out CompressionEnum result))
			{
				CustomProperties.Compression = result;
			}

			// OperationDataInCSV
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.OperationDataInCSV));
			if (prop != null && bool.TryParse(prop, out bool operationDataInCSV))
			{
				CustomProperties.OperationDataInCSV = operationDataInCSV;
			}
		}

		private void ParseReferenceIdArray(string prop)
		{
			List<string> StringArray = prop.Split(';').ToList();
			foreach (var propValue in StringArray)
			{
				if (int.TryParse(propValue, out int referenceId))
				{
					CustomProperties.WorkRecordsToBeExported.Add(referenceId);
				}
			}
		}

		private List<string> ParseStringArray(string prop)
		{
			throw new NotImplementedException();
		}

		private void ParseImportProperties(AgGateway.ADAPT.ApplicationDataModel.ADM.Properties properties, InfoFile infoFile)
		{
			// Simplified
			var prop = properties.GetProperty(GetPropertyName(() => CustomProperties.Simplified));
			if (prop != null && bool.TryParse(prop, out bool simplified))
			{
				CustomProperties.Simplified = simplified;
			}
			// Anonymized
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.Anonymized));
			if (prop != null && bool.TryParse(prop, out bool anonymized))
			{
				CustomProperties.Anonymized = anonymized;
			}
			// CompressionEnum
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.Compression));
			if (prop != null && Enum.TryParse(prop, out CompressionEnum result))
			{
				CustomProperties.Compression = result;
			}

			// OperationDataInCSV
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.OperationDataInCSV));
			if (prop != null && bool.TryParse(prop, out bool operationDataInCSV))
			{
				CustomProperties.OperationDataInCSV = operationDataInCSV;
			}

			// To get Source string for UniqueIds
			CustomProperties.InfoFile = infoFile;
		}

		// <summary>
		// Get the name of a static or instance property from a property access lambda.
		// </summary>
		// <typeparam name="T">Type of the property</typeparam>
		// <param name="propertyLambda">lambda expression of the form: '() => Class.Property' or '() => object.Property'</param>
		// <returns>The name of the property</returns>
		public string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
		{
			var me = propertyLambda.Body as MemberExpression;

			if (me == null)
			{
				throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
			}

			return me.Member.Name;
		}
	}
}

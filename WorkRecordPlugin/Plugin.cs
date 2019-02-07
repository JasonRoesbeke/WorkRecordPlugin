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
		public List<IError> ErrorList1 { get; private set; }

		public Plugin() : this(new InternalJsonSerializer())
		{
			//ToDo: using ProtoBuf
			//ToDo: Memory optimisation
		}

		public Plugin(InternalJsonSerializer internalJsonSerializer)
		{
			_internalJsonSerializer = internalJsonSerializer;
			// ToDo _workRecordImporter = new WorkRecordImporter(_internalJsonSerializer);
			_workRecordExporter = new WorkRecordExporter(_internalJsonSerializer);
			Properties = new PluginProperties();
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
						workRecordFolders.Add(path);
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

			ParseImportProperties(properties);

			List<ApplicationDataModel> adms = new List<ApplicationDataModel>();

			foreach (string folder in workRecordFolders)
			{
				//Deserialize each workRecord.json as a seperate adm 
				WorkRecordImporter workRecordImporter = new WorkRecordImporter(CustomProperties);
				List<ApplicationDataModel> dataModels = workRecordImporter.Import(folder);
				adms.AddRange(dataModels);
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

			WorkRecordMapper _workRecordsMapper = new WorkRecordMapper(dataModel, Properties);
			_workRecordExporter.WriteInfoFile(newPath, Name, Version, dataModel.Catalog.Description, Properties);
			// ToDo: check if dataModel contains workrecords
			foreach (var workRecord in dataModel.Documents.WorkRecords)
			{
				WorkRecordDto fieldWorkRecordDto = _workRecordsMapper.Map(workRecord);
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
			var prop = properties.GetProperty(GetPropertyName(() => PluginProperties.MaximumMappingDepth));
			if (prop != null && int.TryParse(prop, out int depth))
			{
				PluginProperties.MaximumMappingDepth = depth;
			}
			// WorkRecordsToBeExported
			prop = properties.GetProperty(GetPropertyName(() => PluginProperties.WorkRecordsToBeExported));
			if (prop != null)
			{
				if (int.TryParse(prop, out int referenceId))
				{
					PluginProperties.WorkRecordsToBeExported.Add(referenceId);
				}
			}
			// OperationTypeToBeExported
			prop = properties.GetProperty(GetPropertyName(() => PluginProperties.OperationTypeToBeExported));
			if (prop != null && Enum.TryParse(prop, out OperationTypeEnum operationTypeEnum))
			{
				PluginProperties.OperationTypeToBeExported = operationTypeEnum;
			}

			// Simplified
			prop = properties.GetProperty(GetPropertyName(() => PluginProperties.Simplified));
			if (prop != null && bool.TryParse(prop, out bool simplified))
			{
				PluginProperties.Simplified = simplified;
			}
			// Anonymized
			prop = properties.GetProperty(GetPropertyName(() => PluginProperties.Anonymized));
			if (prop != null && bool.TryParse(prop, out bool anonymized))
			{
				PluginProperties.Anonymized = anonymized;
			}
			// CompressionEnum
			prop = properties.GetProperty(GetPropertyName(() => PluginProperties.Compression));
			if (prop != null && Enum.TryParse(prop, out CompressionEnum result))
			{
				PluginProperties.Compression = result;
			}



			// OperationDataInCSV
			prop = properties.GetProperty(GetPropertyName(() => PluginProperties.OperationDataInCSV));
			if (prop != null && bool.TryParse(prop, out bool operationDataInCSV))
			{
				PluginProperties.OperationDataInCSV = operationDataInCSV;
			}
		}

		private void ParseImportProperties(AgGateway.ADAPT.ApplicationDataModel.ADM.Properties properties)
		{
			// Simplified
			var prop = properties.GetProperty(GetPropertyName(() => PluginProperties.Simplified));
			if (prop != null && bool.TryParse(prop, out bool simplified))
			{
				PluginProperties.Simplified = simplified;
			}
			// Anonymized
			prop = properties.GetProperty(GetPropertyName(() => PluginProperties.Anonymized));
			if (prop != null && bool.TryParse(prop, out bool anonymized))
			{
				PluginProperties.Anonymized = anonymized;
			}
			// CompressionEnum
			prop = properties.GetProperty(GetPropertyName(() => PluginProperties.Compression));
			if (prop != null && Enum.TryParse(prop, out CompressionEnum result))
			{
				PluginProperties.Compression = result;
			}

			// OperationDataInCSV
			prop = properties.GetProperty(GetPropertyName(() => PluginProperties.OperationDataInCSV));
			if (prop != null && bool.TryParse(prop, out bool operationDataInCSV))
			{
				PluginProperties.OperationDataInCSV = operationDataInCSV;
			}
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

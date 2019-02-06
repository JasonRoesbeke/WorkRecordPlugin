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
using static WorkRecordPlugin.ExportProperties;

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
			ExportProperties = new ExportProperties();
			_infoFileReader = new InfoFileReader();
		}

		public string Name { get { return "WorkRecord Plugin - IoF2020"; } }
		public string Version
		{
			get
			{
				var version = Assembly.GetExecutingAssembly().GetName().Version;
				return version.ToString();
			}
		}
		public string Owner { get { return "CNH Industrial"; } }

		public ExportProperties ExportProperties { get; private set; }


		public Properties GetProperties(string dataPath)
		{
			// ToDo GetProperties of a file
			return new Properties();
		}

		public void Initialize(string args = null)
		{
		}

		public bool IsDataCardSupported(string path, Properties properties = null)
		{
			if (!path.EndsWith(InfoFileConstants.PluginFolder))
			{
				path = Path.Combine(path, InfoFileConstants.PluginFolder);
			}

			// Check if folder contains any json files
			if (!(Directory.Exists(path) && Directory.GetFiles(path, String.Format(InfoFileConstants.FileFormat, "*"), SearchOption.AllDirectories).Any()))
			{
				return false;
			}



			// First read root folder
			// Read InfoFileName & check if same version
			var fileName = Path.Combine(path, InfoFileConstants.InfoFileName);
			var infoFile = _infoFileReader.ReadVersionInfoModel(fileName);
			if (infoFile == null)
			{
				// ToDo: through subDirectories
				return false;
			}

			// Check stated versions are equal to current ADAPT- & pluginVersion
			if (infoFile.ADAPTVersion != Assembly.LoadFrom("AgGateway.ADAPT.ApplicationDataModel.dll").GetName().Version.ToString() || infoFile.VersionPlugin != Version)
			{
				return false;
			}

			// ToDo: Import support
			return true;
		}

		public IList<IError> ValidateDataOnCard(string dataPath, Properties properties = null)
		{
			// ToDo: When mapping to ADAPT, log errors
			return new List<IError>();
		}

		public IList<ApplicationDataModel> Import(string dataPath, Properties properties = null)
		{
			if(!IsDataCardSupported(dataPath, properties))
			{
				return null;
			}

			return null;
		}

		public void Export(ApplicationDataModel dataModel, string exportPath, Properties properties = null)
		{
			ParseExportProperties(properties);

			if (!Directory.Exists(exportPath))
				Directory.CreateDirectory(exportPath);

			// ToDo: versionFile/Header containing additional metadata (version plugin, version ADAPT, date of conversion, origin such as TASKDATA folder/catalog/datacard description...)
			var newPath = Path.Combine(exportPath, ZipUtils.GetSafeName(dataModel.Catalog.Description));

			WorkRecordMapper _workRecordsMapper = new WorkRecordMapper(dataModel, ExportProperties);
			_workRecordExporter.WriteInfoFile(newPath, Name, Version, dataModel.Catalog.Description, ExportProperties);
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

		private void ParseExportProperties(Properties properties)
		{
			// MaximumMappingDepth
			var prop = properties.GetProperty(GetPropertyName(() => ExportProperties.MaximumMappingDepth));
			if (prop != null && int.TryParse(prop, out int depth))
			{
				ExportProperties.MaximumMappingDepth = depth;
			}
			// WorkRecordsToBeExported
			prop = properties.GetProperty(GetPropertyName(() => ExportProperties.WorkRecordsToBeExported));
			if (prop != null)
			{
				if (int.TryParse(prop, out int referenceId))
				{
					ExportProperties.WorkRecordsToBeExported.Add(referenceId);
				}
			}
			// OperationTypeToBeExported
			prop = properties.GetProperty(GetPropertyName(() => ExportProperties.OperationTypeToBeExported));
			if (prop != null && Enum.TryParse(prop, out OperationTypeEnum operationTypeEnum))
			{
				ExportProperties.OperationTypeToBeExported = operationTypeEnum;
			}

			// Simplified
			prop = properties.GetProperty(GetPropertyName(() => ExportProperties.Simplified));
			if (prop != null && bool.TryParse(prop, out bool simplified))
			{
				ExportProperties.Simplified = simplified;
			}
			// Anonymized
			prop = properties.GetProperty(GetPropertyName(() => ExportProperties.Anonymized));
			if (prop != null && bool.TryParse(prop, out bool anonymized))
			{
				ExportProperties.Anonymized = anonymized;
			}
			// CompressionEnum
			prop = properties.GetProperty(GetPropertyName(() => ExportProperties.Compression));
			if (prop != null && Enum.TryParse(prop, out CompressionEnum result))
			{
				ExportProperties.Compression = result;
			}



			// OperationDataInCSV
			prop = properties.GetProperty(GetPropertyName(() => ExportProperties.OperationDataInCSV));
			if (prop != null && bool.TryParse(prop, out bool operationDataInCSV))
			{
				ExportProperties.Anonymized = operationDataInCSV;
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

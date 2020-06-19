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
using ADAPT.DTOs.Documents;
using WorkRecordPlugin.Utils;
using static WorkRecordPlugin.PluginProperties;
using GeoJSON.Net.Feature;
using AgGateway.ADAPT.ApplicationDataModel.Prescriptions;

namespace WorkRecordPlugin
{
	public class Plugin : IPlugin
	{
		private readonly JsonExporter _JsonExporter;
		private readonly InfoFileReader _infoFileReader;

		private Dictionary<int, string> _adaptIdMap = new Dictionary<int, string>();

		// ToDo: "context": "url...",
		// ToDo: "version plugin": "x.x.x-pre-alpha"
		// ToDo: "Modified": "DateTime"

		public Plugin() : this(new InternalJsonSerializer())
		{
			//ToDo: using ProtoBuf
			//ToDo: Memory optimisation
		}

		public Plugin(InternalJsonSerializer internalJsonSerializer)
		{
			// ToDo _workRecordImporter = new WorkRecordImporter(_internalJsonSerializer);
			_JsonExporter = new JsonExporter(internalJsonSerializer);
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

		public string Owner { get { return "AgGateway"; } }

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

		public bool IsDataCardSupported(string path, Properties properties = null)
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

		public IList<IError> ValidateDataOnCard(string dataPath, Properties properties = null)
		{
			// ToDo: When mapping to ADAPT, log errors
			return new List<IError>();
		}

		public IList<ApplicationDataModel> Import(string dataPath, Properties properties = null)
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

		

		public void Export(ApplicationDataModel dataModel, string exportPath, Properties properties = null)
		{
			ParseExportProperties(properties);

			if (!Directory.Exists(exportPath))
				Directory.CreateDirectory(exportPath);

			// Path of exportfolder: "PluginFolder - [Name of Catalog]"
			var newPath = Path.Combine(exportPath, InfoFileConstants.PluginFolderPrefix + "-" + ZipUtils.GetSafeName(dataModel.Catalog.Description));

			WorkRecordMapper workRecordsMapper = new WorkRecordMapper(dataModel, CustomProperties);
			// ToDo: add more meta data of this export
			_JsonExporter.WriteInfoFile(newPath, Name, Version, dataModel.Catalog.Description, CustomProperties);

			switch (CustomProperties.ApplyingAnonymiseValuesPer)
			{
				case ApplyingAnonymiseValuesEnum.PerField:
					foreach (var fieldId in dataModel.Catalog.Fields.Select(f => f.Id.ReferenceId))
					{
						List<int> workRecordIds =
							dataModel.Documents.WorkRecords
								.Where(wr => wr.FieldIds.Contains(fieldId))
								.Select(wr => wr.Id.ReferenceId)
								.ToList();

						if (CustomProperties.Anonymise)
						{
							// Randomize the Anonymization values
							AnonymizeUtils.GenerateRandomAffineTransformation(CustomProperties);
						};

						// OperationDataMapper
						// ToDo: replace with OperationDataMapper
						//var workRecordDtos = workRecordsMapper.MapAll(workRecordIds);
						//_JsonExporter.WriteWorkRecordDtos(newPath, workRecordDtos);

						// FieldBoundaryMapper
						// ToDo: FieldBoundaryMapper.MapAsSingleFeature([fieldId])

						// GuidanceGroupMapper
						// ToDo: GuidanceGroupMapper.MapAs...([all GuidanceGroups for fieldId])

						// WorkItemOperationMapper
						// ToDo: GuidanceGroupMapper.MapAs...([all WorkItemOperations for fieldId])

					}
					break;
				case ApplyingAnonymiseValuesEnum.PerWorkRecord:
					foreach (var workRecord in dataModel.Documents.WorkRecords)
					{
						if (CustomProperties.Anonymise)
						{
							// Randomize the Anonymization values
							AnonymizeUtils.GenerateRandomAffineTransformation(CustomProperties);
						};

						// OperationDataMapper
						// ToDo: replace with OperationDataMapper
						//WorkRecordDto workRecordDto_mapped = workRecordsMapper.MapSingle(workRecord);
						//_JsonExporter.WriteWorkRecordDto(newPath, workRecordDto_mapped);

						// FieldBoundaryMapper
						// ToDo: FieldBoundaryMapper.MapAsMultipleFeatures([all FieldBoundaries for workRecord.Id])

						// GuidanceGroupMapper
						// ToDo: GuidanceGroupMapper.MapAs...([all GuidanceGroups for workRecord.Id])

						// WorkItemOperationMapper
						// ToDo: GuidanceGroupMapper.MapAs...([all WorkItemOperations for workRecord.Id])

					}
					break;
				default:
					if (CustomProperties.Anonymise)
					{
						// Randomize the Anonymization values
						AnonymizeUtils.GenerateRandomAffineTransformation(CustomProperties);
					};
					foreach (var workRecord in dataModel.Documents.WorkRecords)
					{

						// OperationDataMapper
						// ToDo: OperationDataMapper.MapAsFeatureCollection([workRecord])

					}
					FieldBoundaryMapper fieldBoundaryMapper = new FieldBoundaryMapper(CustomProperties, dataModel);
					foreach (var fieldBoundary in dataModel.Catalog.FieldBoundaries)
					{

						// FieldBoundaryMapper
						Feature fieldBoundaryFeature = fieldBoundaryMapper.MapAsSingleFeature(fieldBoundary);
						string fileName = FieldBoundaryMapper.GetFieldBoundaryPrefix();
						if (fieldBoundaryFeature.Properties.ContainsKey("FieldId"))
						{
							fileName = fileName + "_for_field_" + fieldBoundaryFeature.Properties["FieldId"];
						}
						else if (fieldBoundaryFeature.Properties.ContainsKey("Guid"))
						{
							fileName = fileName + "_" + fieldBoundaryFeature.Properties["Guid"];
						}
						else
						{
							fileName = fileName + "_" + Guid.NewGuid();
						}

						_JsonExporter.WriteAsGeoJson(newPath, new List<Feature>() { fieldBoundaryFeature }, fileName);


					}
					foreach (var guidanceGroup in dataModel.Catalog.GuidanceGroups)
					{

						// GuidanceGroupMapper
						// ToDo: FieldBoundaryMapper.MapAs...([guidanceGroup])

						// Todo: [Check] if all dataModel.Catalog.GuidancePatterns has been mapped

					}
					// @ToDo where is gridType?
					int gridType = 1;
					if (properties != null)
					{
						Int32.TryParse(properties.GetProperty("GridType"), out gridType);
						if (gridType != 1 && gridType != 2)
						{
							Console.WriteLine($"Invalid Grid Type {gridType}.");
						}
					}
					//Prescriptions
					List<Feature> prescriptionFeatures = new List<Feature>();
					List<Feature> prescriptionFeaturesSingle = new List<Feature>();
					PrescriptionMapper prescriptionMapper = new PrescriptionMapper(CustomProperties, dataModel);
					foreach (var workItemOperation in dataModel.Documents.WorkItemOperations)
                    {
                        Console.WriteLine("WorkItemOperation: " + workItemOperation.Description + " OperationType " + workItemOperation.OperationType + " gridType " + gridType);

                        Prescription adaptPrescription = dataModel.Catalog.Prescriptions.Where(f => f.Id.ReferenceId == workItemOperation.PrescriptionId).FirstOrDefault();
                        if (adaptPrescription != null)
                        {
                            Feature prescriptionFeature = prescriptionMapper.MapAsSingleFeature(adaptPrescription, gridType);
                            prescriptionFeature.Properties.Add("OperationType", Enum.GetName(typeof(OperationTypeEnum), workItemOperation.OperationType));	// Enum: 
                            //workItemOperationFeature.Properties.Add("Description", workItemOperation.Description);
                            //workItemOperationFeature.Properties.Add("ID", workItemOperation.Id);
                            prescriptionFeaturesSingle.Add(prescriptionFeature);

                            prescriptionFeatures.AddRange(prescriptionMapper.MapAsMultipleFeatures(adaptPrescription, gridType));
                        }

                        // Todo: [Check] if all dataModel.Catalog.Prescriptions has been mapped
                        if (dataModel.Catalog.Prescriptions.Count() != prescriptionFeaturesSingle.Count)
                        {
							Console.WriteLine("Count prescriptions and features (single) differ: " + dataModel.Catalog.Prescriptions.Count() +" "+ prescriptionFeaturesSingle.Count);
						}
                        
                    }

                    string fileNameP;
					// @ToDo only when count > 0?
					fileNameP = PrescriptionMapper.GetWorkItemOperationPrefix();
					fileNameP = fileNameP + "_prescriptions_single_" + Guid.NewGuid();
					_JsonExporter.WriteAsGeoJson(newPath, prescriptionFeaturesSingle, fileNameP);

					fileNameP = PrescriptionMapper.GetWorkItemOperationPrefix();
					fileNameP = fileNameP + "_prescriptions_" + Guid.NewGuid();
					_JsonExporter.WriteAsGeoJson(newPath, prescriptionFeatures, fileNameP);
					break;
			}
		}

		//public List<WorkRecordDto> ExportToWorkRecordDtos(ApplicationDataModel dataModel, Properties properties = null, bool anonymize = false, ApplyingAnonymiseValuesEnum anonymiseMethod = ApplyingAnonymiseValuesEnum.PerWorkRecord)
		//{
		//	ParseExportProperties(properties);
		//	var workRecordDtos = new List<WorkRecordDto>();


		//	WorkRecordMapper workRecordsMapper = new WorkRecordMapper(dataModel, CustomProperties);
		//	switch (anonymiseMethod)
		//	{
		//		case ApplyingAnonymiseValuesEnum.PerField:
		//			foreach (var fieldId in dataModel.Catalog.Fields.Select(f => f.Id.ReferenceId))
		//			{
		//				List<int> workRecordIds =
		//					dataModel.Documents.WorkRecords
		//						.Where(wr => wr.FieldIds.Contains(fieldId))
		//						.Select(wr => wr.Id.ReferenceId)
		//						.ToList();
		//				workRecordDtos.AddRange(workRecordsMapper.MapAll(workRecordIds));

						
		//			}
		//			break;
		//		case ApplyingAnonymiseValuesEnum.PerWorkRecord:
		//		default:
		//			foreach (var workRecord in dataModel.Documents.WorkRecords)
		//			{
		//				WorkRecordDto workRecordDto_mapped = workRecordsMapper.MapSingle(workRecord);
		//				workRecordDtos.Add(workRecordDto_mapped);						
		//			}
		//			break;
		//	}
		//	return workRecordDtos;
		//}

		private void ParseExportProperties(Properties properties)
		{
			if (properties == null)
			{
				CustomProperties.Anonymise = false;
				CustomProperties.ApplyingAnonymiseValuesPer = ApplyingAnonymiseValuesEnum.PerAdm; // Default
				return;
			}

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
				CustomProperties.WorkRecordsToBeExported.AddRange(ParseReferenceIdArray(prop));
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

			// Anonymise
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.Anonymise));
			if (prop != null && bool.TryParse(prop, out bool anonymized))
			{
				CustomProperties.Anonymise = anonymized;
			}

			// ApplyingAnonymiseValuesPer
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.ApplyingAnonymiseValuesPer));
			if (prop != null && Enum.TryParse(prop, out ApplyingAnonymiseValuesEnum enumResult))
			{
				CustomProperties.ApplyingAnonymiseValuesPer = enumResult;
			}

			// FieldIdsWithWorkRecordsToBeExported
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.FieldIdsWithWorkRecordsToBeExported));
			if (prop != null)
			{
				CustomProperties.FieldIdsWithWorkRecordsToBeExported.AddRange(ParseReferenceIdArray(prop));
			}

			// CompressionEnum
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.Compression));
			if (prop != null && Enum.TryParse(prop, out CompressionEnum enumResult2))
			{
				CustomProperties.Compression = enumResult2;
			}

			// OperationDataInCSV
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.OperationDataInCsv));
			if (prop != null && bool.TryParse(prop, out bool operationDataInCsv))
			{
				CustomProperties.OperationDataInCsv = operationDataInCsv;
			}
		}

		private List<int> ParseReferenceIdArray(string prop)
		{
			var IdList = new List<int>();
			List<string> stringArray = prop.Split(';').ToList();
			foreach (var propValue in stringArray)
			{
				if (int.TryParse(propValue, out int referenceId))
				{
					IdList.Add(referenceId);
				}
			}

			return IdList;
		}

		private void ParseImportProperties(Properties properties, InfoFile infoFile)
		{
			// Simplified
			var prop = properties.GetProperty(GetPropertyName(() => CustomProperties.Simplified));
			if (prop != null && bool.TryParse(prop, out bool simplified))
			{
				CustomProperties.Simplified = simplified;
			}
			// Anonymise
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.Anonymise));
			if (prop != null && bool.TryParse(prop, out bool anonymized))
			{
				CustomProperties.Anonymise = anonymized;
			}
			// CompressionEnum
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.Compression));
			if (prop != null && Enum.TryParse(prop, out CompressionEnum result))
			{
				CustomProperties.Compression = result;
			}

			// OperationDataInCSV
			prop = properties.GetProperty(GetPropertyName(() => CustomProperties.OperationDataInCsv));
			if (prop != null && bool.TryParse(prop, out bool operationDataInCsv))
			{
				CustomProperties.OperationDataInCsv = operationDataInCsv;
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

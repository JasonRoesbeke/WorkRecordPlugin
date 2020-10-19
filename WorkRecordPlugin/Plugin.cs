using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WorkRecordPlugin.Mappers;
using WorkRecordPlugin.Utils;
using static WorkRecordPlugin.PluginProperties;
using GeoJSON.Net.Feature;
using AgGateway.ADAPT.ApplicationDataModel.Prescriptions;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.ApplicationDataModel.FieldBoundaries;


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
			throw new NotImplementedException();
		}

		public void Export(ApplicationDataModel dataModel, string exportPath, Properties properties = null)
		{
			ParseExportProperties(properties);

			if (!Directory.Exists(exportPath))
				Directory.CreateDirectory(exportPath);

			// Path of exportfolder: "PluginFolder - [Name of Catalog]"
			var newPath = Path.Combine(exportPath, InfoFileConstants.PluginFolderPrefix + "-" + ZipUtils.GetSafeName(dataModel.Catalog.Description));

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
						// ToDo: WorkItemOperationMapper.MapAs...([all WorkItemOperations for fieldId])

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
						string fileNamePrescriptions = FieldBoundaryMapper.GetFieldBoundaryPrefix();
						if (fieldBoundaryFeature.Properties.ContainsKey("FieldId"))
						{
							fileNamePrescriptions = fileNamePrescriptions + "_for_field_" + fieldBoundaryFeature.Properties["FieldId"];
						}
						else if (fieldBoundaryFeature.Properties.ContainsKey("Guid"))
						{
							fileNamePrescriptions = fileNamePrescriptions + "_" + fieldBoundaryFeature.Properties["Guid"];
						}
						else
						{
							fileNamePrescriptions = fileNamePrescriptions + "_" + Guid.NewGuid();
						}

						_JsonExporter.WriteAsGeoJson(newPath, new List<Feature>() { fieldBoundaryFeature }, fileNamePrescriptions);


                        // DrivenHeadlandMapper
                        foreach (Headland headland in fieldBoundary.Headlands)
                        {
                            if (headland is DrivenHeadland)
                            {
                                DrivenHeadlandMapper drivenHeadlandMapper = new DrivenHeadlandMapper(CustomProperties, dataModel);
                                Feature drivenHeadlandFeature = drivenHeadlandMapper.MapAsSingleFeature(headland as DrivenHeadland, fieldBoundaryFeature);
                                if (drivenHeadlandFeature == null)
                                    drivenHeadlandMapper = null;
                                else
                                {
                                    string drivenHeadlandFileName = DrivenHeadlandMapper.GetPrefix();
                                    if (drivenHeadlandFeature.Properties.ContainsKey("FieldId"))
                                        drivenHeadlandFileName += "_for_field_" + drivenHeadlandFeature.Properties["FieldId"];
                                    _JsonExporter.WriteAsGeoJson(newPath, new List<Feature>() { drivenHeadlandFeature }, drivenHeadlandFileName);
                                }
                            }
                        }

					}
					GuidanceGroupMapper guidanceGroupMapper = new GuidanceGroupMapper(CustomProperties, dataModel);
					foreach (var guidanceGroup in dataModel.Catalog.GuidanceGroups)
					{

						// GuidanceGroupMapper
						List<Feature> guidanceGroupFeatures = guidanceGroupMapper.MapAsMultipleFeatures(guidanceGroup);
						string fileNameGuidanceGroup = GuidanceGroupMapper.GetPrefix();
						if (guidanceGroupFeatures[0] != null)
						{
                            if (guidanceGroupFeatures[0].Properties.ContainsKey("GuidancePatternType"))
                            {
                                fileNameGuidanceGroup = fileNameGuidanceGroup + "_type_" + guidanceGroupFeatures[0].Properties["GuidancePatternType"];
                            }
                            if (guidanceGroupFeatures[0].Properties.ContainsKey("FieldId"))
                            {
                            	fileNameGuidanceGroup = fileNameGuidanceGroup + "_for_field_" + guidanceGroupFeatures[0].Properties["FieldId"];
                            }
                            else if (guidanceGroupFeatures[0].Properties.ContainsKey("Guid"))
                            {
                            	fileNameGuidanceGroup = fileNameGuidanceGroup + "_" + guidanceGroupFeatures[0].Properties["Guid"];
                            }
                            else
                            {
                            	fileNameGuidanceGroup = fileNameGuidanceGroup + "_" + Guid.NewGuid();
                            }
						}

						_JsonExporter.WriteAsGeoJson(newPath, guidanceGroupFeatures, fileNameGuidanceGroup);

					}
					
					//Prescriptions (without gridType)
					List<Feature> prescriptionFeatures = new List<Feature>();
					List<Feature> prescriptionFeaturesSingle = new List<Feature>();
					PrescriptionMapper prescriptionMapper = new PrescriptionMapper(CustomProperties, dataModel);
					foreach (var workItemOperation in dataModel.Documents.WorkItemOperations)
                    {
                        Console.WriteLine("WorkItemOperation: " + workItemOperation.Description + " OperationType " + workItemOperation.OperationType);

                        Prescription adaptPrescription = dataModel.Catalog.Prescriptions.Where(f => f.Id.ReferenceId == workItemOperation.PrescriptionId).FirstOrDefault();
                        if (adaptPrescription != null)
                        {
							// single
							var prescriptionFeature = prescriptionMapper.MapAsSingleFeature(adaptPrescription);
							if (prescriptionFeature != null)
                            {
								prescriptionFeature.Properties.Add("OperationType", Enum.GetName(typeof(OperationTypeEnum), workItemOperation.OperationType));  // Enum: 
								//workItemOperationFeature.Properties.Add("Description", workItemOperation.Description);
								//workItemOperationFeature.Properties.Add("ID", workItemOperation.Id);
								prescriptionFeaturesSingle.Add(prescriptionFeature);
							}
							else Console.WriteLine("prescriptionFeature single null for: " + workItemOperation.PrescriptionId);

							// multiple
							var features = prescriptionMapper.MapAsMultipleFeatures(adaptPrescription);
							if (features != null && features.Count > 0)
								prescriptionFeatures.AddRange(features);
							else Console.WriteLine("prescriptionFeatures null or empty for: " + workItemOperation.PrescriptionId);
						}
						else Console.WriteLine("adaptPrescription not found : " + workItemOperation.PrescriptionId);
					}

					// Todo: [Check] if all dataModel.Catalog.Prescriptions has been mapped
					if (dataModel.Catalog.Prescriptions.Count() != dataModel.Documents.WorkItemOperations.Count())
						Console.WriteLine("Count prescriptions and WorkItemOperations differ: " + dataModel.Catalog.Prescriptions.Count() + " " + dataModel.Documents.WorkItemOperations.Count());
					//else 
					//	Console.WriteLine("Count prescriptions and WorkItemOperations same: " + dataModel.Catalog.Prescriptions.Count() + " " + dataModel.Documents.WorkItemOperations.Count());

					string fileNamePs;
					// @ToDo only when count > 0?
					fileNamePs = PrescriptionMapper.GetWorkItemOperationPrefix();
					fileNamePs = fileNamePs + "_single_" + Guid.NewGuid();
					_JsonExporter.WriteAsGeoJson(newPath, prescriptionFeaturesSingle, fileNamePs);

					string fileNamePm;
					fileNamePm = PrescriptionMapper.GetWorkItemOperationPrefix();
					fileNamePm = fileNamePm + "_" + Guid.NewGuid();
					_JsonExporter.WriteAsGeoJson(newPath, prescriptionFeatures, fileNamePm);

					// LoggedData
					OperationTimelogMapper operationTimelogMapper = new OperationTimelogMapper(CustomProperties, dataModel);

					// starting from workRecords --> Not doing this as some loggedData may have not been referenced in a workrecord; we want to catch all logged data in the adm
					// starting from LoggedData
					foreach (var loggedData in dataModel.Documents.LoggedData)
					{
						foreach (OperationData operation in loggedData.OperationData)
						{
							Console.WriteLine("OperationTimelog - operationData: " + operation.Id.ReferenceId + " " + operation.OperationType + " maxDepth " + operation.MaxDepth);

							IEnumerable<SpatialRecord> spatialRecords = operation.GetSpatialRecords != null ? operation.GetSpatialRecords() : null;
							if (spatialRecords != null && spatialRecords.Any()) //No need to export a timelog if no data
							{
								var operationTimelogFeatures = operationTimelogMapper.MapMultiple(operation, spatialRecords);

								string fileNameL = OperationTimelogMapper.GetPrefix() + "_" + operation.OperationType + "_" + Guid.NewGuid();
								_JsonExporter.WriteAsGeoJson(newPath, operationTimelogFeatures, fileNameL);
							}
						}
					}

					break;
			}
		}

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

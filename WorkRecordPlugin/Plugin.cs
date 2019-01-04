/*******************************************************************************
  * Copyright (c) 2019 CNH Industrial N.V.
  * All rights reserved. This program and the accompanying materials
  * are made available under the terms of the Eclipse Public License v1.0
  * which accompanies this distribution, and is available at
  * http://www.eclipse.org/legal/epl-v20.html
  *
  * Contributors:
  *    Jason Roesbeke - Initial version.
  *******************************************************************************/
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WorkRecordPlugin.Mappers;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;

namespace WorkRecordPlugin
{
	public class Plugin : IPlugin
	{
		private readonly InternalJsonSerializer _internalJsonSerializer;
		private readonly WorkRecordExporter _workRecordExporter;

		public List<IError> ErrorList1 { get; private set; }

		public Plugin() : this(new InternalJsonSerializer())
		{
			//ToDo: using ProtoBuf;
		}

		public Plugin(InternalJsonSerializer internalJsonSerializer)
		{
			_internalJsonSerializer = internalJsonSerializer;
			// ToDo _workRecordImporter = new WorkRecordImporter(_internalJsonSerializer);
			_workRecordExporter = new WorkRecordExporter(_internalJsonSerializer);

		}

		public string Name { get { return "WorkRecord Plugin IoF2020"; } }
		public string Version
		{
			get
			{
				var version = Assembly.GetExecutingAssembly().GetName().Version;
				return version.ToString();
			}
		}
		public string Owner { get { return "CNH Industrial"; } }


		public Properties GetProperties(string dataPath)
		{
			return new Properties();
		}

		public void Initialize(string args = null)
		{
			// ToDo: add "Data anonymization" option in the plugin!
		}

		public bool IsDataCardSupported(string dataPath, Properties properties = null)
		{
			return false;
		}

		public IList<IError> ValidateDataOnCard(string dataPath, Properties properties = null)
		{
			// ToDo: When mapping to ADAPT, log errors
			return new List<IError>();
		}

		public IList<ApplicationDataModel> Import(string dataPath, Properties properties = null)
		{
			// ToDo: reading the generated JSON files and mapping to ADAPT
			return null;
		}

		public void Export(ApplicationDataModel dataModel, string exportPath, Properties properties = null)
		{
			if (!Directory.Exists(exportPath))
				Directory.CreateDirectory(exportPath);

			WorkRecordMapper _workRecordMapper = new WorkRecordMapper();
			// ToDo: check if dataModel contains workrecords
			List<FieldWorkRecordDto> FieldWorkRecordDtos = _workRecordMapper.MapWorkRecords(dataModel);

			// ToDo: versionFile/Header containing additional metadata (version plugin, version ADAPT, date of conversion, origin such as CN1 folder/catalog/datacard description...)
			foreach (var fieldWorkRecordDto in FieldWorkRecordDtos)
			{
				bool success = _workRecordExporter.Write(exportPath, fieldWorkRecordDto);
			}
		}
	}
}

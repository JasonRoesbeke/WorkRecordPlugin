using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GeoJSON.Net.Feature;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin
{
	public class JsonExporter
	{
		private InternalJsonSerializer _internalJsonSerializer;

		public JsonExporter(InternalJsonSerializer internalJsonSerializer)
		{
			_internalJsonSerializer = internalJsonSerializer;
		}

		public bool WriteInfoFile(string path, string name, string version, string description, PluginProperties exportProperties)
		{
			var adaptVersion = "2.0.4";
			return WriteJson(path, new InfoFile(name, version, adaptVersion, description, exportProperties, DateTime.Now), InfoFileConstants.InfoFileName);
		}

		public bool WriteAsGeoJson(string path, List<Feature> features, string fileName)
		{
			FeatureCollection featureCollection = new FeatureCollection(features);
			return WriteJson(path, featureCollection, fileName);
		}

		public bool WriteJson<T>(string path, T objectToSerialize, string fileName) where T : class
		{
			var jsonFormat = Path.GetTempFileName();
			try
			{
				// Ensure path exists
				Directory.CreateDirectory(path);

				_internalJsonSerializer.Serialize(objectToSerialize, jsonFormat);
				var safeFileName = ZipUtils.GetSafeName(fileName);
				// ToDo: The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters.
				// ToDo: add option to zip, using ZipUtil => +-8% of original size
				//ZipUtil.Zip(Path.Combine(path, fileName + ".zip"), jsonFormat);

				var exportFileName = Path.Combine(path, safeFileName + InfoFileConstants.JsonFileExtension);
				// Check if no file is already created with same name
				if (File.Exists(exportFileName))
				{
					exportFileName = AlreadyExits(exportFileName);

					//// Check if is the same file. Is not, then do not overwrite
					//FileInfo alreadyExitsFile = new FileInfo(exportFileName);
					//FileInfo tempFile = new FileInfo(jsonFormat);
					//if (alreadyExitsFile.Length != tempFile.Length)
					//{
					//	exportFileName = AlreadyExits(exportFileName);
					//}

				}

				File.Copy(jsonFormat, exportFileName, true);
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: " + e.Message);
				return false;
			}
			finally
			{
				try
				{
					File.Delete(jsonFormat);
				}
				catch
				{
				}
			}
			return true;
		}

		private string AlreadyExits(string fullPath)
		{
			int count = 1;

			string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
			string extension = Path.GetExtension(fullPath);
			string path = Path.GetDirectoryName(fullPath) ?? "";
			string newFullPath = fullPath;

			while (File.Exists(newFullPath))
			{
				string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
				newFullPath = Path.Combine(path, tempFileName + extension);
			}
			return newFullPath;
		}
	}
}
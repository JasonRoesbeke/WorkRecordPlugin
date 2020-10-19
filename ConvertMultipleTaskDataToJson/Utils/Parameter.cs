using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConvertMultipleTaskDataToJson.Utils
{
	public static class Parameters
	{
		private static readonly string _jsonExtension = ".json";

		public static ConsoleParameters ParseArguments(string[] args)
		{
			ConsoleParameters parameters = new ConsoleParameters();

			// ToDo: if no args is given, read the "name to be defined"-file for parameters in same directory as console application

			// Command line parsing
			Arguments CommandLine = new Arguments(args);

			// PluginsFolder
			parameters.PluginsFolderPath = ParameterFound(CommandLine, parameters, GetPropertyName(() => parameters.PluginsFolderPath));
			if (parameters.PluginsFolderPath == null)
			{
				return null;
			}

			#region Import parameters
			// [Required] ImportPluginName
			parameters.ImportPluginName = ParameterFound(CommandLine, parameters, GetPropertyName(() => parameters.ImportPluginName));
			if (parameters.ImportPluginName == null)
			{
				return null;
			}
			// [Required] ImportDataPath
			parameters.ImportDataPath = ParameterFound(CommandLine, parameters, GetPropertyName(() => parameters.ImportDataPath));
			if (parameters.ImportDataPath == null)
			{
				return null;
			}
			// ImportPluginVersion - Not required. If not given, then latest version
			parameters.ImportPluginVersion = ParameterFound(CommandLine, parameters, GetPropertyName(() => parameters.ImportPluginVersion));
			// ImportPluginPropertiesFile - Not required.
			parameters.ImportPluginPropertiesFile = ParameterFound(CommandLine, parameters, GetPropertyName(() => parameters.ImportPluginPropertiesFile));
			#endregion

			#region Export parameters
			// [Required] ExportPluginName
			parameters.ExportPluginName = ParameterFound(CommandLine, parameters, GetPropertyName(() => parameters.ExportPluginName));
			if (parameters.ExportPluginName == null)
			{
				return null;
			}
			// [Required] ExportDataPath
			parameters.ExportDataPath = ParameterFound(CommandLine, parameters, GetPropertyName(() => parameters.ExportDataPath));
			if (parameters.ExportDataPath == null)
			{
				return null;
			}
			// ExportPluginVersion - Not required. If not given, then latest version
			parameters.ExportPluginVersion = ParameterFound(CommandLine, parameters, GetPropertyName(() => parameters.ExportPluginVersion));
			// ExportPluginPropertiesFile  - Not required.
			parameters.ExportPluginPropertiesFile = ParameterFound(CommandLine, parameters, GetPropertyName(() => parameters.ExportPluginPropertiesFile));
			#endregion

			return parameters;
		}

		private static string ParameterFound(Arguments commandLine, ConsoleParameters parameters, string parameterName)
		{
			string parameter = null;
			if (commandLine[parameterName] != null)
			{
				parameter = commandLine[parameterName];
				if (parameterName.ToLower().EndsWith("path") || parameterName.ToLower().EndsWith("file"))
				{
					if (File.Exists(parameter))
					{
						FileInfo fileInfo = new FileInfo(parameter);
						if (!(fileInfo.Extension == _jsonExtension))
						{
							Console.WriteLine($"{parameterName} is not of type {_jsonExtension}!");
							return null;
						}
					}
					else if (!Directory.Exists(parameter))
					{
						Console.WriteLine($"{parameterName} does not exists!");
						return null;
					}
				}
			}
			else
			{
				Console.WriteLine($"{parameterName} not given!");
				return null;
			}
			return parameter;
		}

		// ToDo: change to Private?
		/// <summary>
		/// Get the name of a static or instance property from a property access lambda.
		/// </summary>
		/// <typeparam name="T">Type of the property</typeparam>
		/// <param name="propertyLambda">lambda expression of the form: '() => Class.Property' or '() => object.Property'</param>
		/// <returns>The name of the property</returns>
		private static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
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

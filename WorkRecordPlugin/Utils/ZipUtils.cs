using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace WorkRecordPlugin.Utils
{
	public static class ZipUtils
	{
		public static void Zip(string file, string tempFile)
		{
			using (var openTempStream = File.Open(tempFile, FileMode.Open))
			using (var admFileStream = File.Open(file, FileMode.Create, FileAccess.ReadWrite))
			using (var gzipStream = new GZipStream(admFileStream, CompressionLevel.Optimal))
			{
				openTempStream.CopyTo(gzipStream);
			}
		}

		public static void Unzip(string file, string tempFile)
		{
			using (var openStream = File.Open(file, FileMode.Open))
			using (var admStream = File.Open(tempFile, FileMode.Create, FileAccess.ReadWrite))
			using (var gzipStream = new GZipStream(openStream, CompressionMode.Decompress))
			{
				gzipStream.CopyTo(admStream);
			}
		}

		public static string GetSafeName(string filename)
		{
			return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
		}
	}
}

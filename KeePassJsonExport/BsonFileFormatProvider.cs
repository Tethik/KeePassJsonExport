using System;
using KeePass.DataExchange;
using KeePassLib;
using System.IO;
using KeePassLib.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace KeePassJsonExport
{
	public class BsonFileFormatProvider : JsonFileFormatProvider
	{

		public override bool SupportsExport {
			get {
				return true;
			}
		}

		public override bool SupportsImport { 
			get { return true; } 
		}

		public override bool SupportsUuids
		{
			get { return true; }
		}

		public override string FormatName { 
			get { return "bson"; } 
		} 

		public override string DisplayName {
			get { return "BSON File Format (Not Encrypted)"; }
		}

		public BsonFileFormatProvider ()
		{
		}

		/// <summary>
		/// Default file name extension, without leading dot.
		/// If there are multiple default/equivalent extensions
		/// (like e.g. "html" and "htm"), specify all of them
		/// separated by a '|' (e.g. "html|htm").
		/// </summary>
		public override string DefaultExtension
		{
			get { return "bson"; }
		}

		/// <summary>
		/// Import a stream into a database. Throws an exception if an error
		/// occurs. Do not call the base class method when overriding it.
		/// </summary>
		/// <param name="pwStorage">Data storage into which the data will be imported.</param>
		/// <param name="sInput">Input stream to read the data from.</param>
		/// <param name="slLogger">Status logger. May be <c>null</c>.</param>
		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{	
			var serializer = new JsonSerializer();
			using (var jsonReader = new BsonReader (sInput)) {
				var root = serializer.Deserialize<GroupModel> (jsonReader);
				pwStorage.RootGroup = TraverseIntoAndWriteToDb (root, pwStorage);
			}
		}

		private string GenerateJsonForEntry(PwEntry entry) {
			var obj = new Dictionary<string,string> ();	
			foreach (var kp in entry.Strings) {
				obj[kp.Key] = kp.Value.ReadString();
			}	
			return JsonConvert.SerializeObject(obj);
		}



		/// <summary>
		/// Export data into a stream. Throws an exception if an error
		/// occurs (like writing to stream fails, etc.). Returns <c>true</c>,
		/// if the export was successful.
		/// </summary>
		/// <param name="pwExportInfo">Contains the data source and detailed
		/// information about which entries should be exported.</param>
		/// <param name="sOutput">Output stream to write the data to.</param>
		/// <param name="slLogger">Status logger. May be <c>null</c>.</param>
		/// <returns>Returns <c>false</c>, if the user has aborted the export
		/// process (like clicking Cancel in an additional export settings
		/// dialog).</returns>
		public override bool Export(PwExportInfo pwExportInfo, Stream sOutput,
			IStatusLogger slLogger)
		{
			using (var writer = new BsonWriter(sOutput)) {				
				JsonSerializer serializer = new JsonSerializer();

				// serialize product to BSON
				var rootNode = TraverseAndWriteGroupToStream (pwExportInfo.DataGroup);	

				serializer.Serialize(writer, rootNode);
			}

			return true;
		}



	}
}


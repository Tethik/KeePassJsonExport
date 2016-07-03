using System;
using KeePass.DataExchange;
using KeePassLib;
using System.IO;
using KeePassLib.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace KeePassJsonExport
{
	public class JsonFileFormatProvider : FileFormatProvider
	{

		public override bool SupportsExport {
			get {
				return true;
			}
		}

		public override bool SupportsImport { 
			get { return false; } 
		}

		public override string FormatName { 
			get { return "json"; } 
		} 

		public override string DisplayName {
			get { return "JSON File Format"; }
		}

		public JsonFileFormatProvider ()
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
			get { return "json"; }
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
	
			throw new NotSupportedException();
		}

		private string GenerateJsonForEntry(PwEntry entry) {
			var obj = new Dictionary<string,string> ();	
			foreach (var kp in entry.Strings) {
				obj[kp.Key] = kp.Value.ReadString();
			}	
			return JsonConvert.SerializeObject(obj);
		}

		private void TraverseAndWriteGroupToStream(PwGroup group, StreamWriter writer, IStatusLogger slLogger) {
			foreach (var entry in group.Entries) {			
				var json = GenerateJsonForEntry (entry);
				writer.Write(json);
				//	slLogger.SetProgress ((uint) (100 * (++i / (float) pwExportInfo.DataGroup.Entries.UCount)));
			}

			foreach (var subgroup in group.Groups) {
				TraverseAndWriteGroupToStream(subgroup, writer, slLogger);
			}
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
			using (var writer = new StreamWriter(sOutput)) {				
				// I could just convert a list of objects, however this way I dont have to buffer as much text.
				writer.Write ('[');
				TraverseAndWriteGroupToStream (pwExportInfo.ContextDatabase.RootGroup, writer, slLogger);	
				writer.Write (']');
			}

			return true;
		}



	}
}


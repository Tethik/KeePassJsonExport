using System;
using KeePass.DataExchange;
using KeePassLib.Security;
using KeePassLib;
using System.IO;
using KeePassLib.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using KeePassLib.Utility;

namespace KeePassJsonExport
{
	public class JsonFileFormatProvider : FileFormatProvider
	{

		public override bool SupportsExport {
			get {
				return true;
			}
		}

		public override bool SupportsImport 
		{ 
			get { return true; } 
		}

		public override bool SupportsUuids
		{
			get { return true; }
		}

		public override string FormatName { 
			get { return "json"; } 
		} 

		public override string DisplayName {
			get { return "JSON File Format (Not Encrypted)"; }
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
			var serializer = new JsonSerializer();
			using (var jsonReader = new JsonTextReader (new StreamReader (sInput))) {
				var root = serializer.Deserialize<GroupModel> (jsonReader);
				pwStorage.RootGroup = TraverseIntoAndWriteToDb (root, pwStorage);
			}
		}

		protected String GetDefault(PwEntry entry, String key) {			
			return entry.Strings.Exists (key) ? entry.Strings.Get (key).ReadString() : "";
		}

		protected PwGroup TraverseIntoAndWriteToDb(GroupModel current, PwDatabase pwStorage) {			
			PwGroup group = new PwGroup (false, false);
			group.Name = current.Name;
			group.Uuid = new PwUuid (MemUtil.HexStringToByteArray(current.Uuid));
			group.CreationTime = current.Created;
			group.LastModificationTime = current.Modified;
			group.LastAccessTime = current.LastAccess;
			group.ExpiryTime = current.Expiry;
			group.Expires = current.Expires;

			foreach (var entry in current.Entries) {
				var pwEntry = new PwEntry (false, true);
				pwEntry.Strings.Set (PwDefs.PasswordField, 
					new ProtectedString(pwStorage.MemoryProtection.ProtectPassword, entry.Password));
				pwEntry.Strings.Set (PwDefs.UserNameField,
					new ProtectedString(pwStorage.MemoryProtection.ProtectUserName, entry.Username));
				pwEntry.Strings.Set (PwDefs.UrlField, 
					new ProtectedString(pwStorage.MemoryProtection.ProtectUrl, entry.URL));
				pwEntry.Strings.Set (PwDefs.TitleField, 
					new ProtectedString(pwStorage.MemoryProtection.ProtectTitle, entry.Title));
				pwEntry.Strings.Set (PwDefs.NotesField, 
					new ProtectedString(pwStorage.MemoryProtection.ProtectNotes, entry.Notes));
				pwEntry.Uuid = new PwUuid (MemUtil.HexStringToByteArray(entry.Uuid));
				pwEntry.CreationTime = entry.Created;
				pwEntry.LastModificationTime = entry.Modified;
				pwEntry.LastAccessTime = entry.LastAccess;
				pwEntry.ExpiryTime = entry.Expiry;
				pwEntry.Expires = entry.Expires;
				group.AddEntry (pwEntry, true);
			}

			foreach (var subgroup in current.Subgroups) {
				group.AddGroup(TraverseIntoAndWriteToDb (subgroup, pwStorage), true);				
			}

			return group;
		}


		protected GroupModel TraverseAndWriteGroupToStream(PwGroup group) {			
			var root = new GroupModel { 
				Name = group.Name, 
				Uuid = group.Uuid.ToString(),
				Created = group.CreationTime,
				Modified = group.LastModificationTime,
				LastAccess = group.LastAccessTime,
				Expiry = group.ExpiryTime,
				Expires = group.Expires
			};

			foreach (var entry in group.Entries) {			
				if (entry == null || entry.Strings == null)
					continue;

				Console.WriteLine (entry.ToString ());
				var entryModel = new EntryModel {
					Uuid = entry.Uuid.ToString(),
					Password = GetDefault(entry, PwDefs.PasswordField),
					Title = GetDefault(entry, PwDefs.TitleField),
					Username = GetDefault (entry, PwDefs.UserNameField),
					URL = GetDefault(entry, PwDefs.UrlField),
					Notes = GetDefault(entry, PwDefs.NotesField),
					Created = entry.CreationTime,
					Expires = entry.Expires,
					Expiry = entry.ExpiryTime,
					LastAccess = entry.LastAccessTime,
					Modified = entry.LastModificationTime,
				};
				root.Entries.Add (entryModel);
			}

			foreach (var subgroup in group.Groups) {
				var groupModel = TraverseAndWriteGroupToStream(subgroup);
				root.Subgroups.Add(groupModel);
			}

			return root;
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
				JsonSerializer serializer = new JsonSerializer();

				// serialize product to BSON
				var rootNode = TraverseAndWriteGroupToStream (pwExportInfo.DataGroup);	

				serializer.Serialize(writer, rootNode);
			}

			return true;
		}



	}
}


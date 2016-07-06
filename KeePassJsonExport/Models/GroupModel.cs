using System;
using System.Collections.Generic;

namespace KeePassJsonExport
{
	/// <summary>
	/// JSON Model for Groups.
	/// </summary>
	public class GroupModel : ObjectMetadata
	{
		public GroupModel ()
		{
			Entries = new List<EntryModel> ();
			Subgroups = new List<GroupModel> ();
		}

		public string Name { get; set; }

		public List<EntryModel> Entries { get; set; }

		public List<GroupModel> Subgroups { get; set; } 

	}
}


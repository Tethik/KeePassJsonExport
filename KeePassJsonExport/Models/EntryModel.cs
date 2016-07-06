using System;

namespace KeePassJsonExport
{
	public class EntryModel : ObjectMetadata
	{
		public EntryModel ()
		{
			
		}
			
		public string Title { get; set; }

		public string Password { get; set; }

		public string Username { get; set; }

		public string URL { get; set; }

		public string Notes { get; set; }
	
	}
}


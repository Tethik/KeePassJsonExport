using System;

namespace KeePassJsonExport
{
	public abstract class ObjectMetadata
	{
		public string Uuid { get; set; }

		public DateTime Created { get; set; }

		public bool Expires { get; set; }

		public DateTime Expiry { get; set; }

		public DateTime LastAccess { get; set; }

		public DateTime Modified { get; set; }
	}
}


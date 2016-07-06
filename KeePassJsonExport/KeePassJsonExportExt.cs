using System;
using KeePass.Plugins;

namespace KeePassJsonExport
{
	public sealed class KeePassJsonExportExt : Plugin
	{
		private IPluginHost m_host = null;

		public override bool Initialize(IPluginHost host)
		{
			m_host = host;

			m_host.FileFormatPool.Add (new JsonFileFormatProvider ());
			m_host.FileFormatPool.Add (new BsonFileFormatProvider ());

			return true;
		}

		public override void Terminate()
		{
		}
	}
}


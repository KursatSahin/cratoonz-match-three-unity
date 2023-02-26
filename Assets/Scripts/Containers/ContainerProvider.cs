using System;

namespace Containers
{
    public class ContainerProvider : IDisposable
    {
        public PrefabContainer PrefabContainer => PrefabContainer.Instance;
        public BoardSettingsContainer BoardSettingsContainer  => BoardSettingsContainer.Instance;
        public GemContainer GemContainer => GemContainer.Instance;

        public ContainerProvider()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}
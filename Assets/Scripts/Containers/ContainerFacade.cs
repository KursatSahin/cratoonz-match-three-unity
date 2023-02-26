namespace Containers
{
    public static class ContainerFacade
    {
        public static BoardSettingsContainer BoardSettingsProvider => BoardSettingsContainer.Instance;
        public static PrefabContainer PrefabProvider => PrefabContainer.Instance;
        public static GemContainer GemProvider => GemContainer.Instance;
    }
}
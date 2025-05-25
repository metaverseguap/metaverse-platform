using LDR.SUAI_Metaverse.SDK.Core.Types.Devices;
using MainMenu.Containers;

namespace OfflineScene
{
    /// <summary>
    /// <para>Константы офлайн сцены.</para>
    /// </summary>
    public static class OfflineSceneConstants
    {
        public static readonly SceneInfo SCENE_INFO = new SceneInfo()
        {
            Name = "Void",
            DisplayName = "Lobby",
            Device = Device.PC
        };
    }
}
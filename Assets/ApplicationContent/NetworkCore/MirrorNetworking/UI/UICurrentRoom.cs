using Localization;

namespace NetworkCore.MirrorNetworking.UI
{
    /// <summary>
    /// <para>Отображение названия текущей комнаты на ui.</para>
    /// </summary>
    public sealed class UICurrentRoom : UIAbstractSmartTextLocalization
    {
        /// <summary>
        /// <inheritdoc cref="UIAbstractSmartTextLocalization.StringArgs"/>
        /// </summary>
        /// <returns><inheritdoc cref="UIAbstractSmartTextLocalization.StringArgs"/></returns>
        protected override object[] StringArgs()
        {
            string currentRoomName = "Offline";
            
            if (MVNetworkManager.IsOnline())
            {
                MVNetworkManager networkManager = MVNetworkManager.singleton;
                currentRoomName = networkManager.NetworkStore.Scenes.CurrentScene?.DisplayName;
            }
            
            return new object[] { currentRoomName };
        }
    }
}
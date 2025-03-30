using Mirror;
using TMPro;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Player.Base
{
    /// <summary>
    /// <para>Компонент отображающий имя пользователя.</para>
    /// </summary>
    [RequireComponent(typeof(RotateTowardsMainCamera))]
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransformReliable))]
    public sealed class NetworkPlayerDisplayName : MonoBehaviour
    {
        [SerializeField] private TMP_Text _displayName;
        
        private NetworkBasePlayer _networkPlayer;

        /// <summary>
        /// Сетевой игрок, чье имя отображается в данном Display Name.
        /// </summary>
        public NetworkBasePlayer NetworkPlayer
        {
            get => _networkPlayer;
            set
            {
                _networkPlayer = value;
                ChangeDisplayName(_networkPlayer.DisplayName);
            }
        }

        private void ChangeDisplayName(string displayName)
        {
            _displayName.text = displayName;
        }
    }
}
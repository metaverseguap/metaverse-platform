using TMPro;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Player.Base
{
    /// <summary>
    /// <para>Компонент отображающий имя пользователя.</para>
    /// </summary>
    [RequireComponent(typeof(RotateTowardsMainCamera))]
    public sealed class NetworkPlayerDisplayName : MonoBehaviour
    {
        [SerializeField] private NetworkBasePlayer _networkPlayer;
        [SerializeField] private TMP_Text _displayName;

        private void Start()
        {
            ChangeDisplayName(_networkPlayer.DisplayName);
        }

        private void ChangeDisplayName(string displayName)
        {
            _displayName.text = displayName;
        }
    }
}
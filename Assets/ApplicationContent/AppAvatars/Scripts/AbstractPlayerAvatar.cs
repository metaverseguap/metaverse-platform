using AppAvatars.AvatarSetups;
using NetworkCore.MirrorNetworking.Types.Devices;
using Player.EmbeddedPlayers;
using UnityEngine;

namespace AppAvatars
{
    /// <summary>
    /// <para>Класс объекта игрока.</para>
    ///
    /// Данный класс используется как внешняя точка доступа к аватару игрока из внешних скриптов.
    /// Например, игрок, при взаимодействии со шкафом меняющим одежду аватару,
    /// должен воспроизвести анимацию переодевания, а затем изменить свой аватар на аватар в другой одежде.
    /// Компонент шкафа получит из данного класса аватар игрока и применит на него анимацию переодевания.
    /// Затем, шкаф через данный компонент просто заменит аватар на нужный. Таким образом будет производиться внешняя обработка аватара.
    /// <remarks>данный класс должен иметь <see cref="PlayerAvatar"/> компонент среди своих потомков</remarks>
    /// </summary>
    public abstract class AbstractPlayerAvatar : MonoBehaviour
    {
        /// <summary>
        /// Устройство контролирующее игрока.
        /// </summary>
        public abstract Device PlayerControlDevice { get; }

        private PlayerAvatar avatarComponent;
        private AbstractPlayerController playerController;

        /// <summary>
        /// <see cref="PlayerAvatar">Компонент аватара игрока</see>.
        /// </summary>
        public PlayerAvatar AvatarComponent => avatarComponent;
        
        /// <summary>
        /// <see cref="AbstractPlayerController">Контроллер</see>.
        /// </summary>
        public AbstractPlayerController PlayerController => playerController;

        private void Awake()
        {
            avatarComponent = GetComponentInChildren<PlayerAvatar>();
            playerController = GetComponentInChildren<AbstractPlayerController>();
        }
    }
}
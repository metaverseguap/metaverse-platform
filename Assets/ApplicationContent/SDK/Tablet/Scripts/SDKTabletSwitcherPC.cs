using LDR.SUAI_Metaverse.SDK.Player;
using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Tablet.Scripts
{
    /// <summary>
    /// <para>Скрипт отвечающий за появление/исчезновение планшета по нажатию клавиши.</para>
    /// </summary>
    public sealed class SDKTabletSwitcherPC : MonoBehaviour
    {
        [SerializeField] private KeyCode _tabletSwitchKey = KeyCode.F2;
        [SerializeField] private SDKPlayerControllerPC _player;
        [SerializeField] private GameObject _tablet;

        private bool isTabletActive;

        private void Start()
        {
            isTabletActive = _tablet.activeSelf;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_tabletSwitchKey))
            {
                SwitchTabletVisibility();
            }
        }

        /// <summary>
        /// <para>Показать/скрыть планшет.</para>
        /// </summary>
        public void SwitchTabletVisibility()
        {
            isTabletActive = !isTabletActive;
            _tablet.SetActive(isTabletActive);
            Cursor.visible = isTabletActive;
            _player.ActiveController = !isTabletActive;
        }
    }
}
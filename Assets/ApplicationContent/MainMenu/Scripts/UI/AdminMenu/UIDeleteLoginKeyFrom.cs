using System.Collections.Generic;
using Global.UI.ScrollList;
using MainMenu.Containers;
using MainMenu.UI.ScrollListItems;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using UnityEngine;

namespace MainMenu.UI.AdminMenu
{
    /// <summary>
    /// <para>Компонент, управляющий меню удаления ключей авторизации.</para>
    /// </summary>
    public sealed class UIDeleteLoginKeyFrom : MonoBehaviour
    {
        [SerializeField] private UIScrollList _loginKeyList;
        [SerializeField] private UILoginKeyItem _loginKeyItemPrefab;

        private APIContainer serverAPI;
        
        private IList<LoginKeyInfo> loginKeys = new List<LoginKeyInfo>();

        private void OnEnable()
        {
            serverAPI = EnsureServerAPI();
            RefreshLoginKeysList();
        }

        private APIContainer EnsureServerAPI()
        {
            if (serverAPI == null)
            {
                return MVNetworkManager.singleton.NetworkStore.FileServer;
            }

            return serverAPI;
        }

        /// <summary>
        /// <para>Обновить список ключей авторизации.</para>
        /// </summary>
        public void RefreshLoginKeysList()
        {
            _loginKeyList.Clear();
            loginKeys.Clear();
            
            loginKeys = serverAPI.LoginKey.GetAllLoginKeys();
            foreach (var loginKey in loginKeys)
            {
                _loginKeyItemPrefab.Key.text = loginKey.Key;
                _loginKeyItemPrefab.DateFrom.text = loginKey.DateFrom.ToShortDateString();
                _loginKeyItemPrefab.DateTo.text = loginKey.DateTo.ToShortDateString();
                
                _loginKeyList.AddItemWithContent(_loginKeyItemPrefab.gameObject);
            }
        }

        /// <summary>
        /// <para>Удалить ключи авторизации, которые в данный момент выбраны.</para>
        /// </summary>
        public void DeleteSelectedLoginKey()
        {
            IList<int> deletedIndexes = _loginKeyList.GetSelectedItemsIndexes();
            List<string> deletedNames = new List<string>();
            foreach (int deletedIndex in deletedIndexes)
            {
                deletedNames.Add(loginKeys[deletedIndex].Key);
            }

            bool success = serverAPI.LoginKey.DeleteMany(deletedNames);

            if (success)
            {
                RefreshLoginKeysList();
            }
        }
    }
}
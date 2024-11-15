using System.Collections.Generic;
using Global.UI.ScrollList;
using MainMenu.Containers;
using MainMenu.UI.ScrollList.Items;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using UnityEngine;

namespace MainMenu.UI.AdminMenu
{
    /// <summary>
    /// <para>Компонент управлюящий меню удаления ключей регистрации.</para>
    /// </summary>
    public sealed class UIDeleteRegistrationKeyForm : MonoBehaviour
    {
        [SerializeField] private UIScrollList _registrationKeyList;
        [SerializeField] private UIRegistrationKeyItem _registrationKeyItemPrefab;

        private APIContainer serverAPI;
        
        private IList<RegistrationKeyInfo> registrationKeys = new List<RegistrationKeyInfo>();

        private void OnEnable()
        {
            serverAPI = EnsureServerAPI();
            RefreshRegistrationKeysList();
        }

        private APIContainer EnsureServerAPI()
        {
            if (serverAPI == null)
            {
                return MVNetworkManager.singleton.FileServer;
            }

            return serverAPI;
        }
        
        /// <summary>
        /// <para>Обновить список ключей регистрации.</para>
        /// </summary>
        public void RefreshRegistrationKeysList()
        {
            _registrationKeyList.Clear();
            registrationKeys.Clear();
            
            registrationKeys = serverAPI.RegistrationKey.GetAllRegistrationKeys();
            foreach (var registrationKey in registrationKeys)
            {
                _registrationKeyItemPrefab.Key.text = registrationKey.Key;
                _registrationKeyItemPrefab.Organization.text = registrationKey.Organization;
                _registrationKeyItemPrefab.ServerRole.text = registrationKey.ServerRole;
                _registrationKeyItemPrefab.Role.text = registrationKey.Role.ToString();
                _registrationKeyItemPrefab.DateFrom.text = registrationKey.DateFrom.ToShortDateString();
                _registrationKeyItemPrefab.DateTo.text = registrationKey.DateTo.ToShortDateString();

                _registrationKeyList.AddItemWithContent(_registrationKeyItemPrefab.gameObject);
            }
        }
        
        /// <summary>
        /// <para>Удалить ключи регистрации, которые в данный момент выбраны.</para>
        /// </summary>
        public void DeleteSelectedRegistrationKey()
        {
            IList<int> deletedIndexes=_registrationKeyList.GetSelectedItemsIndexes();
            List<string> deletedNames = new List<string>();
            foreach (int deletedIndex in deletedIndexes)
            {
                deletedNames.Add(registrationKeys[deletedIndex].Key);
            }

            bool success = serverAPI.RegistrationKey.DeleteMany(deletedNames);

            if (success)
            {
                RefreshRegistrationKeysList();
            }
        }
    }
}
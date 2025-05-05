using System.Collections.Generic;
using Mirror;
using NetworkCore.MirrorNetworking.Containers.Store;
using TMPro;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Synchronization.UI
{
    /// <summary>
    /// <para>Компонент, синхронизирующий ui текст с сервером.</para>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MVNetworkUI : NetworkBehaviour
    {
        [SerializeField] private List<TMP_Text> _texts;

        [SyncVar]
        private string[] texts;

        /// <summary>
        /// Синхронизируемые тексты.
        /// </summary>
        public List<TMP_Text> UITexts
        {
            set => _texts = value;
        }

        /// <summary>
        /// Содержимое UI текстов.
        /// </summary>
        public string[] TextsContent => texts;

        private void Start()
        {
            if (isServer)
            {
                SetupTexts();
            }

            for (int i = 0; i < _texts.Count; i++)
            {
                _texts[i].text = texts[i];
            }
        }

        private void SetupTexts()
        {
            texts = new string[_texts.Count];
            
            SetTextsFromCache();

            for (int i = 0; i < _texts.Count; i++)
            {
                texts[i] = _texts[i].text;
            }
        }

        private void SetTextsFromCache()
        {
            NetworkDataStore store = MVNetworkManager.singleton.NetworkStore;
            CacheStore caches = store.HostMigration.Caches;
            if (caches.CurrentSceneCache.SyncUITextsCache.TryGetValue(netIdentity.sceneId, out var cachedTexts))
            {
                if (cachedTexts.Length != _texts.Count)
                {
                    return;
                }

                for (int i = 0; i < cachedTexts.Length; i++)
                {
                    _texts[i].text = cachedTexts[i];
                }
            }
        }

        private void FixedUpdate()
        {
            UpdateTexts();
        }

        private void UpdateTexts()
        {
            for (int i = 0; i < _texts.Count; i++)
            {
                TMP_Text text = _texts[i];
                if (text.text != texts[i])
                {
                    CmdUpdateText(i, text.text);
                }
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdUpdateText(int index, string text)
        {
            texts[index] = text;
            _texts[index].text = text;
            RpcUpdateText(index, text);
        }

        [ClientRpc]
        private void RpcUpdateText(int index, string text)
        {
            texts[index] = text;
            _texts[index].text = text;
        }
    }
}
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Synchronization.UI
{
    /// <summary>
    /// <para>Компонент, синхронизирующий ui текст с сервером.</para>
    /// </summary>
    public sealed class MVNetworkUI : NetworkBehaviour
    {
        [SerializeField] private List<TMP_Text> _texts;

        [SyncVar]
        private string[] texts;

        /// <summary>
        /// Синхронизируемые тексты.
        /// </summary>
        public List<TMP_Text> Texts
        {
            set => _texts = value;
        }

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
            for (int i = 0; i < _texts.Count; i++)
            {
                texts[i] = _texts[i].text;
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
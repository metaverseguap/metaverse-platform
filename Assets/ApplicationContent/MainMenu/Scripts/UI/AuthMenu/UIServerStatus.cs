using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Localization;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using TMPro;
using UnityEngine;

namespace MainMenu.UI.AuthMenu
{
    /// <summary>
    /// <para>Отображение статуса подключения к серверу на UI.</para>
    /// </summary>
    public sealed class UIServerStatus : MonoBehaviour
    {
        [SerializeField] private TMP_Text _statusLabel;
        [Range(0.1f, 5f)]
        [SerializeField] private float _updateIntervalSec = 1f;

        private APIContainer serverAPI;
        private bool activeUpdate;
        
        private void OnEnable()
        {
            activeUpdate = true;
            StartCoroutine(UpdateServerStatus());
        }

        private void OnDestroy()
        {
            activeUpdate = false;
        }

        private IEnumerator UpdateServerStatus()
        {
            while (activeUpdate)
            {
                serverAPI = EnsureServerAPI();
                if (serverAPI == null)
                {
                    // Ждем заданное количество секунд
                    yield return new WaitForSeconds(_updateIntervalSec);
                    continue;
                }

                // Создаем токены для отмены выполнения параллельных потоков
                CancellationTokenSource requestToken = new CancellationTokenSource();
                CancellationTokenSource delayToken = new CancellationTokenSource();
                Task<bool> ping = serverAPI.ServerStatus.IsServerOnline(requestToken.Token);
                Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(_updateIntervalSec), delayToken.Token);
                
                // Ожидаем завершения одной из задач
                while (!ping.IsCompleted && !timeoutTask.IsCompleted)
                {
                    yield return null; 
                }

                bool isOnline = ping.IsCompleted ? ping.Result : false;
                
                requestToken.Cancel();
                delayToken.Cancel();
                
                _statusLabel.color = isOnline ? Color.green : Color.red;
                _statusLabel.text = isOnline
                    ? LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.online")
                    : LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.offline");

                // Ждем заданное количество секунд + таймаут запроса
                yield return new WaitForSeconds(_updateIntervalSec);
            }
        }
        
        private APIContainer EnsureServerAPI()
        {
            if (serverAPI == null && MVNetworkManager.singleton != null)
            {
                return MVNetworkManager.singleton.NetworkStore.FileServer;
            }

            return serverAPI;
        }
    }
}
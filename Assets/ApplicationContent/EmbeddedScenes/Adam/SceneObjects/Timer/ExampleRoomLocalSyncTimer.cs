using System;
using LDR.SUAI_Metaverse.SDK.Core.NetworkSync;
using TMPro;
using UnityEngine;

namespace Adam.SceneObjects.Timer
{
    /// <summary>
    /// <para>Пример локального таймера, который ведет локальный отсчет синхронизируясь с сервером.</para>
    /// <remarks>скрипт является примером использования механики и не предназначен для использования в готовом проекте</remarks>
    /// </summary>
    public sealed class ExampleRoomLocalSyncTimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;

        private float currentTime = 0.0f;
        private bool isRunning = false;

        private void Start()
        {
            NetworkEnvironment.WhenConnectToRoom(ResetTimer);
        }

        private void Update()
        {
            if (!isRunning)
            {
                return;
            }

            currentTime += Time.deltaTime;
            TimeSpan showedTime = TimeSpan.FromSeconds(currentTime);
            _timerText.text = $"{showedTime.Hours:00} : {showedTime.Minutes:00} : {showedTime.Seconds:00} : {showedTime.Milliseconds:000}";
        }

        /// <summary>
        /// <para>Остановить таймер.</para>
        /// </summary>
        public void StopTimer()
        {
            isRunning = false;
        }

        /// <summary>
        /// <para>Запустить таймер.</para>
        /// </summary>
        public void StartTimer()
        {
            isRunning = true;
        }

        /// <summary>
        /// <para>Синхронизировать таймер с сетью.</para>
        /// </summary>
        public void ResetTimer()
        {
            currentTime = (float)NetworkEnvironment.GetRoomLifetime().TotalSeconds;
            StartTimer();
        }
    }
}
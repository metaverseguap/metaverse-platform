using UnityEngine;

namespace Global.Audio
{
    /// <summary>
    /// <para>Класс, манипулирующий звуками сцены.</para>
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource[] _controlledSources;

        private bool _isMute;

        private void Start()
        {
            _isMute = false;
            SetMuteToAll();
        }
        
        /// <summary>
        /// <para>Вкл/выкл конкретный аудио источник.</para>
        /// <param name="index">Индекс переключаемого источника</param>
        /// </summary>
        public void SwitchMute(int index)
        {
            if (index >= 0 && index < _controlledSources.Length)
            {
                _controlledSources[index].mute = !_controlledSources[index].mute;
            }
        }

        /// <summary>
        /// <para>Вкл/выкл все контролируемые аудио источники.</para>
        /// </summary>
        public void SwitchMuteToAll()
        {
            _isMute = !_isMute;
            SetMuteToAll();
        }

        private void SetMuteToAll()
        {
            foreach (AudioSource audioSource in _controlledSources)
            {
                audioSource.mute = _isMute;
            }
        }
    }
}
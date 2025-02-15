using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player.Tablet.PC.UI
{
    /// <summary>
    /// <para>Класс отвечающий за переключение окна контента в планшете.</para>
    /// </summary>
    public sealed class UITabletContentSelector : MonoBehaviour
    {
        [SerializeField] private GameObject _roomContent;
        [SerializeField] private GameObject _roomSelectionContent;

        private GameObject[] contents;

        private void Awake()
        {
            contents = new GameObject[] { _roomContent, _roomSelectionContent };
            SceneManager.sceneLoaded += UpdateRoomContent;
            UpdateRoomContent(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= UpdateRoomContent;
        }

        private void UpdateRoomContent(Scene newScene, LoadSceneMode loadSceneMode)
        {
            DestroyOldRoomContent();
            UITabletRoomContentSupplier supplier = FindObjectOfType<UITabletRoomContentSupplier>();
            if (supplier is null)
            {
                return;
            }

            GameObject content = supplier.GetRoomContent();
            if (content is null)
            {
                return;
            }
            
            GameObject result = Instantiate(content, _roomContent.transform);
            result.SetActive(true);
        }

        private void DestroyOldRoomContent()
        {
            foreach (Transform child in _roomContent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// <para>Показать контент комнаты.</para>
        /// </summary>
        public void ShowRoomContent()
        {
            TurnOffAllContent();
            _roomContent.SetActive(true);
        }

        /// <summary>
        /// <para>Показать контент выбора комнаты.</para>
        /// </summary>
        public void ShowRoomSelectionContent()
        {
            TurnOffAllContent();
            _roomSelectionContent.SetActive(true);
        }

        private void TurnOffAllContent()
        {
            foreach (var content in contents)
            {
                content.SetActive(false);
            }
        }
    }
}
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Global.Scene
{
    /// <summary>
    /// <para>Компонент переключающий сцену.</para>
    /// </summary>
    public sealed class SceneChanger : MonoBehaviour
    {
        [SerializeField] [Scene] private string _nextScene;

        /// <summary>
        /// <para>Переключает сцену на следующую.</para>
        /// </summary>
        public void ChangeSceneToNext()
        {
            SceneManager.LoadScene(_nextScene, LoadSceneMode.Single);
        }
    }
}
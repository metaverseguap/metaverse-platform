using UnityEngine;

namespace Global.Controllers
{
    /// <summary>
    /// <para>Компонент дающий доступ к методам <see cref="Application">Application</see>.</para>
    /// </summary>
    public sealed class ApplicationController : MonoBehaviour
    {
        public void ExitFromApplication()
        {
            Application.Quit(0);
        }
    }
}
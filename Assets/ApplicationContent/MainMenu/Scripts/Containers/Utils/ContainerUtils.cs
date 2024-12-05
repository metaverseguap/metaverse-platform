using System.Collections.Generic;
using MainMenu.Containers.Interfaces;

namespace MainMenu.Containers.Utils
{
    /// <summary>
    /// <para>Набор вспомогательных методов для работы с контейнерами.</para>
    /// </summary>
    public static class ContainerUtils
    {
        /// <summary>
        /// <para>Удалить из списка элементы другого списка.</para>
        /// </summary>
        /// <param name="removeFromList">список, из которого будут удалены элементы</param>
        /// <param name="matchingElementsList">элементы, которые должны быть удалены из списка</param>
        /// <typeparam name="T">тип контейнера, реализующего интерфейс <see cref="INamedContainer"/></typeparam>
        /// <returns>исходный список, из которого удалены указанные элементы</returns>
        public static IList<T> RemoveMatchingElements<T>(IList<T> removeFromList, IList<T> matchingElementsList)
            where T : INamedContainer
        {
            ISet<string> matchingNames = new HashSet<string>();
            foreach (var element in matchingElementsList)
            {
                matchingNames.Add(element.Name);
            }
            IList<T> result = new List<T>(removeFromList);
            for (int i = result.Count - 1; i >= 0; i--)
            {
                if (matchingNames.Contains(result[i].Name))
                {
                    result.RemoveAt(i);
                }
            }

            return result;
        }
    }
}
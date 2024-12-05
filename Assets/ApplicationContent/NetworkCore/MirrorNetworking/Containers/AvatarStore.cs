using System.Collections.Generic;
using MainMenu.Containers;

namespace NetworkCore.MirrorNetworking.Containers
{
    /// <summary>
    /// <para>Хранилище аватаров.</para>
    /// </summary>
    public sealed class AvatarStore
    {
        /// <summary>
        /// <para>Информация о всех аватарах хранящихся на файловом сервере.</para>
        /// </summary>
        public IList<AvatarInfo> AvatarInfos { get; set; } = new List<AvatarInfo>();

        /// <summary>
        /// <para>Файлы аватаров.</para>
        ///
        /// Ключом служит имя файла
        /// </summary>
        public IDictionary<string, AvatarFile> AvatarFiles { get;} = new Dictionary<string, AvatarFile>();
    }
}
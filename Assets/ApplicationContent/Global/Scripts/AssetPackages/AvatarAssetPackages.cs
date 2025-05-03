using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppAvatars.Types;
using Global.Converters;
using Global.Files;
using Global.Logger;
using MainMenu.Containers;
using NetworkCore.ServerInteraction.Type.Avatar;
using UnityEngine;

namespace Global.AssetPackages
{
    /// <summary>
    /// <para>Методы работы с файлами ассетов автаров.</para>
    /// </summary>
    public static class AvatarAssetPackages
    {
        /// <summary>
        /// Директория хранения ассетов.
        /// </summary>
        public static readonly string ASSETS_DIRECTORY = Path.Combine(Application.streamingAssetsPath, "Avatars");
        private static readonly string INFO_FILE_PATH = Path.Combine(ASSETS_DIRECTORY, "avatarsInfo.json");

        /// <summary>
        /// <para>Сохранить список информации об аватарах в файл.</para>
        /// </summary>
        public static void SaveAvatarInfo(IList<AvatarInfoDTO> avatars)
        {
            AssetBundleUtils.SerializeAsset(INFO_FILE_PATH, avatars);
        }

        /// <summary>
        /// <para>Получить список информации об аватарах из файла ассета.</para>
        /// </summary>
        /// <returns>список информации об аватарах из файла ассета</returns>
        public static IList<AvatarInfoDTO> GetAvatarInfos()
        {
            return AssetBundleUtils.DeserializeAsset<IList<AvatarInfoDTO>>(INFO_FILE_PATH);
        }
        
        /// <summary>
        /// <para>Получить локальные аватары, соответсвующие аватарам сервера.</para>
        ///
        /// Если локальный аватар присутствует в списке аватаров сервера, то его не нужно повторно скачивать.
        /// Если локальный аватар отсутствует в списке аватаров сервера, то он будет удален с локальной машины
        /// 
        /// <remarks>локальные аватары отсутствующие в списке аватаров сервера будут <b>УДАЛЕНЫ</b> с локальной машины</remarks>
        /// </summary>
        /// <param name="serverAvatarsInfos">список аватаров сервера</param>
        /// <returns>список локальных аватаров, имеющихся на сервере</returns>
        public static IList<AvatarInfo> GetMatchingLocalAvatars(IList<AvatarInfo> serverAvatarsInfos)
        {
            ISet<string> remoteAvatarNames = new HashSet<string>();
            foreach (AvatarInfo avatarInfo in serverAvatarsInfos)
            {
                remoteAvatarNames.Add(avatarInfo.Name);
            }

            IList<AvatarInfoDTO> localAvatarsInfos = GetAvatarInfos();
          
            for (int i = localAvatarsInfos.Count - 1; i >= 0; i--)
            {
                if (!remoteAvatarNames.Contains(localAvatarsInfos[i].name))
                {
                    AppLogger.Log($"Deleting avatar {localAvatarsInfos[i].name} because it doesn't exist on server");
                    string avatarFilePath = Path.Combine(ASSETS_DIRECTORY, localAvatarsInfos[i].name);
                    FileUtils.RemoveFile(avatarFilePath);
                    FileUtils.RemoveFile(avatarFilePath + ".meta");
                    localAvatarsInfos.RemoveAt(i);
                }
            }
            
            return localAvatarsInfos
                .Select(ToAvatarInfo)
                .ToList();
        }

        private static AvatarInfo ToAvatarInfo(AvatarInfoDTO dto)
        {
            AvatarInfo avatarInfo = new AvatarInfo();
            avatarInfo.Name = dto.name;
            avatarInfo.DisplayName = dto.displayName;
            if (Enum.TryParse(dto.animationControllerType, out AnimationControllerType controllerType))
            {
                avatarInfo.AvatarAnimationControllerType = controllerType;
            }
            avatarInfo.Image = DataConverter.SpriteFromRowData(dto.imageData);
                    
            return avatarInfo;
        }
    }
}
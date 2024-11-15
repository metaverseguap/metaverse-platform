using System;
using System.Collections.Generic;
using Global.Logger;
using MainMenu.Containers;
using NetworkCore.ServerInteraction.API.Utils;
using NetworkCore.ServerInteraction.Type.RegistrationKey;
using NetworkCore.ServerInteraction.Type.RegistrationKey.Request;
using NetworkCore.ServerInteraction.Type.RegistrationKey.Response;
using NetworkCore.ServerInteraction.Type.Request;
using NetworkCore.ServerInteraction.Type.Response;
using NetworkCore.ServerInteraction.Type.Role;
using RoleSystem.Core;

namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Взаимодействие с /api/registration-key файлового сервера.</para>
    /// </summary>
    public sealed class RegistrationKeyAPI : AbstractServerAPI
    {
        private const string ALL_KEYS_URL = "/api/registration-key/all";
        private const string CREATE_KEY_URL = "/api/registration-key/create";
        private const string DELETE_KEYS_URL = "/api/registration-key/delete-by-names";

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        public RegistrationKeyAPI(string serverUri) : base(serverUri)
        {
        }

        /// <summary>
        /// <para>Получает все ключи регистрации.</para>
        /// </summary>
        /// <returns>список всех ключей регистрации</returns>
        public IList<RegistrationKeyInfo> GetAllRegistrationKeys()
        {
            RegistrationKeysResponse response = restAPI.GetRequest<RegistrationKeysResponse>(ALL_KEYS_URL);
            if (response.success)
            {
                IList<RegistrationKeyInfo> result = new List<RegistrationKeyInfo>();

                foreach (var registrationKeyRO in response.keys)
                {
                    RegistrationKeyInfo registrationKeyInfo = new RegistrationKeyInfo();
                    registrationKeyInfo.Key = registrationKeyRO.key;
                    registrationKeyInfo.DateTo = registrationKeyRO.dateTo;
                    registrationKeyInfo.DateFrom = registrationKeyRO.dateFrom;
                    registrationKeyInfo.Organization = registrationKeyRO.organization;
                    registrationKeyInfo.ServerRole = registrationKeyRO.securityRole.name;
                    if (Enum.TryParse(registrationKeyRO.role.name, out AppRole role))
                    {
                        registrationKeyInfo.Role = role;
                    }

                    result.Add(registrationKeyInfo);
                }

                return result;
            }
            else
            {
                AppLogger.Warning($"Get All Registration keys request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }

            return new List<RegistrationKeyInfo>();
        }

        /// <summary>
        /// <para>Создает ключ регистрации на файловом сервере.</para>
        /// </summary>
        /// <param name="regKeyInfo"><see cref="RegistrationKeyInfo">ключ регистрации</see></param>
        /// <param name="exceptionMessage">сообщение об ошибках создания ключа</param>
        /// <returns>true, если ключь был успешно создан</returns>
        public bool CreateRegistrationKey(RegistrationKeyInfo regKeyInfo, out string exceptionMessage)
        {
            RegistrationKeyRO regKeyRO = new RegistrationKeyRO();
            regKeyRO.key = regKeyInfo.Key;
            regKeyRO.dateFrom = regKeyInfo.DateFrom;
            regKeyRO.dateTo = regKeyInfo.DateTo;
            regKeyRO.organization = regKeyInfo.Organization;

            RoleRO roleRO = new RoleRO();
            roleRO.name = regKeyInfo.Role.ToString();
            regKeyRO.role = roleRO;

            SecurityRoleRO securityRoleRO = new SecurityRoleRO();
            securityRoleRO.name = regKeyInfo.ServerRole;
            regKeyRO.securityRole = securityRoleRO;

            CreateRegistrationKeyRequest request = new CreateRegistrationKeyRequest();
            request.registrationKey = regKeyRO;

            ResponseDetails response =
                restAPI.PostRequest<CreateRegistrationKeyRequest, ResponseDetails>(CREATE_KEY_URL, request);

            if (!response.success)
            {
                AppLogger.Warning($"Create Registration Key request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                exceptionMessage = ResponseUtils.GetErrorMessagesAsString(response);
                return false;
            }

            exceptionMessage = "";
            return true;
        }

        /// <summary>
        /// <para>Удаляет множество ключей регистрации.</para>
        /// </summary>
        /// <param name="deletedNames">список удаляемых ключей регистрации</param>
        /// <returns>true, если удаление прошло успешно</returns>
        public bool DeleteMany(List<string> deletedNames)
        {
            DeleteByNamesRequest request = new DeleteByNamesRequest()
            {
                names = deletedNames
            };
            
            ResponseDetails response =
                restAPI.PostRequest<DeleteByNamesRequest, ResponseDetails>(DELETE_KEYS_URL, request);

            if (!response.success)
            {
                AppLogger.Warning($"Delete Registration Keys request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                return false;
            }

            return true;
        }
    }
}
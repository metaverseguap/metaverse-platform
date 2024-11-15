using System;
using System.Collections.Generic;
using System.Linq;
using Global.Logger;
using NetworkCore.ServerInteraction.API.Utils;
using NetworkCore.ServerInteraction.Type.Request;
using NetworkCore.ServerInteraction.Type.Response;
using NetworkCore.ServerInteraction.Type.Role;
using NetworkCore.ServerInteraction.Type.Role.Request;
using NetworkCore.ServerInteraction.Type.Role.Response;
using RoleSystem.Core;
using RoleSystem.Types;

namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Взаимодействие с /api/role файлового сервера.</para>
    /// </summary>
    public sealed class RoleAPI : AbstractServerAPI
    {
        private const string MY_ROLE_URL = "/api/role/me";
        private const string ALL_SERVER_ROLES_URL = "/api/role/server/all";
        private const string ALL_ROLES_URL = "/api/role/all";
        private const string DELETE_ROLES_URL = "/api/role/delete-by-names";
        private const string UPDATE_ROLES_URL = "/api/role/update-all";
        private const string UPDATE_ROLE_URL = "/api/role/update";
        private const string ALL_PERMISSIONS_URL = "/api/role/permissions/all";
        private const string DELETE_PERMISSIONS_URL = "/api/role/permissions/delete-by-names";
        private const string UPDATE_PERMISSIONS_URL = "/api/role/permissions/update-all";

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        public RoleAPI(string serverUri) : base(serverUri)
        {
        }

        /// <summary>
        /// <para>Получает роль пользователя.</para>
        /// </summary>
        /// <returns><see cref="RoleInfo"/></returns>
        public RoleInfo GetMyRole()
        {
            RoleResponse response = restAPI.GetRequest<RoleResponse>(MY_ROLE_URL);
            if (response.success)
            {
                return ConvertToRoleInfo(response.roleInfo);
            }
            else
            {
                AppLogger.Error($"User role request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }

            return new RoleInfo();
        }

        /// <summary>
        /// <para>Получает все роли файлового сервера.</para>
        /// </summary>
        /// <returns>список <see cref="ServerRoleInfo">ролей файлового сервера</see></returns>
        public IList<ServerRoleInfo> GetServerRoles()
        {
            IList<ServerRoleInfo> roles = new List<ServerRoleInfo>();

            ServerRolesResponse response = restAPI.GetRequest<ServerRolesResponse>(ALL_SERVER_ROLES_URL);

            if (response.success)
            {
                foreach (var roleRO in response.roles)
                {
                    roles.Add(
                        new ServerRoleInfo
                        {
                            Name = roleRO.name
                        }
                    );
                }

                return roles;
            }
            else
            {
                AppLogger.Error($"User server roles request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }

            return roles;
        }
        
        /// <summary>
        /// <para>Обновляет <see cref="AppRole">роли</see> и <see cref="AppPermission">права (разрешения)</see> на сервере согласно состоянию файлов в приложении.</para>
        ///
        /// <returns>true, если обновление прошло успешно</returns>
        /// <remarks>Роли хранятся в файле <c>AppRole.cs</c>, а права (разрешения) в файле <c>AppPermission.cs</c>.
        /// После добавления, удаления ролей из этих файлов необходимо обновить состояние ролей и разрешений на сервере</remarks>
        /// </summary>
        public bool Refresh()
        {
            bool roleSuccess = RefreshRoles();
            if (!roleSuccess)
            {
                return false;
            }
            
            bool permissionSuccess = RefreshPermissions();

            return permissionSuccess;
        }

        private bool RefreshRoles()
        {
            var roles = GetAppRoles();
            var deletingRoles = CalculateRoleChanges(ref roles);
            bool deleteSuccess = DeleteManyRoles(deletingRoles);
            if (!deleteSuccess)
            {
                return false;
            }
            
            bool upsertSuccess = UpsertRoles(roles);
            
            return upsertSuccess;
        }

        private bool RefreshPermissions()
        {
            var permissions = GetAppPermissions();
            var deletingPermissions = CalculatePermissionChanges(ref permissions);
            bool deleteSuccess = DeleteManyPermissions(deletingPermissions);
            if (!deleteSuccess)
            {
                return false;
            }
            
            bool upsertSuccess = UpsertPermissions(permissions);
            
            return upsertSuccess;
        }

        private static ISet<AppRole> GetAppRoles()
        {
            AppRole[] roles = (AppRole[])Enum.GetValues(typeof(AppRole));
            return roles.ToHashSet();
        }

        private static ISet<AppPermission> GetAppPermissions()
        {
            AppPermission[] permissions = (AppPermission[])Enum.GetValues(typeof(AppPermission));
            return permissions.ToHashSet();
        }

        /// <summary>
        /// <para>Подсчитывает изменения, которые необходимо внести на сервер для ролей.</para>
        /// </summary>
        /// <param name="roles">cписок ролей в приложении. Из него будут удалены роли, которые уже есть на сервере</param>
        /// <returns>список ролей, которые есть на сервере, но которых нет в приложении</returns>
        private List<string> CalculateRoleChanges(ref ISet<AppRole> roles)
        {
            List<string> deletedRoles = new List<string>();
            IList<RoleInfo> serverRoles = GetRoles();
            foreach (var serverRole in serverRoles)
            {
                if (!roles.Contains(serverRole.Name))
                {
                    deletedRoles.Add(serverRole.Name.ToString());
                }
                else
                {
                    roles.Remove(serverRole.Name);
                }
            }

            return deletedRoles;
        }

        /// <summary>
        /// <para>Подсчитывает изменения, которые необходимо внести на сервер для прав (разрешений).</para>
        /// </summary>
        /// <param name="permissions">список прав (разрешений) в приложении. Из него будут удалены права (разрешения), которые уже есть на сервере</param>
        /// <returns>список прав (разрешений), которые есть на сервере, но которых нет в приложении</returns>
        private List<string> CalculatePermissionChanges(ref ISet<AppPermission> permissions)
        {
            List<string> deletedPermissions = new List<string>();
            IList<AppPermission> serverPermissions = GetPermissions();
            foreach (var serverPermission in serverPermissions)
            {
                if (!permissions.Contains(serverPermission))
                {
                    deletedPermissions.Add(serverPermission.ToString());
                }
                else
                {
                    permissions.Remove(serverPermission);
                }
            }

            return deletedPermissions;
        }
        
        /// <summary>
        /// <para>Получает все роли синхронизированные с сервером роли.</para>
        /// </summary>
        /// <returns>список <see cref="RoleInfo">ролей</see></returns>
        public List<RoleInfo> GetRoles()
        {
            List<RoleInfo> roles = new List<RoleInfo>();

            RolesResponse response = restAPI.GetRequest<RolesResponse>(ALL_ROLES_URL);

            if (response.success)
            {
                foreach (var roleRO in response.roles)
                {
                    roles.Add(ConvertToRoleInfo(roleRO));
                }

                return roles;
            }
            else
            {
                AppLogger.Warning($"User roles request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }

            return roles;
        }

        private RoleInfo ConvertToRoleInfo(RoleRO roleRO)
        {
            RoleInfo role = new RoleInfo();
            if (Enum.TryParse(roleRO.name, out AppRole appRole))
            {
                role.Name = appRole;
            }
            else
            {
                AppLogger.Error($"Cannot parse {roleRO.name} to AppRole");
                return role;
            }

            role.Permissions = new List<AppPermission>();
            foreach (var permission in roleRO.permissions)
            {
                if (Enum.TryParse(permission.name, out AppPermission appPermission))
                {
                    role.Permissions.Add(appPermission);
                }
                else
                {
                    AppLogger.Error($"Cannot parse {permission.name} to AppPermission");
                }
            }

            return role;
        }

        /// <summary>
        /// <para>Получает все синхронизированные с сервером права (разрешения).</para>
        /// </summary>
        /// <returns>список <see cref="AppPermission">прав (разрешений)</see></returns>
        public IList<AppPermission> GetPermissions()
        {
            IList<AppPermission> permissions = new List<AppPermission>();

            PermissionsResponse response = restAPI.GetRequest<PermissionsResponse>(ALL_PERMISSIONS_URL);

            if (response.success)
            {
                foreach (var permissionRO in response.permissions)
                {
                    if (Enum.TryParse(permissionRO.name, out AppPermission appPermission))
                    {
                        permissions.Add(appPermission);
                    }
                    else
                    {
                        AppLogger.Log($"Cannot parse {permissionRO.name} to AppPermission. Refresh roles from administrator menu.");
                    }
                }

                return permissions;
            }
            else
            {
                AppLogger.Warning($"User Permissions request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }

            return permissions;
        }

        /// <summary>
        /// <para>Удаляет множество ролей.</para>
        /// </summary>
        /// <param name="deletedNames">список удаляемых ролей</param>
        /// <returns>true, если удаление прошло успешно</returns>
        public bool DeleteManyRoles(List<string> deletedNames)
        {
            DeleteByNamesRequest request = new DeleteByNamesRequest()
            {
                names = deletedNames
            };

            ResponseDetails response =
                restAPI.PostRequest<DeleteByNamesRequest, ResponseDetails>(DELETE_ROLES_URL, request);

            if (!response.success)
            {
                AppLogger.Warning($"Delete Roles request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// <para>Удаляет множество прав (разрешений).</para>
        /// </summary>
        /// <param name="deletedNames">список удаляемых разрешений</param>
        /// <returns>true, если удаление прошло успешно</returns>
        public bool DeleteManyPermissions(List<string> deletedNames)
        {
            DeleteByNamesRequest request = new DeleteByNamesRequest()
            {
                names = deletedNames
            };
            
            ResponseDetails response =
                restAPI.PostRequest<DeleteByNamesRequest, ResponseDetails>(DELETE_PERMISSIONS_URL, request);

            if (!response.success)
            {
                AppLogger.Warning($"Delete permissions request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// <para>Обновляет <see cref="AppRole">роли</see> на сервере.</para>
        /// </summary>
        /// <param name="roles">коллекция ролей</param>
        public bool UpsertRoles(ISet<AppRole> roles)
        {
            List<RoleRO> creatingRoles = new List<RoleRO>();
            foreach (var role in roles)
            {
                RoleRO roleRo = new RoleRO();
                roleRo.name = role.ToString();
                creatingRoles.Add(roleRo);
            }

            CreateRolesRequest createRolesRequest = new CreateRolesRequest();
            createRolesRequest.roles = creatingRoles;

            ResponseDetails response =
                restAPI.PostRequest<CreateRolesRequest, ResponseDetails>(UPDATE_ROLES_URL, createRolesRequest);
            
            if (!response.success)
            {
                AppLogger.Warning($"Update Roles request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// <para>Обновляет <see cref="AppPermission">права (разрешения)</see> на сервере.</para>
        /// </summary>
        /// <param name="permissions">коллекция прав (разрешений)</param>
        public bool UpsertPermissions(ISet<AppPermission> permissions)
        {
            List<PermissionRO> creatingPermissions = new List<PermissionRO>();
            foreach (var permission in permissions)
            {
                PermissionRO permissionRO = new PermissionRO();
                permissionRO.name = permission.ToString();
                creatingPermissions.Add(permissionRO);
            }

            CreatePermissionsRequest createPermissionsRequest = new CreatePermissionsRequest();
            createPermissionsRequest.permissions = creatingPermissions;

            ResponseDetails response =
                restAPI.PostRequest<CreatePermissionsRequest, ResponseDetails>(UPDATE_PERMISSIONS_URL, createPermissionsRequest);
            
            if (!response.success)
            {
                AppLogger.Warning($"Update Permissions request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// <para>Обновляет разрешения роли.</para>
        /// </summary>
        /// <param name="role"><see cref="RoleInfo">роль</see></param>
        /// <param name="exceptionMessage">сообщение об ошибках обновления роли</param>
        /// <returns>true, если обновление роли прошло успешно</returns>
        public bool UpsertRole(RoleInfo role, out string exceptionMessage)
        {
            List<PermissionRO> permissionsRo = new List<PermissionRO>();
            foreach (var permission in role.Permissions)
            {
                PermissionRO permissionRO = new PermissionRO();
                permissionRO.name = permission.ToString();
                permissionsRo.Add(permissionRO);
            }

            RoleRO roleRo = new RoleRO();
            roleRo.name = role.Name.ToString();
            roleRo.permissions = permissionsRo;
            
            CreateRoleRequest createRolesRequest = new CreateRoleRequest();
            createRolesRequest.role = roleRo;

            ResponseDetails response =
                restAPI.PostRequest<CreateRoleRequest, ResponseDetails>(UPDATE_ROLE_URL, createRolesRequest);
            
            if (!response.success)
            {
                exceptionMessage = ResponseUtils.GetErrorMessagesAsString(response);
                AppLogger.Warning($"Update Role request ended with error: {exceptionMessage}");
                return false;
            }

            exceptionMessage = "";
            return true;
        }
    }
}
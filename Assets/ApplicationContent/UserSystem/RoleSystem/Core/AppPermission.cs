namespace UserSystem.RoleSystem.Core
{
    /// <summary>
    /// <para>Права (разрешения) пользователей в прилжении.</para>
    /// </summary>
    public enum AppPermission
    {
        ADMIN_MENU_ACCESS, // Доступ к меню администратора
        USER_KICK, // Исключить игрока из комнаты
        USER_MUTE // Выключить микрофон игрока
    }
}
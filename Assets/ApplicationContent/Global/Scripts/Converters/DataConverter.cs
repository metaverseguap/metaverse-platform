using UnityEngine;

namespace Global.Converters
{
    /// <summary>
    /// <para>Класс содержащий методы конвертации данных.</para>
    /// </summary>
    public static class DataConverter
    {
        /// <summary>
        /// <para>Конвертировать массив байт в Sprite.</para>
        /// </summary>
        /// <param name="rowData">массив байт</param>
        /// <returns>Sprite</returns>
        public static Sprite SpriteFromRowData(byte[] rowData)
        {
            Texture2D sceneIcon = new Texture2D(2, 2);
            sceneIcon.LoadImage(rowData);

            return Sprite.Create(
                sceneIcon,
                new Rect(0, 0, sceneIcon.width, sceneIcon.height),
                new Vector2(0.5f, 0.5f)
            );
        }
        
        /// <summary>
        /// <para>Конвертировать Sprite в массив байт.</para>
        /// </summary>
        /// <param name="sprite">Sprite</param>
        /// <returns>массив байт</returns>
        public static byte[] SpriteToRowData(Sprite sprite)
        {
            if (sprite == null)
            {
                return null;
            }
            
            Texture2D texture = SpriteToTexture(sprite);

            return texture.EncodeToPNG();
        }

        /// <summary>
        /// <para>Конвертировать Sprite в Texture2D.</para>
        /// </summary>
        /// <param name="sprite">Sprite</param>
        /// <returns>Texture2D</returns>
        public static Texture2D SpriteToTexture(Sprite sprite)
        {
            Rect spriteRect = sprite.rect;
            Texture2D texture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);
            
            texture.SetPixels(
                sprite.texture.GetPixels(
                    (int)spriteRect.x,
                    (int)spriteRect.y,
                    (int)spriteRect.width,
                    (int)spriteRect.height)
            );

            texture.Apply();
            
            return texture;
        }
    }
}
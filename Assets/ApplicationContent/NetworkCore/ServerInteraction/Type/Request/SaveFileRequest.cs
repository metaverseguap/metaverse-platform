namespace NetworkCore.ServerInteraction.Type.Request
{
    /// <summary>
    /// <para>Запрос на сохранение файла.</para>
    ///
    /// Данная структура используется в случае, когда результатом запрос на файловый сервер является не объект Unity,
    /// а создаваемый файл.
    /// В таком случае в данной структуре описываются сведения позволяющие сохранить в файл результат запроса на сервер
    /// </summary>
    public struct SaveFileRequest
    {
        /// <summary>
        /// Директория, в которую будет сохранен файл.
        /// </summary>
        public readonly string DirectoryPath;

        /// <summary>
        /// Имя сохраняемого файла.
        /// </summary>
        public readonly string FileName;

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="directoryPath">директория, в которую будет сохранен файл</param>
        /// <param name="fileName">имя сохраняемого файла</param>
        private SaveFileRequest(string directoryPath, string fileName)
        {
            this.DirectoryPath = directoryPath;
            this.FileName = fileName;
        }

        /// <summary>
        /// <para>Фабричный метод.</para>
        /// </summary>
        /// <param name="directoryPath">директория, в которую будет сохранен файл</param>
        /// <param name="fileName">имя сохраняемого файла</param>
        /// <returns>новый экземпляр структуры</returns>
        public static SaveFileRequest Form(string directoryPath, string fileName)
        {
            return new SaveFileRequest(directoryPath, fileName);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Global.Files;
using Global.Logger;
using NetworkCore.ServerInteraction.Type.Request;
using NetworkCore.ServerInteraction.Type.Response;
using NetworkCore.ServerInteraction.Type.Response.Details;
using Newtonsoft.Json;
using UnityEngine;

namespace NetworkCore.ServerInteraction.API.Utils
{
    /// <summary>
    /// <para>Класс осуществляющий REST запросы.</para>
    /// </summary>
    public sealed class RestAPI
    {
        private readonly HttpClient client = new HttpClient();

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="baseUrl">основной url сервера. Он будет подставлятся в начало конкретных запросов</param>
        public RestAPI(string baseUrl)
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// <para>Устанавливает JWT, необходимый для авторизации на сервере.</para>
        /// </summary>
        /// <param name="token">Json Web Token</param>
        public void SetAuthorization(string token)
        {
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token);
        }

        /// <summary>
        /// <para>Осуществляет GET запрос на сервер.</para>
        /// </summary>
        /// <param name="url">url запроса</param>
        /// <param name="parameters">параметры запроса в виде <see cref="GetParam"/></param>
        /// <typeparam name="R">тип к которому будет преобразован ответ на запрос. Данный тип должен наследоваться от <see cref="ResponseDetails"/> и иметь конструктор по умолчанию</typeparam>
        /// <returns>ответ на запрос обернутый в тип R или <see cref="ResponseDetails"/> с сообщением об ошибке</returns>
        public R GetRequest<R>(string url, params GetParam[] parameters)
            where R : ResponseDetails, new()
        {
            var urlWithParams = URLWithParams(url, parameters);

            try
            {
                var response = client.GetAsync(urlWithParams).Result;

                if (response.IsSuccessStatusCode)
                {
                    string strResult = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<R>(strResult);
                }
                else
                {
                    string message = $"GET request failed. Code: {response.StatusCode}";
                    AppLogger.Error(message);

                    return ErrorResponse<R>(message);
                }
            }
            catch (Exception exception)
            {
                string message = $"Error when trying to execute GET request: {exception.Message}";
                AppLogger.Error(message);

                return ErrorResponse<R>(message);
            }
        }

        /// <summary>
        /// <para>Осуществляет GET запрос на сервер с сохранением результата запроса в файл.</para>
        ///
        /// <remarks>данный метод <b>должен выполняться в параллельном потоке</b>, а его результат должен получаться через <c>await</c>
        /// и <b>ни в коем случае</b> нельзя дожидаться результата данного метода в основном потоке через <c>.Result</c></remarks>
        /// </summary>
        /// <param name="url">url запроса</param>
        /// <param name="fileRequest"><see cref="SaveFileRequest">сведенья о файле</see>, в который необходимо сохранить результат</param>
        /// <param name="parameters">параметры запроса в виде <see cref="GetParam"/></param>
        public async Task<bool> GetFileRequest(string url, SaveFileRequest fileRequest, params GetParam[] parameters)
        {
            string urlWithParams = URLWithParams(url, parameters);
            return await DownloadFileStreamAsync(urlWithParams, fileRequest);
        }

        private async Task<bool> DownloadFileStreamAsync(string url, SaveFileRequest fileRequest)
        {
            FileUtils.EnsureDirectoryExists(fileRequest.DirectoryPath);

            string path = Path.Combine(fileRequest.DirectoryPath, fileRequest.FileName);

            try
            {
                // Отправляем запрос и получаем поток данных
                using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // Открываем поток для записи файла
                        using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                               fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            // Копируем поток напрямую в файл
                            await contentStream.CopyToAsync(fileStream);
                            AppLogger.Log($"File successfully save: {path}");
                            return true;
                        }
                    }
                    else
                    {
                        AppLogger.Error($"File downloading error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"File downloading exception: {ex.Message}");
            }

            return false;
        }
        
        private static string URLWithParams(string url, GetParam[] parameters)
        {
            StringBuilder urlWithParams = new StringBuilder(url);
            if (parameters.Length != 0)
            {
                urlWithParams.Append('?');
                foreach (GetParam param in parameters)
                {
                    urlWithParams
                        .Append(param.Name)
                        .Append('=')
                        .Append(param.Value)
                        .Append('&');
                }

                urlWithParams.Length -= 1;
            }

            return urlWithParams.ToString();
        }

        /// <summary>
        /// <para>Осуществляет POST запрос на сервер.</para>
        /// </summary>
        /// <param name="url">url запроса</param>
        /// <param name="body">тело запроса</param>
        /// <typeparam name="T">тип тела запроса</typeparam>
        /// <typeparam name="R">тип к которому будет преобразован ответ на запрос. Данный тип должен наследоваться от <see cref="ResponseDetails"/> и иметь конструктор по умолчанию</typeparam>
        /// <returns>ответ на запрос обернутый в тип R или <see cref="ResponseDetails"/> с сообщением об ошибке</returns>
        public R PostRequest<T, R>(string url, T body)
            where R : ResponseDetails, new()
        {
            var json = JsonConvert.SerializeObject(body);
            var strContent = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = client.PostAsync(url, strContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    string strResult = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<R>(strResult);
                }
                else
                {
                    string message = $"POST request failed. Code: {response.StatusCode}";
                    AppLogger.Error(message);

                    return ErrorResponse<R>(message);
                }
            }
            catch (Exception exception)
            {
                string message = $"Error when trying to execute POST request: {exception.Message}";
                AppLogger.Error(message);

                return ErrorResponse<R>(message);
            }
        }
        
        /// <summary>
        /// <para>Осуществляет POST запрос на сервер.</para>
        /// </summary>
        /// <param name="url">url запроса</param>
        /// <param name="body">тело запроса</param>
        /// <typeparam name="R">тип к которому будет преобразован ответ на запрос. Данный тип должен наследоваться от <see cref="ResponseDetails"/> и иметь конструктор по умолчанию</typeparam>
        /// <returns>ответ на запрос обернутый в тип R или <see cref="ResponseDetails"/> с сообщением об ошибке</returns>
        public R PostMultipartRequest<R>(string url, MultipartFormDataContent body)
            where R : ResponseDetails, new()
        {
            try
            {
                var response = client.PostAsync(url, body).Result;
           
                if (response.IsSuccessStatusCode)
                {
                    string strResult = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<R>(strResult);
                }
                else
                {
                    string message = $"POST Multipart request failed. Code: {response.StatusCode}";
                    AppLogger.Error(message);

                    return ErrorResponse<R>(message);
                }
            }
            catch (Exception exception)
            {
                string message = $"Error when trying to execute POST Multipart request: {exception.Message}";
                AppLogger.Error(message);

                return ErrorResponse<R>(message);
            }
        }

        /// <summary>
        /// <para>Осуществляет DELETE запрос на сервер.</para>
        /// </summary>
        /// <param name="url">url запроса</param>
        /// <typeparam name="R">тип к которому будет преобразован ответ на запрос. Данный тип должен наследоваться от <see cref="ResponseDetails"/> и иметь конструктор по умолчанию</typeparam>
        /// <returns>ответ на запрос обернутый в тип R или <see cref="ResponseDetails"/> с сообщением об ошибке</returns>
        public R DeleteRequest<R>(string url)
            where R : ResponseDetails, new()
        {
            try
            {
                var response = client.DeleteAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string strResult = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<R>(strResult);
                }
                else
                {
                    string message = $"DELETE request failed. Code: {response.StatusCode}";
                    AppLogger.Error(message);

                    return ErrorResponse<R>(message);
                }
            }
            catch (Exception exception)
            {
                string message = $"Error when trying to execute DELETE request: {exception.Message}";
                AppLogger.Error(message);

                return ErrorResponse<R>(message);
            }
        }

        private static R ErrorResponse<R>(string message)
            where R : ResponseDetails, new()
        {
            ErrorDetails errorMessage = new ErrorDetails();
            errorMessage.exceptionMessage = message;
            errorMessage.code = NetworkCode.INTERNAL_SERVER_ERROR;
            R result = new R();
            result.error = new List<ErrorDetails> { errorMessage };

            return result;
        }
    }
}
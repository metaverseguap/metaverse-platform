using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Global.Files;
using Global.Logger;
using NetworkCore.ServerInteraction.Type.Auth.Response;
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
        #region Singleton

        private static RestAPI instance = null;

        /// <summary>
        /// <para>singleton конструктор.</para>
        /// </summary>
        /// <param name="baseUrl">основной url сервера. Он будет подставляться в начало конкретных запросов</param>
        /// <returns>instance</returns>
        public static RestAPI singleton(string baseUrl)
        {
            if (instance == null)
            {
                instance = new RestAPI(baseUrl);
            }

            return instance;
        }

        #endregion
        
        private const string REFRESH_TOKEN_URL = "/api/auth/refresh";
        
        private readonly HttpClient client = new HttpClient(new HttpClientHandler
        {
            UseCookies = true
        });
        
        private TimeSpan asyncTimeout;

        /// <summary>
        /// <para>Конструктор.</para>
        /// Конструктор приватный. Используйте метод <see cref="RestAPI.singleton"/>
        /// </summary>
        /// <param name="baseUrl">основной url сервера. Он будет подставляться в начало конкретных запросов</param>
        private RestAPI(string baseUrl)
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
            asyncTimeout = TimeSpan.FromSeconds(1);
        }
        
        /// <summary>
        /// Время ожидания выполнения асинхронного запроса на сервер.
        /// </summary>
        public TimeSpan AsyncTimeout
        {
            get => asyncTimeout;
            set => asyncTimeout = value;
        }
        
        /// <summary>
        /// Время ожидания выполнения запроса на сервер.
        /// </summary>
        public TimeSpan Timeout
        {
            get => client.Timeout;
            set => client.Timeout = value;
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
        /// <para>Задать url сервера.</para>
        /// </summary>
        /// <param name="baseUrl">основной url сервера. Он будет подставляться в начало конкретных запросов</param>
        public void SetBaseUrl(string baseUrl)
        {
            client.BaseAddress = new Uri(baseUrl);
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
            return GetRequest<R>(url, true, parameters);
        }

        private R GetRequest<R>(string url, bool refreshToken, GetParam[] parameters)
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

                if (response.StatusCode == HttpStatusCode.Unauthorized && refreshToken)
                {
                    AppLogger.Log("Access token expired. Trying to refresh...");
                    bool isTokenSuccessfullyRefreshed = TryRefreshAccessToken();
                    if (isTokenSuccessfullyRefreshed)
                    {
                        return GetRequest<R>(urlWithParams, false, parameters);
                    }
                }

                string message = $"GET request failed. Code: {response.StatusCode}";
                AppLogger.Error(message);
                return ErrorResponse<R>(message);
            }
            catch (Exception exception)
            {
                string message = $"Error when trying to execute GET request: {exception.Message}";
                AppLogger.Error(message);

                return ErrorResponse<R>(message);
            }
        }

        /// <summary>
        /// <para>Осуществляет асинхронный GET запрос на сервер.</para>
        /// Данный метод используется, когда приложение должно продолжать свою работу параллельно выполнению запроса на сервер
        /// </summary>
        /// <param name="url">url запроса</param>
        /// <param name="token">CancellationToken для отмены выполнения запроса из другого потока</param>
        /// <param name="parameters">параметры запроса в виде <see cref="GetParam"/></param>
        /// <typeparam name="R">тип к которому будет преобразован ответ на запрос. Данный тип должен наследоваться от <see cref="ResponseDetails"/> и иметь конструктор по умолчанию</typeparam>
        /// <returns>ответ на запрос обернутый в тип R или <see cref="ResponseDetails"/> с сообщением об ошибке</returns>
        public async Task<R> AsyncGetRequest<R>(string url, CancellationToken token, params GetParam[] parameters)
            where R : ResponseDetails, new()
        {
            return await AsyncGetRequest<R>(url, token, true, parameters);
        }

        /// <summary>
        /// <para>Осуществляет асинхронный GET запрос на сервер.</para>
        /// Данный метод используется, когда приложение должно продолжать свою работу параллельно выполнению запроса на сервер
        /// </summary>
        /// <param name="url">url запроса</param>
        /// <param name="token">CancellationToken для отмены выполнения запроса из другого потока</param>
        /// <param name="log">нужно ли логгировать ошибки</param>
        /// <param name="parameters">параметры запроса в виде <see cref="GetParam"/></param>
        /// <typeparam name="R">тип к которому будет преобразован ответ на запрос. Данный тип должен наследоваться от <see cref="ResponseDetails"/> и иметь конструктор по умолчанию</typeparam>
        /// <returns>ответ на запрос обернутый в тип R или <see cref="ResponseDetails"/> с сообщением об ошибке</returns>
        public async Task<R> AsyncGetRequest<R>(string url, CancellationToken token, bool log, params GetParam[] parameters)
            where R : ResponseDetails, new()
        {
            return await AsyncGetRequest<R>(url, token, log, true, parameters);
        }
        
        private async Task<R> AsyncGetRequest<R>(string url, CancellationToken token, bool log, bool refreshToken, GetParam[] parameters)
            where R : ResponseDetails, new()
        {
            var urlWithParams = URLWithParams(url, parameters);

            try
            {
                var response = await client.GetAsync(urlWithParams, token);

                if (response.IsSuccessStatusCode)
                {
                    string strResult = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<R>(strResult);
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized && refreshToken)
                {
                    AppLogger.Log("Access token expired. Trying to refresh...");
                    bool isTokenSuccessfullyRefreshed = await AsyncTryRefreshAccessToken();
                    if (isTokenSuccessfullyRefreshed)
                    {
                        return await AsyncGetRequest<R>(urlWithParams, token, log, false, parameters);
                    }
                }

                string message = $"GET request failed. Code: {response.StatusCode}";
                if (log)
                {
                    AppLogger.Error(message);
                }
                return ErrorResponse<R>(message);
            }
            catch (Exception exception)
            {
                string message = $"Error when trying to execute GET request: {exception.Message}";
                if (log)
                {
                    AppLogger.Error(message);
                }

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
                bool isAccessTokenCorrect = await AsyncTryRefreshAccessToken();
                if (!isAccessTokenCorrect)
                {
                    return false;
                }
                
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
            return PostRequest<T, R>(url, true, body);
        }

        private R PostRequest<T, R>(string url, bool refreshToken, T body)
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

                if (response.StatusCode == HttpStatusCode.Unauthorized && refreshToken)
                {
                    AppLogger.Log("Access token expired. Trying to refresh...");
                    bool isTokenSuccessfullyRefreshed = TryRefreshAccessToken();
                    if (isTokenSuccessfullyRefreshed)
                    {
                        return PostRequest<T, R>(url, false, body);
                    }
                }

                string message = $"POST request failed. Code: {response.StatusCode}";
                AppLogger.Error(message);

                return ErrorResponse<R>(message);
            }
            catch (Exception exception)
            {
                string message = $"Error when trying to execute POST request: {exception.Message}";
                AppLogger.Error(message);

                return ErrorResponse<R>(message);
            }
        }
        
        /// <summary>
        /// <para>Осуществляет асинхронный POST запрос на сервер.</para>
        /// Данный метод используется, когда приложение должно продолжать свою работу параллельно выполнению запроса на сервер
        /// </summary>
        /// <param name="url">url запроса</param>
        /// <param name="token">CancellationToken для отмены выполнения запроса из другого потока</param>
        /// <param name="log">нужно ли логгировать ошибки</param>
        /// <param name="body">тело запроса</param>
        /// <typeparam name="T">тип тела запроса</typeparam>
        /// <typeparam name="R">тип к которому будет преобразован ответ на запрос. Данный тип должен наследоваться от <see cref="ResponseDetails"/> и иметь конструктор по умолчанию</typeparam>
        /// <returns>ответ на запрос обернутый в тип R или <see cref="ResponseDetails"/> с сообщением об ошибке</returns>
        public async Task<R> AsyncPostRequest<T, R>(string url, CancellationToken token, bool log, T body)
            where R : ResponseDetails, new()
        {
            return await AsyncPostRequest<T, R>(url, token, log, true, body);
        }
        
        private async Task<R> AsyncPostRequest<T, R>(string url, CancellationToken token, bool log, bool refreshToken, T body)
            where R : ResponseDetails, new()
        {
            var json = JsonConvert.SerializeObject(body);
            var strContent = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync(url, strContent, token);

                if (response.IsSuccessStatusCode)
                {
                    string strResult = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<R>(strResult);
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized && refreshToken)
                {
                    AppLogger.Log("Access token expired. Trying to refresh...");
                    bool isTokenSuccessfullyRefreshed = await AsyncTryRefreshAccessToken();
                    if (isTokenSuccessfullyRefreshed)
                    {
                        return await AsyncPostRequest<T, R>(url, token, log, false, body);
                    }
                }

                string message = $"POST request failed. Code: {response.StatusCode}";
                if (log)
                {
                    AppLogger.Error(message);
                }

                return ErrorResponse<R>(message);
            }
            catch (Exception exception)
            {
                string message = $"Error when trying to execute POST request: {exception.Message}";
                if (log)
                {
                    AppLogger.Error(message);
                }

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
            return PostMultipartRequest<R>(url, true, body);
        }

        private R PostMultipartRequest<R>(string url, bool refreshToken, MultipartFormDataContent body)
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

                if (response.StatusCode == HttpStatusCode.Unauthorized && refreshToken)
                {
                    AppLogger.Log("Access token expired. Trying to refresh...");
                    bool isTokenSuccessfullyRefreshed = TryRefreshAccessToken();
                    if (isTokenSuccessfullyRefreshed)
                    {
                        return PostMultipartRequest<R>(url, false, body);
                    }
                }

                string message = $"POST Multipart request failed. Code: {response.StatusCode}";
                AppLogger.Error(message);

                return ErrorResponse<R>(message);
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
            return DeleteRequest<R>(url, true);
        }

        private R DeleteRequest<R>(string url, bool refreshToken)
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

                if (response.StatusCode == HttpStatusCode.Unauthorized && refreshToken)
                {
                    AppLogger.Log("Access token expired. Trying to refresh...");
                    bool isTokenSuccessfullyRefreshed = TryRefreshAccessToken();
                    if (isTokenSuccessfullyRefreshed)
                    {
                        return DeleteRequest<R>(url, false);
                    }
                }

                string message = $"DELETE request failed. Code: {response.StatusCode}";
                AppLogger.Error(message);

                return ErrorResponse<R>(message);
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
        
        /// <summary>
        /// <para>Выполнить асинхронный запрос, прерываемый по таймеру.</para>
        /// Выполнить указанный асинхронный запрос.
        /// Если время выполнение запроса превысит указанное,
        /// то выполнение запроса прервется и результат выполнения будет равен <see cref="ResponseDetails"/>,
        /// содержащий сообщение об ошибке timeout-а
        /// </summary>
        /// <param name="request">асинхронный запрос. <c>(token)=>request.Invoke(token)</c></param>
        /// <typeparam name="R">тип к которому будет преобразован ответ на запрос. Данный тип должен наследоваться от <see cref="ResponseDetails"/> и иметь конструктор по умолчанию</typeparam>
        /// <returns>результат выполнения запроса или default, если превышено время ожидания выполнения запроса</returns>
        public async Task<R> ExecuteAsyncRequest<R>(
            Func<CancellationToken, Task<R>> request)
            where R : ResponseDetails, new()
        {
            return await ExecuteAsyncRequest(asyncTimeout, request);
        } 
        
        /// <summary>
        /// <para>Выполнить асинхронный запрос, прерываемый по таймеру.</para>
        /// Выполнить указанный асинхронный запрос.
        /// Если время выполнение запроса превысит указанное,
        /// то выполнение запроса прервется и результат выполнения будет равен <see cref="ResponseDetails"/>,
        /// содержащий сообщение об ошибке timeout-а
        /// </summary>
        /// <param name="timeout">время ожидания выполнения запроса</param>
        /// <param name="request">асинхронный запрос. <c>(token)=>request.Invoke(token)</c></param>
        /// <typeparam name="R">тип к которому будет преобразован ответ на запрос. Данный тип должен наследоваться от <see cref="ResponseDetails"/> и иметь конструктор по умолчанию</typeparam>
        /// <returns>результат выполнения запроса или default, если превышено время ожидания выполнения запроса</returns>
        public async Task<R> ExecuteAsyncRequest<R>(
            TimeSpan timeout,
            Func<CancellationToken, Task<R>> request)
            where R : ResponseDetails, new()
        {
            using CancellationTokenSource requestToken = new CancellationTokenSource();
            using CancellationTokenSource timeoutToken = new CancellationTokenSource();

            Task<R> operationTask = request.Invoke(requestToken.Token);
            Task timeoutTask = Task.Delay(timeout, timeoutToken.Token);

            Task completed = await Task.WhenAny(operationTask, timeoutTask);
            
            timeoutToken.Cancel();
            requestToken.Cancel();
            
            if (completed == operationTask && operationTask.IsCompletedSuccessfully)
            {
                return operationTask.Result;
            }
            else
            {
                ErrorDetails errorMessage = new ErrorDetails();
                errorMessage.exceptionMessage = "The timeout occurred while waiting for a response from the server";
                errorMessage.code = NetworkCode.TIMEOUT_OCCURRED;
                R result = new R();
                result.error = new List<ErrorDetails> { errorMessage };

                return result;
            }
        }

        private bool TryRefreshAccessToken()
        {
            AuthResponse response = PostRequest<object, AuthResponse>(REFRESH_TOKEN_URL, false, null);

            if (response.success)
            {
                SetAuthorization($"Bearer {response.token}");
                AppLogger.Log("Access token refreshed successfully.");
                return true;
            }

            AppLogger.Error($"Refresh token request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            return false;
        }
        
        private async Task<bool> AsyncTryRefreshAccessToken()
        {
            AuthResponse response = await ExecuteAsyncRequest(
                (token) => AsyncPostRequest<object, AuthResponse>(REFRESH_TOKEN_URL, token, false, false, null)
            );

            if (response.success)
            {
                SetAuthorization($"Bearer {response.token}");
                AppLogger.Log("Access token refreshed successfully.");
                return true;
            }

            AppLogger.Error($"Refresh token request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Global.Logger;
using NetworkCore.ServerInteraction.Type.Request;
using NetworkCore.ServerInteraction.Type.Response;
using NetworkCore.ServerInteraction.Type.Response.Details;
using Newtonsoft.Json;

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
            client.Timeout = TimeSpan.FromSeconds(3);
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

            try
            {
                var response = client.GetAsync(urlWithParams.ToString()).Result;

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
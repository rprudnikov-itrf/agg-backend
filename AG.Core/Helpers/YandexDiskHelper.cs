using System.Configuration;
using System.Linq;
using AG.Core.Models.YandexDisk;
using RestSharp;
using System.Threading;

namespace AG.Core.Helpers
{
    public static class YandexDiskHelper
    {
        //получить токен
        //https://oauth.yandex.ru/authorize?response_type=token&client_id=df38b923119b42e1a734e888627f0c58

        private static RestClient _client;
        private static RestClient _clientPublic;

        static YandexDiskHelper()
        {
            var token = "OAuth " + ConfigurationManager.AppSettings["YandexDiskToken"];

            _client = new RestClient("https://cloud-api.yandex.net/v1/disk/resources");
            _client.AddDefaultHeader("Authorization", token);

            _clientPublic = new RestClient("https://cloud-api.yandex.net/v1/disk/public/resources");
            _clientPublic.AddDefaultHeader("Authorization", token);
        }

        public static class Files
        {
            //добавить файл
            public static void Upload(string path, byte[] file)
            {
                var request = new RestRequest("/upload");
                request.AddParameter("overwrite", "true");
                request.AddParameter("path", path);
                var response = _client.GetRetry<FileInfoUpload>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                var buffer = response.Data;
                if (buffer != null && !string.IsNullOrEmpty(buffer.href))
                {
                    request = new RestRequest();
                    request.AddParameter("file", file, ParameterType.RequestBody);

                    var clientUpload = new RestClient(buffer.href);
                    var responseUpload = clientUpload.Put(request);
                    if (responseUpload.ResponseStatus != ResponseStatus.Completed)
                        throw response.ErrorException;
                }
            }

            //переименовать файл
            public static void Rename(string from, string path)
            {
                var request = new RestRequest("/move");
                request.AddParameter("from", from, ParameterType.QueryString);
                request.AddParameter("path", path, ParameterType.QueryString);
                var response = _client.PostRetryNotFound(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;
            }

            //удалить файл
            public static void Delete(string path)
            {
                var request = new RestRequest();
                request.AddParameter("path", path, ParameterType.QueryString);
                var response = _client.Delete(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;
            }

            //скачать файл
            public static byte[] Download(string path)
            {
                var request = new RestRequest("/download");
                request.AddParameter("path", path);
                var response = _client.GetRetry<FileInfoUpload>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                var buffer = response.Data;
                if (buffer != null && !string.IsNullOrEmpty(buffer.href))
                {
                    var clientDownload = new RestClient(buffer.href);
                    var file = clientDownload.DownloadData(new RestRequest());
                    return file;
                }

                return null;
            }
        }

        public static class Folders
        {
            public static bool Exist(string path)
            {
                var response = Resources(path);
                return response != null && !string.IsNullOrEmpty(response.path);
            }

            public static void Add(string path)
            {
                var request = new RestRequest();
                request.AddParameter("path", path, ParameterType.QueryString);
                var response = _client.Put(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;
            }

            public static string Add(params string[] paths)
            {
                if (paths == null || paths.Count() == 0)
                    return string.Empty;

                var path = paths.First();
                foreach (var item in paths.Skip(1))
                {
                    path += "/" + item;
                    if (!Exist(path))
                        Add(path);
                }
                return path;
            }

            public static Resource Resources(string path)
            {
                var request = new RestRequest();
                request.AddParameter("path", path);
                request.AddParameter("limit", 10000);
                var response = _client.GetRetry<Resource>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    return null;

                return response.Data;
            }

            public static string Combine(params string[] paths)
            {
                if (paths == null || paths.Length == 0)
                    return string.Empty;

                return string.Join("/", paths.Select(p => p.TrimEnd('/')));
            }
        }

        public static class Share
        {
            public static Resource Publish(string path)
            {
                var request = new RestRequest("/publish");
                request.AddParameter("path", path, ParameterType.QueryString);
                var response = _client.PutRetry<FileInfoUpload>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                Resource result = null;
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(500);
                    result = Folders.Resources(path);

                    if (result != null && !string.IsNullOrEmpty(result.public_key))
                        break;
                }
                return result;
            }

            public static Resource UnPublish(string path)
            {
                var request = new RestRequest("/unpublish");
                request.AddParameter("path", path, ParameterType.QueryString);
                var response = _client.PutRetry<FileInfoUpload>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                return Folders.Resources(path);
            }

            public static Resource Resources(string public_key)
            {
                var request = new RestRequest();
                request.AddParameter("public_key", public_key);
                var response = _clientPublic.GetRetry<Resource>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    return null;

                return response.Data;
            }

            public static FileInfoUpload Download(string public_key)
            {
                if (string.IsNullOrEmpty(public_key))
                    return null;

                var request = new RestRequest("download");
                request.AddParameter("public_key", public_key);
                var response = _clientPublic.GetRetry<FileInfoUpload>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    return null;

                return response.Data;
            }
        }
    }
}
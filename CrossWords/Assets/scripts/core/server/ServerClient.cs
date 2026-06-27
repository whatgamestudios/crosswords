// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace CrossWords {

    public class ServerException : Exception {
        public int Code { get; }
        public ServerException(int code, string message) : base(message) {
            Code = code;
        }
    }

    public static class ServerClient {
        public const string RPC_URL = "https://worcadian.vercel.app/rpc";

        private static readonly HttpClient _httpClient;
        private static readonly Random _random = new Random();
        private static int _requestId = 0;

        static ServerClient() {
            var handler = new HttpClientHandler {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10
            };
            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public static async Task<JObject> SendAsync(string method, JObject parameters) {
            int id = Interlocked.Increment(ref _requestId);

            var requestBody = new JObject {
                ["jsonrpc"] = "2.0",
                ["method"] = method,
                ["params"] = parameters ?? new JObject(),
                ["id"] = id
            };

            AuditLog.Log($"Server RPC: {method}");

            const int maxAttempts = 6;
            Exception lastException = null;

            for (int attempt = 1; attempt <= maxAttempts; attempt++) {
                try {
                    var content = new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _httpClient.PostAsync(RPC_URL, content);
                    string responseString = await response.Content.ReadAsStringAsync();

                    JObject responseJson = JObject.Parse(responseString);

                    if (responseJson["error"] != null) {
                        JToken error = responseJson["error"];
                        int code = (int)(error["code"] ?? -32603);
                        string message = (string)(error["message"] ?? "Unknown server error");
                        AuditLog.Log($"Server RPC error [{code}]: {message}");
                        throw new ServerException(code, message);
                    }

                    JToken result = responseJson["result"];
                    if (result == null || result.Type != JTokenType.Object) {
                        throw new ServerException(-32603, $"Unexpected result format from {method}");
                    }
                    return (JObject)result;
                // } catch (ServerException) {
                //     throw;
                } catch (Exception ex) {
                    lastException = ex;
                    AuditLog.Log($"Server RPC attempt {attempt}/{maxAttempts} failed for {method}: {ex.Message}");
                    if (attempt < maxAttempts) {
                        await Task.Delay(500 + _random.Next(0, 501));
                    }
                }
            }

            throw lastException;
        }
    }
}

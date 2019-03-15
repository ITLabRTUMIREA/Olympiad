﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Executor.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Exercises;
using Shared.Models;
using Models.Solutions;
using PublicAPI.Requests;

namespace Executor
{
    class DbManager
    {
        public const string DbManagerHttpClientName = nameof(DbManagerHttpClientName);

        private readonly IOptions<UserInfo> options;
        private readonly ILogger<DbManager> logger;
        private readonly HttpClient client;
        

        public DbManager(
            IOptions<UserInfo> options,
            IHttpClientFactory httpClientFactory,
            ILogger<DbManager> logger)
        {
            this.options = options;
            this.logger = logger;

            logger.LogInformation($"user name : {options.Value.UserName}");
            client = httpClientFactory.CreateClient(DbManagerHttpClientName);
            Authorize();
        }
        public Task<ExerciseData[]> GetExerciseData(Guid exId)
        {
            return Invoke<ExerciseData[]>($"api/ExerciseData/all/{exId}");
        }

        public Task SaveChanges(Guid solutionId, SolutionStatus status)
            => InvokePost<object>($"api/Executor/{solutionId}/{(int)status}");


        public Task<List<Solution>> GetInQueueSolutions()
        {
            return Invoke<List<Solution>>("api/Executor");
        }


        private async Task<T> Invoke<T>(string path)
        {
            try
            {
                var strResponse = await client.GetStringAsync(path);
                return JsonConvert.DeserializeObject<T>(strResponse);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"cant invoke GET action with path >{path}<, try auth", ex);
                await Authorize();
                return await Invoke<T>(path);
            }
        }

        private async Task<T> InvokePost<T>(string path)
        {
            try
            {
                var strResponse = await client.PostAsync(path, null).Result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(strResponse);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"cant invoke POST action with path >{path}<, try auth", ex);
                await Authorize();
                return await InvokePost<T>(path);
            }

        }


        private async Task Authorize(int deep = 0)
        {
            var pack = new CredentialsRequest
            {
                Login = options.Value.UserName,
                Password = options.Value.Password
            };
            var content = JsonConvert.SerializeObject(pack);
            var body = new StringContent(content, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync("api/auth/login", body);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Can't connect to server, check credentials");
                }
                var strResponse = await response.Content.ReadAsStringAsync();
                logger.LogDebug($"server return {strResponse}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<JObject>(strResponse)["Token"].ToString());
            }
            catch (Exception ex)
            {
                if (deep > 10)
                    throw;
                logger.LogWarning($"exception when auth #{deep}, wait for retry...", ex);
                await Task.Delay(TimeSpan.FromSeconds(5));
                await Authorize(deep + 1);
            }
        }
    }
}

﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using TVDBSharp.Models.Exceptions;

namespace TVDBSharp.Services
{
    public abstract class ScraperBase
    {
        protected TVDBConfiguration ApiConfiguration { get; }

        protected ScraperBase(TVDBConfiguration apiConfiguration)
        {
            this.ApiConfiguration = apiConfiguration ?? throw new ArgumentNullException(nameof(apiConfiguration));
        }

        private void CreateHeaders(HttpClient client, bool requiresAuth)
        {
            if (requiresAuth) client.DefaultRequestHeaders.Add("Authorization", ApiConfiguration.Token);
        }

        protected async Task<HttpResponseMessage> GetAsync(string url, bool requiresAuth = true)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            try
            {
                var uri = new Uri(url);

                using (var client = new HttpClient())
                {
                    CreateHeaders(client, requiresAuth);
                    var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);
                    if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound) throw new Exception(response.StatusCode.ToString());
                    return response;
                }
            }
            catch (Exception e)
            {
                throw new ServerNotAvailableException(inner: e);
            }
        }

        protected async Task<HttpResponseMessage> PostAsync(string url, string data)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            try
            {
                var uri = new Uri(url);

                using (var client = new HttpClient())
                {
                    //CreateHeaders(client);
                    var response = await client.PostAsync(uri, new StringContent(data, new System.Text.UTF8Encoding(), "application/json"));
                    if (!response.IsSuccessStatusCode) throw new Exception(response.StatusCode.ToString());

                    return response;
                }
            }
            catch (Exception e)
            {
                throw new ServerNotAvailableException(inner: e);
            }
        }
    }
}
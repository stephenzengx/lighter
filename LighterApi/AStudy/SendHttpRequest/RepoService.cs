using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LighterApi
{
    public class RepoService
    {
        private readonly HttpClient _httpClient;

        public RepoService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<IEnumerable<GitHubBranch>> GetRepos()
        {
            var response = await _httpClient.GetAsync("repos/aspnet/AspNetCore.Docs/branches");

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IEnumerable<GitHubBranch>>(responseStream);
        }
    }
}

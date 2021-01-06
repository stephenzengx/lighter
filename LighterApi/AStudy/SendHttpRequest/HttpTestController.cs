using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LighterApi
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class HttpTestController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public IEnumerable<GitHubBranch> Branches { get; private set; }

        public bool GetBranchesError { get; private set; }

        private readonly RepoService _repoService;

        //public HttpTestController(IHttpClientFactory clientFactory)
        //{
        //    _clientFactory = clientFactory;
        //}

        /// <summary>
        /// DI 类型化客户端
        /// </summary>
        /// <param name="clientFactory"></param>
        public HttpTestController(RepoService repoService)
        {
            _repoService = repoService;
        }

        //基础用法
        [Route("basicUsage")]
        [HttpGet]
        public async Task<IActionResult> basicUsage()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://api.github.com/repos/aspnet/AspNetCore.Docs/branches");
            request.Headers.Add("Accept", "application/vnd.github.v3+json");
            request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                Branches = await JsonSerializer.DeserializeAsync<IEnumerable<GitHubBranch>>(responseStream);
            }
            else
            {
                GetBranchesError = true;
                Branches = Array.Empty<GitHubBranch>();
            }

            return Ok(Branches);
        }

        //命名客户端
        [Route("nameClient")]
        [HttpGet]
        public async Task<IActionResult> nameClient()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "repos/aspnet/AspNetCore.Docs/branches");

            var client = _clientFactory.CreateClient("github");
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                Branches = await JsonSerializer.DeserializeAsync<IEnumerable<GitHubBranch>>(responseStream);
            }
            else
            {
                GetBranchesError = true;
                Branches = Array.Empty<GitHubBranch>();
            }

            // post put delete
            //var branchesJson = new StringContent(
            //    JsonSerializer.Serialize(Branches,new JsonSerializerOptions { 
                    
            //    }),
            //    Encoding.UTF8,
            //    "application/json");
            //using var httpResponse = await client.PostAsync("/api/TodoItems", branchesJson);
            //client.PutAsync();
            //client.DeleteAsync();

            return Ok(Branches);
        }

        [Route("typedClient")]
        [HttpGet]
        public async Task<IActionResult> typedClient()
        {
            Branches = await _repoService.GetRepos();

            return Ok(Branches);
        }

    }
}

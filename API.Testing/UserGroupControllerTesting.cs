using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API.Testing.Enums;
using API.Testing.Utils;
using Data.Entities;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace API.Testing
{
    public class UserGroupControllerTesting
    {
        private readonly HttpClient _httpClient;
        private readonly ITestOutputHelper _logHelper;
        private readonly ClientHelper _clientHelper;

        public UserGroupControllerTesting(ITestOutputHelper logHelper)
        {
            _httpClient = new();
            _httpClient.BaseAddress = new Uri("http://localhost:5000");
            _clientHelper = new ClientHelper(_httpClient);
            _logHelper = logHelper;
        }

        private async Task<Tuple<string, string, string, string>> Login()
        {
            var token1 = await _clientHelper.Login("adminerp", "admin");
            var token2 = await _clientHelper.Login("user1", "123123");
            var token3 = await _clientHelper.Login("user2", "123123");
            var token4 = await _clientHelper.Login("user3", "123123");
            return new Tuple<string, string, string, string>(token1, token2, token3, token4);
        }

        [Theory]
        [InlineData("system/group", EnumHttpMethod.POST)]
        [InlineData("system/group", EnumHttpMethod.PUT)]
        [InlineData("system/group/adduser", EnumHttpMethod.POST)]
        [InlineData("system/group", EnumHttpMethod.DELETE)]
        [InlineData("system/group", EnumHttpMethod.GET)]
        [InlineData("system/group/search", EnumHttpMethod.POST)]
        [InlineData("system/group/fetch", EnumHttpMethod.POST)]
        [InlineData("system/group/all", EnumHttpMethod.GET)]
        [InlineData("system/group/allpermission", EnumHttpMethod.GET)]
        [InlineData("warehouse/group", EnumHttpMethod.POST)]
        [InlineData("warehouse/group", EnumHttpMethod.PUT)]
        [InlineData("warehouse/group/adduser", EnumHttpMethod.POST)]
        [InlineData("warehouse/group", EnumHttpMethod.DELETE)]
        [InlineData("warehouse/group", EnumHttpMethod.GET)]
        [InlineData("warehouse/group/search", EnumHttpMethod.POST)]
        [InlineData("warehouse/group/fetch", EnumHttpMethod.POST)]
        [InlineData("warehouse/group/all", EnumHttpMethod.GET)]
        [InlineData("warehouse/group/allpermission", EnumHttpMethod.GET)]
        public async Task CheckAuthentication(string route, EnumHttpMethod method)
        {
            var emptyData = JsonContent.Create(new { });
            var res = method switch
            {
                EnumHttpMethod.POST => await _httpClient.PostAsync(route, emptyData),
                EnumHttpMethod.GET => await _httpClient.GetAsync(route),
                EnumHttpMethod.DELETE => await _httpClient.DeleteAsync(route),
                EnumHttpMethod.PUT => await _httpClient.PutAsync(route, emptyData),
                _ => throw new ArgumentOutOfRangeException(nameof(method), $"Not expected direction value: {method}"),
            };
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }
    }
}
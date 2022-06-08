using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.ViewModels;
using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json.Linq;

namespace API.Testing
{
    public class AuthControllerTesting
    {
        private readonly HttpClient _httpClient;
        private readonly ITestOutputHelper _logHelper;

        public AuthControllerTesting(ITestOutputHelper logHelper)
        {
            _httpClient = new();
            _httpClient.BaseAddress = new Uri("http://localhost:5000");
            _logHelper = logHelper;
        }

        [Fact]
        public async Task Welcome()
        {
            var res = await _httpClient.GetAsync("/");
            res.EnsureSuccessStatusCode();
            var rs = await res.Content.ReadFromJsonAsync<ApiResponse>();
            Assert.Equal("Welcome to Warehouse Management", rs?.Data.ToString());
        }

        [Fact]
        public async Task Login_Success()
        {
            const string username = "test";
            const string password = "123123";

            var res = await _httpClient.PostAsync("/auth/login", JsonContent.Create(new { username, password }));

            res.EnsureSuccessStatusCode();

            dynamic rs = JObject.Parse(res.Content.ReadAsStringAsync().Result);

            Assert.True(Guid.TryParse(rs.data.userId.ToString(), out Guid a));
            Assert.False(string.IsNullOrWhiteSpace(rs.data.accessToken.ToString()));
            Assert.False(string.IsNullOrWhiteSpace(rs.data.refreshToken.ToString()));
            if (!TestingConfig.SkipUserAvatar)
            {
                Assert.False(string.IsNullOrWhiteSpace(rs.data.avatar.ToString()));
            }
        }

        [Fact]
        public async Task Login_InvalidParams_MinLength()
        {
            const string u1 = "a";
            const string p1 = "a";

            var res = await _httpClient.PostAsync("/auth/login", JsonContent.Create(new {username = u1, password = p1 }));
            dynamic rs = JObject.Parse(res.Content.ReadAsStringAsync().Result);

            Assert.Equal(400, (int)res.StatusCode);
            Assert.Equal("Invalid params", rs.message.ToString());
            Assert.Equal("The field Password must be a string or array type with a minimum length of '3'.", rs.data[0].ToString());
            Assert.Equal("The field Username must be a string or array type with a minimum length of '3'.", rs.data[1].ToString());
        }

        [Fact]
        public async Task Login_InvalidParams_Required()
        {
            const string u1 = null;
            const string p1 = null ;

            var res = await _httpClient.PostAsync("/auth/login", JsonContent.Create(new {username = u1, password = p1 }));
            dynamic rs = JObject.Parse(res.Content.ReadAsStringAsync().Result);

            Assert.Equal(400, (int)res.StatusCode);
            Assert.Equal("Invalid params", rs.message.ToString());
            Assert.Equal("The Password field is required.", rs.data[0].ToString());
            Assert.Equal("The Username field is required.", rs.data[1].ToString());
        }
        
        [Fact]
        public async Task Login_Username_NotExist()
        {
            const string u1 = "usernamefake";
            const string p1 = "wrongpassword" ;

            var res = await _httpClient.PostAsync("/auth/login", JsonContent.Create(new {username = u1, password = p1 }));
            dynamic rs = JObject.Parse(res.Content.ReadAsStringAsync().Result);

            Assert.Equal(404, (int)res.StatusCode);
            Assert.Equal("Username does not exist", rs.message.ToString());
        }
    }
}

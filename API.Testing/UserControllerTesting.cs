using System;
using System.Net.Http;
using System.Threading.Tasks;
using API.Testing.Utils;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace API.Testing
{
    public class UserControllerTesting
    {
        private readonly HttpClient _httpClient;
        private readonly ClientHelper _clientHelper;
        private readonly ITestOutputHelper _logHelper;

        public UserControllerTesting(ITestOutputHelper logHelper)
        {
            _httpClient = new();
            _httpClient.BaseAddress = new Uri("http://localhost:5000");
            _clientHelper = new ClientHelper(_httpClient);
            _logHelper = logHelper;
        }

        [Fact]
        public async Task Profile()
        {
            var username = "test";
            var password = "123123";

            var loginRes = await _clientHelper.Login(username, password);
            var token = _clientHelper.ParseToGetToken(loginRes);

            Assert.NotEmpty(token);
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);

            _logHelper.WriteLine(token);
            
            var res = await _httpClient.GetAsync("/profile");
            res.EnsureSuccessStatusCode();

            // Assert data
            dynamic rs = JObject.Parse(res.Content.ReadAsStringAsync().Result);
            var profile = rs.data;
            
            _logHelper.WriteLine(profile.ToString());

            Assert.NotEmpty(profile);
            Assert.NotEmpty(profile.email.ToString());
            Assert.NotEmpty(profile.firstName.ToString());
            Assert.NotEmpty(profile.lastName.ToString());
            if (!TestingConfig.SkipUserAvatar)
            {
                Assert.NotEmpty(profile.avatar.ToString());
            }
            Assert.NotEmpty(profile.phoneNumber.ToString());
            Assert.NotEmpty(profile.gender.ToString());
            Assert.NotEmpty(profile.inWarehouseId.ToString());
            Assert.NotEmpty(profile.createdAt.ToString());
        }
    }
}
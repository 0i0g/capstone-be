using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace API.Testing.Utils
{
    public class ClientHelper
    {
        private readonly HttpClient _client;

        public ClientHelper(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> Login(string username, string password)
        {
            var res = await _client.PostAsync("/auth/login", JsonContent.Create(new {username, password}));
            res.EnsureSuccessStatusCode();
            return res.Content.ReadAsStringAsync().Result;
        }

        public string ParseToGetToken(string value)
        {
            dynamic rs = JObject.Parse(value);
            return rs.data.accessToken.ToString();
        }
    }
}
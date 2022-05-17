using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Utilities.Helper
{
    public class DataHelper
    {
        public static ICollection<T> ReadSeedData<T>(string path)
        {
            using var r = new StreamReader(path);
            var json = r.ReadToEnd();
            var data = JsonConvert.DeserializeObject<List<T>>(json);

            return data;
        }
    }
}

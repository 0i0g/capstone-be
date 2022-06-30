using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        
        public static string MapPath(string seedFile)
        {
            // if(HttpContext.Current!=null)
            //     return HostingEnvironment.MapPath(seedFile);

            var absolutePath =
                new Uri(Assembly.GetExecutingAssembly().CodeBase)
                    .LocalPath; //was AbsolutePath but didn't work with spaces according to comments
            var directoryName = Path.GetDirectoryName(absolutePath);
            var path = Path.Combine(directoryName, seedFile.TrimStart('~').Replace('/', Path.DirectorySeparatorChar));

            return path;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TeconMoon_s_WiiVC_Injector.Properties;
using static TeconMoon_s_WiiVC_Injector.StringUtil;

namespace TeconMoon_s_WiiVC_Injector
{
    public class GameTdb
    {
        public static string GetName(string id)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var wiitdb = "TeconMoon_s_WiiVC_Injector.Resources.wiitdb.txt";

            using (var stream = assembly.GetManifestResourceStream(wiitdb))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var split = line.Split(new[] { " = " }, 2, StringSplitOptions.None);
                    if (split[0] == id)
                        return split[1];
                }
                return null;
            }
        }
    }
}

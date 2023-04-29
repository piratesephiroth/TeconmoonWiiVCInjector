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
        public static List<string> GetIds(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var wiitdb = "TeconMoon_s_WiiVC_Injector.Resources.wiitdb.txt";

            using (var stream = assembly.GetManifestResourceStream(wiitdb))
            using (var reader = new StreamReader(stream))
            {
                var ids = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var split = line.Split(new[] { " = " }, 2, StringSplitOptions.None);
                    if (split[1] == name)
                        ids.Add(split[0]);
                }
                return ids;
            }
        }
        public static List<string> GetIdsStartingWith(string idStart)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var wiitdb = "TeconMoon_s_WiiVC_Injector.Resources.wiitdb.txt";

            using (var stream = assembly.GetManifestResourceStream(wiitdb))
            using (var reader = new StreamReader(stream))
            {
                var ids = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("TITLES ="))
                        continue;
                    var split = line.Split(new[] { " = " }, 2, StringSplitOptions.None);
                    if (split[0].StartsWith(idStart))
                        ids.Add(split[0]);
                    // Gametdb titles are ordered alphabetically by id, so stop searching
                    if (string.Compare(idStart, split[0].Substring(0, idStart.Length)) < 0)
                        break;
                }
                return ids;
            }
        }

        internal static IEnumerable<string> GetAlternativeIds(string initialId)
        {
            var tried = new HashSet<string>
            {
                initialId,
                initialId.ReplaceAt(3, 'E'),
                initialId.ReplaceAt(3, 'P'),
                // don't try Japanese just yet
                // (e.g. don't want Pandora's Tower SX3J01 before SX3EXJ)
            };

            foreach(var id in tried)
            {
                yield return id;
            }

            var gameName = GetName(initialId);
            var ids = GetIds(gameName)
                .Where(id => !tried.Contains(id));

            foreach(var id in ids)
            {
                yield return id;
            }
            tried.UnionWith(ids);

            // as last resort, try a match on only the 3 first characters of
            // the key (e.g. for Obscure 2)
            var moreIds = GetIdsStartingWith(initialId.Substring(0, 3))
                .Where(id => !tried.Contains(id));

            foreach (var id in moreIds)
            {
                yield return id;
            }
        }
    }
}

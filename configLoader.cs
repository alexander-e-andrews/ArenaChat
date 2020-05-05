using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YamlDotNet.RepresentationModel;


namespace chatwindow
{
    class configLoader
    {
        public static string logLocation;

        public static int timeOut;

        public static void LoadYaml()
        {
            using (var reader = new StreamReader("config.yaml"))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);

                var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

                // List all the items
                logLocation = mapping.Children[new YamlScalarNode("logLocation")].ToString();
                timeOut = int.Parse( mapping.Children[new YamlScalarNode("timeout")].ToString());
            }
        }
    }
}

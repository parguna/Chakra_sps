using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DI.Chakra.Infrastructure
{
    public class AppTextLocation
    {
        public static string Path = string.Empty;
    }
    public class AppTextSource
    {
        private static AppTextSource _instance = new AppTextSource();
        private static object syncObject = new Object();
        private Dictionary<string, string> _lookup;


        private AppTextSource()
        {
            try
            {
                _lookup = new Dictionary<string, string>();

                AppTextLocation.Path = AppDomain.CurrentDomain.BaseDirectory + "\\assets\\apptexts\\AppTexts.xml";
                var xdoc = XDocument.Load(AppTextLocation.Path);
                _lookup = xdoc.Root.Elements()
                                   .ToDictionary(a => (string)a.Attribute("key"),
                                                 a => (string)a.Attribute("value"));

            }
            catch (Exception)
            {

                throw;
            }
        }
        public static AppTextSource Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncObject)
                    {
                        if (_instance == null)
                            _instance = new AppTextSource();
                    }
                }

                return _instance;
            }
        }

        public static void ReloadAppText()
        {
            _instance = new AppTextSource();
        }

        public string GetString(string key)
        {
            return _lookup[key];
        }
    }
}

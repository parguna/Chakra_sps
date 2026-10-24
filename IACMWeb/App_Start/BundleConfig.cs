using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace DI.Chakra.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //BundleTable.Bundles.IgnoreList.Clear();
            using (var file = File.OpenText(HttpContext.Current.Server.MapPath("~/bundling.json")))
            {
                JsonSerializer serializer = new JsonSerializer();
                var data = serializer.Deserialize(file, typeof(List<BundleValue>)) as List<BundleValue>;
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        if (item != null)
                        {
                            if (item.Type.ToUpper() == "JS")
                            {
                                var bund = new System.Web.Optimization.ScriptBundle(item.Name);
                                foreach (var path in item.Paths)
                                {
                                    if (!string.IsNullOrWhiteSpace(path))
                                        bund.Include(path);
                                }
                                bundles.Add(bund);
                            }
                            if (item.Type.ToUpper() == "CSS")
                            {
                                if (!string.IsNullOrWhiteSpace(item.Name))
                                {
                                    var bund = new System.Web.Optimization.StyleBundle(item.Name);
                                    foreach (var path in item.Paths)
                                    {
                                        if (!string.IsNullOrWhiteSpace(path))
                                            bund.Include(path, new System.Web.Optimization.CssRewriteUrlTransform());
                                    }
                                    bundles.Add(bund);
                                }
                            }
                        }
                    }
                }
            }
            BundleTable.EnableOptimizations = true;
            //System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.Optimization.ScriptBundle("~/bundle/LoginPageJS")
            //      .Include("~/assets/js/jquery-1.11.2.min.js", "~/assets/bootstrap/js/bootstrap.min.js", "~/assets/js/validatr.js", "~/assets/js/jquery.placeholder.js"));

            //System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.Optimization.StyleBundle("~/bundle/LoginPageCSS")
            //     .Include("~/assets/bootstrap/css/bootstrap.css", "~/assets/styles/stylesheet.css",
            //     "~/assets/styles/non-responsive.css", "~/assets/styles/Window.css", "~/IACMSkin/Window.IACMSkin.css"));


            //System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.Optimization.ScriptBundle("~/bundle/MasterPageJS")
            //     .Include("~/assets/js/jquery-1.11.2.min.js", "~/assets/bootstrap/js/bootstrap.min.js", "~/assets/datepicker/js/bootstrap-datepicker.js",
            //     "~/assets/extendbootstrap/js/bootbox.js", "~/assets/extendbootstrap/js/bootstro.min.js", "~/assets/extendbootstrap/js/bootstro.min.js", "~/assets/select/js/bootstrap-select.js",
            //     "~/assets/js/validatr.js", "~/assets/iacm/appMain.js", "~/assets/js/AlertJS.js",
            //     "~/assets/js/jquery.placeholder.js"));
            //System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.Optimization.StyleBundle("~/bundle/MasterPageCSS")
            //  .Include("~/assets/iacm/*.css", new System.Web.Optimization.CssRewriteUrlTransform())
            //  .Include("~/assets/styles/stylesheet.css", new System.Web.Optimization.CssRewriteUrlTransform())
            //  );


        }
}
    public class BundleValue
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public List<string> Paths { get; set; }
    }
}
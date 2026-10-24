using System;
using DI.Chakra.Service.MiscProcessor;

namespace DI.Chakra.TestHarness_Console
{
   public class Program
    {

       // [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public  static void Main(string[] args)
        {
            StartUp();
            //if(args != null && args.Length > 0 && args[0] == "WFR")
            {
                WorkflowReminderService srv = new WorkflowReminderService();
                srv.Start();
            }
            //else
            //{
            //    var config = DomainModel.ConfigurationMaster.GetByConfigKey("CRON_SCHEDULE_EXPRESSION", Infrastructure.Enums.Product.IACM);

            //    var dt = DateTime.Now;

            //    config.ConfigValue = $"0 {dt.Minute + 10} {dt.Hour} {dt.Day} * ?";
            //    DomainModel.ConfigurationMaster.SaveConfiguration(config, Infrastructure.Enums.Product.IACM, "System");
            //    PUBService pUBService = new PUBService();
            //    pUBService.Start();
            //}

            


            Console.WriteLine("Press \'q\' to quit the sample.");
            Console.WriteLine();
            
            while (Console.Read() != 'q') ;



            Test();



        }


        static void Test()
        {
            //DataTable csvData = new DataTable("temp");
            //var attachment = DomainModel.Attachment.GetById(5);
            //using (Stream stream = new MemoryStream(attachment.AttachmentFile))
            //using (TextFieldParser parser = new TextFieldParser(stream))
            //{
            //    parser.SetDelimiters(",");
            //    parser.TrimWhiteSpace = true;
            //    parser.HasFieldsEnclosedInQuotes = true;
            //    string[] colFields = parser.ReadFields();

            //    foreach (string column in colFields)
            //    {
            //        if (csvData.Columns.Contains(column)) continue;
            //        DataColumn datecolumn = new DataColumn(column);
            //        datecolumn.AllowDBNull = true;
            //        csvData.Columns.Add(datecolumn);
            //    }

            //    while (!parser.EndOfData)
            //    {
            //        string[] fieldData = parser.ReadFields();
            //        for (int i = 0; i < fieldData.Length; i++)
            //        {
            //            if (fieldData[i] == "")
            //            {
            //                fieldData[i] = null;
            //            }
            //        }
            //        csvData.Rows.Add(fieldData);
            //    }

           
            //}

        }


        static void StartUp()
        {
            DomainModel.AutoMapperHelper.BootStrap(); 
            
        }
    }
}

namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DI.Chakra.Model.IACMDBContext>
    {
        public static string DROP_TRIGGER_DI_ADS_DI_ADS_GroupInfo_DELETE_Trigger = @"IF OBJECT_ID ('DI_ADS_GroupInfo_DELETE_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_ADS_GroupInfo_DELETE_Trigger";
        public static string DROP_TRIGGER_DI_ADS_GroupInfo_Trigger = @"IF OBJECT_ID ('DI_ADS_GroupInfo_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_ADS_GroupInfo_Trigger";
        public static string DROP_TRIGGER_DI_ADS_OuInfo_DELETE_Trigger = @"IF OBJECT_ID ('DI_ADS_OuInfo_DELETE_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_ADS_OuInfo_DELETE_Trigger";
        public static string DROP_TRIGGER_DI_ADS_OuInfo_Trigger = @"IF OBJECT_ID ('DI_ADS_OuInfo_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_ADS_OuInfo_Trigger";
        public static string DROP_TRIGGER_DI_ADS_UserInfo_DELETE_Trigger = @"IF OBJECT_ID ('DI_ADS_UserInfo_DELETE_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_ADS_UserInfo_DELETE_Trigger";
        public static string DROP_TRIGGER_DI_ADS_UserInfo_Trigger = @"IF OBJECT_ID ('DI_ADS_UserInfo_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_ADS_UserInfo_Trigger";
        public static string DROP_TRIGGER_DI_M_AccessRequest_Trigger = @"IF OBJECT_ID ('DI_M_AccessRequest_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_M_AccessRequest_Trigger";
        public static string DROP_TRIGGER_DI_M_AttestationReview_EndpointSystem_L2Detail_Trigger = @"IF OBJECT_ID ('DI_M_AttestationReview_EndpointSystem_L2Detail_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_M_AttestationReview_EndpointSystem_L2Detail_Trigger";
        public static string DROP_TRIGGER_DI_M_AttestationReview_EndpointSystem_L3Detail_Trigger = @"IF OBJECT_ID ('DI_M_AttestationReview_EndpointSystem_L3Detail_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_M_AttestationReview_EndpointSystem_L3Detail_Trigger";
        public static string DROP_TRIGGER_DI_M_AttestationReview_EndpointSystem_Trigger = @"IF OBJECT_ID ('DI_M_AttestationReview_EndpointSystem_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_M_AttestationReview_EndpointSystem_Trigger";
        public static string DROP_TRIGGER_DI_M_DiscoveryProcess_Trigger = @"IF OBJECT_ID ('DI_M_DiscoveryProcess_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_M_DiscoveryProcess_Trigger";
        public static string DROP_TRIGGER_DI_M_Email_Trigger = @"IF OBJECT_ID ('DI_M_Email_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_M_Email_Trigger";
        public static string DROP_TRIGGER_DI_SAP_USERINFO_TRIGGER = @"IF OBJECT_ID ('DI_SAP_USERINFO_TRIGGER', 'TR') IS NOT NULL DROP TRIGGER DI_SAP_USERINFO_TRIGGER";
        public static string DROP_TRIGGER_DI_TRS_UserInfo_DELETE_Trigger = @"IF OBJECT_ID ('DI_TRS_UserInfo_DELETE_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_TRS_UserInfo_DELETE_Trigger";
        public static string DROP_TRIGGER_DI_TRS_UserInfo_Trigger = @"IF OBJECT_ID ('DI_TRS_UserInfo_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_TRS_UserInfo_Trigger";
        public static string DROP_TRIGGER_DI_Win_GroupInfo_DELETE_Trigger = @"IF OBJECT_ID ('DI_Win_GroupInfo_DELETE_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_Win_GroupInfo_DELETE_Trigger";
        public static string DROP_TRIGGER_DI_Win_GroupInfo_Trigger = @"IF OBJECT_ID ('DI_Win_GroupInfo_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_Win_GroupInfo_Trigger";
        public static string DROP_TRIGGER_DI_Win_ResourceInfo_DELETE_Trigger = @"IF OBJECT_ID ('DI_Win_ResourceInfo_DELETE_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_Win_ResourceInfo_DELETE_Trigger";
        public static string DROP_TRIGGER_DI_Win_ResourceInfo_Trigger = @"IF OBJECT_ID ('DI_Win_ResourceInfo_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_Win_ResourceInfo_Trigger";
        public static string DROP_TRIGGER_DI_WIN_UserInfo_DELETE_Trigger = @"IF OBJECT_ID ('DI_WIN_UserInfo_DELETE_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_WIN_UserInfo_DELETE_Trigger";
        public static string DROP_TRIGGER_DI_WIN_UserInfo_Trigger = @"IF OBJECT_ID ('DI_WIN_UserInfo_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_WIN_UserInfo_Trigger";
        public static string DROP_TRIGGER_DI_M_Organisation_DELETE_Trigger = @"IF OBJECT_ID ('DI_M_Organisation_DELETE_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_M_Organisation_DELETE_Trigger";
        public static string DROP_TRIGGER_DI_M_Organisation_Trigger = @"IF OBJECT_ID ('DI_M_Organisation_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_M_Organisation_Trigger";
        public static string DROP_TRIGGER_DI_SAPEP_Groups_DELETE_Trigger = @"IF OBJECT_ID ('DI_SAPEP_GroupInfo_DELETE_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_SAPEP_GroupInfo_DELETE_Trigger";
        public static string DROP_TRIGGER_DI_SAPEP_Groups_Trigger = @"IF OBJECT_ID ('DI_SAPEP_GroupInfo_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_SAPEP_GroupInfo_Trigger";
        public static string DROP_TRIGGER_DI_SAPEP_Roles_DELETE_Trigger = @"IF OBJECT_ID ('DI_SAPEP_RoleInfo_DELETE_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_SAPEP_RoleInfo_DELETE_Trigger";
        public static string DROP_TRIGGER_DI_SAPEP_Roles_Trigger = @"IF OBJECT_ID ('DI_SAPEP_RoleInfo_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_SAPEP_RoleInfo_Trigger";
        public static string DROP_TRIGGER_DI_SAPEP_Users_DELETE_Trigger = @"IF OBJECT_ID ('DI_SAPEP_UserInfo_DELETE_Trigger', 'TR') IS NOT NULL DROP TRIGGER DI_SAPEP_UserInfo_DELETE_Trigger";
        public static string DROP_TRIGGER_DI_SAPEP_Users_Trigger = @"IF OBJECT_ID ('DI_SAPEP_UserInfo_TRIGGER', 'TR') IS NOT NULL DROP TRIGGER DI_SAPEP_UserInfo_TRIGGER";

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DI.Chakra.Model.IACMDBContext context)
        {
            //  This method will be called after migrating to the latest version.


            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_ADS_DI_ADS_GroupInfo_DELETE_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_ADS_GroupInfo_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_ADS_OuInfo_DELETE_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_ADS_OuInfo_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_ADS_UserInfo_DELETE_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_ADS_UserInfo_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_M_AccessRequest_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_M_AttestationReview_EndpointSystem_L2Detail_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_M_AttestationReview_EndpointSystem_L3Detail_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_M_AttestationReview_EndpointSystem_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_M_DiscoveryProcess_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_M_Email_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_SAP_USERINFO_TRIGGER);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_TRS_UserInfo_DELETE_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_TRS_UserInfo_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_Win_GroupInfo_DELETE_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_Win_GroupInfo_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_Win_ResourceInfo_DELETE_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_Win_ResourceInfo_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_WIN_UserInfo_DELETE_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_WIN_UserInfo_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_M_Organisation_DELETE_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_M_Organisation_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_SAPEP_Groups_DELETE_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_SAPEP_Groups_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_SAPEP_Roles_DELETE_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_SAPEP_Roles_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_SAPEP_Users_DELETE_Trigger);
            //context.Database.ExecuteSqlCommand(DROP_TRIGGER_DI_SAPEP_Users_Trigger);


            //foreach (var filePath in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "\\SqlScripts\\Triggers\\", "*.sql"))
            //{

            //    context.Database.ExecuteSqlCommand(File.ReadAllText(filePath));
            //}
        }
    }
}

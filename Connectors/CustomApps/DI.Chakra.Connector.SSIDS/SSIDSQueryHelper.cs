namespace DI.Chakra.Connector
{
    public class SSIDSQueryHelper
    {
        internal const string ConnectionStringFormatter = "Data Source={0}; Initial Catalog={1}; Persist Security Info= True; {2} Connection Timeout=30";

        internal const string GUI_SelectUserQuery = "p_spSSIDS_USERS_Rpt";                                                     
        internal const string GUI_InsertUserQuery = @"p_spSSIDS_User_Insert";
        internal const string GUI_UpdateUserQuery = @"p_spSSIDS_User_Update";
        internal const string GUI_LockUserQuery = @"p_spAdmin_Deactivate_UserAccount";
        internal const string GUI_UnLockUserQuery = @"p_spAdmin_Activate_Password";

        internal const string SMS_SelectUserQuery = "p_spSSIDS_IDMS_Discovery";//old SP:p_spSSIDS_SMS_USERS_Rpt
        internal const string SMS_InsertUserQuery = @"p_spAdmin_UsersIOD_GUI_Create";
        internal const string SMS_UpdateUserQuery = @"p_spAdmin_UsersIOD_GUI_Update";
        internal const string SMS_LockUserQuery = @"p_spAdmin_UsersIOD_VFlag_Update";
        internal const string SMS_UnLockUserQuery = @"p_spAdmin_UsersIOD_VFlag_Update";

        internal const string SMS_HR_InsertUserQuery = @"p_spSSIDS_employeehr_Insert";
        //internal const string SMS_HR_UpdateUserQuery = @"p_spSSIDS_employeehr_Update";

        internal const string GUI_GET_DefaultPassword = @"p_spAdmin_UserInfo_DefaultPwd";
    }
}

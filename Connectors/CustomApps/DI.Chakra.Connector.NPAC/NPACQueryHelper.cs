using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector
{
    public class NPACQueryHelper
    {
        internal const string ConnectionStringFormatter = "Data Source={0}; Initial Catalog={1}; Persist Security Info= True; {2} Connection Timeout=30";
        //internal const string ConnectionStringFormatter = "Data Source={0}; Initial Catalog={1}; Persist Security Info= True; Integrated Security=SSPI; Connection Timeout=30";
        internal const string SelectUserQuery = "SELECT * FROM USERS";

        internal const string SelectSectionsQuery = "SELECT * FROM [omdb].[dbo].[ref_master] WHERE mtype = 'SECT'";

        internal const string SelectUserByUserLoginQuery = @"SELECT * FROM USERS WHERE userid = @username";
        internal const string updateUserQuery = @"UPDATE USERS SET 
                                                    [phone] = @phone, 
                                                    [username] = @username,
                                                    [pager]= @pager, 
                                                    [section]=@section , 
                                                    [zone]=@zone,
                                                    [email] =@email,
                                                    [user_adid]= @users_adid,                                                  
                                                    [company] =@company ,
                                                    [section_name] = @section_name,
                                                    [designation] = @designation
                                                    WHERE userid = @userid";

        internal const string updateUserIdentityQuery = @"UPDATE USERS SET userid = @userid WHERE userid = @olduserid";
        internal const string ReOrgUserQuery = @"UPDATE USERS SET section = @section,designation=@designation, [company]=@company, [section_name] = @section_name WHERE userid = @userid";

        internal const string GetEncryptPasswordQuery = @"SELECT [dbo].[f_encrypt] (@encrypt_pwd)";

        internal const string InsertUserQuery = @"INSERT INTO USERS ([emp_id], [userid],[username],[passwds],[designation],[phone],[pager],[fax],[groups],[section],[grade],[zone],[create_date],[encrypt_pwd],[invalid_date],[email],[user_adid],[company],[section_name] )
                                                        VALUES (@emp_id, @userid, @username, @passwds, @designation, @phone, @pager, @fax, @groups, @section, @grade, @zone, @create_date, @encrypt_pwd, @invalid_date,@email, @users_adid, @company, @section_name)";
        internal const string InsertUserLevelQuery = @"UPDATE USERS SET levels = @levels WHERE userid = @username";

        internal const string DeleteUserLevelQuery = @"UPDATE USERS SET levels = @levels WHERE userid = @username AND levels = @currentlevel";
        internal const string RemoveUserLevelQuery = @"UPDATE USERS SET levels = @levels WHERE userid = @username";
        internal const string DeleteUserQuery = @"DELETE USERS WHERE userid = @username";

        internal const string LockUserQuery = @"UPDATE USERS SET invalid_date = @invalid_date, levels = @levels, section = @section,section_name = @sectionname,zone=@zone WHERE userid = @username";
        internal const string UnLockUserQuery = @"UPDATE USERS SET invalid_date = @invalid_date WHERE userid = @username";

        internal const string DisableUserQuery = @"UPDATE USERS SET levels = @levels, section = @section WHERE userid = @username";
        internal const string EnableUserQuery = @"UPDATE USERS SET invalid_date = @invalid_date WHERE userid = @username";

        //internal const string SelectMaxUserIdQuery = "SELECT MAX(userid) FROM USERS";

        internal const string CheckUserLoginExistQuery = @"SELECT * FROM USERS WHERE userid = @username";
        internal const string ChangePasswordQuery = @"UPDATE USERS SET [encrypt_pwd] = @encrypt_pwd WHERE userid = @username";

        internal const string ExtendUserQuery = @"UPDATE USERS SET invalid_date = @invalid_date, section = @section WHERE userid = @username";
        internal const string ChangePasswordQueryForPasswordReset = @"UPDATE USERS SET [valid_date] = @valid_date,[encrypt_pwd] = @encrypt_pwd, invalid_date = NULL WHERE userid = @username";
        internal const string UnLockUserForPasswordResetQuery = @"UPDATE USERS SET [valid_date] = @valid_date, invalid_date = NULL WHERE userid = @username";
    }
}

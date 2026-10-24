using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector
{
    public class GEMSQueryHelper
    {
        internal const string ConnectionStringFormatter = "Data Source={0}; Initial Catalog={1}; Persist Security Info= True; {2} Connection Timeout=30";
        //internal const string ConnectionStringFormatter = "Data Source={0}; Initial Catalog={1}; Persist Security Info= True; Integrated Security=true; Connection Timeout=30";

        internal const string SelectUserQuery = "SELECT [USER_ID],[USER_NAME],DISPLAY_NAME,DESCRIPTION,EMAIL,DATEMODIFIED,LASTUSER  FROM (SELECT DENSE_RANK() over(PARTITION by [USER_NAME] ORDER BY [USER_ID] DESC ) as Id,[USER_ID],[USER_NAME],DISPLAY_NAME,DESCRIPTION,EMAIL,DATEMODIFIED,LASTUSER FROM [process].[MM_PX_USER]) AS A WHERE A.ID = 1";
        internal const string SelectRoleQuery = "Select * from [process].[MM_PX_ROLE]";
        internal const string SelectUserRoleQuery = "Select * from [process].[MM_PX_USER_ROLE]";

        internal const string SelectUserByUserLoginQuery = @"SELECT * FROM [process].[MM_PX_USER] WHERE USER_NAME = @USER_NAME";
        internal const string updateUserQuery = @"UPDATE [process].[MM_PX_USER] SET DISPLAY_NAME = @DISPLAY_NAME, DESCRIPTION = @DESCRIPTION, EMAIL = @EMAIL,
                                                    LASTUSER = @LASTUSER, DATEMODIFIED = @DATEMODIFIED WHERE  USER_NAME = @USER_NAME";

        internal const string InsertUserQuery = @"INSERT INTO [process].[MM_PX_USER] ([USER_ID],[USER_NAME],[DISPLAY_NAME],[DESCRIPTION],[EMAIL],[LASTUSER],[DATEMODIFIED])
                                                        VALUES (@USER_ID, @USER_NAME, @DISPLAY_NAME, @DESCRIPTION, @EMAIL, @LASTUSER,@DATEMODIFIED)";
        internal const string InsertUserRoleQuery = @"INSERT INTO [process].[MM_PX_USER_ROLE] ([USER_ID],[ROLE_ID],[DATEMODIFIED],[LASTUSER]) VALUES (@USER_ID, @ROLE_ID, @DATEMODIFIED,@LASTUSER)";

        internal const string DeleteUserRolesByUserIdQuery = @"DELETE [process].[MM_PX_USER_ROLE] WHERE USER_ID = @USER_ID";
        internal const string DeleteUserRoleQuery = @"DELETE [process].[MM_PX_USER_ROLE] WHERE USER_ID = @USER_ID AND ROLE_ID = @ROLE_ID";
        internal const string DeleteUserQuery = @"DELETE [process].[MM_PX_USER] WHERE USER_NAME = @USER_NAME";
        internal const string DeleteRoleQuery = @"DELETE [process].[MM_PX_ROLE] WHERE NAME = @NAME";

        internal const string LockUserQuery = @"UPDATE [process].[MM_PX_USER] SET LASTUSER = @LASTUSER WHERE USER_NAME = @USER_NAME";
        internal const string UnLockUserQuery = @"UPDATE [process].[MM_PX_USER] SET LASTUSER = @LASTUSER WHERE USER_NAME = @USER_NAME";

        internal const string DisableUserQuery = @"UPDATE [process].[MM_PX_USER] SET LASTUSER = @LASTUSER WHERE USER_NAME = @USER_NAME";
        internal const string EnableUserQuery = @"UPDATE [process].[MM_PX_USER] SET LASTUSER = @LASTUSER WHERE USER_NAME = @USER_NAME";

        //internal const string SelectMaxUserIdQuery = "SELECT MAX(USER_ID) FROM [process].[MM_PX_USER]";
        //internal const string SelectMaxUserRoleIdQuery = "SELECT MAX(UserRoleID) FROM [process].[MM_PX_USER_ROLE]";
        internal const string GetUserIdByUserNameQuery = "SELECT UserID FROM [process].[MM_PX_USER] WHERE USER_NAME = @USER_NAME";

        internal const string CheckUserLoginExistQuery = @"SELECT * FROM [process].[MM_PX_USER] WHERE USER_NAME = @USER_NAME";
        internal const string CheckUserRoleExistQuery = @"SELECT * FROM [process].[MM_PX_USER_ROLE] WHERE USER_ID = @USER_ID AND ROLE_ID = @ROLE_ID";

        internal const string CheckUserExistQuery = @"SELECT 1 FROM [process].[MM_PX_USER] WHERE USER_ID = @USER_ID";

    }
}

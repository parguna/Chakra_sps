using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.Logger
{
    internal class QueryHelper
    {
        internal static List<string> TableNamesList = new List<string> { "DI_Log_UI", "DI_Log_UIHistory", "DI_Log_UIBulk", "DI_Log_Service", "DI_Log_ServiceHistory", "DI_Log_ServiceBulk" };

        #region UI Queries

        internal const string CreateLogTable_UIQuery = @"CREATE TABLE [dbo].[DI_Log_UI](
                                                        Id			[int] IDENTITY(1,1) NOT NULL,
                                                        UserId		[INT] NULL,					
                                                        UserName	[varchar](150) NULL,
                                                        SessionId	[varchar](100) NULL,
                                                        IP			[varchar](20) NULL, 
                                                        TabName		[varchar](100) NULL,
                                                        MenuName	[varchar](100) NULL,
                                                        UIAction	[varchar](50) NULL,
                                                        MessageText	[varchar](max) NULL,
                                                        LocalTime	[datetime] NULL,
                                                        ServerTime	[datetime] NULL
                                                        CONSTRAINT [PK_DI_Log_UI] PRIMARY KEY CLUSTERED 
                                                        ([Id] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]";


        internal const string CreateLogHistoryTable_UIQuery = @"CREATE TABLE [dbo].[DI_Log_UIHistory](
                                                        Id			[int] IDENTITY(1,1) NOT NULL,
                                                        UserId		[INT] NULL,					
                                                        UserName	[varchar](150) NULL,
                                                        SessionId	[varchar](100) NULL,
                                                        IP			[varchar](20) NULL, 
                                                        TabName		[varchar](100) NULL,
                                                        MenuName	[varchar](100) NULL,
                                                        UIAction	[varchar](50) NULL,
                                                        MessageText	[varchar](max) NULL,
                                                        LocalTime	[datetime] NULL,
                                                        ServerTime	[datetime] NULL
                                                        CONSTRAINT [PK_DI_Log_UIHistory] PRIMARY KEY CLUSTERED 
                                                        ([Id] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]";

        internal const string CreateLogBulkTable_UIQuery = @"CREATE TABLE [dbo].[DI_Log_UIBulk](
                                                        UserId		[INT] NULL,					
                                                        UserName	[varchar](150) NULL,
                                                        SessionId	[varchar](100) NULL,
                                                        IP			[varchar](20) NULL, 
                                                        TabName		[varchar](100) NULL,
                                                        MenuName	[varchar](100) NULL,
                                                        UIAction	[varchar](50) NULL,
                                                        MessageText	[varchar](max) NULL,
                                                        LocalTime	[datetime] NULL,
                                                        ServerTime	[datetime] NULL)";

        internal const string IsLogTableExistQuery = @"IF EXISTS(SELECT * FROM SYS.TABLES WHERE name ='{0}') SELECT 'YES'AS [IsTableExists] ELSE SELECT 'NO' AS [IsTableExists]";


        internal const string InsertLog_UIQuery = @"INSERT INTO [DI_Log_UI]
                                                ([UserId],[UserName],[SessionId],[IP],[TabName],[MenuName],[UIAction],[MessageText],[LocalTime],[ServerTime])VALUES 
                                                (@UserId,@UserName,@SessionId,@IP,@TabName,@MenuName,@UIAction,@MessageText,@LocalTime,@ServerTime)";

        internal const string InsertIntoHistoryFromLive_UIQuery = @"INSERT INTO DI_Log_UIHistory (UserId,UserName,SessionId,IP,TabName,MenuName,UIAction,MessageText,LocalTime,ServerTime)
                                                                SELECT UserId,UserName,SessionId,IP,TabName,MenuName,UIAction,MessageText,LocalTime,ServerTime FROM DI_Log_UI";

        internal const string InsertIntoBulkFromHistory_UIQuery = @"INSERT INTO DI_Log_UIBulk(UserId,UserName,SessionId,IP,TabName,MenuName,UIAction,MessageText,LocalTime,ServerTime)
                                                                SELECT UserId,UserName,SessionId,IP,TabName,MenuName,UIAction,MessageText,LocalTime,ServerTime FROM DI_Log_UIHistory
                                                                WHERE DATEDIFF(DAY,GETDATE(),ServerTime) > @NoOfDays";

        internal const string TruncateUILogTable_UIQuery = @"TRUNCATE TABLE DI_Log_UI";

        internal const string DeleteLogHistoryExcessData_UIQuery = @"DELETE DI_Log_UIHistory WHERE DATEDIFF(DAY,GETDATE(),ServerTime) >  @NoOfDays";

        internal const string GetAllLog_UIQuery = @"SELECT *  FROM DI_Log_UI WITH (NOLOCK)";

        internal const string GetAllLogByUserName_UIQuery = @"SELECT *  FROM DI_Log_UI WITH (NOLOCK) WHERE UserName LIKE @UserName";

        internal const string GetAllLogHistoryByUserName_UIQuery = @"SELECT *  FROM DI_Log_UIHistory WITH (NOLOCK) WHERE UserName LIKE @UserName AND ServerTime >= @StartDate AND ServerTime <= @EndDate
                                                                    UNION ALL SELECT *  FROM DI_Log_UI WITH (NOLOCK) WHERE UserName LIKE @UserName AND ServerTime >= @StartDate AND ServerTime <= @EndDate";

        #endregion

        #region Service Queries
        internal const string CreateLogTable_ServiceQuery = @"CREATE TABLE [dbo].[DI_Log_Service](
                                                            Id					[int] IDENTITY(1,1) NOT NULL,
                                                            LogType				[varchar](50) NULL,					
                                                            Type			[varchar](50) NULL,
                                                            EndpointType		[varchar](100) NULL,
                                                            EndpointSystemId	[varchar](20) NULL, 
                                                            [Action]			[varchar](100) NULL,
                                                            [Message]			[varchar](max) NULL,
                                                            LogDateTime			[datetime] NULL
                                                            CONSTRAINT [PK_DI_Log_Service] PRIMARY KEY CLUSTERED 
                                                            ([Id] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]";

        internal const string CreateLogHistoryTable_ServiceQuery = @"CREATE TABLE [dbo].[DI_Log_ServiceHistory](
                                                            Id					[int] IDENTITY(1,1) NOT NULL,
                                                            LogType				[varchar](50) NULL,					
                                                            Type			[varchar](50) NULL,
                                                            EndpointType		[varchar](100) NULL,
                                                            EndpointSystemId	[varchar](20) NULL, 
                                                            [Action]			[varchar](100) NULL,
                                                            [Message]			[varchar](max) NULL,
                                                            LogDateTime			[datetime] NULL
                                                            CONSTRAINT [PK_DI_Log_ServiceHistory] PRIMARY KEY CLUSTERED 
                                                            ([Id] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]";

        internal const string CreateLogBulkTable_ServiceQuery = @"CREATE TABLE [dbo].[DI_Log_ServiceBulk](
                                                                LogType				[varchar](50) NULL,					
                                                                Type			[varchar](50) NULL,
                                                                EndpointType		[varchar](100) NULL,
                                                                EndpointSystemId	[varchar](20) NULL, 
                                                                [Action]			[varchar](100) NULL,
                                                                [Message]			[varchar](max) NULL,
                                                                LogDateTime			[datetime] NULL)";

        internal const string InsertLog_ServiceQuery = @"INSERT INTO DI_Log_Service ([LogType],[Type],[EndpointType],[EndpointSystemId],[Action],[Message],[LogDateTime])
                                                        VALUES (@LogType,@Type,@EndpointType,@EndpointSystemId,@Action,@Message,@LogDateTime)";

        internal const string InsertIntoHistoryFromLive_ServiceQuery = @"INSERT INTO DI_Log_ServiceHistory ([LogType],[Type],[EndpointType],[EndpointSystemId],[Action],[Message],[LogDateTime])
                                                                        SELECT [LogType],[Type],[EndpointType],[EndpointSystemId],[Action],[Message],[LogDateTime] FROM DI_Log_Service";

        internal const string InsertIntoBulkFromHistory_ServiceQuery = @"INSERT INTO DI_Log_ServiceBulk ([LogType],[Type],[EndpointType],[EndpointSystemId],[Action],[Message],[LogDateTime])
                                                                SELECT [LogType],[Type],[EndpointType],[EndpointSystemId],[Action],[Message],[LogDateTime] FROM DI_Log_ServiceHistory
                                                                WHERE DATEDIFF(DAY,GETDATE(),LogDateTime) > @NoOfDays";

        internal const string TruncateLogTable_ServiceQuery = @"TRUNCATE TABLE DI_Log_Service";

        internal const string DeleteLogHistoryExcessData_ServiceQuery = @"DELETE DI_Log_ServiceHistory WHERE DATEDIFF(DAY,GETDATE(),LogDateTime) >  @NoOfDays";

        internal const string GetAllLog_ServiceQuery = @"SELECT *  FROM DI_Log_ServiceHistory WITH (NOLOCK) WHERE LogDateTime >= @StartDate AND LogDateTime <= @EndDate
                                                         UNION ALL SELECT *  FROM DI_Log_Service WITH (NOLOCK) WHERE  LogDateTime >= @StartDate AND LogDateTime <= @EndDate";

        internal const string GetLogPerActivity_ServiceQuery = @"SELECT *  FROM DI_Log_ServiceHistory WITH (NOLOCK) WHERE LogDateTime >= @StartDate AND LogDateTime <= @EndDate AND LogType= @Actiity
                                                                 UNION ALL SELECT *  FROM DI_Log_Service WITH (NOLOCK) WHERE  LogDateTime >= @StartDate AND LogDateTime <= @EndDate AND LogType= @Actiity";


        #endregion

    }

    internal class CheckDBNull
    {
        public static int AgainstInt(object obj)
        {

            if (obj == DBNull.Value)
                return -1;

            if (null == obj)
                return -1;

            if (string.Empty == obj.ToString())
                return -1;

            return Convert.ToInt32(obj);
        }
        public static Int32? AgainstNullableInt(object obj)
        {

            if (obj == DBNull.Value)
                return null;

            if (null == obj)
                return null;

            if (string.Empty == obj.ToString())
                return null;


            return Convert.ToInt32(obj);
        }
        public static double AgainstDouble(object obj)
        {

            if (obj == DBNull.Value)
                return -1;

            if (null == obj)
                return -1;

            if (string.Empty == obj.ToString())
                return -1;

            return Convert.ToDouble(obj);
        }
        public static double? AgainstNullableDouble(object obj)
        {

            if (obj == DBNull.Value)
                return null;

            if (null == obj)
                return null;

            if (string.Empty == obj.ToString())
                return null;

            return Convert.ToDouble(obj);
        }
        public static string AgainstString(object obj)
        {
            if (obj == DBNull.Value)
                return string.Empty;
            if (null == obj)
                return string.Empty;

            return Convert.ToString(obj);
        }
        public static bool AgainstBit(object obj)
        {
            if (obj == DBNull.Value)
                return false;
            if (null == obj)
                return false;

            if (string.Empty == obj.ToString())
                return false;

            return Convert.ToBoolean(obj);
        }
        public static bool? AgainstNullableBit(object obj)
        {
            if (obj == DBNull.Value)
                return null;
            if (null == obj)
                return null;

            if (string.Empty == obj.ToString())
                return null;

            return Convert.ToBoolean(obj);
        }
        public static DateTime? AgainstNullableDatetime(object obj)
        {

            if (obj == DBNull.Value)
                return null;

            if (null == obj)
                return null;

            if (string.Empty == obj.ToString())
                return null;

            return Convert.ToDateTime(obj);
        }
        public static DateTime AgainstDatetime(object obj)
        {

            if (obj == DBNull.Value)
                return DateTime.MinValue;

            if (null == obj)
                return DateTime.MinValue;

            if (string.Empty == obj.ToString())
                return DateTime.MinValue;

            return Convert.ToDateTime(obj);
        }
        public static TimeSpan? AgainstNullableTimeSpan(object obj)
        {

            if (obj == DBNull.Value)
                return null;

            if (null == obj)
                return null;

            if (string.Empty == obj.ToString())
                return null;

            return TimeSpan.Parse(obj.ToString());
        }
        public static TimeSpan AgainstTimeSpan(object obj)
        {

            if (obj == DBNull.Value)
                return new TimeSpan();

            if (null == obj)
                return new TimeSpan();

            if (string.Empty == obj.ToString())
                return new TimeSpan();

            return TimeSpan.Parse(obj.ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DI.Chakra.Services.EmailingProcessor
{
    internal class QueryHelperEmail
    {

        internal const string GetAllEmailInfo_UIQuery = @"with Email AS 
                                                        ( Select TOP(100) * from DI_M_Email where Priority = 'High' and [Status] ='Pending'
                                                        UNION Select TOP(100) * from DI_M_Email where Priority = 'Normal' and [Status] ='Pending'
                                                        UNION Select TOP(100) * from DI_M_Email where Priority = 'Low' and [Status] ='Pending'
                                                        UNION Select TOP(100) * from DI_M_Email where Priority = 'High' and [Status] ='Failed'
                                                        UNION Select TOP(100) * from DI_M_Email where Priority = 'Normal' and [Status] ='Failed'
                                                        UNION Select TOP(100) * from DI_M_Email where Priority = 'Low' and [Status] ='Failed'
                                                        ) Select Top(100) * from Email where DispatchDateTime <= getdate()";

        internal const string GetOTPEmailInfo_UIQuery = @"with Email AS 
                                                        ( Select TOP(100) * from DI_M_Email where Priority = 'High' and [Status] ='Pending' and ActivityType='OTP'
                                                        UNION Select TOP(100) * from DI_M_Email where Priority = 'Normal' and [Status] ='Pending'   and ActivityType='OTP'
                                                        UNION Select TOP(100) * from DI_M_Email where Priority = 'Low' and [Status] ='Pending'  and ActivityType='OTP'
                                                        UNION Select TOP(100) * from DI_M_Email where Priority = 'High' and [Status] ='Failed'  and ActivityType='OTP'
                                                        UNION Select TOP(100) * from DI_M_Email where Priority = 'Normal' and [Status] ='Failed'  and ActivityType='OTP'
                                                        UNION Select TOP(100) * from DI_M_Email where Priority = 'Low' and [Status] ='Failed'  and ActivityType='OTP'
                                                        ) Select Top(100) * from Email where DispatchDateTime <= getdate()";


        internal const string GetAllEmailInfoByStatus_UIQuery = @"SELECT *  FROM DI_M_Email WITH (NOLOCK) WHERE [Status] = @Status";



        internal const string InsertEmail_UIQuery = @"INSERT INTO [DI_M_Email]
                                                ([ActivityType],[Priority],[From],[To],[Cc],[Bcc],[Subject],[Body],[IsHtml],[Attachment],[Status],[CreatedBy],[CreatedDateTime])VALUES 
                                                (@ActivityType,@Priority,@From,@To,@Cc,@Bcc,@Subject,@Body,@IsHtml,@Attachment,@Status,@CreatedBy,getdate())";

        internal const string UpdateEmail_UIQuery = @"UPDATE [DI_M_Email] SET [Status]=@Status,SentDateTime=@SentDateTime 
                                                    WHERE Id = @Id";

        internal const string UpdateFailedEmail_UIQuery = @"UPDATE [DI_M_Email] SET [Status]=@Status
                                                    WHERE Id = @Id";

        internal const string DeleteSentEmail_UIQuery = @"DELETE  FROM DI_M_Email
                                                            WHERE [Status] in ('Sent', 'Excluded')";

        internal const string GetAllServerInfo_UIQuery = @"SELECT *  FROM DI_M_MailServerConfiguration ORDER BY Id ASC";

        internal const string GetAllEmailInfoByActivity_UIQuery = @"Select * from DI_M_Email 
                                                                    where ([Status] = @Status or @Status = '') AND
                                                                    (ActivityType = @ActivityType or @ActivityType = '')";

        internal const string UpdateEmailInfo_Query = @"UPDATE [DI_M_Email] SET [To]=@To,Cc=@Cc,Bcc=@Bcc
                                                    WHERE Id = @Id";

        internal const string DeleteEmailInfo_UIQuery = @"DELETE  FROM DI_M_Email
                                                            WHERE Id = @Id";

        internal const string GetAllEmailInfoHistory_UIQuery = @"SELECT *  FROM DI_M_EmailHistory WITH (NOLOCK) where SentDateTime >= @SentDateTime";

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

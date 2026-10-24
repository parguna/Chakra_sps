using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Services.EmailingProcessor
{
    public class EmailSave
    {
        protected DBHelperEmail m_dbHelper;
        internal EmailSave()
        {
            m_dbHelper = DBHelperEmail.CreateDBHelper();
        }

        public IList<DomainModel.EmailInfo> GetEmailsToSend(EmailActivityType emailActivityType)
        {
            return m_dbHelper.GetEmailInfo(emailActivityType);
        }

        public void Update(DomainModel.EmailInfo _info)
        {
            m_dbHelper.UpdateEmailtoDB(_info);
        }
        public void MoveSentItems(IList<DomainModel.EmailInfo> _lst)
        {
            //1. Find SENT mails
            //2. Bulk insert into History Table
            //3. Remove SENT mails

            if (_lst.Count <= 0)
                return;
            m_dbHelper.GetEmailInfo("Sent");
            m_dbHelper.SqlBulkInsert("DI_M_EmailHistory", _lst);

            var _tmp_lst = m_dbHelper.GetEmailInfo("Excluded");
            m_dbHelper.SqlBulkInsert("DI_M_EmailHistory", _tmp_lst);

            m_dbHelper.DeleteSentMails();
        }

        public void Insert(DomainModel.EmailInfo _info)
        {
            m_dbHelper.SaveEmailtoDB(_info);
        }
        public void Insert(IList<DomainModel.EmailInfo> _lst)
        {
            m_dbHelper.SqlBulkInsert("DI_M_Email",_lst);
        }
        public IList<MailServerConfigInfo> GetServerInfo()
        {
            return m_dbHelper.GetServerInfo();
        }
    }
}

using System;

namespace DI.Chakra.Web.AccessRequest.Common
{
    public partial class ViewCurrentAccessUC : DI.Chakra.Web.UIBaseUC
    {
        private long _mUserId;
        public long UserId
        {
            set
            {
                _mUserId = value;
                SetValues(_mUserId);
            }
        }

        private void SetValues(long userId)
        {
            if (userId == 0)
                return;
            
            CurrentAccessUC.EntityId = userId;
            CurrentAccess.Width = GetUnit(80);
            CurrentAccessUC.HideAccountDetail = true;
            CurrentAccessUC.Rebind();
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            CurrentAccess.VisibleOnPageLoad = false;
        }
    }
}
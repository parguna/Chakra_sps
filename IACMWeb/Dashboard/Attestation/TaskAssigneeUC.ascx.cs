using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI;
using DI.Chakra.Dto;


namespace DI.Chakra.Web.Dashboard.Attestation
{
    public partial class TaskAssigneeUC :DI.Chakra.Web.UIBaseUC
    {
        protected void Page_Load( object sender , EventArgs e )
        {
            if (!IsPostBack)
            {
                if (null != Session["AtnDashboardDefault"])
                {
                    PopulateAssigneeDetails(Convert.ToString(Session["AtnDashboardDefault"]));
                    HideNoData();
                }
                else
                {
                    AttestationTypeUC.LoadAttestationType();
                    if (null != Session["AtnDashboardDefault"])
                    {
                        PopulateAssigneeDetails(Convert.ToString(Session["AtnDashboardDefault"]));
                        HideNoData();
                    }
                    else
                    {
                        ShowNoData();
                    }
                }
            }
            DashBoardHeaderUC.HeaderImageUrl = Imageurl.HEADERIMAGE;
            AttestationTypeUC.SelectedIndexChanged += new EventHandler(ddlAttestation_SelectedIndexChanged);

        }

        private void PopulateAssigneeDetails( string attestationName )
        {
            Dictionary<string , string> parameter = new Dictionary<string , string>();
            parameter.Add("attestationName" , attestationName);

            var taskInfo = DomainApiClient<Dto.Task>.GetByAction<List<Dto.Task>>(DomainApiAction.Tasks.Task.GetTaskInfo , parameter);
            Image[] imgArray = new Image[taskInfo.Count];

            int arrayCount = 0;
            parameter.Clear();
            if ((taskInfo != null && taskInfo.Count > 0))
            {
                foreach (DI.Chakra.Dto.Task task in taskInfo)
                {
                    var userId = task.ActionUserId;
                    parameter.Add("userId" , userId.ToString());
                    var userInfo = DomainApiClient<User>.GetByAction<List<User>>(DomainApiAction.User.GetAssigneeInfo , parameter);
                    User user = null;
                    if (userInfo != null && userInfo.Count > 0)
                    {
                        user = userInfo[0];
                    }

                    if (task.IsCompleted && !task.IsDelegated)
                    {
                        CallImageCreator(imgArray[arrayCount] , "approved" , user);
                    }
                    else if (!task.IsCompleted && !task.IsDelegated)
                    {
                        CallImageCreator(imgArray[arrayCount] , "pending" , user);
                    }
                    else if (task.IsDelegated && task.IsCompleted)
                    {
                        CallImageCreator(imgArray[arrayCount] , "delegated" , user);
                    }
                    else if (task.AccessRequestId != null)
                    {
                        CallImageCreator(imgArray[arrayCount] , "revoked" , user);
                    }
                    arrayCount++;
                }

            }

        }

        private void CallImageCreator( Image imgControl , string imageName , User user )
        {

            imgControl = new ImageCreator<Image>().GetImage(imageName);
            imgControl.ToolTip = string.Format("Name: {0}  MobileNo:  {1}   EmailId:  {2} DepartmentName: {3} PositionName:   {4} SectionName:    {5}" ,
                                                                                     !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : "" , !string.IsNullOrEmpty(user.MobileNumber) ? user.MobileNumber : "" , !string.IsNullOrEmpty(user.EmailId) ? user.EmailId : "" , !string.IsNullOrEmpty(user.NOrganisation.Name) ? user.NOrganisation.Name : "" , !string.IsNullOrEmpty(user.NBusinessRole.Description) ? user.NBusinessRole.Description : "" , !string.IsNullOrEmpty(user.NOrganisation.Name) ? user.NOrganisation.Name : "");

            pnlAssignee.Controls.Add(imgControl);

        }

        protected void ddlAttestation_SelectedIndexChanged( object sender , EventArgs e )
        {
            DropDownList ddlAttestation = (DropDownList)AttestationTypeUC.FindControl("ddlAttestation");
            PopulateAssigneeDetails(ddlAttestation.SelectedItem.Text.Trim());
        }

        private void ShowNoData()
        {

            divChartControl.Visible = false;
            divNoData.Visible = true;
        }
        private void HideNoData()
        {

            divChartControl.Visible = true;
            divNoData.Visible = false;
        }

    }

    public class ImageCreator<TimageClass> :Image
        where TimageClass : Image
    {

        public Image GetImage( string imgName )
        {
            ImageCreator<TimageClass> imgObject = new ImageCreator<TimageClass>();
            if (!string.IsNullOrEmpty(imgName))
            {
                if (imgName.ToLower().Equals("approved"))
                {
                    imgObject.ImageUrl = Imageurl.APPROVED;
                }
                else if (imgName.ToLower().Equals("revoked"))
                {
                    imgObject.ImageUrl = Imageurl.REVOKED;
                }
                else if (imgName.ToLower().Equals("delegated"))
                {
                    imgObject.ImageUrl = Imageurl.DELEGATED;
                }
                else if (imgName.ToLower().Equals("pending"))
                {
                    imgObject.ImageUrl = Imageurl.PENDING;
                }

                imgObject.Height = new Unit("70px");
                imgObject.Height = new Unit("70px");

            }
            return imgObject;
        }

    }

    public class Imageurl
    {
        public static string APPROVED = "~/Images/Attestation/approved.png";
        public static string REVOKED = "~/Images/Attestation/revoked.png";
        public static string DELEGATED = "~/Images/Attestation/delegated.png";
        public static string PENDING = "~/Images/Attestation/pending.png";
        public static string LINE = "~/Images/Attestation/line.png";
        public static string HEADERIMAGE = "../Images/Attestation/task-assignee-info.jpg"; //for design time 
    }

}
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NReviewUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.UC.NReviewUC" %>


<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Admin/Attestation/UC/UserViewUC.ascx" TagPrefix="uc1" TagName="UserViewUC" %>
<%@ Register Src="~/Admin/Attestation/UC/RoleViewUC.ascx" TagPrefix="uc1" TagName="RoleViewUC" %>
<%@ Register Src="~/Admin/Attestation/UC/AttestationDetailUC.ascx" TagPrefix="uc1" TagName="AttestationDetailUC" %>        
<%@ Register Src="~/Admin/Attestation/UC/AttestationUploadUC.ascx" TagPrefix="uc1" TagName="AttestationUploadUC" %>
 

<div>
    <style type="text/css">
         
    </style>
   
    
    <asp:UpdatePanel ID="updSearch" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <uc1:UserViewUC runat="server" ID="UserViewUC" Visible="false"/>
            <uc1:RoleViewUC runat="server" ID="RoleViewUC" Visible="false" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <telerik:RadWindow runat="server" ID="AttestationDetails" Modal="true" Width="500px" Height="100%" Title="Attestation Detail"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title"><%= T("App.Attestation.AttestationDetail") %></h3>
                    </div>
                    <div class="modal-body">
                        <div class="row-fluid">
                            <uc1:AttestationDetailUC runat="server" ID="AttestationDetailUC" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" ID="btnOK" class="btn btn-primary bordered-button" Text="OK" OnClick="btnOK_Click" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="AttestationUpload" Modal="true" Width="800px" Height="100%"
        RenderMode="Lightweight" Behaviors="Close,Resize" VisibleOnPageLoad="false" CssClass="windowcss" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title">Attestation Batch Upload</h3>
                    </div>
                    <div class="modal-body">
                        <div class="row-fluid">
                            <uc1:AttestationUploadUC runat="server" ID="AttestationUploadUC" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" CssClass="btn btn-primary transparent-button" ID="Button1" Text="Cancel" UseSubmitBehavior="false" OnClick="Cancel_Comment_Upload_Click"></asp:Button>
                        <asp:Button runat="server" ID="Button2" CssClass="btn btn-primary bordered-button" Text="Save" OnClick="Save_Comment_Upload_Click"></asp:Button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="Confirm" Modal="true" Width="300px" Height="700"
        RenderMode="Lightweight" Behaviors="Close" VisibleOnPageLoad="false" CssClass="windowcss" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title confirmationHeader">Attestation Submit Confirmation</h3>
                    </div>
                    <div class="modal-body">
                        <div class="row-fluid">
                            <asp:Image runat="server" ID="imgCommit" ImageUrl="~/Images/action-buttons/submit.png" />
                            <asp:Label ID="lblConfirm" runat="server" Text="Are you sure you want to submit review changes ?" class="control-label"></asp:Label>
                        </div>
                    </div>
                    <div class="modal-footer confirmButtonDiv">
                        <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancelSubmit" Text="Cancel" UseSubmitBehavior="false" OnClick="Cancel_Submit_Click"></asp:Button>
                        <asp:Button runat="server" ID="btnConfirmSubmit" CssClass="btn btn-primary bordered-button" Text="Ok" OnClick="Confirm_Submit_Click" OnClientClick="showWaitDialog();"></asp:Button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

</div>
 <script>
        function OnClientValidationFailed(sender, args) {
            var fileExtention = args.get_fileName().substring(args.get_fileName().lastIndexOf('.') + 1, args.get_fileName().length);
            if (args.get_fileName().lastIndexOf('.') != -1) {//this checks if the extension is correct
                if (sender.get_allowedFileExtensions().indexOf(fileExtention) == -1) {
                    showAlert("Wrong Extension!", "Error");
                }
                else {
                    showAlert("Wrong file size!", "Error");
                }
            }
            else {
                showAlert("not correct extension!", "Error");
            }
            setTimeout(function () { deleteInvalid(sender) }, 250);//wait for the file to actually be added to the async uploads array before trying to delete it
        }

        function showWaitDialog(){

            $(".confirmButtonDiv").attr("disabled","disabled");
            $("#<%= lblConfirm.ClientID %>").text("Please wait while your changes being Saved.");
            $("#<%= imgCommit.ClientID %>").attr("src", "../../Images/action-buttons/save.png");
            $(".confirmationHeader").text("Saving Attestation Changes");
            $("#UpdateProgressBar").show(); 
        }

        function deleteInvalid(sender) {
            var invalidIndexes = [];
            $telerik.$(".ruUploadSuccess, .ruUploadFailure", sender.get_element()).each(function (index) {
                if ($telerik.$(this).hasClass('ruUploadFailure')) {
                    invalidIndexes.push(index);
                }
            });
            while (invalidIndexes.length) {
                sender.deleteFileInputAt(invalidIndexes.pop());
            }
        }

     $(document).ready(function () { 
            
            validateActionAllowed();
        });

        function validateActionAllowed() {
            try {
                var sessionUserId = <%=SessionUserId %>;
                var taskActionUserId = <%=TaskActionUserId %>;
                var isTaskCompleted = <%=IsTaskCompleted %>; 
                var actionAllowed = sessionUserId != taskActionUserId;
                var count = 0;
                count = <%=HasAttachments %>;
                
                if(!actionAllowed){actionAllowed = isTaskCompleted > 0;}

                window.parent.$("#Save").find("input")
                        .attr('disabled', actionAllowed);
                window.parent.$("#Submit").find("input")
                            .attr('disabled', actionAllowed);
                window.parent.$("#Revert").find("input")
                        .attr('disabled', actionAllowed);
                //window.parent.$("#Remark").find("input")
                //        .attr('disabled', actionAllowed);
                //set different icon if they can take any action on that button
                if(count > 0)
                {
                    window.parent.$("#Remark").find("input")
                            .attr('src', '/Images/action-buttons/remark_count.png');
                }
            }
            catch(err) {
                //alert(err.message);
            }
        }
    </script>
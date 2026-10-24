<%@ Page Title="Attestation Review" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="NReview.aspx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.NReview" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Admin/Attestation/UC/UserViewUC.ascx" TagPrefix="uc1" TagName="UserViewUC" %>
<%@ Register Src="~/Admin/Attestation/UC/RoleViewUC.ascx" TagPrefix="uc1" TagName="RoleViewUC" %>
<%@ Register Src="~/Admin/Attestation/UC/AttestationDetailUC.ascx" TagPrefix="uc1" TagName="AttestationDetailUC" %>
<%@ Register Src="~/Admin/Attestation/UC/AttestationUploadUC.ascx" TagPrefix="uc1" TagName="AttestationUploadUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <style type="text/css">
        /*.windowcss
{
  overflow:scroll !important;
}*/
        .rdAtt {
            margin-right: 20px;
        }

        .imgAttestationComparision {
            height: 100px;
            width: 349px;
            padding: 0px;
        }

        .rdAtt label {
            padding-right: 10px !important;
        }

        .my-flex {
            display: flex;
            justify-content: space-between;
            align-items: center;
            float: right;
        }

            .my-flex .input-container {
                flex-grow: 1;
            }

            .my-flex button {
                margin-left: 10px;
            }

        .text-muted {
            color: #ff0000 !important;
            padding-left: 10px;
        }

        .text-muted1 {
            color: #2077bd;
            font-size: 13px;
            font-weight: 700;
            padding: 4px 4px;
            line-height: 1.5;
        }

        .listing {
            background-color: #ffffff;
            padding: 13px;
            margin-bottom: 2px;
            box-shadow: 0px 4px 15px -3px rgba(0,0,0,0.1);
            display:flex;         
            border-radius: 5px;
        }

        .listing-image {
            width: 64px;
            height: 64px; /* Adjust width as needed */
            float: left; /* Image positioned to the left */
            margin-right: 15px; /* Spacing between image and text */
           box-shadow: 0px 4px 15px -3px rgba(0, 0, 0, 0.1);
            border-radius: 0px;
        }

        .listing-details {
            padding: 2px 0px 2px 0px;
        }

        .listing-title {
            font-size: 20px;
            margin-bottom: 10px;
        }

        .listing-description {
            font-size: 13px;
            line-height: 1.5; /* Adjust line spacing for readability */
        }
        
    </style>
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

        function showWaitDialog() {

            $(".confirmButtonDiv").attr("disabled", "disabled");
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

                if (!actionAllowed) { actionAllowed = isTaskCompleted > 0; }

                try {
                    window.parent.$("#Save").find("input")
                        .attr('disabled', actionAllowed);
                    window.parent.$("#Submit").find("input")
                        .attr('disabled', actionAllowed);
                } catch (err) { }
                window.parent.$("#Revert").find("input")
                    .attr('disabled', actionAllowed);

                //set different icon if they can take any action on that button
                if (count > 0) {
                    window.parent.$("#Remark").find("input")
                        .attr('src', '/Images/action-buttons/remark_count.png');
                }
            }
            catch (err) {
                //alert(err.message);
            }
        }
        $(window).bind('beforeunload', function () {
            try {
                $("#UpdateProgressBar").hide();
            } catch (e) {

            }
        });
        function redirectToComparisionPage() {
            var taskId = <%=TaskId %>;
            var pageUrlDomain = location.protocol + "//" + location.host;
            window.location.href = `${pageUrlDomain}/Admin/Attestation/AttestationUserComparision.aspx?SelectedtaskId=${taskId}`;
        }
    </script>

    <asp:UpdatePanel ID="updSearch" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div style="margin: 10px;" runat="server" id="divRetainRevoke" visible="false">
                <asp:RadioButton ID="AllRetain" runat="server" OnCheckedChanged="AllRetain_CheckedChanged" AutoPostBack="true" Text="Retain" GroupName="RetainRevoke" CssClass="rdAtt" />
                <asp:RadioButton ID="AllRevoke" runat="server" OnCheckedChanged="AllRevoke_CheckedChanged" AutoPostBack="true" Text="Revoke" GroupName="RetainRevoke" CssClass="rdAtt" />
            </div>
            <div class="" style="cursor:pointer !important;width:100%;padding-right:0px;" runat="server" id="divComparision" onclick="redirectToComparisionPage()">
                <div class="row" style="margin-right: 1px;  margin-left: 1px;">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="padding-left:0px;padding-right:0px;">
                    <div class="listing">
                        <img id="img" class="listing-image" src="../../images/Attestation/userprofile.png" ></img>
                        <div class="listing-details">
                            <p class="listing-description">
                                <asp:Label runat="server" ID="lblComparisionNotes"></asp:Label>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
            </div>
            <div class="container-fluid" id="divSapMessage" runat="server" style="padding-top:10px;" >
                <div class="row">
                    <div class="col-xs-8">
                        <div class="text-left">
                            <asp:Label ID="lblFlinsMessage" runat="server" Text="" Visible="false" Font-Bold="true" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                    <div class="col-xs-4">
                        <div class="row" runat="server" id="divTCodeSection" visible="false">
                            <div class="col-xs-7">
                                <asp:DropDownList runat="server" CssClass="form-control" ID="ddlApplicationType" AutoPostBack="true" OnSelectedIndexChanged="ddlApplicationType_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="col-xs-5">
                                <div class="text-right">
                                    <asp:Button runat="server" CssClass="btn btn-default btn-sm" ID="btnDownloadTCode" AutoPostBack="true" Text="Download TCodes" Visible="false" OnClick="btnDownloadTCode_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <uc1:UserViewUC runat="server" ID="UserViewUC" Visible="false" />
            <uc1:RoleViewUC runat="server" ID="RoleViewUC" Visible="false" />
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownloadTCode" />
        </Triggers>
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

    <telerik:RadWindow runat="server" ID="RevertConfirm" Modal="true" Width="300px" Height="700"
        RenderMode="Lightweight" Behaviors="Close" VisibleOnPageLoad="false" CssClass="windowcss" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanelRevert" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title confirmationHeader">Attestation Revert Confirmation</h3>
                    </div>
                    <div class="modal-body">
                        <div class="row-fluid">
                            <asp:Label ID="Label1" runat="server" Text="Are you sure want to revert your changes since last saved?" class="control-label"></asp:Label>
                        </div>
                    </div>
                    <div class="modal-footer confirmButtonDiv">
                        <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnRevertCancelSubmit" Text="Cancel" UseSubmitBehavior="false" OnClick="btnRevertCancelSubmit_Click"></asp:Button>
                        <asp:Button runat="server" ID="btnRevertConfirmSubmit" CssClass="btn btn-primary bordered-button" Text="Ok" OnClick="btnRevertConfirmSubmit_Click"></asp:Button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    <style>
        .button-box {
            width: 250px;
            height: 30px;
            /* border: 1px solid black; */
            margin: 10px;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .hyperlink-attestation {
            width: 200px;
            padding: 12px;
            font-size: 12px;
            font-weight: 700;
            cursor: pointer;
            border: none;
            text-align: center;
        }

            .hyperlink-attestation:focus {
                outline: none;
            }

        .four {
            background: #1bbbac;
            color: #fff !important;
            border-radius: 5px;
            box-shadow: 0 3px 5px #777777, inset 0 0 0px #777777;
            transition: transform 200ms;
        }

        .comparisionlink {
            text-decoration: underline;
            font-weight: bold;
        }
    </style>
</asp:Content>


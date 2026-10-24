<%@ Page Title="Password Reset" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="PasswordSync.aspx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.PasswordSync.PasswordSync" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EnterprisePasswordPolicy.ascx" TagPrefix="uc1" TagName="EnterprisePasswordPolicy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <style>
        .hiddenCol {
            display: none;
        }
        /*.RadGrid .rgSelectedRow {
            background-image: none !important;
            background-color: #ADD8E6 !important;
        }*/
        .lnheight {
            line-height: 20px;
        }

        .pwdmodal-footer {
            padding: 14px 15px 15px;
            margin-bottom: 0;
            text-align: right;
            background-color: #2077bd;
            border-top: 1px solid #ddd;
            -webkit-border-radius: 0 0 6px 6px;
            -moz-border-radius: 0 0 6px 6px;
            border-radius: 0 0 6px 6px;
            -webkit-box-shadow: inset 0 1px 0 #ffffff;
            -moz-box-shadow: inset 0 1px 0 #ffffff;
            box-shadow: inset 0 1px 0 #ffffff;
            *zoom: 1;
            }
            .pwdmodal-footer:before,
            .pwdmodal-footer:after {
            display: table;
            content: "";
            line-height: 0;
            }
            .pwdmodal-footer:after {
            clear: both;
            }
            .pwdmodal-footer .btn + .btn {
            margin-left: 5px;
            margin-bottom: 0;
            }
            .pwdmodal-footer .btn-group .btn + .btn {
            margin-left: -1px;
            }
            .pwdmodal-footer .btn-block + .btn-block {
            margin-left: 0;
            }

        
    </style>
    
    <telerik:RadWindowManager RenderMode="Lightweight" runat="server" ID="RadWindowManager1"></telerik:RadWindowManager>

    <asp:UpdatePanel ID="MainUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="content-wrapper">
                <div class="form">
                    <div class="form-horizontal left clearfix">

                        <div class="col-xs-6">
                            <span class="lnheight"><b>Instructions</b><br />
                                1) &nbsp;&nbsp;Select the Account Name of the Endpoint System<br />
                                2) &nbsp;&nbsp;Click “Reset Password” or "Unlock Account"<br /></span>
                        </div>
                    </div>
                </div>
            </div>

            <div id="PwdSync">
                <cc1:IACMRadGridControl ID="AccountDetail" runat="server" OnNeedDataSource="AccountDetail_NeedDataSource" PageSize="50" FilterMenu-CausesValidation="false"
                    OnItemDataBound="AccountDetail_ItemDataBound" OnPreRender="AccountDetail_PreRender">
                    <MasterTableView AllowAutomaticUpdates="true" DataKeyNames="AcountReconcilationId,ApplicationType,Reserved1,EndpointUserId,EndPointSystemType,EndpointSystemId,UserName,AccountType,AccountLocked,EndpointSystemName,IsAllowPasswordReset,IsAllowAccountUnlock" NoMasterRecordsText="No Records Available" TableLayout="Fixed" PageSize="50" Width="100%">
                        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="CheckTemp" AllowFiltering="false" ItemStyle-Width="3%" HeaderStyle-Width="3%" Visible="true">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ClientSelectColumn" CausesValidation="false"  runat="server" AutoPostBack="true" Visible='<%# Convert.ToBoolean(Eval("AllowReset")) %>'   OnCheckedChanged="ClientSelectColumn_CheckedChanged" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn FilterControlWidth="50%" SortExpression="UserName" HeaderText="Account Name" UniqueName="AccountName" DataField="UserName" ItemStyle-Width="10%" HeaderStyle-Width="10%" >
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn FilterControlWidth="77%" SortExpression="FullName" HeaderText="Display Name" HeaderButtonType="TextButton" DataField="FullName" ItemStyle-Width="20%" HeaderStyle-Width="20%" >
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn FilterControlWidth="50%" SortExpression="AccountType" HeaderText="Account Type" HeaderButtonType="TextButton" DataField="AccountType" ItemStyle-Width="7%" HeaderStyle-Width="7%" >
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn FilterControlWidth="69%" SortExpression="AccountLocked" HeaderText="Account Status" HeaderButtonType="TextButton" Display="false" DataField="AccountLocked" ItemStyle-Width="15%" HeaderStyle-Width="15%" >
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn FilterControlWidth="50%" UniqueName="EndpointSystemName" SortExpression="EndpointSystemName" HeaderText="Endpoint System" HeaderButtonType="TextButton" DataField="EndpointSystemName" ItemStyle-Width="15%" HeaderStyle-Width="15%" >
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn FilterControlWidth="69%" UniqueName="Description" SortExpression="Reserved3" HeaderText="Description" HeaderButtonType="TextButton" DataField="Reserved3" ItemStyle-Width="30%" HeaderStyle-Width="30%" >
                                  <ItemTemplate>
                                     <asp:Label ID="LabelReserved3" runat="server" Text='<%# Eval("Reserved3") %>' CssClass="description"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn FilterControlWidth="73%" SortExpression="AccountLockedstring" DataField="AccountLockedstring" UniqueName="TemplateIsUnlockColumn" HeaderText="Is Locked" HeaderStyle-Width="17%" Visible="false" Display="false">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("AccountLockedstring") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn FilterControlWidth="73%" UniqueName="TemplateDisplayPolicyColumn" Display="false" HeaderText="Password Policy" HeaderStyle-Width="17%">
                                <ItemTemplate>
                                    <asp:Button ID="DisplayPolicies" runat="server" CssClass="btn btn-default btn-sm" Text="View Policy" UseSubmitBehavior="false" OnClick="DisplayPolicies_Click" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn FilterControlWidth="45%" UniqueName="TemplateStatusColumn" HeaderText="Status" Display="false" HeaderStyle-Width="9%">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnMessage" ImageUrl="~/Images/action-buttons/detail.png" Enabled="true" AlternateText="Click to see the details" runat="server" CssClass="status-info"
                                        OnClientClick='<%# String.Format("javascript: return OpenInfo(\"{0}\"); return false;",Eval("Reserved2")) %>'></asp:ImageButton>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings EnablePostBackOnRowClick="true">
                        <Selecting AllowRowSelect="true" />
                        <ClientEvents OnRowSelecting="RowSelectingDisable" />
                    </ClientSettings>
                </cc1:IACMRadGridControl>

                <div class="buttons text-right nav-rectangle topBuffer">
                    <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
                    <asp:Button ID="btnUnlockAccount" runat="server" CssClass="btn btn-default btn-sm" Text="Unlock Account" Visible='<%# Convert.ToBoolean(Eval("IsAllowAccountUnlock")) && Convert.ToBoolean(Eval("AccountLocked")) %>' Enabled='<%# Convert.ToBoolean(Eval("AccountLocked")) %>' OnClick="btnUnlockAccount_Click" />

                    <asp:Button ID="btnSubmit"  CssClass="btn btn-default btn-sm" Text="Reset Password" runat="server" OnClick="btnSubmit_Click" TabIndex="4"></asp:Button>
                    <%--<asp:Button ID="btnRefresh" CssClass="btn btn-default btn-sm" Text="Refresh" runat="server" OnClick="btnRefresh_Click" TabIndex="4"></asp:Button>--%>
                </div>
            </div>
            
        </ContentTemplate>
      
    </asp:UpdatePanel>
    <telerik:RadWindowManager runat="server" ID="UserListWindow" EnableShadow="true">
                    <Windows>
                        <telerik:RadWindow ID="UserListDialog" runat="server" Title="Editing record" Height="320px" VisibleStatusbar="false" RenderMode="Lightweight" Behaviors="Close"
                            Width="510px" Left="150px" ReloadOnShow="true" ShowContentDuringLoad="false" Modal="true" EnableViewState="true" VisibleTitlebar="false">
                        </telerik:RadWindow>
                    </Windows>
                </telerik:RadWindowManager>

                <telerik:RadWindow ID="DisplayPolicy" runat="server" VisibleStatusbar="false" Height="550px" Width="500px" RenderMode="Lightweight" Behaviors="Close"
                    Modal="true" EnableViewState="true" VisibleTitlebar="false">
                    <ContentTemplate>
                        <uc1:EnterprisePasswordPolicy runat="server" ID="EnterprisePasswordPolicy1" />
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="ok" CssClass="btn btn-primary transparent-button" Text="OK" UseSubmitBehavior="false" OnClick="ok_Click" />
                        </div>

                    </ContentTemplate>
                </telerik:RadWindow>
                <asp:Button ID="btnOk" runat="server" Text="hide" OnClick="btnOk_Click" CssClass="hiddenCol" />
    <telerik:RadWindow runat="server" ID="Confirm" Modal="true" Width="300px" Height="700"
                RenderMode="Lightweight" Behaviors="Close" VisibleOnPageLoad="false" CssClass="windowcss" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
                <ContentTemplate>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <div class="modal-header">
                                <h3 class="modal-title">Password Reset Notification</h3>
                            </div>
                            <div class="modal-body">
                                <div class="row-fluid">

                                    <asp:Label ID="lblConfirm" runat="server" class="control-label"></asp:Label>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancelSubmit" Text="OK"
                                    UseSubmitBehavior="false" OnClick="Cancel_Submit_Click"></asp:Button>

                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>

            <telerik:RadWindow runat="server" ID="RadConfirmWindow" Modal="true" Width="300px" Height="700"
                RenderMode="Lightweight" Behaviors="Close" VisibleOnPageLoad="false" CssClass="windowcss" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
                <ContentTemplate>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                             <asp:Panel runat="server" ID="pnlRadConfirmWindow" DefaultButton ="btnConfirmSubmit">
                            <div class="modal-header">
                                <h3 class="modal-title confirmationHeader">Password Reset Confirmation</h3>
                            </div>
                            <div class="modal-body">
                                <div class="row-fluid">
                                    <asp:Image runat="server" ID="imgCommit" ImageUrl="~/Images/action-buttons/submit.png" />
                                    <asp:Label ID="Label2" runat="server" Text="Are you sure want to continue ?" class="control-label"></asp:Label>
                                </div>
                            </div>
                            <div class="modal-footer confirmButtonDiv">
                                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="CancelButton" Text="Cancel" UseSubmitBehavior="false" OnClick="CancelButton_Click"></asp:Button>
                                <asp:Button runat="server" ID="btnConfirmSubmit" CssClass="btn btn-primary bordered-button" Text="Ok" OnClick="btnConfirmSubmit_Click"></asp:Button>
                            </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>

            <telerik:RadWindow runat="server" ID="RadUnlockWindow" Modal="true" Width="300px" Height="700"
                RenderMode="Lightweight" Behaviors="Close" VisibleOnPageLoad="false" CssClass="windowcss" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
                <ContentTemplate>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                             <asp:Panel runat="server" ID="pnlUnlockWindow" DefaultButton ="btnUnlockSubmit">
                            <div class="modal-header">
                                <h3 class="modal-title confirmationHeader">Account Unlock Confirmation</h3>
                            </div>
                            <div class="modal-body">
                                <div class="row-fluid">
                                    <asp:Image runat="server" ID="Image1" ImageUrl="~/Images/action-buttons/submit.png" />
                                    <asp:Label ID="Label3" runat="server" Text="Are you sure you want to continue ?" class="control-label"></asp:Label>
                                </div>
                            </div>
                            <div class="modal-footer confirmButtonDiv">
                                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="CancelUnlockButton" Text="Cancel" UseSubmitBehavior="false" OnClick="CancelUnlockButton_Click"></asp:Button>
                                <asp:Button runat="server" ID="btnUnlockSubmit" CssClass="btn btn-primary bordered-button" Text="Ok" OnClick="btnUnlockSubmit_Click"></asp:Button>
                            </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   
                </ContentTemplate>
            </telerik:RadWindow>

            <telerik:RadWindow runat="server" ID="RadResetPwdWindow" Modal="true" Width="1000px" Height="1200px" OnClientShow="OnClientClose"
                RenderMode="Lightweight"  Behaviors="Close" VisibleOnPageLoad="false" CssClass="windowcss" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
                <ContentTemplate>
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="pnlResetWindow" DefaultButton ="btnResetConfirm">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h3 class="modal-title" id="H1">Reset Password
                                    </h3>
                                </div>
                                <div class="modal-body">
                                    <div class="col-xs-7">
                                                <div class="row">
                                                    <div >
                                                        <label class="col-xs-5 control-label">Account Name </label>
                                                        <div class="col-xs-7">
                                                            <asp:Label ID="lblAccountName" runat="server" Style="font-weight: normal;"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div >
                                                        <label class="col-xs-5 control-label">Account Type</label>
                                                        <div class="col-xs-7">
                                                            <asp:Label ID="lblAccountType" runat="server" Style="font-weight: normal;"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div >
                                                        <label class="col-xs-5 control-label">EndPoint System Name</label>
                                                        <div class="col-xs-7">
                                                            <asp:Label ID="lblendpointname" runat="server" Style="font-weight: normal;"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-xs-12">&nbsp; </div>
                                                </div>
                                                <div class="row">
                                                    <div class="form-group required">
                                                        <asp:Label ID="lblNewPassword" AssociatedControlID="txtNewResetPassword" autocomplete="off" runat="server" Text="New Password" CssClass="col-xs-5 control-label" Style="font-weight: bold;"></asp:Label>
                                                        <div class="col-xs-7">
                                                            <asp:TextBox ID="txtNewResetPassword" AutoCompleteType="None" TextMode="Password" CssClass="form-control" required="required" data-required-msg="New login password must be required" runat="server" onblur="javascript:ValidateInput()" onkeypress="javascript:ValidateInput()" TabIndex="2" ValidationGroup="Val" autocomplete="off"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="Requiredfieldvalidator1" ErrorMessage="New Password is Required!"
                                                                ControlToValidate="txtNewResetPassword" runat="server" ValidationGroup="Val" ForeColor="Red" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-xs-12">&nbsp; </div>
                                                </div>
                                                <div class="row">
                                                    <div class="form-group required">
                                                        <asp:Label ID="lblConfirmPassword"  AssociatedControlID="txtConfirmPassword" autocomplete="off" runat="server" CssClass="col-xs-5 control-label" Text="Confirm Password" Style="font-weight: bold;"></asp:Label>
                                                        <div class="col-xs-7">
                                                            <asp:TextBox ID="txtConfirmPassword" Text="" AutoCompleteType="None" TextMode="Password" class="form-control" required="required" data-required-msg="Confirm password login password must be required" runat="server" onblur="javascript:ValidateInput()" onkeypress="javascript:ValidateInput()" TabIndex="3" ValidationGroup="Val" autocomplete="off"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="Requiredfieldvalidator2" ErrorMessage="Confirm Password is Required!"
                                                                ControlToValidate="txtConfirmPassword" runat="server" ValidationGroup="Val" ForeColor="Red" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="form-group">
                                                        <div class="col-xs-5"></div>
                                                        <div class="col-xs-7">
                                                            <asp:CompareValidator ID="cvPasswordCheck" runat="server" ErrorMessage="Password did not match" ValidationGroup="Val" ControlToValidate="txtConfirmPassword" ControlToCompare="txtNewResetPassword" ForeColor="Red" />
                                                        </div>
                                                    </div>
                                                </div>
                                       
                                    </div>
                                    <div class="col-xs-5">
                                        <div class="row">
                                            <div class="col-xs-12">
                                                <div class="panel">
                                                    <div class="panel-header" style="padding: 3px">
                                                        <h3 class="modal-title" runat="server">Password Policies</h3>
                                                    </div>
                                                    <div class="panel">
                                                        <asp:Repeater ID="Repeater1" runat="server">
                                                            <ItemTemplate>
                                                                <div class="row" style="padding-bottom:4px">
                                                                    <label class="col-xs-10 " style="font-weight: normal; font-size: 8pt;">
                                                                        <%#DataBinder.Eval(Container.DataItem, "name") %>
                                                                    </label>
                                                                    <div class="col-xs-2" style="word-wrap: break-word">
                                                                        <label id="PolicyName" runat="server" style="font-weight: normal; font-size: 8pt;">
                                                                            <%#DataBinder.Eval(Container.DataItem, "value") %>
                                                                        </label>
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="content-wrapper">
                                        <div class="form clearfix">
                                            <div class="form-horizontal right">
                                                
                                            </div>
                                        </div>
                                    </div>
                                     <div class="col-xs-12">
                                         <asp:Label runat="server" ID="lblPasswordPolicyMsg" Text="" ForeColor="Red" ></asp:Label>
                                        </div>
                                    <div class="content-wrapper">
                                        <div class="form clearfix">
                                            <div class="form-horizontal right">
                                                
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="pwdmodal-footer">
                                    <asp:Button runat="server" CausesValidation="false" class="btn btn-primary transparent-button" ID="btnResetCancel" Text="Cancel" AutoPostBack="false" OnClientClick="closeCommentWindow()" OnClick="btnResetCancel_Click"></asp:Button>
                                    <asp:Button runat="server" ID="btnResetConfirm" CssClass="btn btn-primary bordered-button" Text="Reset" onfocus="javascript:ValidateInput()" ValidationGroup="Val" OnClick="btnResetConfirm_Click"></asp:Button>
                                </div>
                            </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>
    <script type="text/javascript">
        function OnClientClose() {
        var TextBox1 = document.getElementById('ctl00_cplMain_RadResetPwdWindow_C_txtConfirmPassword');
            var TextBox2 = document.getElementById('ctl00_cplMain_RadResetPwdWindow_C_txtNewResetPassword');
            TextBox1.value="";
            TextBox2.value = "";
            
        }
        function openWinContentTemplate() {
            $find("<%=DisplayPolicy.ClientID %>").show();
        }

        function OpenInfo(message) {
            messageDialog(message);
            return false;
        }

        function RowSelected(sender, args) {
            Close();
        }

        function RowSelectingDisable(sender, args) {
            args.set_cancel(true);
        } 
        function ValidateInput() {

            var validated = Page_ClientValidate('Val');
            $("#UpdateProgressBar").hide();
            setTimeout(function(){$(".disableDIv").hide();},1000);
            if (!validated) {
                $(document.getElementById('<%=btnResetConfirm.ClientID  %>')).attr("disabled", true);
                return false;
                 
            }
            else {

                $(document.getElementById('<%=btnResetConfirm.ClientID  %>')).attr("disabled", false);
                    return true;
            }
        }

        function closeCommentWindow()
        {
            try {
                setTimeout(function () {$("#UpdateProgressBar").hide();}, 10);
                var oWnd = document.getElementById("<%= RadResetPwdWindow.ClientID %>");
                oWnd.control.close();
            } catch (e) {
            console.log(e);
            }
        }
    </script>
</asp:Content>

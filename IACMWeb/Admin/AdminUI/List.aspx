<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Admin.AdminUI.List" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">

    <script type="text/javascript">
        function SetToday() {
            var datepicker = $find("<%=DatePickerFromDate.ClientID%>");
            var dt = new Date();
            datepicker.set_selectedDate(dt);
            datepicker.hidePopup();
        }
    </script>
    <style>
        footer {
            position: fixed;
            overflow: hidden;
            bottom: 10;
            z-index: 9999;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <div class="content-wrapper">
                <table>
                    <tr>
                        <td>

                            <div class="form">
                                <div class="form-horizontal left clearfix">
                                    <div class="row-fluid">
                                        <div class="col-xs-3">
                                            <asp:Button ID="btnOpenDataViewer" CssClass="btn btn-default btn-sm" runat="server" Text="Open DataViewer" OnClick="btnOpenDataViewer_Click" />&nbsp;
                                        </div>
                                        <div class="col-xs-3">
                                            &nbsp;<asp:Button ID="btnAdminConsole" CssClass="btn btn-default btn-sm" runat="server" Text="Admin Console" OnClick="btnAdminConsole_Click" />&nbsp;
                                        </div>
                                        <div class="col-xs-6">
                                            <asp:Button ID="btnMassPropagateO365" CssClass="btn btn-default btn-sm " runat="server" Text="Mass Propagate O365 Licenses and Subscriptions" OnClick="btnMassPropagateO365_Click" />
                                            &nbsp;<asp:Button ID="btnHRMS_Details" CssClass="btn btn-default btn-sm" runat="server" Text="HRMS Details" OnClick="btnHRMS_Details_Click" />
                                            &nbsp;<asp:Button runat="server" Text="Deactivate Expired Delegation" CssClass="btn btn-default btn-sm" ID="btnDeactivateExpiredDelegation" OnClick="btnDeactivateExpiredDelegation_Click" />
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-12">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-12">
                                            <h4>Download users from HRMS</h4>
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-3">
                                            <span class="control-label">Start date for back scan :</span>
                                        </div>
                                        <div class="col-xs-5">
                                            <telerik:RadDatePicker runat="server" ID="DatePickerFromDate" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                                DatePopupButton-Visible="true" CssClass="form-control" AutoPostBack="false">
                                                <Calendar ID="Calendar1" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                                    <FooterTemplate>
                                                        <div style="text-align: center; background-color: Gray;">
                                                            <asp:Button ID="EButton" runat="server" Text="Today" OnClientClick="SetToday()" AutoPostBack="false" />
                                                        </div>
                                                    </FooterTemplate>
                                                </Calendar>
                                                <DateInput ID="DateInput1" ReadOnly="false" runat="server" ToolTip="Date input For From Date"
                                                    BorderStyle="NotSet" Label="" AutoPostBack="false">
                                                </DateInput>
                                            </telerik:RadDatePicker>
                                        </div>
                                        <div class="col-xs-4">
                                            <asp:Button ID="btnDownloadAllFromHRMS" Style="margin-left: 5px" CssClass="btn btn-default btn-sm" runat="server" Text="Download All User" OnClick="btnDownloadAllFromHRMS_Click" />
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-12">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-12">
                                            <h4>Download specific user from HRMS</h4>
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-3">
                                            <span class="control-label">Enter Pernr :</span>
                                        </div>
                                        <div class="col-xs-5">
                                            <asp:TextBox runat="server" ID="txtPernrDownload" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-xs-4">
                                            <asp:Button ID="btnDownloadUserFromHRMS" Style="margin-left: 5px" CssClass="btn btn-default btn-sm" runat="server" Text="Download User" OnClick="btnDownloadUserFromHRMS_Click" />
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-12">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-12">
                                            <h4>Discover specific user from HRMS</h4>
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-3">
                                            <span class="control-label">Enter Pernr :</span>
                                        </div>
                                        <div class="col-xs-5">
                                            <asp:TextBox runat="server" ID="txtPernrDiscover" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-xs-4">
                                            <asp:Button ID="btnDiscoverUserFromHRMS" Style="margin-left: 5px" CssClass="btn btn-default btn-sm" runat="server" Text="Discover User" OnClick="btnDiscoverUserFromHRMS_Click" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row-fluid">
                                    <div class="col-xs-12">
                                        <hr />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-xs-6">
                                        <asp:Panel ID="Panel2" runat="server">
                                            <div class="content-wrapper">
                                                <div class="form">
                                                    <div class="form-horizontal">
                                                        <div class="row-fluid">
                                                            <div class="col-xs-12">
                                                                <h3>
                                                                    <label class="label label-danger">Task WithDraw/Reject (Request Withdraw/Rejected but task is Pending)</label></h3>
                                                            </div>
                                                        </div>
                                                        <div class="row-fluid">&nbsp;</div>
                                                        <div class="form-group">
                                                            <div class="col-xs-2">TaskId </div>
                                                            <div class="col-xs-4">
                                                                <asp:TextBox ID="txtTaskId" runat="server" ToolTip="TaskId" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <div class="col-xs-2">Comments </div>
                                                            <div class="col-xs-4">
                                                                <asp:TextBox ID="txtComments" TextMode="MultiLine" runat="server" ToolTip="Comments" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="row-fluid">
                                                            <div class="form-group">
                                                                <div class="col-xs-2">&nbsp; </div>
                                                                <div class="col-xs-9">
                                                                    <asp:Button runat="server" ID="btnTaskWithDraw" OnClick="btnTaskWithDraw_Click" Text="WithDraw Task" CssClass="btn btn-default btn-sm" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                    <div class="col-xs-6">
                                        <asp:Panel ID="Panel3" runat="server">
                                            <div class="content-wrapper">
                                                <div class="form">
                                                    <div class="form-horizontal">
                                                        <div class="row-fluid">
                                                            <div class="col-xs-12">
                                                                <h3>
                                                                    <label class="label label-danger">Resubmit Request (If Task is Hang during approval).</label></h3>
                                                            </div>
                                                        </div>
                                                        <div class="row-fluid">&nbsp;</div>
                                                        <div class="form-group">
                                                            <div class="col-xs-2 required">Reques Type </div>
                                                            <div class="col-xs-4">
                                                                <asp:DropDownList ID="ddlRequestType" AutoPostBack="true" runat="server" CssClass="form-control">
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <div class="col-xs-2">RequesId </div>
                                                            <div class="col-xs-4">
                                                                <asp:TextBox ID="txtRequestId" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                            <div class="col-xs-6">&nbsp;</div>
                                                        </div>
                                                        <div class="form-group required">
                                                            <div class="col-xs-2">&nbsp; </div>
                                                            <div class="col-xs-3">
                                                                <asp:Button runat="server" ID="btnRequestResubmit" OnClick="btnRequestResubmit_Click" Text="Reinitiate Request" CssClass="btn btn-default btn-sm" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>

                                    </div>
                                </div>

                                <div class="form-horizontal">
                                    <div class="row-fluid">
                                        <div class="col-xs-12">
                                            <hr />
                                        </div>
                                    </div>
                                    <div class="row-fluid">&nbsp;</div>
                                    <div class="row-fluid">&nbsp;</div>
                                    <div class="row-fluid">
                                        <div class="col-xs-3">Enter User: </div>
                                        <div class="col-xs-6">
                                            <uc1:UserSearchUC runat="server" ID="UserSearchUC" OnUserChanged="UserSearchUC_UserChanged" ConsiderPriorOnBoardDays="false" AllowSingleUser="true" />
                                        </div>
                                         <div class="col-xs-3">&nbsp;</div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-12">&nbsp;</div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-3">
                                            Current Token:
                            <asp:Label runat="server" ID="lblExistingToken" Text="" ForeColor="Green" Font-Bold="true"></asp:Label>
                                        </div>
                                        <div class="col-xs-6">
                                            <asp:TextBox runat="server" ID="txtOneKeyToken" CssClass="form-control" MaxLength="24"></asp:TextBox>&nbsp;

                                        </div>
                                        <div class="col-xs-3">
                                            <asp:Button ID="btnUpdateOneKeyToken" Style="margin-left: 5px" CssClass=" btn btn-default btn-sm" runat="server" Text="Update OneKey" OnClick="btnUpdateOneKeyToken_Click" />

                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-12">&nbsp;</div>
                                    </div>
                                    <div class="row-fluid">&nbsp;</div>
                                    <div class="row-fluid">
                                        <div class="col-xs-3">
                                            <asp:Button ID="btnUpdateTelephone" CssClass="btn btn-default btn-sm" runat="server" Text="Initiate HR Update" OnClick="btnUpdateTelephone_Click" ToolTip="Initiate HR update for TelephoneNumber,Email, FLINSLoginId" />&nbsp;
                                        </div>
                                        <div class="col-xs-3">
                                            &nbsp;<asp:Button ID="btnADPropagation" CssClass="btn btn-default btn-sm" runat="server" Text="AD Propagation" OnClick="btnADPropagation_Click" />
                                        </div>
                                        <div class="col-xs-6">
                                            <b>Note:Select the User for specific user AD Propagation.</b>
                                        </div>
                                    </div>

                                    <div class="row-fluid">
                                        <div class="col-xs-12">
                                            <asp:Panel ID="Panel4" runat="server">
                                                <div class="content-wrapper">
                                                    <div class="form">
                                                        <div class="form-horizontal">
                                                            <div class="row-fluid">
                                                                <div class="col-xs-3"></div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>


                                    <asp:Panel ID="Panel1" runat="server">
                                        <div class="content-wrapper">
                                            <div class="form">
                                                <div class="form-horizontal">
                                                    <div class="row-fluid">

                                                        <div class="row-fluid">&nbsp;</div>
                                                        <div class="row-fluid">&nbsp;</div>
                                                        <div class="col-xs-2">Application </div>
                                                        <div class="col-xs-2">

                                                            <telerik:RadAutoCompleteBox ID="EndpointAutoCompleteBox" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
                                                                InputType="Token">
                                                                <TextSettings SelectionMode="Single" />
                                                                <WebServiceSettings Method="GetApplicableApplications" Path="../../Services/AutoCompleteDataService.svc" />
                                                            </telerik:RadAutoCompleteBox>
                                                        </div>
                                                    </div>


                                                    <div class="col-xs-2">
                                                        <asp:Button ID="Decommission" runat="server" Text="Decommission"
                                                            OnClick="Decommission_Click" CssClass="btn btn-default btn-sm" />
                                                    </div>
                                                </div>
                                            </div>
                                    </asp:Panel>

                                    <div class="row-fluid">
                                        <div class="col-xs-12">
                                            <hr />
                                        </div>
                                    </div>

                                    <div class="row-fluid">
                                        <div class="col-xs-12">&nbsp;</div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="col-xs-12">&nbsp;</div>
                                    </div>


                                    <div class="row-fluid">
                                        <div class="col-xs-12">&nbsp;</div>
                                    </div>
                                   <asp:Panel runat="server" ID="pnlServiceAccount">
                                        <div class="row-fluid">
                                            <div class="col-xs-12">
                                                <b>Service Account</b>
                                            </div>
                                        </div>

                                        <div class="row-fluid">
                                            <div class="col-xs-10"></div>
                                            <div class="col-xs-2" style="padding-bottom: 15px;">
                                                <asp:Button runat="server" ID="btnNewRecord" Text="New Record" CssClass="btn btn-default btn-sm" OnClick="btnNewRecord_Click" />&nbsp;&nbsp;
                                <asp:Button runat="server" ID="btnEditRecord" Text="Edit" CssClass="btn btn-default btn-sm" OnClick="btnEditRecord_Click" Visible="false" />
                                            </div>
                                        </div>
                                        <div class="row-fluid">
                                            <div class="col-xs-12">
                                                <cc1:IACMRadGridControl ID="serviceAccountQuestion" runat="server" AllowCustomPaging="false" OnNeedDataSource="serviceAccountQuestion_NeedDataSource" OnItemCreated="serviceAccountQuestion_ItemCreated"
                                                    OnInsertCommand="serviceAccountQuestion_InsertCommand" OnUpdateCommand="serviceAccountQuestion_UpdateCommand">
                                                    <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" ShowHeadersWhenNoRecords="true" Width="100%"
                                                        DataKeyNames="Id" TableLayout="fixed" NoMasterRecordsText="No Records Available">
                                                        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                                        <Columns>
                                                            <telerik:GridEditCommandColumn UniqueName="EditCommandColumn" HeaderText="Edit" HeaderStyle-Width="20%">
                                                            </telerik:GridEditCommandColumn>
                                                            <telerik:GridBoundColumn DataField="Id" HeaderText="ID" SortExpression="Id" UniqueName="Id" HeaderStyle-Width="100%" FilterControlWidth="100%">
                                                            </telerik:GridBoundColumn>
                                                            <telerik:GridBoundColumn DataField="Description" HeaderText="Description" SortExpression="Description" UniqueName="Description" HeaderStyle-Width="100%" FilterControlWidth="100%">
                                                            </telerik:GridBoundColumn>
                                                            <telerik:GridBoundColumn DataField="IsActive" HeaderText="IsActive" SortExpression="IsActive" UniqueName="IsActive" HeaderStyle-Width="100%" FilterControlWidth="100%">
                                                            </telerik:GridBoundColumn>
                                                        </Columns>
                                                    </MasterTableView>
                                                </cc1:IACMRadGridControl>
                                            </div>
                                        </div>
                                        <div class="row-fluid">&nbsp;</div>
                                    </asp:Panel>
                                </div>

                            </div>

                        </td>
                    </tr>
                </table>

                <telerik:RadWindow runat="server" ID="Decommision" Modal="true" Height="100%" Width="500px" Title="Decommision"
                    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" ReloadOnShow="true" ShowContentDuringLoad="false" VisibleTitlebar="false" VisibleStatusbar="false">
                    <ContentTemplate>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>

                                <div class="modal-body">
                                    <div class="form-horizontal left clearfix">
                                        <div class="row-fluid">
                                            <div class="col-xs-12">
                                                <div class="form-group requried">
                                                    <asp:Label ID="Label2" AssociatedControlID="txtComment" runat="server" Text="Comment" class="col-xs-3 control-label required"></asp:Label>
                                                    <div class="col-xs-9">
                                                        <asp:TextBox ID="txtComment" TextMode="MultiLine" class="form-control" ValidationGroup="submit" Rows="3" runat="server" CausesValidation="true"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                </div>
            <div class="modal-footer">
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="CancelradWindow" Text="Cancel" OnClick="CancelradWindow_Click" CausesValidation="false"></asp:Button>
                <asp:Button ID="btnSubmit" CssClass="add btn btn-primary bordered-button" runat="server" CausesValidation="true" UseSubmitBehavior="false"
                    Text="Ok" OnClick="btnSubmit_Click"></asp:Button>
            </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </telerik:RadWindow>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


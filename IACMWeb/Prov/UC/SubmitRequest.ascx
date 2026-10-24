<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubmitRequest.ascx.cs" Inherits="DI.Chakra.Web.Prov.UC.SubmitRequest" %>




<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxCtrl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>


<script language="javascript" type="text/javascript">
    function TrimStr(StrValue) {
        return StrValue.replace(/(?:(?:^|\n)\s+|\s+(?:$|\n))/g, '').replace(/\s+/g, ' ');
    }

    function selectAll(invoker) {
        var inputElements = document.getElementsByTagName('input');
        for (var i = 0; i < inputElements.length; i++) {

            var myElement = inputElements[i];

            if (myElement.type === "checkbox") {

                myElement.checked = invoker.checked;
            }
        }
    }


</script>
<style>
    input[type=checkbox] + .lbl, input[type=checkbox] + .rtIn, input[type=radio] + .lbl, input[type=checkbox] + label {
        padding-right: 1px !important;
    }
</style>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">

    <ContentTemplate>

        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">

                    <br />

                    <div class="col-xs-12">
                        <div class="form-group">
                            <div class="col-xs-3">
                                <asp:RadioButton ID="ExecutingRadioOption" runat="server" Text="Executing Access Requests" AutoPostBack="true" GroupName="DepChange" Checked="true" OnCheckedChanged="FailedSelectionChanged" />
                            </div>
                            <div class="col-xs-3">
                                <asp:RadioButton ID="ValidationFailedRadioButton" runat="server" AutoPostBack="true" GroupName="DepChange" OnCheckedChanged="FailedSelectionChanged" Visible="false" />
                            </div>
                            <div class="col-xs-3">
                                <asp:RadioButton ID="ExecutionFailedRadioButton" Text="Execution Failed Requests" runat="server" AutoPostBack="true" GroupName="DepChange" OnCheckedChanged="FailedSelectionChanged" />
                            </div>
                            <div class="col-xs-3">
                                <asp:RadioButton ID="WorkflowNotFoundRadioButton" Text="Workflow Not Found Requests" runat="server" AutoPostBack="true" GroupName="DepChange" OnCheckedChanged="FailedSelectionChanged" />
                            </div>
                        </div>
                    </div>

                    <br />

                </div>
            </div>

            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <asp:Panel ID="Panel4" runat="server" Height="90%" ScrollBars="Auto" Width="100%">

                        <cc1:IACMRadGridControl ID="grdNewUsers" runat="server" OnNeedDataSource="grdNewUsers_NeedDataSource"
                            OnItemCommand="grdNewUsers_ItemCommand"
                            AllowFilteringByColumn="true" AllowPaging="true" AllowSorting="true" CellSpacing="0" GridLines="None" AllowAutomaticUpdates="false"
                            EnableAjaxSkinRendering="true">
                            <ExportSettings
                                HideStructureColumns="true"
                                IgnorePaging="true" ExportOnlyData="true"
                                Excel-Format="ExcelML"
                                OpenInNewWindow="true">
                                <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="297mm"
                                    DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                                    BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
                                </Pdf>
                            </ExportSettings>
                            <ClientSettings EnableRowHoverStyle="True">
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"></Scrolling>
                            </ClientSettings>
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" Width="150%"
                                EnableHeaderContextMenu="true" DataKeyNames="AccessRequestId,RequestSource,AdHocAccessRequestId">
                                <CommandItemSettings ShowAddNewRecordButton="false"
                                    ShowRefreshButton="true" ShowExportToExcelButton="true"
                                    ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="5%" HeaderText=""
                                        DataField="" FilterControlAltText="" SortExpression=""
                                        UniqueName="" AllowFiltering="false">
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkheader" runat="server" OnClick="selectAll(this)" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkUser" runat="server" />
                                            <asp:HiddenField ID="hdnUserid" runat="server" Value='<%#Eval("AccessRequestId") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="AccessRequestId"
                                        FilterControlAltText="AccessRequestId" HeaderText="Request Id"
                                        SortExpression="AccessRequestId" UniqueName="AccessRequestId" HeaderStyle-Width="10%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="AccessRequestSourceText" HeaderText="Request Source" SortExpression="AccessRequestSourceText" UniqueName="RequestSource" DataType="System.String"
                                        FilterControlAltText="Filter RequestSource column" HeaderStyle-Width="20%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="RequestSourceType" FilterControlAltText="RequestSourceType" SortExpression="RequestSourceType" HeaderText="Request Source" UniqueName="RequestSourceType">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="SystemName"
                                        FilterControlAltText="SystemName" HeaderText="System Name"
                                        SortExpression="SystemName" UniqueName="SystemName" HeaderStyle-Width="20%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="DisplayName"
                                        FilterControlAltText="DisplayName" HeaderText="Display Name"
                                        SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="20%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="EmployeeID"
                                        FilterControlAltText="EmployeeID" HeaderText="Employee ID"
                                        SortExpression="EmployeeID" UniqueName="EmployeeID" HeaderStyle-Width="20%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Company"
                                        FilterControlAltText="Company" HeaderText="Company"
                                        SortExpression="Company" UniqueName="Company" HeaderStyle-Width="20%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="CostCenter"
                                        FilterControlAltText="CostCenter" HeaderText="Cost Center"
                                        SortExpression="CostCenter" UniqueName="CostCenter" HeaderStyle-Width="20%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Department"
                                        FilterControlAltText="Department" HeaderText="Department"
                                        SortExpression="Department" UniqueName="Department" HeaderStyle-Width="20%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <%--<telerik:GridBoundColumn DataField="Division"
                                        FilterControlAltText="Division" HeaderText="Division"
                                        SortExpression="Division" UniqueName="Division" HeaderStyle-Width="20%"  FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>--%>
                                    <telerik:GridBoundColumn DataField="Section"
                                        FilterControlAltText="Section" HeaderText="Section"
                                        SortExpression="Section" UniqueName="Section" HeaderStyle-Width="20%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="RequestDate" FilterControlAltText="RequestDate" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}"
                                        HeaderText="Request Date" SortExpression="RequestDate" UniqueName="RequestDate" HeaderStyle-Width="20%" FilterControlWidth="80%">
                                    </telerik:GridDateTimeColumn>

                                </Columns>
                            </MasterTableView>
                        </cc1:IACMRadGridControl>

                        <cc1:IACMRadGridControl ID="grdRequest" runat="server" OnNeedDataSource="grdRequest_NeedDataSource"
                            OnItemCommand="grdNewUsers_ItemCommand" OnSelectedIndexChanged="grdRequest_SelectedIndexChanged"
                            AllowFilteringByColumn="true" AllowPaging="true" AllowSorting="true" CellSpacing="0" GridLines="None" AllowAutomaticUpdates="false"
                            EnableAjaxSkinRendering="true">
                            <ExportSettings
                                HideStructureColumns="true"
                                IgnorePaging="true" ExportOnlyData="true"
                                Excel-Format="ExcelML"
                                OpenInNewWindow="true">
                                <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="297mm"
                                    DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                                    BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
                                </Pdf>
                            </ExportSettings>
                            <ClientSettings EnableRowHoverStyle="True">
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"></Scrolling>
                            </ClientSettings>
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" Width="150%"
                                EnableHeaderContextMenu="true" DataKeyNames="AccessRequestID,AdHocAccessRequestId,RequestSource">
                                <CommandItemSettings ShowAddNewRecordButton="false"
                                    ShowRefreshButton="true" ShowExportToExcelButton="true"
                                    ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="5%" HeaderText=""
                                        DataField="" FilterControlAltText="" SortExpression=""
                                        UniqueName="" AllowFiltering="false">
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkheader" runat="server" OnClick="selectAll(this)" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkUser" runat="server" />
                                            <asp:HiddenField ID="hdnUserid" runat="server" Value='<%#Eval("AccessRequestId") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <%-- <telerik:GridBoundColumn DataField="AccessRequestID" FilterControlAltText="AccessRequestID" SortExpression="AccessRequestID" HeaderText="Request ID" HeaderStyle-Width="10%" UniqueName="AccessRequestID" >
                                    </telerik:GridBoundColumn>--%>
                                    <telerik:GridTemplateColumn HeaderText="Request ID" DataField="AccessRequestID" FilterControlAltText="AccessRequestID" SortExpression="AccessRequestID" UniqueName="AccessRequestID" HeaderStyle-Width="80px" FilterControlWidth="75%">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkTaskId" runat="server" Text='<%#Eval("AccessRequestID") %>' CommandArgument='<%#Container.ItemIndex%>' OnCommand="lnkAdhocTaskId_Command" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Name" SortExpression="Name" HeaderText="Application Name" UniqueName="Name" HeaderStyle-Width="25%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <%-- <telerik:GridBoundColumn DataField="AdHocAccessRequestId" FilterControlAltText="AdHocAccessRequestId" SortExpression="AdHocAccessRequestId" HeaderText="AdHoc Request Id" UniqueName="AdHocAccessRequestId" HeaderStyle-Width="12%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>--%>

                                    <%-- <telerik:GridHyperLinkColumn
                                                DataNavigateUrlFields="AdHocAccessRequestId" SortExpression="Name" UniqueName="Name" DataNavigateUrlFormatString="~/Home/MyRequestDetails.aspx?Id={0}&prev=AdhocFailed"
                                                HeaderText="Application Name" DataTextField="Name" HeaderStyle-Width="15%" FilterControlWidth="80%">
                                            </telerik:GridHyperLinkColumn>--%>
                                    <telerik:GridBoundColumn DataField="RequestSource" DataType="System.String" FilterControlAltText="RequestSource" SortExpression="RequestSource" HeaderText="Request Source" UniqueName="RequestSource" HeaderStyle-Width="20%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="AccountTypeText" HeaderText="Account Type" HeaderStyle-Width="15%" UniqueName="AccountType">
                                        <%--  <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# DI.Chakra.Infrastructure.UtilityMethods.GetEnumAttribute<Description>((DI.Chakra.Infrastructure.Enums.AccountType) Eval("AccountType")) %>' />
                                        </ItemTemplate>--%>
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="RequestType" FilterControlAltText="RequestType" SortExpression="RequestType" HeaderText="Request Type" UniqueName="RequestType" HeaderStyle-Width="20%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="RequesteeName" FilterControlAltText="RequesteeName" SortExpression="RequesteeName" HeaderText="Request For" HeaderStyle-Width="20%" FilterControlWidth="80%"
                                        UniqueName="RequesteeName">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="RequesterName" FilterControlAltText="RequesterName" SortExpression="RequesterName" HeaderText="Request By" UniqueName="RequesterName" HeaderStyle-Width="20%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" SortExpression="Entitlement" HeaderText="Entitlement" HeaderStyle-Width="20%" FilterControlWidth="80%"
                                        UniqueName="Entitlement">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="RequestedDate" HeaderText="Request Date" SortExpression="RequestedDate" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" HeaderStyle-Width="22%" FilterControlWidth="80%">
                                    </telerik:GridDateTimeColumn>
                                    <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="ValidFrom" DataFormatString="{0}" FilterControlAltText="StartDate" SortExpression="StartDate" HeaderText="Start Date" UniqueName="StartDate" HeaderStyle-Width="15%" FilterControlWidth="80%">
                                    </telerik:GridDateTimeColumn>
                                    <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="ValidTo" DataFormatString="{0}" FilterControlAltText="ValidTo" SortExpression="ValidTo" HeaderText="End Date" UniqueName="ValidTo" HeaderStyle-Width="15%" FilterControlWidth="80%">
                                    </telerik:GridDateTimeColumn>
                                    <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="ExecutionDate" HeaderText="Execution Time" SortExpression="ExecutionDate" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" HeaderStyle-Width="22%" FilterControlWidth="80%">
                                    </telerik:GridDateTimeColumn>
                                    <telerik:GridBoundColumn DataField="RequestStatusText" FilterControlAltText="RequestStatus" SortExpression="RequestStatusText" HeaderText="Request Status" UniqueName="RequestStatus" HeaderStyle-Width="10%" FilterControlWidth="80%">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </cc1:IACMRadGridControl>

                    </asp:Panel>

                    <br />

                    <div style="float: right">
                        <%--<asp:Button ID="btnResolvedManually" Text="Manually Resolved" CssClass="btn btn-default btn-sm"
                                ToolTip="Manually Resolved" runat="server" OnClick="btnResolvedManually_Click" />
                            &nbsp;S
                             <asp:Button ID="btnCreateProvisionAcnt" Text="Submit" CssClass="btn btn-default btn-sm"
                                        ToolTip="Provision  A/C" runat="server" OnClick="btnCreateProvisionAcnt_Click" />
                                    &nbsp;--%>
                        <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-default btn-sm" Text="Refresh"
                            ToolTip="Refresh the page." OnClick="btnRefresh_Click" />
                        <asp:Button ID="btnAPACCancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel"
                            ToolTip="Go back to Provisioning Home page." OnClick="btnAPACCancel_Click" />
                    </div>
                </div>
            </div>

        </div>

    </ContentTemplate>
</asp:UpdatePanel>

<telerik:RadWindow runat="server" ID="UserOldNewDetailsModalPopupExtender" Modal="true" MinWidth="1100px" Title="User Details:"
    AutoSizeBehaviors="Width, Height" AutoSize="true" KeepInScreenBounds="true" VisibleOnPageLoad="false"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">

    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <div>
                    <div class="modal-header">
                        <h3 class="modal-title">Error Message:</h3>
                    </div>

                    <div class="modal-body">
                        <asp:Label ID="txtError" runat="server" Enabled="false"></asp:Label>
                    </div>

                    <div class="mBottom15" style="float: right;">

                        <asp:Button ID="Button3" runat="server" OnClick="btnOk_Close" CssClass="btn btn-default btn-sm" Text="Ok" />
                    </div>
                </div>
            </ContentTemplate>

        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>


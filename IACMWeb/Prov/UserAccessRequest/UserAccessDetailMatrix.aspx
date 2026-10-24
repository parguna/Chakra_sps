<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="UserAccessDetailMatrix.aspx.cs" Inherits="DI.Chakra.Web.Prov.UserAccessRequest.UserAccessDetailMatrix" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <script type="text/javascript">

        function RequestFromControls(sender, eventArgs) {
            //debugger;
            var context = eventArgs.get_context();
            var Item = sender._originalText;

            context["EntryValues"] = Item;
        }

        function OnClientEntryAddedName(sender, eventArgs) {

            $telerik.$(eventArgs.get_entry().get_token()).find('a').attr("title", "Remove Name");

        }

        function OnClientEntryAddedCompany(sender, eventArgs) {

            $telerik.$(eventArgs.get_entry().get_token()).find('a').attr("title", "Remove your Company Name");

        }
        function OnClientEntryAddedDepartment(sender, eventArgs) {

            $telerik.$(eventArgs.get_entry().get_token()).find('a').attr("title", "Remove your Department Name");

        }
        function OnClientEntryAddedDivision(sender, eventArgs) {

            $telerik.$(eventArgs.get_entry().get_token()).find('a').attr("title", "Remove your Division Name");

        }
        function OnClientEntryAddedBranch(sender, eventArgs) {

            $telerik.$(eventArgs.get_entry().get_token()).find('a').attr("title", "Remove your Branch Name");

        }
        function OnClientEntryAddedSection(sender, eventArgs) {

            $telerik.$(eventArgs.get_entry().get_token()).find('a').attr("title", "Remove your Section Name");

        }
        function OnClientEntryAddedCostCenter(sender, eventArgs) {

            $telerik.$(eventArgs.get_entry().get_token()).find('a').attr("title", "Remove your CostCenter Name");

        }
        function OnClientEntryAddingHandler(sender, eventArgs) {



            if (sender.get_entries().get_count() > 0) {

                eventArgs.set_cancel(true);
            }
        }
        function OnClientEntryAddingHandlerForUserName(sender, eventArgs) {
        if (sender.get_entries().get_count() > 0) {
            eventArgs.set_cancel(true);
        }
    }
    </script>
    <style>
        #ctl00_cplMain_UserAutoCompleteBox{
            width:100% !important;
        }
        .company-label {
            padding-left: 0px !important;
        }
    </style>
    <div class="row-fluid">
        <div class="col-xs-12 form-group">
            <div class="col-xs-6" id="autocompany" visible="false" runat="server">
                <asp:Label ID="Label6" runat="server" CssClass="col-xs-6 control-label company-label" AssociatedControlID="autoCompleteCompany">Company</asp:Label>
                <div class="col-xs-6">
                    <telerik:RadAutoCompleteBox ID="autoCompleteCompany" runat="server" EmptyMessage="Enter * to view full list" CssClass="form-control"
                        DropDownHeight="200px" DropDownWidth="300px" OnClientEntryAdding="OnClientEntryAddingHandler" OnClientEntryAdded="OnClientEntryAddedCompany"
                        OnClientRequesting="RequestFromControls" TextSettings-SelectionMode="Single" InputType="Token" Width="300px" AutoPostBack="true"
                        OnEntryAdded="autoCompleteCompany_EntryAdded" OnEntryRemoved="autoCompleteCompany_EntryRemoved">
                        <TextSettings SelectionMode="Single" />
                        <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetOrgCompanies" />
                    </telerik:RadAutoCompleteBox>
                </div>
            </div>
            <div class="col-xs-6">
                <asp:Label ID="Label7" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="autoCompleteCompany">User Name</asp:Label>
                <div class="col-xs-7" style="padding-left: 0px !important;">
                    <telerik:RadAutoCompleteBox ID="UserAutoCompleteBox" runat="server" HighlightFirstMatch="true"  MinFilterLength="2" OnClientEntryAdding="OnClientEntryAddingHandlerForUserName" EmptyMessage="Please enter at least 2 character to view the list"
                        OnClientEntryAdded="OnClientEntryAddedName" DropDownHeight="200px"  InputType="Token" CssClass="form-control" TextSettings-SelectionMode="Single"
                        Width="300px" AutoPostBack="true">
                        <TextSettings SelectionMode="Single" />
                        <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetReportingUsersOfThisManagerIDandAdmin_New" />
                    </telerik:RadAutoCompleteBox>
                </div>
                <div class="col-xs-3">
                    <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" CssClass="btn btn-default btn-sm" />
                </div>
            </div>
        </div>
    </div>
    <div class="row-fluid">
        <div class="col-xs-12 form-group" id="autoDept" visible="false" runat="server">
            <div class="col-xs-6">
                <asp:Label ID="Label8" runat="server" CssClass="col-xs-6 control-label company-label" AssociatedControlID="autoCompleteCompany">Department</asp:Label>
                <div class="col-xs-6">
                    <telerik:RadAutoCompleteBox ID="autoCompleteDepartment" runat="server" EmptyMessage="Enter * to view full list"
                        DropDownHeight="200px" OnClientEntryAdding="OnClientEntryAddingHandler" OnClientEntryAdded="OnClientEntryAddedDepartment" OnClientRequesting="RequestFromControls"
                        DropDownWidth="300px" InputType="Token" Width="300px" AutoPostBack="true"
                        OnEntryAdded="autoCompleteDepartment_EntryAdded" OnEntryRemoved="autoCompleteDepartment_EntryRemoved" CssClass="form-control"
                        Style="margin-top: 4px">
                        <TextSettings SelectionMode="Single" />
                        <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetOrgDepartments" />
                    </telerik:RadAutoCompleteBox>
                </div>
            </div>
            
        </div>
    </div>
    <%--<div class="row-fluid">
        <div class="col-xs-12 form-group" id="autoDivision" visible="false" runat="server">
            <asp:Label ID="Label1" runat="server" CssClass="col-xs-3 control-label" AssociatedControlID="autoCompleteCompany">Division</asp:Label>
            <div class="col-xs-9">
                <telerik:RadAutoCompleteBox ID="autoCompleteDivision" runat="server" EmptyMessage="Enter * to view full list"
                    DropDownHeight="200px" OnClientEntryAdding="OnClientEntryAddingHandler" OnClientEntryAdded="OnClientEntryAddedDivision" DropDownWidth="300px"
                    OnClientRequesting="RequestFromControls" InputType="Token" Width="300px" CssClass="form-control"
                    OnEntryAdded="autoCompleteDivision_EntryAdded" OnEntryRemoved="autoCompleteDivision_EntryRemoved"
                    AutoPostBack="true" Style="margin-top: 4px">
                    <TextSettings SelectionMode="Single" />
                    <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetOrgDivisions" />
                </telerik:RadAutoCompleteBox>
            </div>
        </div>
    </div>--%>
    <div class="row-fluid">
        <div class="col-xs-12 form-group" id="autobranch" visible="false" runat="server">
            <asp:Label ID="Label2" runat="server" CssClass="col-xs-3 control-label" AssociatedControlID="autoCompleteCompany">Branch</asp:Label>
            <div class="col-xs-9">
                <telerik:RadAutoCompleteBox ID="autoCompleteBranch" runat="server" EmptyMessage="Enter * to view full list"
                    DropDownHeight="200px" OnClientEntryAdding="OnClientEntryAddingHandler" OnClientEntryAdded="OnClientEntryAddedBranch" DropDownWidth="300px"
                    OnClientRequesting="RequestFromControls" InputType="Token" Width="300px" CssClass="form-control"
                    OnEntryAdded="autoCompleteBranch_EntryAdded" OnEntryRemoved="autoCompleteBranch_EntryRemoved"
                    AutoPostBack="true" Style="margin-top: 4px">
                    <TextSettings SelectionMode="Single" />
                    <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetOrgBranches" />
                </telerik:RadAutoCompleteBox>
            </div>
        </div>
    </div>
    <div class="row-fluid">
        <div class="col-xs-12 form-group" id="autoSection" visible="false" runat="server">
            <asp:Label ID="Label5" runat="server" CssClass="col-xs-3 control-label" AssociatedControlID="autoCompleteCompany">Section</asp:Label>
            <div class="col-xs-9">
                <telerik:RadAutoCompleteBox ID="autoCompleteSection" runat="server" EmptyMessage="Enter * to view full list" OnClientEntryAdding="OnClientEntryAddingHandler"
                    OnClientEntryAdded="OnClientEntryAddedSection"
                    OnEntryAdded="autoCompleteSection_EntryAdded" OnEntryRemoved="autoCompleteSection_EntryRemoved"
                    AutoPostBack="true" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" CssClass="form-control"
                    OnClientRequesting="RequestFromControls" Width="300px" Style="margin-top: 4px">
                    <TextSettings SelectionMode="Single" />
                    <WebServiceSettings Method="GetOrgSections" Path="../../Services/AutoCompleteDataService.svc" />
                </telerik:RadAutoCompleteBox>
            </div>
        </div>
    </div>
    <div class="row-fluid">
        <div class="col-xs-12 form-group" id="autoUnit" visible="false" runat="server">
            <asp:Label ID="Label3" runat="server" CssClass="col-xs-3 control-label" AssociatedControlID="autoCompleteCompany">Unit</asp:Label>
            <div class="col-xs-9">
                <telerik:RadAutoCompleteBox ID="autoCompleteUnit" runat="server" EmptyMessage="Enter * to view full list" OnClientEntryAdding="OnClientEntryAddingHandler"
                    OnClientEntryAdded="OnClientEntryAddedSection"
                    OnEntryAdded="autoCompleteUnit_EntryAdded" OnEntryRemoved="autoCompleteUnit_EntryRemoved"
                    AutoPostBack="true" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" CssClass="form-control"
                    OnClientRequesting="RequestFromControls" Width="300px" Style="margin-top: 4px">
                    <TextSettings SelectionMode="Single" />
                    <WebServiceSettings Method="GetOrgUnits" Path="../../Services/AutoCompleteDataService.svc" />
                </telerik:RadAutoCompleteBox>
            </div>
        </div>
    </div>
    <div class="row-fluid">
        <div class="col-xs-12" id="buttonsTR" runat="server">
            <div class="col-xs-3" id="divSpace" runat="server" visible="false"></div>
            <div class="form-group col-xs-9">
                <asp:Button ID="btnShowALL" runat="server" Width="180px" OnClick="btnShowALL_Click" Text="All Users" CssClass="btn btn-default btn-sm" />
                <asp:Button ID="btnDirectSubordinates" runat="server" Width="180px" OnClick="btnDirectSubordinates_Click" Text="Direct Subordinates" CssClass="btn btn-default btn-sm" />
                <asp:Button ID="btnAllSubordinates" runat="server" Width="180px" OnClick="btnAllSubordinates_Click" Text="All Subordinates" CssClass="btn btn-default btn-sm" />
                
            </div>
        </div>
    </div>
    <div class="row-fluid" runat="server" id="divGridResult">
        <div class="col-xs-12">
            <cc1:IACMRadGridControl ID="UserRadTreeList" ShowStatusBar="True" GridLines="None" CellSpacing="0" DisableRowSelect="true" OnPreRender="UserRadTreeList_PreRender"
                runat="server" AutoGenerateColumns="False" PageSize="50" AllowSorting="True" AllowPaging="true"  AllowCustomPaging="true"
                OnNeedDataSource="UserRadTreeList_NeedDataSource">
                <HeaderStyle Font-Bold="True" />
                <PagerStyle Mode="NumericPages"></PagerStyle>
                <GroupingSettings CaseSensitive="false" />
                <ExportSettings
                    HideStructureColumns="true"
                    IgnorePaging="true" ExportOnlyData="true"
                    Excel-Format="ExcelML"
                    OpenInNewWindow="true">
                    <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="470mm"
                        DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                        BorderStyle="Medium" BorderColor="#666666"
                        BorderType="AllBorders">
                    </Pdf>
                </ExportSettings>
                <MasterTableView EnableHierarchyExpandAll="true" DataKeyNames="AccountReconcilationId" PagerStyle-AlwaysVisible="true" PageSize="50"
                    AllowMultiColumnSorting="True" AllowFilteringByColumn="True" Width="180%">
                    <Columns>
                        <telerik:GridBoundColumn DataField="ApplicationType" HeaderText="Application Type" UniqueName="ApplicationType"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EndpointSystemName" HeaderText="Application Name" UniqueName="EndpointSystemName"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="FullName" HeaderText="Full Name" UniqueName="FullName"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="UserName" HeaderText="User Name" UniqueName="UserName"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Name" HeaderText="Account Name" UniqueName="Name"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Entitlement" HeaderText="Entitlement" UniqueName="Entitlement"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Description" HeaderText="Description" UniqueName="Description"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Institution" HeaderText="TCode" UniqueName="Institution"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EntitlementType" HeaderText="EntitlementType" UniqueName="EntitlementType"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="ReportingOfficer" HeaderText="Reporting Officer" UniqueName="ReportingOfficer"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="LastLogonDate" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" HeaderText="Last Logon" UniqueName="LastLogonDate"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="AccountType" HeaderText="AccountType" UniqueName="AccountType"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="IsActive" HeaderText="IsActive" UniqueName="IsActive"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Company" HeaderText="Company" UniqueName="Company"></telerik:GridBoundColumn>
                        <%--<telerik:GridBoundColumn DataField="Division" HeaderText="Division" UniqueName="Division"></telerik:GridBoundColumn>--%>
                        <telerik:GridBoundColumn DataField="Department" HeaderText="Department" UniqueName="Department"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Branch" HeaderText="Branch" UniqueName="Branch"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Section" HeaderText="Section" UniqueName="Section"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Unit" HeaderText="Unit" UniqueName="Unit"></telerik:GridBoundColumn>
                    </Columns>

                </MasterTableView>
            </cc1:IACMRadGridControl>
        </div>
    </div>
</asp:Content>

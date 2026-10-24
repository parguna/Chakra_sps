<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Employee.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/NUserDetailUC.ascx" TagPrefix="uc" TagName="NUserDetailUC" %>
<style>
    #ctl00_cplMain_ListUC_Users_ctl00_ctl02_ctl03_FilterCheckBox_IsActive{
        position: inherit;
        opacity:100;
        transform:scale(1.2);
    }
    input#ctl00_cplMain_ListUC_Users_ctl00_ctl02_ctl03_FilterCheckBox_IsActive+label.lbl{
        display:none;
    }
    
    .rddlPopup_IACMSkin .rddlList{
        height:100%;
    }
    .rddlPopup_IACMSkin{
        width:60px !important;
    }
</style>
<cc1:IACMRadGridControl ID="Users" runat="server" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CellSpacing="0" AllowCustomPaging="true" DisableRowSelect="true" PageSize="50"
    FilterMenu-CssClass="RadMenu_IACMFilterSkin" ClientSettings-EnablePostBackOnRowClick="true" EnterToFilterEnable="true" Width="100%"  Height="100%"
    OnNeedDataSource="Users_NeedDataSource" OnSelectedIndexChanged="Users_SelectedIndexChanged" OnItemCommand="Users_ItemCommand">
    <PagerStyle Mode="NextPrevAndNumeric" Height="100%" AlwaysVisible="true" PageSizeControlType="RadDropDownList" />
    <MasterTableView AutoGenerateColumns="False"  Width="210%" CommandItemDisplay="Top" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" DataKeyNames="Id"  PageSize="50">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridTemplateColumn HeaderText="User Name" DataField="UserName" FilterControlAltText="UserName" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="120px" FilterControlWidth="82%">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkUsername" runat="server" Text='<%#Eval("UserName") %>' CommandArgument='<%#Container.ItemIndex%>' OnCommand="lnkUsername_Command" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="EmployeeId" DataFormatString="{0}" FilterControlAltText="EmployeeId" HeaderText="Employee Id" SortExpression="EmployeeId" UniqueName="EmployeeId" HeaderStyle-Width="100px" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="DisplayName" DataFormatString="{0}" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Company" DataFormatString="{0}" FilterControlAltText="Company" HeaderText="Company" SortExpression="Company" UniqueName="Company" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Department" DataFormatString="{0}" FilterControlAltText="Department" HeaderText="Department" SortExpression="Department" UniqueName="Department" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Branch" DataFormatString="{0}" FilterControlAltText="Branch" HeaderText="Branch" SortExpression="Branch" UniqueName="Branch" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="SectionName" DataFormatString="{0}" FilterControlAltText="SectionName" HeaderText="Section Name" SortExpression="SectionName" UniqueName="SectionName" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="OrganisationUnit" DataFormatString="{0}" FilterControlAltText="OrganisationUnit" HeaderText="Unit" SortExpression="OrganisationUnit" UniqueName="OrganisationUnit" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="IsActive" AllowFiltering="true" FilterControlAltText="IsActive" HeaderText="Is Active" SortExpression="IsActive" UniqueName="IsActive" HeaderStyle-Width="50%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn DataField="JoinDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="JoinDate" HeaderText="Join Date" SortExpression="JoinDate" UniqueName="JoinDate" HeaderStyle-Width="100%" FilterControlWidth="77.5%">
            </telerik:GridDateTimeColumn>
            <telerik:GridDateTimeColumn DataField="TerminationDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="Last Service Date" HeaderText="Last Service Date" SortExpression="TerminationDate" UniqueName="TerminationDate" HeaderStyle-Width="100%" FilterControlWidth="77.5%">
            </telerik:GridDateTimeColumn>
              <telerik:GridBoundColumn DataField="ReportingOfficer" DataFormatString="{0}" FilterControlAltText="ReportingOfficer" HeaderText="Reporting Officer" SortExpression="ReportingOfficer" UniqueName="ReportingOfficer" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
               <telerik:GridBoundColumn DataField="CounterSigningOfficer" DataFormatString="{0}" FilterControlAltText="CounterSigningOfficer" HeaderText="CounterSigning Officer" SortExpression="CounterSigningOfficer" UniqueName="CounterSigningOfficer" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
           <telerik:GridBoundColumn DataField="BusinessRoleName" DataFormatString="{0}" FilterControlAltText="BusinessRoleName" HeaderText="Position" SortExpression="BusinessRoleName" UniqueName="BusinessRoleName" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="EmailId" DataFormatString="{0}" FilterControlAltText="EmailId" HeaderText="EmailId" SortExpression="EmailId" UniqueName="EmailId" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn DataField="CostCenter" DataFormatString="{0}" FilterControlAltText="CostCenter" HeaderText="Cost Center" SortExpression="CostCenter" UniqueName="CostCenter" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="CompanyCode" DataFormatString="{0}" FilterControlAltText="CompanyCode" HeaderText="Company Code" SortExpression="CompanyCode" UniqueName="CompanyCode" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn DataField="DepartmentCode" DataFormatString="{0}" FilterControlAltText="DepartmentCode" HeaderText="Department Code" SortExpression="DepartmentCode" UniqueName="DepartmentCode" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="BranchCode" DataFormatString="{0}" FilterControlAltText="BranchCode" HeaderText="Branch Code" SortExpression="BranchCode" UniqueName="BranchCode" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="SectionCode" DataFormatString="{0}" FilterControlAltText="SectionCode" HeaderText="SectionCode" SortExpression="SectionCode" UniqueName="SectionCode" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="UnitCode" DataFormatString="{0}" FilterControlAltText="UnitCode" HeaderText="Unit Code" SortExpression="UnitCode" UniqueName="UnitCode" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="OrganisationCode" DataFormatString="{0}" FilterControlAltText="OrganisationCode" HeaderText="Organisation Code" SortExpression="OrganisationCode" UniqueName="OrganisationCode" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn> 
            <telerik:GridDateTimeColumn DataField="CreatedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridDateTimeColumn>

        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>
<telerik:RadWindow runat="server" ID="UserDetail" Modal="true" Width="500px" Height="100%" Title="User Detail"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">User Detail</h3>
                </div>
                <div class="modal-body">
                    <%--<uc:UserDetailUC runat="server" ID="UserDetailUC" />--%>
                    <uc:NUserDetailUC runat="server" ID="NUserDetailUC" />
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

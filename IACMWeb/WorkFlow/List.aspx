<%@ Page Title="Workflow" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.WorkFlow.List" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="WorkflowUpdatePanel" runat="server">
        <ContentTemplate>
            <cc1:IACMRadGridControl ID="WorkflowConfigurationGrid" runat="server" OnNeedDataSource="WorkflowConfigurationGrid_NeedDataSource" OnSelectedIndexChanged="WorkflowConfigurationGrid_SelectedIndexChanged">
                <MasterTableView  NoMasterRecordsText="No Records Available" DataKeyNames="WorkflowConfigurationId" TableLayout="Fixed" Width="100%">
                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" />
                    <Columns>
                        <telerik:GridBoundColumn DataField="WorkflowName" FilterControlAltText="WorkflowName" HeaderText="Name" SortExpression="WorkflowName" UniqueName="WorkflowName" HeaderStyle-Width="20%" FilterControlWidth="77%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Description" FilterControlAltText="Filter Description column" HeaderText="Description" SortExpression="Description" UniqueName="Description" HeaderStyle-Width="25%" FilterControlWidth="82%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="RequestType" FilterControlAltText="RequestType" HeaderText="Request Type" SortExpression="RequestType" UniqueName="RequestType" HeaderStyle-Width="15%" FilterControlWidth="69%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="ApplicationType" FilterControlAltText="ApplicationType" HeaderText="Application Name" SortExpression="ApplicationType" UniqueName="ApplicationType" HeaderStyle-Width="15%" FilterControlWidth="69%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="IsActive" FilterControlAltText="IsActive" HeaderText="Is Active" SortExpression="IsActive" UniqueName="IsActive" HeaderStyle-Width="10%" FilterControlWidth="72%" AllowFiltering="true">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="CreatedBy" FilterControlAltText="Filter CreatedBy column" HeaderText="Created By" SortExpression="CreatedBy" UniqueName="CreatedBy" HeaderStyle-Width="15%" FilterControlWidth="69%">
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </cc1:IACMRadGridControl>

            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Title="SAP Enterprise Portal Groups" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Groups.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAPEP.Groups" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <div class="form-group">
                            <EndpointSystem:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="DropDownSelectectionChanged" DisplayControl="Application" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="Panel1" runat="server">
        <telerik:RadGrid ID="grdGroup" FilterMenu-CssClass="RadMenu_IACMFilterSkin"
            runat="server" AllowFilteringByColumn="True" OnNeedDataSource="grdGroup_NeedDataSource" AllowPaging="True"
            AllowSorting="True" CellSpacing="0">
            <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" />
            <ClientSettings>
                <Scrolling UseStaticHeaders="true" />
                <Selecting AllowRowSelect="true" />
            </ClientSettings>
            <GroupingSettings CaseSensitive="false" />
            <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" CommandItemDisplay="None" NoMasterRecordsText="No Records Available" EnableHeaderContextMenu="false" DataKeyNames="Id">
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn DataField="GroupName" FilterControlAltText="GroupName" HeaderText="Group Name" SortExpression="GroupName" UniqueName="GroupName" HeaderStyle-Width="20%" FilterControlWidth="88%">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn DataField="GroupDescription" FilterControlAltText="GroupDescription" SortExpression="GroupDescription" HeaderText="Group Description" UniqueName="GroupDescription" HeaderStyle-Width="40%" FilterControlWidth="92%">
                        <ItemTemplate>
                            <div style="overflow: hidden;">
                                <asp:Label ID="Label1" runat="server" ToolTip='<%# DataBinder.Eval(Container.DataItem, "GroupDescription") %>' Text='<%# DataBinder.Eval(Container.DataItem, "GroupDescription") %>'></asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Id" FilterControlAltText="Id" HeaderText="Group Id" SortExpression="Id" UniqueName="Id" HeaderStyle-Width="10%" FilterControlWidth="80%">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </asp:Panel>

</asp:Content>

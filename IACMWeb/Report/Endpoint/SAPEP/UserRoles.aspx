<%@ Page Title="SAP Enterprise Portal User To Roles Details" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="UserRoles.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAPEP.UserRoles" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
   <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <div class="form-group">  
                            <endpointsystem:endpointuc runat="server" id="EndpointUC" OnEndpointChanged="DropDownSelectectionChanged" DisplayControl="Application" />
                      
                    </div>
                </div>
            </div>
        </div>
    </div>  
       </div>

    <asp:Panel ID="Panel1" runat="server">                                   
    <telerik:RadGrid ID="grdUser" runat="server" AllowFilteringByColumn="True" OnNeedDataSource="grdUser_NeedDataSource" AllowPaging="True"
        AllowSorting="True" FilterMenu-CssClass="RadMenu_IACMFilterSkin">
        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" />
        <ClientSettings>
            <Scrolling UseStaticHeaders="true" />
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
        <GroupingSettings CaseSensitive="false" />
        <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" CommandItemDisplay="None" NoMasterRecordsText="No Records Available" EnableHeaderContextMenu="false"  Width="100%">
            <GroupByExpressions>
                <telerik:GridGroupByExpression>
                    <SelectFields>
                        <telerik:GridGroupByField HeaderText="User Name" FieldName="UserName"></telerik:GridGroupByField>
                    </SelectFields>
                    <GroupByFields>
                        <telerik:GridGroupByField FieldName="UserName" SortOrder="None"></telerik:GridGroupByField>
                    </GroupByFields>
                </telerik:GridGroupByExpression>
            </GroupByExpressions>
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="20%" FilterControlWidth="85%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="25%" FilterControlWidth="88%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="RoleName" FilterControlAltText="RoleName" HeaderText="Role Name" SortExpression="RoleName" UniqueName="RoleName" HeaderStyle-Width="20%" FilterControlWidth="85%">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="RoleDescription" FilterControlAltText="RoleDescription" SortExpression="RoleDescription" HeaderText="Role Description" UniqueName="RoleDescription" HeaderStyle-Width="33%" FilterControlWidth="90%">
                    <ItemTemplate>
                        <div style="overflow: hidden;">
                            <asp:Label ID="Label1" runat="server" ToolTip='<%# DataBinder.Eval(Container.DataItem, "RoleDescription") %>' Text='<%# DataBinder.Eval(Container.DataItem, "RoleDescription") %>'></asp:Label>
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>   
        </asp:Panel>                     
</asp:Content>

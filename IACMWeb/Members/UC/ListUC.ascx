<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Members.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/NUserDetailUC.ascx" TagPrefix="uc" TagName="NUserDetailUC" %>

<cc1:IACMRadGridControl ID="Users" runat="server" OnNeedDataSource="Users_NeedDataSource"  OnSelectedIndexChanged="Users_SelectedIndexChanged" OnItemCommand="Users_ItemCommand">
    <ClientSettings Selecting-AllowRowSelect="true" EnablePostBackOnRowClick="true">
</ClientSettings>    <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" DataKeyNames="Id">
       
    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="25%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="Employee AD-ID" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="25%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="EmployeeId" DataType="System.String" FilterControlAltText="EmployeeId" HeaderText="Employee Number" SortExpression="EmployeeId" UniqueName="EmployeeId" HeaderStyle-Width="15%" FilterControlWidth="69%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="OrganisationName" FilterControlAltText="OrganisationName" HeaderText="Department" SortExpression="OrganisationName" UniqueName="OrganisationName" HeaderStyle-Width="25%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <%--<telerik:GridBoundColumn DataField="EmailId" FilterControlAltText="EmailId" HeaderText="Email Id" SortExpression="EmailId" UniqueName="EmailId" HeaderStyle-Width="33%" FilterControlWidth="86.5%">
            </telerik:GridBoundColumn>--%>
            <telerik:GridDateTimeColumn DataField="JoinDate" FilterControlAltText="JoinDate" HeaderText="Join Date" SortExpression="JoinDate" UniqueName="JoinDate"  HeaderStyle-Width="20%" FilterControlWidth="77.5%">
            </telerik:GridDateTimeColumn>
            <telerik:GridTemplateColumn UniqueName="TemplateInfoColumn" HeaderStyle-Width="7%" FilterControlWidth="52%" AllowFiltering="false" HeaderText ="Detail / Info">
                <ItemTemplate>
                    <asp:ImageButton ID="ShowIcon" runat="server" CommandName="ShowInfo" src="../../Images/action-buttons/detail.png"></asp:ImageButton>
                </ItemTemplate>
                <ItemStyle CssClass="action-info" HorizontalAlign="Center" />
            </telerik:GridTemplateColumn>
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
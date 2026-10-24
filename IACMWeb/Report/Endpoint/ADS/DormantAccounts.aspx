<%@ Page Title="Dormant Accounts" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="DormantAccounts.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.ADS.DormantAccounts" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>



<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">


    <%--    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>

<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="col-xs-4">
                    <EndpointSystem:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="DropDownSelectectionChanged" DisplayControl="Application" />
                </div>
                <div class="form-group">
                    <asp:Label ID="lblDormantDays" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtDormantDays">No of Dormant Days:</asp:Label>
                    <div class="col-xs-2">
                        <asp:TextBox runat="server" ID="txtDormantDays" CssClass="form-control" MaxLength="4"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="rev" runat="server" ValidationExpression="^[1-9]{1}[0-9]{0,3}$" ControlToValidate="txtDormantDays" ErrorMessage="Invalid Input" BackColor="Yellow"></asp:RegularExpressionValidator>
                    </div>
                    <div class="col-xs-2">
                        <asp:Button ID="btnGenerateReport" runat="server" Text="Fetch Dormant Accounts" CssClass="btn btn-default btn-sm" OnClick="btnGenerateReport_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

    <asp:UpdatePanel ID="panel" runat="server">
        <ContentTemplate>
            <cc1:IACMRadGridControl ID="grdDormantAccount" runat="server" OnNeedDataSource="grdDormantAccount_NeedDataSource">
                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" DataKeyNames="UserId" NoMasterRecordsText="No Records Available" TableLayout="Fixed">
                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                    <Columns>
                        <telerik:GridBoundColumn DataField="sAMAccountName" FilterControlAltText="sAMAccountName" HeaderText="User Name" SortExpression="sAMAccountName" UniqueName="sAMAccountName" HeaderStyle-Width="200px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="displayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="200px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridDateTimeColumn DataField="lastLogon" FilterControlAltText="lastLogon" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" HeaderText="Last Login Date" SortExpression="lastLogon" UniqueName="lastLogon" HeaderStyle-Width="200px" FilterControlWidth="90px">
                        </telerik:GridDateTimeColumn>
                        <telerik:GridBoundColumn DataField="ou" FilterControlAltText="ou" HeaderText="OU" SortExpression="ou" UniqueName="ou" HeaderStyle-Width="200px" ItemStyle-Width="200px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="department" FilterControlAltText="department" SortExpression="department" HeaderText="Department" UniqueName="department" HeaderStyle-Width="150px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="IsMapped" FilterControlAltText="IsMapped" HeaderText="IsMapped" SortExpression="IsMapped" UniqueName="IsMapped" HeaderStyle-Width="150px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Owner" FilterControlAltText="Owner" HeaderText="Owner" SortExpression="Owner" UniqueName="Owner" HeaderStyle-Width="200px" FilterControlWidth="75px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="physicalDeliveryOfficeName" FilterControlAltText="physicalDeliveryOfficeName" HeaderText="Employee Number" SortExpression="physicalDeliveryOfficeName" UniqueName="physicalDeliveryOfficeName" HeaderStyle-Width="200px" FilterControlWidth="75px">
                        </telerik:GridBoundColumn>
                        <%--<telerik:GridBoundColumn DataField="company" FilterControlAltText="company" HeaderText="Company" SortExpression="company" UniqueName="company" HeaderStyle-Width="200px" FilterControlWidth="75px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="pager" FilterControlAltText="pager" HeaderText="Section" SortExpression="pager" UniqueName="pager" HeaderStyle-Width="200px" FilterControlWidth="75px">
                        </telerik:GridBoundColumn>--%>
                        <telerik:GridBoundColumn DataField="businessCategory" FilterControlAltText="businessCategory" HeaderText="Business Unit" SortExpression="businessCategory" UniqueName="businessCategory" HeaderStyle-Width="200px" FilterControlWidth="80px">
                        </telerik:GridBoundColumn>
                        <%--<telerik:GridBoundColumn DataField="division" FilterControlAltText="division" HeaderText="Division" SortExpression="division" UniqueName="division" HeaderStyle-Width="200px" FilterControlWidth="80px">
                        </telerik:GridBoundColumn>--%>
                        <telerik:GridBoundColumn DataField="mail" FilterControlAltText="mail" HeaderText="Email ID" SortExpression="mail" UniqueName="mail" HeaderStyle-Width="200px" FilterControlWidth="90px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="title" FilterControlAltText="title" HeaderText="Position" SortExpression="title" UniqueName="title" HeaderStyle-Width="200px" FilterControlWidth="90px">
                        </telerik:GridBoundColumn>
                        <%--<telerik:GridBoundColumn DataField="IsLocked" FilterControlAltText="IsLocked" HeaderText="IsLocked" SortExpression="IsLocked" UniqueName="IsLocked" HeaderStyle-Width="150px" FilterControlWidth="90px">
                        </telerik:GridBoundColumn>--%>
                        <telerik:GridBoundColumn DataField="LockScenario" FilterControlAltText="LockScenario" HeaderText="Lock Scenario" SortExpression="LockScenario" UniqueName="LockScenario" HeaderStyle-Width="150px" FilterControlWidth="90px">
                        </telerik:GridBoundColumn>
                        <telerik:GridDateTimeColumn DataField="whenCreated" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="whenCreated" HeaderText="Account Created Date" SortExpression="whenCreated" UniqueName="whenCreated" HeaderStyle-Width="200px" FilterControlWidth="100px">
                        </telerik:GridDateTimeColumn>
                        <telerik:GridDateTimeColumn DataField="PasswordExpirationDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="PasswordExpirationDate" HeaderText="Password Expiration Date" SortExpression="PasswordExpirationDate" UniqueName="PasswordExpirationDate" HeaderStyle-Width="200px" FilterControlWidth="90px">
                        </telerik:GridDateTimeColumn>
                        <telerik:GridDateTimeColumn DataField="pwdLastSet" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="pwdLastSet" HeaderText="Password Last Set" SortExpression="pwdLastSet" UniqueName="pwdLastSet" HeaderStyle-Width="200px" FilterControlWidth="90px">
                        </telerik:GridDateTimeColumn>
                        <telerik:GridDateTimeColumn DataField="accountExpires" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="accountExpires" HeaderText="Account Expires Date" SortExpression="accountExpires" UniqueName="accountExpires" HeaderStyle-Width="200px" FilterControlWidth="90px">
                        </telerik:GridDateTimeColumn>
                        <telerik:GridBoundColumn DataField="logonCount" FilterControlAltText="logonCount" HeaderText="Logon Count" SortExpression="logonCount" UniqueName="logonCount" HeaderStyle-Width="150px" FilterControlWidth="75px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="cn" FilterControlAltText="cn" HeaderText="CN" SortExpression="cn" UniqueName="cn" HeaderStyle-Width="150px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="IsAccountDisabled" FilterControlAltText="IsAccountDisabled" HeaderText="Disabled" SortExpression="IsAccountDisabled" UniqueName="IsAccountDisabled" HeaderStyle-Width="150px" FilterControlWidth="90px">
                        </telerik:GridBoundColumn>
                        <%--<telerik:GridTemplateColumn HeaderText="Disabled" HeaderStyle-Width="150px" FilterControlWidth="80px">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Convert.ToBoolean(Eval("IsAccountDisabled")) == true ? "Yes" : "No" %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Locked" HeaderStyle-Width="150px" FilterControlWidth="80px">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Convert.ToBoolean(Eval("IsAccountLocked")) == true ? "Yes" : "No" %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>--%>
                    </Columns>
                </MasterTableView>
            </cc1:IACMRadGridControl>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

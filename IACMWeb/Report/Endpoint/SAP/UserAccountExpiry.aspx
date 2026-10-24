<%@ Page Title="SAP User" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="UserAccountExpiry.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAP.UserAccountExpiry" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="col-xs-5">
                        <EndpointSystem:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="DropDownSelectectionChanged" DisplayControl="Application" />
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblAccountExpiryDays" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtAccountExpiryDays">Account Expiry Days:</asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox runat="server" ID="txtAccountExpiryDays" required="required" CssClass="form-control" MaxLength="4"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="rev" runat="server" ValidationExpression="^[1-9]{1}[0-9]{0,3}$" ControlToValidate="txtAccountExpiryDays" ErrorMessage="Invalid Input" BackColor="Yellow"></asp:RegularExpressionValidator>
                        </div>
                    </div>

                </div>
            </div>

        </div>
    </div>
    <div>
        <asp:Button ID="btnGenerateReport" runat="server" Text="Fetch User Expiry Accounts" CssClass="btn btn-default btn-sm" OnClick="btnGenerateReport_Click" />
    </div>
    <br />

    <asp:UpdatePanel ID="panel" runat="server">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server">
                <cc1:IACMRadGridControl ID="grdExpiryAccount" runat="server" OnNeedDataSource="grdExpiryAccount_NeedDataSource">
                    <MasterTableView  TableLayout="fixed"
                        DataKeyNames="UserId" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                        <CommandItemSettings ShowAddNewRecordButton="false"
                            ShowRefreshButton="true"></CommandItemSettings>
                        <Columns>
                            <telerik:GridBoundColumn DataField="UserLogonName" FilterControlAltText="UserLogonName" HeaderText="User Name" SortExpression="UserLogonName" UniqueName="UserLogonName" HeaderStyle-Width="200px" FilterControlWidth="70px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridDateTimeColumn DataField="EndDate" FilterControlAltText="EndDate" HeaderText="Account Expiry Date" SortExpression="EndDate" UniqueName="EndDate" HeaderStyle-Width="200px" FilterControlWidth="90px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridDateTimeColumn>
                            <telerik:GridBoundColumn DataField="Firstname" FilterControlAltText="Firstname" HeaderText="First Name" SortExpression="Firstname" UniqueName="Firstname" HeaderStyle-Width="200px" FilterControlWidth="70px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Lastname" FilterControlAltText="Lastname" HeaderText="Last Name" SortExpression="Lastname" UniqueName="Lastname" HeaderStyle-Width="200px" FilterControlWidth="70px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridDateTimeColumn DataField="lastLogon" FilterControlAltText="lastLogon" HeaderText="Last Login Date" SortExpression="lastLogon" UniqueName="lastLogon" HeaderStyle-Width="200px" FilterControlWidth="90px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridDateTimeColumn>
                            <telerik:GridBoundColumn DataField="E_Mail" FilterControlAltText="E_Mail" SortExpression="E_Mail" HeaderText="Email" UniqueName="E_Mail" HeaderStyle-Width="200px" FilterControlWidth="70px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="UserType" FilterControlAltText="UserType" SortExpression="UserType" HeaderText="User Type" UniqueName="UserType" HeaderStyle-Width="150px" FilterControlWidth="60px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="department" FilterControlAltText="department" SortExpression="department" HeaderText="Department" UniqueName="department" HeaderStyle-Width="200px" FilterControlWidth="60px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="costcenter" FilterControlAltText="costcenter" SortExpression="costcenter" HeaderText="Cost Center" UniqueName="costcenter" HeaderStyle-Width="150px" FilterControlWidth="60px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="IsLocked" FilterControlAltText="IsLocked" SortExpression="IsLocked" HeaderText="IsLocked" UniqueName="IsLocked" HeaderStyle-Width="100px" FilterControlWidth="30px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="LockScenario" FilterControlAltText="LockScenario" SortExpression="LockScenario" HeaderText="LockScenario" UniqueName="LockScenario" HeaderStyle-Width="100px" FilterControlWidth="60px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="IsMapped" FilterControlAltText="IsMapped" HeaderText="IsMapped" SortExpression="IsMapped" UniqueName="IsMapped" HeaderStyle-Width="150px" FilterControlWidth="50px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Owner" FilterControlAltText="Owner" HeaderText="Owner" SortExpression="Owner" UniqueName="Owner" HeaderStyle-Width="150px" FilterControlWidth="50px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>

                            <telerik:GridBoundColumn DataField="IsUserLocked" FilterControlAltText="IsUserLocked" HeaderText="User Locked" SortExpression="IsUserLocked" UniqueName="IsUserLocked" HeaderStyle-Width="150px" FilterControlWidth="40px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="IsActive" FilterControlAltText="IsActive" HeaderText="Active" SortExpression="IsActive" UniqueName="IsActive" HeaderStyle-Width="100px" FilterControlWidth="40px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridDateTimeColumn DataField="StartDate" FilterControlAltText="StartDate" HeaderText="Start Date" SortExpression="StartDate" UniqueName="StartDate" HeaderStyle-Width="200px" FilterControlWidth="90px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridDateTimeColumn>

                        </Columns>
                    </MasterTableView>
                </cc1:IACMRadGridControl>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>


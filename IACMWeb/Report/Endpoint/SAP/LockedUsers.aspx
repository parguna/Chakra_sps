<%@ Page Title="SAP Locked Users" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="LockedUsers.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAP.LockedUsers" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>



<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">

    <asp:UpdatePanel runat="server" ID="userPanel">
        <ContentTemplate>
            <div class="content-wrapper">
                <div class="form">
                    <div class="form-horizontal left clearfix">
                        <div class="row-fluid">
                            <div class="col-xs-7">
                                <EndpointSystem:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="DropDownSelectectionChanged" DisplayControl="Application" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <asp:Panel ID="Panel1" runat="server">
                <cc1:IACMRadGridControl ID="grdUser" runat="server" OnNeedDataSource="grdUser_NeedDataSource">
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
                            <telerik:GridBoundColumn DataField="IsUserLocked" FilterControlAltText="IsUserLocked" HeaderText="User Locked" SortExpression="IsUserLocked" UniqueName="IsUserLocked" HeaderStyle-Width="150px" FilterControlWidth="40px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
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
                            <telerik:GridDateTimeColumn DataField="lastLogon" FilterControlAltText="lastLogon" HeaderText="Last Login Date" SortExpression="lastLogon" UniqueName="lastLogon" HeaderStyle-Width="200px" FilterControlWidth="90px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridDateTimeColumn>
                            <telerik:GridDateTimeColumn DataField="StartDate" FilterControlAltText="StartDate" HeaderText="Start Date" SortExpression="StartDate" UniqueName="StartDate" HeaderStyle-Width="200px" FilterControlWidth="90px">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridDateTimeColumn>
                            <telerik:GridDateTimeColumn DataField="EndDate" FilterControlAltText="EndDate" HeaderText="End Date" SortExpression="EndDate" UniqueName="EndDate" HeaderStyle-Width="200px" FilterControlWidth="90px">
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


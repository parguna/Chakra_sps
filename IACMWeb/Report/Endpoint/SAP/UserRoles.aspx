<%@ Page Title="SAP User To Roles" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="UserRoles.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAP.UserRoles" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">

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

    <asp:UpdatePanel ID="reportPanel" runat="server">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server">
                <cc1:IACMRadGridControl ID="grdUser" runat="server" OnNeedDataSource="grdUser_NeedDataSource">
                    <MasterTableView TableLayout="fixed"
                        DataKeyNames="UserId" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                        <CommandItemSettings ShowAddNewRecordButton="false"
                            ShowRefreshButton="true"></CommandItemSettings>
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

                        <Columns>
                            <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="20%" FilterControlWidth="60%">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="20%" FilterControlWidth="60%">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Department" FilterControlAltText="Department" SortExpression="Department" HeaderText="Department" UniqueName="Department" HeaderStyle-Width="15%" FilterControlWidth="60%">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>


                            <telerik:GridBoundColumn DataField="RoleCode" FilterControlAltText="RoleCode" HeaderText="Role Code" SortExpression="RoleCode" UniqueName="RoleCode" HeaderStyle-Width="20%" FilterControlWidth="60%">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>


                            <telerik:GridBoundColumn DataField="RoleDescription" FilterControlAltText="RoleDescription" HeaderText="Role Description" SortExpression="RoleDescription" UniqueName="RoleDescription" HeaderStyle-Width="15%" FilterControlWidth="60%">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text="" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>



                        </Columns>
                    </MasterTableView>
                </cc1:IACMRadGridControl>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>


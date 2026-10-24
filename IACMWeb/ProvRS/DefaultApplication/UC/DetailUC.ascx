<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.ProvRS.DefaultApplication.UC.DetailUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>
<%@ Register Src="~/UsrControl/NRuleUC.ascx" TagPrefix="uc1" TagName="RuleUC" %>


<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <uc1:RuleUC ID="RuleUC" runat="server" OnRuleChanged="RuleUC_RuleChanged" />
                </div>
                <div class="col-xs-6">
                    <div class="col-xs-10 buttons">
                        <uc1:EndpointUC runat="server" ID="EndpointUC" DisplayControl="Application" EndpointsOnly="true" ShowTrustedSource="true" AllEndPoints="true" ShowAllChildEndpoints="false" OnEndpointChanged="EndpointUC_EndpointChanged" />
                    </div>
                    <div class="col-xs-2">
                        <asp:Button ID="Addbutton" ValidationGroup="Add" runat="server" Text="Add"
                            OnClick="Addbutton_Click" CssClass="btn btn-default btn-sm" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="form-group">
    <div class="col-xs-12">


        <telerik:RadGrid ID="ApplicationDefaultGrid" runat="server" AllowPaging="true" AllowSorting="false" AutoGenerateColumns="false" OnItemCommand="RuleAppicationGrid_ItemCommand">
            <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="true" />
            <ClientSettings>
                <Scrolling UseStaticHeaders="true" />
                <Selecting AllowRowSelect="false" />
            </ClientSettings>
            <GroupingSettings CaseSensitive="false" />

            <MasterTableView NoMasterRecordsText="No RuleSet for Applications Available" DataKeyNames="RuleId,EndpointSystemid">
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn DataField="RuleCode" HeaderText="Rule" SortExpression="RuleCode" UniqueName="RuleCode" HeaderStyle-Width="30%" FilterControlWidth="80%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="EndpointSystemName" HeaderText="Endpoint" SortExpression="Endpoint.DisplayName" UniqueName="EndpointSystemName" HeaderStyle-Width="30%" FilterControlWidth="80%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn DataField="CreatedDate" DataFormatString="{0:dd-MM-yyyy}"  HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" FilterControlWidth="90%" HeaderStyle-Width="13%">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="CreatedBy" HeaderText="Created By" SortExpression="CreatedBy" UniqueName="CreatedBy" FilterControlWidth="70%" HeaderStyle-Width="13%">
                    </telerik:GridBoundColumn>
                    <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="Delete" ConfirmText="Are you sure you want to delete ?" ConfirmDialogType="RadWindow" HeaderText="Delete"
                        UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="5%">
                        <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                    </telerik:GridButtonColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
</div>


<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="BackButton_Click"></asp:Button>

</div>





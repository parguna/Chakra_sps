<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.AdvanceDelegation.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%--<style type="text/css">
    #ctl00_cplMain_ListUC_AdvaceDelegateGrid_ctl00_ctl02_ctl03_FilterCheckBox_IsActive{
        position: inherit;
        opacity:100;
        transform:scale(1.2);
    }
    input#ctl00_cplMain_ListUC_AdvaceDelegateGrid_ctl00_ctl02_ctl03_FilterCheckBox_IsActive+label.lbl{
        display:none;
    }
</style>--%>
<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row" style="padding-bottom:10px;">
                <div class="col-sm-10"></div>
                <div class="col-sm-2">
                    <asp:CheckBox ID="chkIncludeInactive" runat="server"
                        AutoPostBack="True"
                        TextAlign="Right"
                        OnCheckedChanged="chkIncludeInactive_CheckedChanged" /><span class="lbl">Show All</span>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <cc1:IACMRadGridControl ID="AdvaceDelegateGrid" runat="server" AllowCustomPaging="true" OnNeedDataSource="AdvaceDelegateGrid_NeedDataSource" OnItemDataBound="AdvaceDelegateGrid_ItemDataBound" OnSelectedIndexChanged="AdvaceDelegateGrid_SelectedIndexChanged" AllowFilteringByColumn="true">
                        <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" ShowHeadersWhenNoRecords="true" Width="140%"
                            DataKeyNames="DelegationId" TableLayout="fixed" NoMasterRecordsText="No Records Available">
                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                            <Columns>
                                <telerik:GridBoundColumn DataField="Id" FilterControlAltText="Delegation ID" HeaderText="Delegation ID" SortExpression="Id" UniqueName="Id" HeaderStyle-Width="100%" FilterControlWidth="100%">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="ActionUserName" FilterControlAltText="ActionUserName" HeaderText="Delegated From" SortExpression="ActionUserName" UniqueName="ActionUserName" HeaderStyle-Width="100%" FilterControlWidth="100%">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="DelegatedToUserName" FilterControlAltText="DelegatedToUserName" HeaderText="Delegated To" SortExpression="DelegatedToUserName" UniqueName="DelegatedToUserName" HeaderStyle-Width="100%" FilterControlWidth="100%">
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn DataField="DelegateProvisioningTask" HeaderText="Provisioning" UniqueName="DelegateProvisioningTask" HeaderStyle-Width="100%" FilterControlAltText="DelegateProvisioningTask" CurrentFilterFunction="NoFilter">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDelegateProvisioningTask" runat="server" Text='<%#Convert.ToBoolean( Eval("DelegateProvisioningTask")) == true ? "Yes" : "No" %>'></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="DelegateAttestationTask" HeaderText="Attestation" UniqueName="DelegateAttestationTask" HeaderStyle-Width="100%" FilterControlAltText="DelegateAttestationTask" CurrentFilterFunction="NoFilter">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDelegateAttestationTask" runat="server" Text='<%#Convert.ToBoolean( Eval("DelegateAttestationTask")) == true ? "Yes" : "No" %>'></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="DelegationComments" FilterControlAltText="DelegationComments" HeaderText="Comments" SortExpression="DelegationComments" UniqueName="DelegationComments" HeaderStyle-Width="100%" FilterControlWidth="100%">
                                </telerik:GridBoundColumn>
                                <telerik:GridDateTimeColumn DataField="StartDate" FilterControlAltText="StartDate" DataType="System.DateTime" HeaderText="Start Date" SortExpression="StartDate" UniqueName="StartDate" HeaderStyle-Width="100%" FilterControlWidth="69%">
                                </telerik:GridDateTimeColumn>
                                <telerik:GridDateTimeColumn DataField="EndDate" FilterControlAltText="EndDate" DataType="System.DateTime" HeaderText="End Date" SortExpression="EndDate" UniqueName="EndDate" HeaderStyle-Width="100%" FilterControlWidth="69%">
                                </telerik:GridDateTimeColumn>
                                <telerik:GridTemplateColumn DataField="IsActive" HeaderText="Is Active" UniqueName="IsActive" HeaderStyle-Width="100%" FilterControlAltText="IsActive" CurrentFilterFunction="NoFilter" CurrentFilterValue="False">
                                    <ItemTemplate>
                                        <asp:Label ID="Label3" runat="server" Text='<%#Convert.ToBoolean( Eval("IsActive")) == true ? "Yes" : "No" %>'></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="CreatedBy" FilterControlAltText="CreatedBy" SortExpression="CreatedBy" HeaderText="Created By" UniqueName="CreatedBy" HeaderStyle-Width="100%" FilterControlWidth="100%">
                                </telerik:GridBoundColumn>
                                <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="CreatedDate" HeaderStyle-Width="100%" FilterControlAltText="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlWidth="90px">
                                </telerik:GridDateTimeColumn>

                                <telerik:GridBoundColumn DataField="LastModifiedBy" FilterControlAltText="LastModifiedBy" SortExpression="LastModifiedBy" HeaderText="Changed By" UniqueName="LastModifiedBy" HeaderStyle-Width="100%">
                                </telerik:GridBoundColumn>
                                <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="LastModifiedDate" FilterControlAltText="LastModifiedDate" HeaderText="Changed Date" SortExpression="LastModifiedDate" UniqueName="LastModifiedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlWidth="90px" HeaderStyle-Width="100%">
                                </telerik:GridDateTimeColumn>
                            </Columns>
                        </MasterTableView>
                    </cc1:IACMRadGridControl>
                </div>
            </div>
        </div>
    </div>
</div>



<%--<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" ID="BackButton" class="btn btn-default btn-sm" Text="Cancel" OnClick="BackButton_Click"></asp:Button>
</div>--%>
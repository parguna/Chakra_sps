<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="DI.Chakra.Web.InHouse.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc2" TagName="EndpointUC" %>
<%@ Register Src="~/Configuration/Data/Endpoint/XLUploadUC.ascx" TagPrefix="uc1" TagName="XLUploadUC" %>

<style type="text/css">
    .rgSelectedRow > .disabledUserName{
        color:white !important;
    }
</style>
<div class="content-wrapper">

    <telerik:RadWindow ID="modalPopup" runat="server" Width="700px" Height="170px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>

            <div class="modal-header">
                <h3 class="modal-title" id="H1">Data Upload</h3>
            </div>

            <div class="modal-body">
                <div class="form-horizontal left clearfix">
                    <div class="row-fluid">
                        <div class="col-xs-12">
                            <div class="form-group">
                                <uc1:XLUploadUC runat="server" ID="XLUploadUC1" OnUserControlButtonClicked="XLUploadUC1_UserControlButtonClicked1" IsPartialUpload="true" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal-footer">
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click"></asp:Button>
            </div>

        </ContentTemplate>
    </telerik:RadWindow>
    <div class="row-fluid">
        <div class="col-xs-12">
            <div class="form-group">
                <div class="col-xs-3">
                    <asp:Label ID="Label1" runat="server" CssClass="control-label" AssociatedControlID="ddlInhouseSystems">Inhouse system :</asp:Label>
                </div>
                <div class="col-xs-4">
                    <asp:DropDownList ID="ddlInhouseSystems" runat="server" CssClass="form-control selectpicker" AutoPostBack="true" OnSelectedIndexChanged="ddlInhouseSystems_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
            </div>
        </div>
    </div>
    <div class="row-fluid">
        <div class="col-xs-12">
            &nbsp;
        </div>
    </div>
    <div class="row-fluid">
        <div class="col-xs-12">
            <div class="form-group" id="tblValidRecordDetail" runat="server">
                <div class="col-xs-3">
                    <asp:Label ID="Label2" CssClass="wizlbl" runat="server" Text="Display Records :"></asp:Label>
                </div>
                <div class="col-xs-4">
                    <asp:RadioButton ID="rbValid" runat="server" GroupName="ValidRecords" CssClass="wizlbl" Text="Valid"
                        OnCheckedChanged="ValidRecordsSelectionChanged" AutoPostBack="true" />
                    &nbsp;&nbsp;
                            <asp:RadioButton ID="rbInValid" runat="server" GroupName="ValidRecords" CssClass="wizlbl" Checked="True"
                                OnCheckedChanged="ValidRecordsSelectionChanged" Text="InValid" AutoPostBack="true" />
                </div>
            </div>
        </div>
    </div>
     <div class="row-fluid">
        <div class="col-xs-12">
            &nbsp;
        </div>
    </div>
    <div class="row-fluid">
        <div class="col-xs-12">
            <div class="form-group">
                <div class="col-xs-3">
                    <div id="grdHeader" runat="server" class="subTitle">InValid Records</div>
                </div>
                <div class="col-xs-4">
                    <asp:Label ID="lblDetails" Text="" runat="server"></asp:Label>
                </div>
            </div>
        </div>
    </div>
    <div class="row-fluid">
        <div class="col-xs-12">
            &nbsp;
        </div>
    </div>
    <div class="row-fluid">
        <div class="col-xs-12">
            <cc1:IACMRadGridControl ID="Account" runat="server" OnItemDataBound="Account_ItemDataBound" EnableLinqExpressions="false" OnNeedDataSource="Account_NeedDataSource"
                OnSelectedIndexChanged="Account_SelectedIndexChanged">
                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" EnableHeaderContextMenu="true" DataKeyNames="Id,ReconciliationId" EditMode="InPlace"
                    InsertItemPageIndexAction="ShowItemOnFirstPage" TableLayout="Fixed" Width="200%"
                    NoDetailRecordsText="No records found.">
                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                    <Columns>
                        <telerik:GridBoundColumn ItemStyle-CssClass="disabledUserName" DataField="UserName"  FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="InhouseSystemCode" FilterControlAltText="InhouseSystemCode" HeaderText="Inhouse System Code" SortExpression="InhouseSystemCode" UniqueName="InhouseSystemCode" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EmployeeId" FilterControlAltText="EmployeeId" HeaderText="Employee Id" SortExpression="EmployeeId" UniqueName="EmployeeId" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Email" FilterControlAltText="Email" HeaderText="Email" SortExpression="Email" UniqueName="Email" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="FirstName" FilterControlAltText="FirstName" HeaderText="First Name" SortExpression="FirstName" UniqueName="FirstName" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="LastName" FilterControlAltText="LastName" HeaderText="Last Name" SortExpression="LastName" UniqueName="LastName" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="AccountTypeString" FilterControlAltText="AccountTypeString" HeaderText="Account Type" SortExpression="AccountTypeString" UniqueName="AccountTypeString" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="AccountCreatedDate" FilterControlAltText="AccountCreatedDate" HeaderText="Account Created Date" SortExpression="AccountCreatedDate" UniqueName="AccountCreatedDate" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="LastLoginDate" FilterControlAltText="LastLoginDate" HeaderText="Last Login Date" SortExpression="LastLoginDate" UniqueName="LastLoginDate" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="IsLocked" FilterControlAltText="IsLocked" HeaderText="Is Locked" SortExpression="IsLocked" UniqueName="IsLocked" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="IsDisabled" FilterControlAltText="IsDisabled" HeaderText="IsDisabled" SortExpression="IsDisabled" UniqueName="IsDisabled" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="IsDeleted" FilterControlAltText="IsDeleted" HeaderText="IsDeleted" SortExpression="IsDeleted" UniqueName="IsDeleted" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Remarks1" FilterControlAltText="Remarks1" HeaderText="Remarks1" SortExpression="Remarks1" UniqueName="Remarks1" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Remarks2" FilterControlAltText="Remarks2" HeaderText="Remarks2" SortExpression="Remarks2" UniqueName="Remarks2" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Remarks3" FilterControlAltText="Remarks3" HeaderText="Remarks3" SortExpression="Remarks3" UniqueName="Remarks3" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Remarks4" FilterControlAltText="Remarks4" HeaderText="Remarks4" SortExpression="Remarks4" UniqueName="Remarks4" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Remarks5" FilterControlAltText="Remarks5" HeaderText="Remarks5" SortExpression="Remarks5" UniqueName="Remarks5" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Permission" FilterControlAltText="Permission" HeaderText="Permission" SortExpression="Permission" UniqueName="Permission" HeaderStyle-Width="15%" FilterControlWidth="65%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="ValidationMessage" FilterControlAltText="ValidationMessage" HeaderText="Validation Message"
                            SortExpression="ValidationMessage" UniqueName="ValidationMessage" HeaderStyle-Width="260px" FilterControlWidth="80%">
                        </telerik:GridBoundColumn>
                        <telerik:GridEditCommandColumn FilterControlWidth="75%" UniqueName="Edit" HeaderStyle-Width="15%" />
                    </Columns>
                </MasterTableView>
            </cc1:IACMRadGridControl>
        </div>
    </div>
</div>


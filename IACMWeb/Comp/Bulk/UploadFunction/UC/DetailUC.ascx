<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Bulk.UploadFunction.UC.DetailUC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <label id="lblOptions" class="col-xs-3 control-label">Upload Options</label>
                        <div class="col-xs-4">
                            <asp:RadioButton ID="radNewObject" GroupName="options" runat="server" />
                            <span class="lbl">New</span>
                            <asp:RadioButton ID="radAddObject" GroupName="options" runat="server" />
                            <span class="lbl">Add</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblFileName" AssociatedControlID="RadUploadFile" runat="server" Text="Upload File" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-4">
                            <telerik:RadUpload ID="RadUploadFile" runat="server" ControlObjectsVisibility="None">
                                <Localization Select="Browse" />
                            </telerik:RadUpload>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblTemplateCreateFunctions" runat="server" AssociatedControlID="hlTemplateCreateFunctions" Text="Template For Creating Functions" CssClass="col-xs-3"></asp:Label>
                        <div class="col-xs-4">
                            <asp:HyperLink ID="hlTemplateCreateFunctions" Text="FunctionUpload - New.csv" NavigateUrl="~/Comp/Bulk/FunctionUpload - New.csv" runat="server">FunctionUpload - New.csv</asp:HyperLink>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblTemplateCreateFunctionItems" runat="server" AssociatedControlID="hlTemplateAddFunctionItems" Text="Template For Adding Function Items" CssClass="col-xs-3"></asp:Label>
                        <div class="col-xs-4">
                            <asp:HyperLink ID="hlTemplateAddFunctionItems" Text="FunctionUpload - Add.csv" NavigateUrl="~/Comp/Bulk/FunctionUpload - Add.csv" runat="server">FunctionUpload - Add.csv</asp:HyperLink>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> 
<telerik:RadGrid ID="grdUploadErrors" runat="server" Width="65%"
    AllowPaging="true" AllowSorting="true" Height="100%"
    AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
    AllowCustomPaging="false" AllowFilteringByColumn="true" CellSpacing="0" OnInit="grdUploadErrors_Init">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="10" Height="100%" AlwaysVisible="true" />
    <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="false" ShowExportToCsvButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="ErrorMessage" FilterControlAltText="ErrorMessage" HeaderText="Error Message" SortExpression="ErrorMessage" UniqueName="ErrorMessage" HeaderStyle-Width="100" FilterControlWidth="70%">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>
</div>




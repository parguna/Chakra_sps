<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Job.CustomReport.UC.DetailUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Src="~/Job/CustomReport/UC/CustomReportParameterItemUC.ascx" TagPrefix="uc1" TagName="CustomReportParameterItemUC" %>
<%@ Register Src="~/UsrControl/Common/GenericAutoCompleteBox.ascx" TagPrefix="uc1" TagName="GenericAutoCompleteBox" %>


<script type="text/javascript">

</script>

<asp:Panel runat="server" ID="ContentWrapper" CssClass="content-wrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">

                    <div class="form-group">
                        <asp:Label ID="reportName" AssociatedControlID="txtReportName" runat="server" Text="Report Name" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtReportName" CssClass="form-control" runat="server" required="required" data-errMessage="Report Name is required"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="description" AssociatedControlID="txtDescription" runat="server" Text="Report Description" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtDescription" CssClass="form-control" required="required" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label1" AssociatedControlID="txtQuery" runat="server" Text="Report Query" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtQuery" CssClass="form-control" required="required" runat="server" Rows="8" TextMode="MultiLine" Height="200px" Width="546px"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label2" runat="server" Text="" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="buttons col-xs-7">
                            <asp:Button ID="btnValidate" runat="server" CssClass="btn btn-default btn-sm" Text="Validate" OnClick="btnValidate_Click" />
                            <asp:Button ID="btnGenerateParameters" runat="server" Text="Generate Parameter(s)" class="btn btn-default btn-sm" OnClick="btnGenerateParameters_Click" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label3" AssociatedControlID="chkGroupBy" runat="server" Text="Add Group By" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:CheckBox ID="chkGroupBy" runat="server" AutoPostBack="true" OnCheckedChanged="chkGroupBy_CheckedChanged"></asp:CheckBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4 control-label">Column</label>
                        <div class="col-xs-7">
                            <asp:DropDownList runat="server" ID="ddlColumns" CssClass="form-control selectpicker" data-width="100%">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblLinkToMenu" AssociatedControlID="chkLinkToMenu" runat="server" Text="Link To Menu" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:CheckBox ID="chkLinkToMenu" required="required" runat="server" AutoPostBack="true" OnCheckedChanged="chkLinkToMenu_CheckedChanged"></asp:CheckBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4 control-label">Parent Menu</label>
                        <div class="col-xs-7">
                            <uc1:GenericAutoCompleteBox runat="server" ID="GenericAutoCompleteBox" ServiceMethodName="GetAllReportMenu"
                                ServicePath="../../../Services/AutoCompleteDataService.svc" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <telerik:RadListView runat="server" RenderMode="Lightweight" ID="CustomReportParameterList" PageSize="10" AllowPaging="True"
        ItemPlaceholderID="itemPlaceholder" OnNeedDataSource="CustomReportParameterList_NeedDataSource"
        OnItemDataBound="CustomReportParameterList_ItemDataBound">
        <ItemTemplate>
            <uc1:CustomReportParameterItemUC runat="server" ID="CustomReportParameterItemUC" OnItemClicked="CustomReportParameterItemUC_ItemClicked" />
        </ItemTemplate>
        <LayoutTemplate>
            <div class="RadGrid RadGrid_IACMSkin">
                <table class="rgMasterTableList rgClipCells" style="width: 100%; table-layout: fixed; empty-cells: show;">
                    <thead>
                        <tr>
                            <th class="rgHeader" width="5%">Sequence No.</th>
                            <th class="rgHeader" width="27%">Parameter Name</th>
                            <th class="rgHeader" width="28%">Description</th>
                            <th class="rgHeader" width="10%">Data Type</th>
                            <th class="rgHeader" width="15%">Default Value</th>
                            <th class="rgHeader" width="15%">Data Format string</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr id="itemPlaceholder" runat="server">
                        </tr>
                    </tbody>
                </table>
            </div>
        </LayoutTemplate>
    </telerik:RadListView>
    <div class="buttons text-right nav-rectangle">
        <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="BackButton_Click"></asp:Button>
    </div>
</asp:Panel>

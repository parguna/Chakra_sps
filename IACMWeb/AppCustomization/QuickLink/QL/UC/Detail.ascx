<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="DI.Chakra.Web.AppCustomization.QuickLink.QL.UC.Detail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/ACL/Common/UC/ACLDataMethodSearchUC.ascx" TagPrefix="uc1" TagName="ACLDataMethodSearchUC" %>

<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group required">
                        <asp:Label ID="lblRole" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="NameAutoCompleteBox">Name</asp:Label>
                        <div class="col-xs-4">
                            <uc1:ACLDataMethodSearchUC runat="server" id="NameAutoCompleteBox"  DataSourceMethod="GetMenus" AllowSingleSelection="True" />
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblCategory" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtCategory">Category</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtCategory" ReadOnly="true" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click" /></asp:Button>
</div>

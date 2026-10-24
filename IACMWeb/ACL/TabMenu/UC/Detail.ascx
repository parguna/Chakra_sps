<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="DI.Chakra.Web.ACL.TabMenu.UC.Detail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/ACL/Common/UC/ACLDataMethodSearchUC.ascx" TagPrefix="uc1" TagName="ACLDataMethodSearchUC" %>


<div class="content-wrapper"  id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:Label ID="Label1" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtUniqueName">Unique Name</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox runat="server" ID="txtUniqueName" required="required" CssClass="form-control" MaxLength="255"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label3" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtMenuName">Name</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox runat="server" ID="txtMenuName" required="required" CssClass="form-control" MaxLength="255"></asp:TextBox>
                        </div>
                    </div>

                     <div class="form-group">
                         <asp:Label ID="Label2" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="ParentMenuAutoCompleteBox">Parent Menu</asp:Label>
                        <div class="col-xs-4">
                            <uc1:ACLDataMethodSearchUC runat="server" id="ParentMenuAutoCompleteBox"  DataSourceMethod="GetMenus" AllowSingleSelection="True"/>
                        </div>
                    </div>

                    <div class="form-group required">
                        <asp:Label ID="Label4" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="PageAutoCompleteBox">Page</asp:Label>
                        <div class="col-xs-4">
                            <uc1:ACLDataMethodSearchUC runat="server" id="PageAutoCompleteBox"  DataSourceMethod="GetPages" AllowSingleSelection="True"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" TabIndex="8" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click1"></asp:Button>
</div>

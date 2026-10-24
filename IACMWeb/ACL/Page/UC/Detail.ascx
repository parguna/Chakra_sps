<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="DI.Chakra.Web.ACL.Page.UC.Detail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/ACL/Common/UC/ACLDataMethodSearchUC.ascx" TagPrefix="uc1" TagName="ACLDataMethodSearchUC" %>

<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:Label ID="Label1" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtUniqueName">Unique Name</asp:Label>
                        <div class="col-xs-5">
                            <asp:TextBox runat="server" ID="txtUniqueName" required="required" CssClass="form-control" MaxLength="255"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label3" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtName">Name</asp:Label>
                        <div class="col-xs-5">
                            <asp:TextBox runat="server" ID="txtName" required="required" CssClass="form-control" MaxLength="255"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label4" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="PageUrlAutoCompleteBox">Page Url</asp:Label>
                        <div class="col-xs-5">
                            <uc1:ACLDataMethodSearchUC runat="server" id="PageUrlAutoCompleteBox" DataSourceMethod="GetPageURLs" AllowSingleSelection="True" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click"></asp:Button>
</div>
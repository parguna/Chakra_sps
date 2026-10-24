<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="DI.Chakra.Web.ACL.Permission.UC.Detail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:Label ID="Label3" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtPermissionName">Name</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox runat="server" ID="txtPermissionName" ReadOnly="true" required="required" CssClass="form-control" MaxLength="255"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label4" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtIconURL">Icon Url</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox runat="server" ID="txtIconURL" required="required" CssClass="form-control" MaxLength="255"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label1" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtPerClientscript">ClientScript</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox runat="server" ID="txtPerClientscript" CssClass="form-control" TextMode="MultiLine" MaxLength="255"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" TabIndex="8" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click"></asp:Button>
</div>
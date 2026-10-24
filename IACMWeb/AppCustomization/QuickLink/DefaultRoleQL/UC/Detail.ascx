<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="DI.Chakra.Web.AppCustomization.QuickLink.DefaultRoleQL.UC.Detail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:Label ID="lblRole" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="ddlRole">Role</asp:Label>
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlRole" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged" CssClass="form-control selectpicker">
                                <asp:ListItem Value="0">--Select--</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblCategory" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="ddlCategory">Category</asp:Label>
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control selectpicker" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                                <asp:ListItem Value="0">--Select--</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label2" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="ddlName">Name</asp:Label>
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlName" runat="server" DataTextField="Name" DataValueField="Id" CssClass="form-control selectpicker">
                                <asp:ListItem Value="0">--Select--</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                   
                     <div class="form-group">
                        <asp:Label ID="lblPosition" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtPosition">Position</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtPosition" runat="server" pattern="[1-5]{1}" Title="Position 1-5" CssClass="form-control"></asp:TextBox>
                            <asp:RangeValidator ControlToValidate="txtPosition" MinimumValue="1" MaximumValue="5" runat="server"></asp:RangeValidator>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnCancel" Text="Cancel"  CausesValidation="false" UseSubmitBehavior="false" OnClick="btnCancel_Click"></asp:Button>
</div>

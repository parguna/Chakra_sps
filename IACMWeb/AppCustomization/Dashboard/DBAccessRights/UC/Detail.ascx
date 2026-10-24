<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="DI.Chakra.Web.AppCustomization.Dashboard.DBAccessRights.UC.Detail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:Label ID="lblRole" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="ddlRole">Role</asp:Label>
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlRole" runat="server" DataTextField="RoleName" DataValueField="Id" CssClass="form-control selectpicker">
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
                        <asp:Label ID="lblName" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="ddlName">Name</asp:Label>
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlName" runat="server" DataTextField="DashboardName" DataValueField="Id" AutoPostBack="true" OnSelectedIndexChanged="ddlName_SelectedIndexChanged" CssClass="form-control selectpicker">
                                <asp:ListItem Value="0">--Select--</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                   
                     <div class="form-group">
                        <asp:Label ID="lblDBType" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="ddlDBType">Type</asp:Label>
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlDBType" runat="server" Enabled="false" DataTextField="Name" DataValueField="Name" CssClass="form-control selectpicker">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblTargetUrl" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtTargetURL">Target URL</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtTargetURL" ReadOnly="true" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblNavigateURL" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtNavigateURL">Navigate URL</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtNavigateURL" ReadOnly="true"  runat="server"></asp:TextBox>
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
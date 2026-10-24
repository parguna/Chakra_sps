<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EndpointUserUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Special.EndpointUserUC" %>
 <div class="form-group">
        <label class="col-xs-4 control-label">Account Name</label>
        <div class="col-xs-7">
            <asp:DropDownList ID="ddlEndpointUser"  OnSelectedIndexChanged="ddlEndpointUser_SelectedIndexChanged" DataTextField="UserName"  DataValueField="EndpointUserId" runat="server" CssClass="form-control selectpicker" data-width="100%">
            </asp:DropDownList>
        </div>
    </div>
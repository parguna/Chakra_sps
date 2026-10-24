<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewAccountUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.NewAccountUC" %>
 <div class="form-group">
        <label class="col-xs-4 control-label">Account Name</label>
        <div class="col-xs-7">
            <asp:DropDownList ID="ddlEndpointUser"  DataTextField="DisplayName" OnSelectedIndexChanged="ddlEndpointUser_SelectedIndexChanged" DataValueField="AccountName" runat="server" CssClass="form-control selectpicker" data-width="100%">
            </asp:DropDownList>
        </div>
    </div>
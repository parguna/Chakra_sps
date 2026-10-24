<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EndpointUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.EndpointUC" %>
<div class="form-group required" id="ApplicationType" runat="server" visible="false">
    <label class="col-xs-4 control-label"><%= T("App.Common.EndpointType") %></label>
    <div id="DivApplicationTypeDropDown" class="col-xs-7" runat="server">
        <asp:DropDownList ID="ApplicationTypeField"  runat="server" CssClass="form-control selectpicker" data-width="100%" AppendDataBoundItems="true" AutoPostBack="True"
            OnSelectedIndexChanged="ApplicationTypeField_SelectedIndexChanged" DataTextField="DisplayName" DataValueField="Id">
        </asp:DropDownList>
    </div>
</div>

<div class="form-group required" id="Application" runat="server" visible="false">
    <label class="col-xs-4 control-label"><%= T("App.Common.Endpoint") %></label>
    <div id="DivApplicationDropDown" class="col-xs-7" runat="server">
        <asp:DropDownList ID="ApplicationField" runat="server" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="True"
            OnSelectedIndexChanged="ApplicationField_SelectedIndexChanged" DataTextField="DisplayName" DataValueField="Id">
        </asp:DropDownList>
    </div>
    </div>
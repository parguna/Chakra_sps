<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntitlementSpecialAccountUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.EntitlementSpecialAccountUC" %>
<%@ Register Src="~/NACR/Create/App/Special/EndpointUserUC.ascx" TagPrefix="uc1" TagName="EndpointUserUC" %>
<%@ Register Src="~/NACR/Create/App/Special/NewAccountUC.ascx" TagPrefix="uc1" TagName="NewAccountUC" %>

<div class="form-group">
    <div class="col-xs-5">
        <asp:RadioButton ID="newAc" runat="server" Checked="True" GroupName="AcSelect" AutoPostBack="true" OnCheckedChanged="newAc_CheckedChanged" />
        <span class="lbl">Newly Added Account</span>
    </div>
    <div class="col-xs-7">
        <uc1:NewAccountUC runat="server" id="NewAccountUC" Enabled="true"/>
    </div>
</div>

<div class="form-group">
    <div class="col-xs-5">
        <asp:RadioButton ID="existingAc" runat="server" Checked="False" GroupName="AcSelect" AutoPostBack="true" OnCheckedChanged="existingAc_CheckedChanged" />
        <span class="lbl">Existing Account</span>
    </div>
    <div class="col-xs-7">
        <uc1:EndpointUserUC runat="server" ID="EndpointUserUC" Enabled="false" />
    </div>
</div>

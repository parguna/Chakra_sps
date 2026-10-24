<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultAccountInfoUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Special.DefaultAccountInfoUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/GenericAutoCompleteBox.ascx" TagPrefix="uc1" TagName="GenericAutoCompleteBox" %>

<div class="form-group" id="divFirstName" runat="server">
    <label class="col-xs-3 control-label">First Name</label>
    <div class="col-xs-9">
        <asp:TextBox ID="FirstNameTextBox" runat="server" OnTextChanged="FirstNameTextBox_TextChanged" AutoPostBack="true" CssClass="form-control" MaxLength="50" required="required"></asp:TextBox>
    </div>
</div>
<div class="form-group" id="divLastName" runat="server">
    <label class="col-xs-3 control-label">Last Name</label>
    <div class="col-xs-9">
        <asp:TextBox ID="LastNameTextBox" runat="server" OnTextChanged="LastNameTextBox_TextChanged" AutoPostBack="true" CssClass="form-control" MaxLength="50" required="required"></asp:TextBox>
    </div>
</div>
<div class="form-group" id="divDisplayName" runat="server">
    <label class="col-xs-3 control-label">Display Name</label>
    <div class="col-xs-9">
        <asp:TextBox ID="DisplayNameTextBox" runat="server" OnTextChanged="DisplayNameTextBox_TextChanged" AutoPostBack="true" CssClass="form-control" MaxLength="50" required="required"></asp:TextBox>
    </div>
</div>
<div class="form-group">
    <label class="col-xs-3 control-label">Description</label>
    <div class="col-xs-9">
        <asp:TextBox ID="DescriptionTextBox" runat="server" CssClass="form-control" MaxLength="255" TextMode="MultiLine" Rows="3" required="required"></asp:TextBox>
    </div>
</div>
<div class="form-group" id="divDepartment" runat="server">
    <label class="col-xs-3 control-label required">Department</label>
    <div class="col-xs-9">
        <uc1:GenericAutoCompleteBox runat="server" ID="DepartmentAutoCompleteBox" ServiceMethodName="GetDepartMents" ServicePath="~/Services/AutoCompleteDataService.svc" />
    </div>
</div>
<div class="form-group">
    <label class="col-xs-3 control-label">Account Name</label>
    <div class="col-xs-9">
        <asp:TextBox ID="AccountNameTextBox" runat="server" CssClass="form-control" MaxLength="20" required="required"></asp:TextBox>
    </div>
</div>


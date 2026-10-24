<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccountDetailUC.ascx.cs" Inherits="DI.Chakra.Web.Account.UC.DetailUC" %>
<%@ Register Src="~/UsrControl/Common/AccountDetailsUC.ascx" TagPrefix="uc1" TagName="AccountDetailsUC" %>

<link href="../../assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
<link href="../../assets/styles/stylesheet.css" rel="stylesheet" />
<link href="../../assets/styles/Window.css" rel="stylesheet" />
<link href="../../IACMSkin/Window.IACMSkin.css" rel="stylesheet" />

<div class="modal-header">
    <h3 class="modal-title" id="H1">Editing record</h3>
</div>
<div class="modal-body">
    <uc1:AccountDetailsUC runat="server" ID="AccountDetailsUC" />
</div>
<div class="modal-footer">
    <asp:Button runat="server" ID="btnOk" class="btn btn-primary transparent-button" Text="Ok" OnClick="btnOk_Click" />
</div>

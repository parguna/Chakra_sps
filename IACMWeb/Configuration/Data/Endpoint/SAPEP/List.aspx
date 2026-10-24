<%@ Page Title="SAPEP" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true"  CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Configuration.Data.XLUpload.SAPEP.List" %>

<%@ Register Src="~/Configuration/Data/Endpoint/SAPEP/UC/ListUC.ascx" TagPrefix="ListUC" TagName="ListUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <ListUC:ListUC runat="server" id="ListUC" />

    <div class="buttons text-right nav-rectangle">
        <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btnCancel_Click" />
    </div>

</asp:Content>
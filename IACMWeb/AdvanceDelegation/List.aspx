<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" Title="Delegation List" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.AdvanceDelegation.List" %>
<%@ Register Src="~/AdvanceDelegation/UC/ListUC.ascx" TagPrefix="uc1" TagName="ListUC" %>
<asp:Content ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:ListUC runat="server" id="ListUC" />

            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
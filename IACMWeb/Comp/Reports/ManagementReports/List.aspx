<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.Comp.Reports.ManagementReports.List" %>

<%@ Register Src="~/Comp/Reports/ManagementReports/UC/ListUC.ascx" TagPrefix="uc1" TagName="ListUC" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:ListUC runat="server" ID="ListUC" />

            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Job.Attestation.List" %>

<%@ Register Src="~/Job/Attestation/UC/ListUC.ascx" TagPrefix="uc1" TagName="ListUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:ListUC runat="server" ID="ListUC" />

            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>        
               <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-default btn-sm" Text="Refresh" OnClick="btnRefresh_Click"/>
            </div>        

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
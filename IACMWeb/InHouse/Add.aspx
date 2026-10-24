<%@ Page Title="InHouse User Add" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.InHouse.Add" %>
<%@ Register Src="~/InHouse/UC/Detail.ascx" TagPrefix="DetailUC" TagName="DetailUCControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <DetailUC:DetailUCControl runat="server" ID="DetailUC" UIMode="Add"  />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.DataManagement.SkypeForBusiness.NumberRange.List" %>
<%@ Register Src="~/DataManagement/SkypeForBusiness/NumberRange/UC/ListUC.ascx" TagPrefix="ListUC" TagName="ListUCControl" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">
     <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
              <ListUC:ListUCControl runat="server" ID="ListUC1"/>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


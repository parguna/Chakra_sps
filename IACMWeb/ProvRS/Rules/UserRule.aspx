<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="UserRule.aspx.cs" Inherits="DI.Chakra.Web.ProvRS.Rules.UserRule" %>

<%@ Register Src="~/ProvRS/Rules/UC/UserRuleUC.ascx" TagPrefix="uc1" TagName="UserRuleUC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
     <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:UserRuleUC runat="server" id="UserRuleUC" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

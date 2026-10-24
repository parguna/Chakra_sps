<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.UserExclusion.Add" %>

<%@ Register Src="~/UserExclusion/UC/AddUC.ascx" TagPrefix="uc1" TagName="AddUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:AddUC runat="server" id="AddUC" />
            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel"  PostBackUrl="~/UserExclusion/List.aspx"></asp:Button>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
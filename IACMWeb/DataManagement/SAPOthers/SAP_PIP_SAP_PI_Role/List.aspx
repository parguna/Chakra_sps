<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.DataManagement.SAP_PIP_SAP_PI_Role.List" %>

<%@ Register Src="~/DataManagement/SAPOthers/SAP_PIP_SAP_PI_Role/UC/ListUC.ascx" TagPrefix="ListUC" TagName="ListUCControl" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <ListUC:ListUCControl runat="server" ID="ListUC1"/>

            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" ID="Button1" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


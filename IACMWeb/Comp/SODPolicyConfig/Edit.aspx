<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.Comp.SODPolicyConfig.UC.Edit" %>

<%@ Register Src="~/Comp/SODPolicyConfig/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:DetailUC runat="server" id="DetailUC1" UIMode="Edit"  />

</asp:Content> 
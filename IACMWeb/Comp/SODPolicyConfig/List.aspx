<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.Comp.SODPolicyConfig.List" %>

<%@ Register Src="~/Comp/SODPolicyConfig/UC/ListUC.ascx" TagPrefix="uc1" TagName="ListUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
<uc1:ListUC runat="server" id="ListUC1"  />
    </asp:Content> 
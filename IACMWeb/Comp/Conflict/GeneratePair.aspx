<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="GeneratePair.aspx.cs" Inherits="DI.Chakra.Web.Comp.Conflict.GeneratePair" %>

<%@ Register Src="~/Comp/Conflict/Common/ConflictPairsUC.ascx" TagPrefix="uc1" TagName="ConflictPairsUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:ConflictPairsUC runat="server" id="ConflictPairs" />
</asp:Content>

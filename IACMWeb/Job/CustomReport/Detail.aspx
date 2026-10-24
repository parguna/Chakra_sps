<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Job.CustomReport.Detail" %>
<%@ Register Src="~/Job/CustomReport/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:DetailUC runat="server" id="DetailUC" UIMode ="View"/>

</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.Job.CustomReport.Edit" %>


<%@ Register Src="~/Job/CustomReport/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:DetailUC runat="server" id="DetailUC" UIMode ="Edit"/>

</asp:Content>


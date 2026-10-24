<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RiskCodeUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.RiskCodeUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadAutoCompleteBox ID="RiskCodeAutobox" OnClientEntryAdding="OnClientEntryAddingHandler" runat="server" EmptyMessage="Enter * to view full list"  CssClass="form-control" InputType="Token">
    <TextSettings SelectionMode="Single" />
    <WebServiceSettings Method="GetAllRiskCodes" Path="../../Services/AutoCompleteDataService.svc"/>
</telerik:RadAutoCompleteBox>



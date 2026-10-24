<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RiskScoreSearchUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.RiskScoreSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadAutoCompleteBox ID="RiskAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
    <TextSettings SelectionMode="Single" />
    <WebServiceSettings Method="GetAllRiskScore" Path="../../Services/AutoCompleteDataService.svc" />
</telerik:RadAutoCompleteBox>
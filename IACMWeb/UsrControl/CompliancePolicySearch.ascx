<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompliancePolicySearch.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.CompliancePolicySearch" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadAutoCompleteBox ID="PolicyAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token" >
    <TextSettings SelectionMode="Single" />
    <WebServiceSettings Method="GetCompliancePolicy" Path="../../Services/AutoCompleteDataService.svc" />
</telerik:RadAutoCompleteBox>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReviewApprUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Mitigation.UC.ReviewApprUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadAutoCompleteBox ID="ApproverReviwer" required="required" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
    <TextSettings SelectionMode="Single" />
    <WebServiceSettings Path="../../../Services/AutoCompleteDataService.svc" />
</telerik:RadAutoCompleteBox>



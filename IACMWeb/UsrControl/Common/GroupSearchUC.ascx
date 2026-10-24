<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupSearchUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.GroupSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadAutoCompleteBox ID="GroupAutoCompleteBox" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
    <TextSettings SelectionMode="Single" />
    <WebServiceSettings Method="GetAllGroups" Path="../../Services/AutoCompleteDataService.svc" />
</telerik:RadAutoCompleteBox>
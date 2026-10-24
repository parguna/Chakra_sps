<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ACLDataMethodSearchUC.ascx.cs" Inherits="DI.Chakra.Web.ACL.Common.UC.ACLDataMethodSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadAutoCompleteBox ID="ACLMethodAutoCompleteBox" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token"  OnEntryAdded="ACLMethodAutoCompleteBox_EntryAdded"
    OnEntryRemoved="ACLMethodAutoCompleteBox_EntryRemoved" >
    <TextSettings SelectionMode="Single" />
    <WebServiceSettings Path="~/Services/AutoCompleteDataService.svc" />
</telerik:RadAutoCompleteBox>


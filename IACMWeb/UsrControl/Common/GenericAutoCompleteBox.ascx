<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GenericAutoCompleteBox.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.GenericAutoCompleteBox" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadAutoCompleteBox ID="AutoCompleteBoxCompany" OnClientEntryAdding="OnClientEntryAddingHandler" OnEntryAdded="AutoCompleteBox_EntryAdded" OnEntryRemoved="AutoCompleteBox_EntryRemoved" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
    <TextSettings SelectionMode="Single" />
</telerik:RadAutoCompleteBox>
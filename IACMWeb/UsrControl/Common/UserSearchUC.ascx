<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserSearchUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.UserSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script>
    function OnClientDropDownOpening(sender, eventArgs) {
        if (sender.get_entries().get_count() > 0) {
            sender._postBackOnAdd = false;
            eventArgs.set_cancel(true);
        }
        else
            sender._postBackOnAdd = true;
    }

</script>
<asp:UpdatePanel ID="UpdateOnSelect" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:HiddenField ID="ConsiderJoindDate" runat="server" />
        <telerik:RadAutoCompleteBox ID="UserNameAutoCompleteBox" OnEntryRemoved="UserNameAutoCompleteBox_EntryRemoved" OnClientDropDownOpening="OnClientDropDownOpening" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
            InputType="Token" OnEntryAdded="UserNameAutoCompleteBox_EntryAdded">
            <TextSettings SelectionMode="Single" />
            <WebServiceSettings Method="GetUsers" Path="~/Services/AutoCompleteDataService.svc" />

        </telerik:RadAutoCompleteBox>
    </ContentTemplate>
</asp:UpdatePanel>

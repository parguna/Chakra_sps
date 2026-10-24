<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserRuleUC.ascx.cs" Inherits="DI.Chakra.Web.ProvRS.Rules.UC.UserRuleUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script type="text/javascript">

    function OnClientRequesting(sender, eventArgs) {
        var hiddenVal = $("#" + '<%= HiddenValue.ClientID %>').val();
        var context = eventArgs.get_context();
        context["hidden"] = hiddenVal;

    }

</script>

<div class="content-wrapper">
    <div class="form">
        <asp:HiddenField ID="HiddenValue" runat="server" Value="true" />
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group required">
                        <asp:Label ID="Label3" runat="server" CssClass="col-xs-1 control-label" AssociatedControlID="UserNameAutoCompleteBox">User Name</asp:Label>
                        <div class="col-xs-4">
                            <telerik:RadAutoCompleteBox ID="UserNameAutoCompleteBox" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
                                InputType="Token" AutoPostBack="true" OnEntryAdded="UserNameAutoCompleteBox_EntryAdded" OnClientRequesting="OnClientRequesting" OnEntryRemoved="UserNameAutoCompleteBox_EntryRemoved">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Path="~/Services/AutoCompleteDataService.svc" Method="GetUsers" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                        <div class="col-xs-4">
                            <asp:Button ID="btnFind" ValidationGroup="Find" runat="server" Text="Find"
                            OnClick="btnFind_Click" CssClass="btn btn-default btn-sm" />
                        </div>
                    </div>
                    <div class="form-group">
                        <cc1:IACMRadGridControl ID="NUserRuleGrid" runat="server" OnNeedDataSource="NUserRuleGrid_NeedDataSource">
                            <MasterTableView DataKeyNames="Id,RuleId" TableLayout="Fixed"  Width="100%">
                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                <Columns>
                                    <telerik:GridBoundColumn DataField="RuleId" HeaderText="Rule Id" SortExpression="RuleId" UniqueName="RuleId" FilterControlWidth="75%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Code" HeaderText="Code" SortExpression="Code" UniqueName="Code" FilterControlWidth="75%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Description" HeaderText="Description" SortExpression="Description" UniqueName="Description" FilterControlWidth="75%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Value" HeaderText="Value" SortExpression="Value" UniqueName="Value" FilterControlWidth="75%">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </cc1:IACMRadGridControl>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button ID="Button2" runat="server" CssClass="btn btn-default btn-sm cancel" Text="Cancel" PostBackUrl="~/ProvRS/Rules/List.aspx" />
</div>


<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RulesetSearchUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.RulesetSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/NOrgDetail.ascx" TagPrefix="uc1" TagName="NOrgDetail" %>
<script>
    function OnClientEntryAddingHandler(sender, eventArgs) {
        if (sender.get_entries().get_count() > 0) {

            eventArgs.set_cancel(true);
        }

    }
    function Show() {
        $find("<%= UserInfo.ClientID %>").show();
        return false;
    }
</script>

<div class="row-fluid">
    <div class="col-xs-12">
        <div class="form-group">
            <div class="col-xs-2">
                <telerik:RadAutoCompleteBox ID="RulesetAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" ToolTip="Enter * to view full list" runat="server" CssClass="form-control"
                    InputType="Token" OnEntryAdded="RulesetAutoCompleteBox_EntryAdded">
                    <TextSettings SelectionMode="Single" />
                    <WebServiceSettings Method="GetNRulesets" Path="../../Services/AutoCompleteDataService.svc" />
                </telerik:RadAutoCompleteBox>
            </div>
            <div class="col-xs-2">
                <asp:LinkButton ID="LinkButtonRuleset" OnClientClick="return Show();" Text="" runat="server"></asp:LinkButton>
            </div>
        </div>
    </div>
</div>

<telerik:RadWindow runat="server" ID="UserInfo" Modal="true" Width="500px" Height="100%" Title="Ruleset Info"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title" id="lblUserName" runat="server">Ruleset Info</h3>
                </div>
                <div class="modal-body">
                    <asp:Repeater ID="RulesetRepeater" runat="server">
                        <ItemTemplate>
                            <span><%# Eval("TypeId") %> - <%# Eval("Operator") %> - <%# Eval("Value") %></span>
                            <br />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnOk" runat="server" CssClass="btn btn-primary bordered-button" data-dismiss="modal" Text="Ok" Width="60px" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

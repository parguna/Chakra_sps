<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddCartSpeicalAcUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.AddCartSpeicalAcUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/NACR/Create/App/Special/ADAccountInfoUC.ascx" TagPrefix="uc1" TagName="ADAccountInfoUC" %>
<%@ Register Src="~/NACR/Create/App/Special/EntitlementSpecialAccountUC.ascx" TagPrefix="uc1" TagName="EntitlementSpecialAccountUC" %>
<li id="liDropDown" runat="server" class="dropdown">
    <a href="#" class="dropdown-toggle box-toggle" data-toggle="dropdown" role="button" aria-expanded="false"><span class="caret"></span></a>
    <ul class="dropdown-menu" role="menu">
        <asp:Repeater ID="RequestRepeater" runat="server">
            <ItemTemplate>
                <li id="li2" runat="server">
                    <asp:Button CssClass="splCartButton" ID="lnkSecondary" Text='<%# Eval("DisplayName") %>' CommandName='<%# Eval("CommandName") %>' OnClick="lnkAddItem_Click" runat="server" OnClientClick="$('#UpdateProgressBar').show();" /></li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</li>

<%--<asp:UpdateProgress ID="UpdateProgress1" runat="Server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="1">
    <ProgressTemplate>
        <asp:Label ID="lblwait" runat="server" Text="Please wait.."></asp:Label>
    </ProgressTemplate>
</asp:UpdateProgress>--%>

<telerik:RadWindow runat="server" ID="AccountFormWindow" Modal="true" Width="550px" Height="100%" Title="Account Detail"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <%-- <asp:UpdatePanel-- ID="UpdatePanel1" runat="server" UpdateMode="Conditional" DisplayAfter="1">--%>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <%-- <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Ok" EventName="Click" />
            </Triggers>--%>
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title" id="headerText" runat="server">Account Detail</h3>
                    </div>
                    <div class="modal-body">
                        <div class="form-horizontal left clearfix">
                            <div class="row-fluid">
                                <div class="col-xs-12">
                                    <uc1:ADAccountInfoUC runat="server" ID="ADAccountInfoUC" Visible="false" />
                                    <uc1:EntitlementSpecialAccountUC runat="server" ID="EntitlementSpecialAccountUC" Visible="false" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="Cancel" OnClick="btnCancel_Click" />
                        <asp:Button runat="server" ID="Ok" class="btn btn-primary bordered-button" Text="OK" OnClick="Ok_Click" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

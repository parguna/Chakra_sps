<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrgChartUC.ascx.cs" Inherits="DI.Chakra.Web.OrgChart.UC.OrgChartUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/UsrControl/AccessDetailUC.ascx" TagPrefix="uc1" TagName="AccessDetailUC" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <telerik:RadOrgChart RenderMode="Lightweight" runat="server" ID="RadOrgChart1"
            EnableCollapsing="true" EnableGroupCollapsing="true" OnGroupItemDataBound="RadOrgChart1_GroupItemDataBound">

            <ItemTemplate>
                <asp:Panel runat="server" ID="Panel1">
                    <asp:Image ID="OrgItemImg" runat="server" class="orgitem-image" ImageUrl='<%#GetImage(Eval("Image"))%>' Width="65" Height="75" />
                    <asp:Label ID="test" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'></asp:Label>
                    </br>
                    <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Designation") %>'></asp:Label>
                    </br>
                    <asp:LinkButton ID="lnkBtnAccInfo" runat="server" CausesValidation="false" OnClick="lnkBtnAccInfo_Click" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserId") %>'>Account Info</asp:LinkButton>
                </asp:Panel>
            </ItemTemplate>
        </telerik:RadOrgChart>

        <div class="buttons text-right nav-rectangle">
            <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click"></asp:Button>
        </div>

        <telerik:RadWindow runat="server" ID="RadWinAccessDetails" Modal="true" Width="800px" Height="100%"
            RenderMode="Lightweight" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title"><%= T("App.Common.AccountDetails") %></h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:AccessDetailUC runat="server" ID="AccessDetailUC" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnOK" class="btn btn-primary bordered-button" Text="OK" OnClick="btnOK_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </ContentTemplate>
</asp:UpdatePanel>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestRiskUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.RequestRiskUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/UserSODConflicts.ascx" TagPrefix="uc1" TagName="UserSODConflicts" %>

<div>
    <div class="form-group">
        <label class="col-xs-4 orange">Request has conflict:</label>
        <div class="col-xs-7">
            <asp:Label ID="lblHasConflicts" runat="server" class="description orange"></asp:Label>
        </div>
    </div>
    <div id="RiskInfo" runat ="server" visible ="false">
        <div class="form-group">
            <label class="col-xs-4 orange">Conflicts:</label>
            <div class="col-xs-7">
                <asp:Label ID="lblConflictsCount" runat="server" class="description orange"></asp:Label>
            </div>
        </div>
        <div class="form-group">
            <label class="col-xs-4 orange">Sensitive Role:</label>
            <div class="col-xs-7">
                <asp:Label ID="lblSACount" runat="server" class="description orange"></asp:Label>
            </div>
        </div>
        <div class="form-group">
            <label class="col-xs-4 orange">Show Risk Info:</label>
            <div class="col-xs-7">
                <asp:LinkButton ID="MoreLinkButton" class="description red" Text="Risk Info" runat="server" OnClick="MoreLinkButton_Click" />
            </div>
        </div>
    </div>
</div>
<telerik:RadWindow runat="server" ID="SODConflictWindow" Modal="true" Height="100%" Width="700px" Title="Approver Chain"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Risk Information</h3>
                </div>
                <div class="modal-body">
                    <uc1:UserSODConflicts runat="server" ID="UserSODConflicts" />
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>


<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserRiskUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.UserRiskUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/UserSODConflicts.ascx" TagPrefix="uc1" TagName="UserSODConflicts" %>

    <asp:LinkButton ID="lnkShowRisk" runat="server" class="description red" Text="Show Risk Info" OnClick="lnkShowRisk_Click" ></asp:LinkButton>

<telerik:RadWindow runat="server" ID="SensitiveRoleDetails" Modal="true" Width="700px" Height="100%" Title="Account Detail"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Risk Information</h3>
                </div>
                <div class="modal-body">
                    <div class="row-fluid">
                        <uc1:UserSODConflicts runat="server" ID="UserSODConflict" />
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnSensitiveRoleCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnSensitiveRoleCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

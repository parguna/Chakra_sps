<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCurrentAccessUC.ascx.cs" Inherits="DI.Chakra.Web.AccessRequest.Common.ViewCurrentAccessUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/CurrentAccessUC.ascx" TagPrefix="uc1" TagName="CurrentAccessUC" %>
<script>
    function ShowCurrentAccessUC() {
        $find("<%= CurrentAccess.ClientID %>").show();
        return false;
    }
</script>

<asp:LinkButton ID="lnkViewCurrentAccess" OnClientClick="return ShowCurrentAccessUC();" Text="View Current Access" runat="server"></asp:LinkButton>

<telerik:RadWindow runat="server" ID="CurrentAccess" Modal="true" Height="100%" Title="Current Access"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Current Access</h3>
                </div>
                <div class="modal-body">
                    <div class="row-fluid">
                        <uc1:CurrentAccessUC runat="server" ID="CurrentAccessUC" IsShowIcon="false" />
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click"/>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>



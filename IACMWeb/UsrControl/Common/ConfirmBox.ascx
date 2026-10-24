<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmBox.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.ConfirmBox" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<style>
    .rad-window-message-body {
        height: 75px !important;
        overflow-y: unset !important;
    }

    .error {
        color: #a92727 !important;
    }

        .error .modal-header {
            border-bottom: 1px solid #a92727 !important;
        }

        .error .modal-footer {
            background-color: #a92727 !important;
        }

        .error h3 {
            color: #a92727 !important;
        }

        .error button {
            background: #a92727;
        }
</style>
<asp:HiddenField ID="HiddenField" runat="server" />

<asp:UpdatePanel runat="server" ID="IDMessageUpdatePanel" UpdateMode="Always">
    <ContentTemplate>
        <telerik:RadWindow runat="server" ID="IDMesssage" Modal="true" Width="400px" Height="200px" Title="Message"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                    <ContentTemplate>

                        <div class="modal-header">
                            <h3 class="modal-title" id="myModalLabel" runat="server" style="text-align: left"></h3>
                        </div>
                        <div class="modal-body rad-window-message-body">
                            <asp:Label ID="idMessageContent" runat="server"></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnYes" UseSubmitBehavior="false" runat="server" data-dismiss="modal" CssClass="btn btn-primary bordered-button" Text="Yes" Width="60px" CausesValidation="false" OnClick="btnYes_Click" />
                            <asp:Button ID="btnNo" UseSubmitBehavior="false" runat="server" data-dismiss="modal" CssClass="btn btn-primary bordered-button" Text="No" Width="60px" CausesValidation="false" OnClick="btnNo_Click" />
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </ContentTemplate>
</asp:UpdatePanel>

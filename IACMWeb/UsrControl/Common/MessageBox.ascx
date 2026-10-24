<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MessageBox.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.MessageBox" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function openModal() {
        $('#myCartModal').modal('show');
    }
</script>
<style>
    .rad-window-message-body {
        height: 75px !important;
        overflow-y: unset !important;
    }

    .success {
        color: #317659 !important;
    }

        .success .modal-header {
            border-bottom: 1px solid #317659 !important;
        }

        .success .modal-footer {
            background-color: #317659 !important;
        }

        .success h3 {
            color: #317659 !important;
        }

        .success button {
            background: #317659;
        }

    .warning {
        color: #c6642a !important;
    }

        .warning .modal-header {
            border-bottom: 1px solid #c6642a !important;
        }

        .warning .modal-footer {
            background-color: #c6642a !important;
        }

        .warning h3 {
            color: #c6642a !important;
        }

        .warning button {
            background: #c6642a;
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
                            <asp:Button ID="btnOk" UseSubmitBehavior="false" runat="server" data-dismiss="modal" CssClass="btn btn-primary bordered-button" Text="OK" Width="60px" CausesValidation="false" OnClick="btnOk_Click" />
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </ContentTemplate>
</asp:UpdatePanel>

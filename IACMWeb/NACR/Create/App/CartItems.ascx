<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartItems.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.CartItems" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/Common/CartItemUC.ascx" TagPrefix="uc1" TagName="CartItemUC" %>

<script type="text/javascript">
    function openModal() {
        $('#myCartModal').modal('show');
    }
</script>

<asp:HiddenField ID="HiddenField" runat="server" />

<asp:UpdatePanel runat="server" ID="UpdateCartItems" UpdateMode="Always"><ContentTemplate>
<telerik:RadWindow runat="server" ID="CartItem" Modal="true" Width="900px" Height="100%" Title="Cart Items"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
            <ContentTemplate>

                <div class="modal-header">
                    <h3 class="modal-title" id="myModalLabel">Cart Items</h3>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal left clearfix">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <div class="form-group">
                                    <uc1:CartItemUC runat="server" ID="CartItemUC" DisplayDelete="False" />
                                </div>

                            </div>
                        </div>
                    </div>

                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnOk" runat="server" data-dismiss="modal" CssClass="btn btn-primary bordered-button" Text="OK" Width="60px" OnClick="btnOk_Click" />
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
</ContentTemplate></asp:UpdatePanel>
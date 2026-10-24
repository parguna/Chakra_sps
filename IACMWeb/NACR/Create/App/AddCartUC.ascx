<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddCartUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.AddCartUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/NACR/Create/App/Special/StandingOrder.ascx" TagPrefix="uc1" TagName="StandingOrder" %> 


<asp:Button ID="lnkAddItem" CssClass="cartButton" Text="Add Item" runat="server" OnClick="lnkAddItem_Click" OnClientClick="$('#UpdateProgressBar').show();" />
<telerik:RadWindow runat="server" ID="SOFormWindow" Modal="true" Width="550px" Height="100%" Title="Standing Order"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header" runat="server">
                    <h3 class="modal-title" id="titleID" runat="server">Standing Order</h3>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal left clearfix">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <uc1:StandingOrder runat="server" ID="StandingOrder" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="Cancel" OnClick ="btnCancel_Click" />
                    <asp:Button runat="server" ID="Ok" class="btn btn-primary bordered-button" Text="OK" OnClick ="Ok_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
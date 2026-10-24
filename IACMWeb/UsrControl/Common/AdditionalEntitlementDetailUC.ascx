<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdditionalEntitlementDetailUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.AdditionalEntitlementDetailUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

        <asp:LinkButton ID="lnkAdditionalInfo" Visible="false" OnClick="lnkAdditionalInfo_Click" runat="server" CssClass="text-indent">Additional Info</asp:LinkButton>


        <telerik:RadWindow runat="server" ID="AdditionalInfoWindow" Modal="true" Width="500px" Height="100%" Title="Approver Detail"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title">
                                <asp:Label runat="server" ID="lblAdditionInfoHeader">Additional Info</asp:Label></h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <asp:Repeater ID="AdditionalInfoRepeater" runat="server">
                                    <ItemTemplate>
                                        <div class="form-group">
                                            <label><%# Eval("Name") %></label>
                                            :
                                            <asp:Label ID="LabelValue" runat="server" class="description"><%# Eval("Value") %></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnOk" class="btn btn-primary bordered-button" Text="OK" OnClick="btnOk_Click" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="lnkAdditionalInfo" />
                    </Triggers>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

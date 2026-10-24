<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentRowUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.UC.AttachmentRowUC" %>

<div class="row">
    <div class="col-xs-4 name">
        <asp:Label ID="lblAttachedBy" runat="server" Text='<%# Eval("CreatedBy") %>' /></div>
    <div class="col-xs-3 date">
        <asp:Label ID="lblAttachedOn" runat="server" Text='<%# Eval("CreatedDate") %>' /></div>
    <div class="col-xs-0 date">
        <asp:HiddenField ID="lblAttachmentId" runat="server" Value='<%# Eval("Id") %>' />
    </div>
    <div class="col-xs-5 attachment-info">
            <asp:UpdatePanel runat="server" ID="UpdatePanelDownload" UpdateMode="Conditional" ChildrenAsTriggers="True" OnUnload="UpdatePanelDownload_Unload">
                <ContentTemplate>
                    <%--<span class="action-attach"></span>--%>
                    <asp:LinkButton ID="lblFileName" runat="server" OnClick="lblFileName_Click" Text='<%# Eval("FileName") %>' CommandName='<%# Eval("Id") %>' />
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="lblFileName" />
                </Triggers>
            </asp:UpdatePanel>
    </div>
</div>

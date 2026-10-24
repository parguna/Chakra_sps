<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationDetailUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.UC.AttestationDetailUC" %>

<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12">
            <div class="form-group">
                <label class="col-xs-4"><%= T("App.Attestation.Name") %></label>
                <div class="col-xs-8">
                    <asp:Label ID="lblAttName" runat="server" class="description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4"><%= T("App.Attestation.Description") %></label>
                <div class="col-xs-8">
                    <asp:Label ID="lblAttestationDesc" runat="server" class="description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4"><%= T("App.Attestation.Type") %></label>
                <div class="col-xs-8">
                    <asp:Label ID="lblType" runat="server" class="description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4"><%= T("App.Campaign.Name") %></label>
                <div class="col-xs-8">
                    <asp:Label ID="lblCampaignName" runat="server" class="description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4"><%= T("App.Common.RequestDate") %></label>
                <div class="col-xs-8">
                    <asp:Label ID="lblReqDate" runat="server" class="description"></asp:Label>
                </div>
            </div>
             <div class="form-group">
                <label class="col-xs-4"><%= T("App.Attestation.CurrentReviewer") %></label>
                <div class="col-xs-8">
                    <asp:Label ID="lblCurrentReviewer" runat="server" class="description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4"><%= T("App.Attestation.CurrentLevel") %></label>
                <div class="col-xs-8">
                    <asp:Label ID="lblCurrentLevel" runat="server" class="description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4"><%= T("App.Attestation.SubmitBefore") %></label>
                <div class="col-xs-8">
                    <asp:Label ID="lblSubmitBefore" runat="server" class="description"></asp:Label>
                </div>
            </div>
        </div>
    </div>
</div>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationCampaignDetailUC.ascx.cs" Inherits="DI.Chakra.Web.Report.Attestation.UC.AttestationCampaignDetailUC" %>

<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12 mBottom15">

            <div class="form-group">
                <label class="col-xs-4">Default Review Status:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblStatus" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>

            <div class="form-group">
                <label class="col-xs-4">Requires Remediation:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblRemediation" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            
        </div>
    </div>
</div>

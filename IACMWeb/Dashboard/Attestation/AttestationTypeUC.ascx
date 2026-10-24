<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationTypeUC.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.Attestation.AttestationTypeUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <div class="col-xs-5">
                            <asp:Label runat="server" ID="lblEndpoint" Text="Application" CssClass="col-xs-5 control-label" AssociatedControlID="ddlAttestationType"></asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="ddlAttestationType" runat="server" AutoPostBack="true" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlAttestationType_SelectedIndexChanged" data-width="100%"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-7">
                            <asp:Label runat="server" ID="lblAttestation" Text="Campaign" CssClass="col-xs-3 control-label" AssociatedControlID="ddlAttestation"></asp:Label>
                            <div class="col-xs-9">
                                <asp:DropDownList ID="ddlAttestation" runat="server" AutoPostBack="True" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlAttestation_SelectedIndexChanged" data-width="100%"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

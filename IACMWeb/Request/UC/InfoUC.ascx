<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InfoUC.ascx.cs" Inherits="DI.Chakra.Web.Request.UC.InfoUC" %>

<%@ Register Src="~/UsrControl/Common/RequestedEntitlementUC.ascx" TagPrefix="uc1" TagName="RequestedEntitlementUC" %>

<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <label class="col-xs-4"><%= T("App.Request.AccountType") %></label>
                        <div class="col-xs-8">
                            <asp:Label ID="AccountTypeLabel" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4"><%= T("App.Request.Entitlement") %></label>
                        <div class="col-xs-8">
                            <uc1:RequestedEntitlementUC runat="server" ID="RequestedEntitlementUC" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4"><%= T("App.Request.RequestFor") %></label>
                        <div class="col-xs-8">
                            <asp:Label ID="RequestForLabel" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4"><%= T("App.Request.RequestBy") %></label>
                        <div class="col-xs-8">
                            <asp:Label ID="RequestByLabel" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4"><%= T("App.Request.ValidFrom") %></label>
                        <div class="col-xs-5">
                            <asp:Label ID="ValidFromLabel" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4"><%= T("App.Request.ValidTo") %></label>
                        <div class="col-xs-5">
                            <asp:Label ID="ValidToLabel" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4"><%= T("App.Request.ExecutionDate") %></label>
                        <div class="col-xs-8">
                            <asp:Label ID="ExecutionDateLabel" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4"><%= T("App.Request.Status") %></label>
                        <div class="col-xs-8">
                            <asp:Label ID="StatusLabel" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
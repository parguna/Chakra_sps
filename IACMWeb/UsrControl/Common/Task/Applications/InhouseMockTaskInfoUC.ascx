<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InhouseMockTaskInfoUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.Task.Applications.InhouseMockTaskInfoUC" %>

<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12">
            <div class="col-xs-4">
                <div class="form-group">
                    <label class="col-xs-4"><%= T("App.Common.Mock.S.Number") %></label>
                    <div class="col-xs-7">
                        <asp:Label ID="lblNumber" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4"><%= T("App.Common.Mock.S.Status") %></label>
                    <div class="col-xs-7">
                        <asp:Label ID="lblReferenceNumber" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                 <div class="form-group">
                    <label class="col-xs-4"><%= T("App.Common.Mock.S.ApprovedDate") %></label>
                    <div class="col-xs-7">
                        <asp:Label ID="lblApprovedDate" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4"><%= T("App.Common.Mock.Endpoint") %></label>
                    <div class="col-xs-7">
                        <asp:Label ID="lblForEndpoint" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4"><%= T("App.Common.Mock.Instruction") %></label>
                    <div class="col-xs-7">
                        <asp:Label ID="lblInstruction" runat="server" class="description"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

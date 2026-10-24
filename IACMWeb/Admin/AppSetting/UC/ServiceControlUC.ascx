<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServiceControlUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.AppSetting.UC.ServiceControlUC" %>
<div class="col-xs-12">

        <div class="form-group">
            <asp:Label ID="lblTitle" AssociatedControlID="txtSvc" runat="server" CssClass="col-xs-4 control-label" Text=""></asp:Label>
            <div class="col-xs-3">
                <asp:TextBox ID="txtSvc" runat="server" CssClass="form-control numeric" ReadOnly="true" Text=""></asp:TextBox>
            </div>
            <div class="col-xs-3">
                <asp:LinkButton ID="lnkSvcStart" runat="server" Text="Not Installed" OnClick="lnkSvcStart_Click"></asp:LinkButton>
            </div>     
             <div class="col-xs-2">
                <asp:LinkButton ID="lnkSvcRestart" runat="server" Text="Restart" OnClick="lnkSvcRestart_Click"></asp:LinkButton>
            </div>        
        </div>

    </div>
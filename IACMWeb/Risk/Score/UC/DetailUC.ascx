<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Risk.Score.UC.DetailUC" %>
<div class="form-horizontal left clearfix">
    <div class="row-fluid">

         <div class="col-xs-12">
        <div class="form-group">
            <asp:Label ID="lblRiskScore" AssociatedControlID="txtRiskScore" required="required" runat="server" Text="Risk Score" class="col-xs-5 control-label"></asp:Label>
            <div class="col-xs-7">
                <asp:TextBox ID="txtRiskScore" class="form-control" Rows="3" runat="server" onKeyPress="return allowNumerics(event)"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <asp:Label ID="lblRiskScoreDesc" AssociatedControlID="txtRiskScoreDescription" runat="server" Text="Risk Score Description" class="col-xs-5 control-label"></asp:Label>
            <div class="col-xs-7">
                <asp:TextBox ID="txtRiskScoreDescription" class="form-control" Rows="3" runat="server"></asp:TextBox>
            </div>
        </div>
        </div>

    </div>
</div>

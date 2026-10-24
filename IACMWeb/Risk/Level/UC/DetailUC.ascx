<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Risk.Level.UC.DetailUC" %>
<div class="form-horizontal left clearfix">
    <div class="row-fluid">

      <div class="col-xs-12">
            <div class="form-group">
                <asp:Label ID="lblRiskLevel" AssociatedControlID="txtRiskLevelCode" required="required" runat="server" Text="Risk Level Code" class="col-xs-5 control-label"></asp:Label>
                <div class="col-xs-7">
                    <asp:TextBox ID="txtRiskLevelCode" class="form-control" Rows="3" runat="server" onKeyPress="return AllowLetterNumber(event);"></asp:TextBox>
                </div>
            </div>
        
      
            <div class="form-group">
                <asp:Label ID="lblRiskLevelDesc" AssociatedControlID="txtRiskDescription" runat="server" Text="Risk Level Description" class="col-xs-5 control-label"></asp:Label>
                <div class="col-xs-7">
                    <asp:TextBox ID="txtRiskDescription" class="form-control" Rows="3" runat="server"></asp:TextBox>
                </div>
            </div>
       </div>

    </div>
</div>

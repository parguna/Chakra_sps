<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.AppText.UC.DetailUC" %>
<div class="form-horizontal left clearfix">
    <div class="row-fluid">
      <div class="col-xs-12">
          <div class="form-group">
                <asp:Label ID="lblTextKey" AssociatedControlID="lblKeyVal" runat="server" Text="Application Text Key" class="col-xs-5 control-label"></asp:Label>
                <div class="col-xs-7">
                    <asp:Label ID="lblKeyVal" class="form-control" Rows="3" runat="server" ></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <asp:Label ID="lblTextValue" AssociatedControlID="txtTextValue" runat="server" Text="Application Text Value" class="col-xs-5 control-label"></asp:Label>
                <div class="col-xs-7">
                    <asp:TextBox ID="txtTextValue" class="form-control" Rows="3" runat="server" ></asp:TextBox>
                </div>
            </div>      
       </div>
    </div>
</div>

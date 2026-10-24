<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccountDetailsUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.AccountDetailsUC" %>
<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12 left-right-margin15">
            <asp:Repeater ID="DetailRepeater" runat="server">
            <ItemTemplate>
                <div class="form-group">
                <label class="col-xs-3"><%# Eval("ItemName") %></label>
                <div class="col-xs-9 left-padding45">
                    <asp:Label ID="Description" runat="server" class="text-wrap description" Text='<%# Eval("ItemValue") %>'></asp:Label>
                </div>
            </div>
            </ItemTemplate>
        </asp:Repeater>

        </div>
    </div>
</div>

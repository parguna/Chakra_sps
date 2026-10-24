<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentsRowUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.UC.CommentsRowUC" %>

<div class="row">
    <div class="col-xs-12 ">
        <span class="name"><asp:Label ID="lblCommentsBy" runat="server" Text='<%# Eval("CreatedBy") %>' /></span>
        <span class="date"><asp:Label ID="lblCommentsOn" runat="server" Text='<%# Eval("CreatedOn") %>' /></span>
    </div>
    <div class="col-xs-12 comment-dtl">
        <p><asp:Label ID="lblComments" runat="server" Text='<%# Eval("Comments") %>' /></p>
    </div>
</div>

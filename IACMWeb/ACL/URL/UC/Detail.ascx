<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="DI.Chakra.Web.ACL.URL.UC.Detail" %>

<%--<div class="modal-header">
    <h3 class="modal-title" id="H1">URL</h3>
</div>

<div class="modal-body">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-12">
                <div class="form-group">
                    <asp:Label ID="Label2" AssociatedControlID="txtQuestion" runat="server" Text="Question" class="col-xs-3 control-label"></asp:Label>
                    <div class="col-xs-9">
                        <asp:TextBox ID="txtURL" class="form-control" Rows="3" runat="server"></asp:TextBox>

                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="modal-footer">
    <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" data-dismiss="modal" OnClick="btnCancel_Click"></asp:Button>
    <asp:Button ID="btnSubmit" CssClass="add btn btn-primary bordered-button" runat="server"
        Text="Add" OnClick="btnSubmit_Click"></asp:Button>
</div>--%>


<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                    <asp:Label ID="Label2" AssociatedControlID="txtURL" runat="server" Text="URL" class="col-xs-3 control-label"></asp:Label>
                    <div class="col-xs-9">
                        <asp:TextBox ID="txtURL" required="required" cssclass="form-control" Rows="3" runat="server"></asp:TextBox>
                    </div>
                </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" TabIndex="8" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click"></asp:Button>
</div>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.Global_Question.UC.DetailUC" %>

<div class="modal-header">
    <h3 class="modal-title"  runat="server" id="H1">Add Question</h3>
</div>

<div class="modal-body">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-12">
                <div class="form-group">
                    <asp:Label ID="Label2" AssociatedControlID="txtQuestion" runat="server" Text="Question" class="col-xs-3 control-label"></asp:Label>
                    <div class="col-xs-9">
                        <asp:TextBox ID="txtQuestion" class="form-control" Rows="3" runat="server"></asp:TextBox>

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
</div>

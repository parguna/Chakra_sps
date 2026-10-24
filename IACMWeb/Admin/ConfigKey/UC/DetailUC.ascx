<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.ConfigKey.UC.DetailUC" %>

<div class="modal-header">
    <h3 class="modal-title" id="H1" runat="server">Add Configuration</h3>
</div>
<div class="modal-body">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-md-12">
                <div class="form-group">
                    <asp:Label ID="lblkey" AssociatedControlID="txtConfigKey" runat="server" Text="Config Key" class="col-sm-3 control-label"></asp:Label>
                    <div class="col-sm-9">
                        <asp:TextBox ID="txtConfigKey" class="form-control" Rows="3" runat="server"></asp:TextBox>

                    </div>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblvalue" AssociatedControlID="txtConfigValue" runat="server" Text="Config Value" class="col-sm-3 control-label"></asp:Label>
                    <div class="col-sm-9">
                        <asp:TextBox ID="txtConfigValue" class="form-control" Rows="3" runat="server"></asp:TextBox>

                    </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lbldesc" AssociatedControlID="txtDESC" runat="server" Text="Description" class="col-sm-3 control-label"></asp:Label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="txtDESC" class="form-control" Rows="3" runat="server"></asp:TextBox>
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

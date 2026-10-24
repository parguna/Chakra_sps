<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.TrustedSrcConfig.UC.DetailUC" %>


<div class="modal-header">
    <h3 class="modal-title" id="h1">Add Trusted Source</h3>
</div>
<div class="modal-body">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-md-12">
                <div class="form-group">
                    <asp:Label ID="lblUser" AssociatedControlID="ddlDi" runat="server" Text="User" class="col-sm-3 control-label"></asp:Label>
                    <div class="col-sm-9">
                        <asp:DropDownList ID="ddlDi" class="form-control selectpicker" runat="server"></asp:DropDownList>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblTrs" runat="server" AssociatedControlID="ddlEPTypes" Text="Trusted Source" class="col-sm-3 control-label"></asp:Label>
                    <div class="col-sm-9">
                        <asp:DropDownList ID="ddlEPTypes" DataTextField="Description" DataValueField="Id" class="form-control selectpicker" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEPTypes_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblTg" runat="server" AssociatedControlID="ddlTg" class="col-sm-3 control-label"><%= T("App.Common.Endpoint") %></asp:Label>
                    <div class="col-sm-9">
                        <asp:DropDownList ID="ddlTg" runat="server" class="form-control selectpicker numeric"></asp:DropDownList>
                    </div>
                </div>
                <div class="form-group">
                    <div class="checkbox">
                        <label class="col-xs-3 control-label">Self Update</label>
                        <div class="col-xs-5">
                            <asp:Label ID="Label5" AssociatedControlID="chkSelfUpdate" runat="server">
                                <asp:CheckBox ID="chkSelfUpdate" runat="server" Text="" />
                                <span class="lbl"></span>
                            </asp:Label>
                        </div>
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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.House_Keeping.UC.DetailUC" %>  

<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="modal-header">
        <h3 class="modal-title" id="H1" runat="server" >Add Purge Configuration</h3>

    </div>
    <div class="modal-body">
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:Label ID="lblUser" AssociatedControlID="TableNames" runat="server" Text="Table Name" class="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-9">
                             <asp:DropDownList ID="TableNames" runat="server" CssClass="form-control selectpicker" required="required"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblTrs" runat="server" AssociatedControlID="txtDescription" Text="Description" class="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-9">
                            <asp:TextBox ID="txtDescription" class="form-control" Rows="3" runat="server" TextMode="MultiLine"></asp:TextBox>

                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label1" runat="server" AssociatedControlID="txtDaysToKeep" Text="Days To Keep" 
                            class="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-9">
                            <asp:TextBox ID="txtDaysToKeep" Text="365" class="form-control" Rows="3" runat="server" MaxLength="4" onKeyPress="return allowNumerics(event)" required="required"></asp:TextBox>

                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblPurgeMode" runat="server" AssociatedControlID="ddlPurgeMode" class="col-xs-3 control-label" Text="Purge Mode"></asp:Label>
                        <div class="col-xs-9">
                            <asp:DropDownList ID="ddlPurgeMode" runat="server" class="form-control selectpicker numeric" required="required"></asp:DropDownList>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <div class="modal-footer">
    <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" data-dismiss="modal" OnClick="btnCancel_Click"></asp:Button>
    <asp:Button ID="btnSubmit" CssClass="add btn btn-primary bordered-button" runat="server"
        Text="Add" OnClick="btnSubmit_Click"></asp:Button>
</div>
</div>


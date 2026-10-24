<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddUC.ascx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.PrivateQuestion.UC.AddUC" %>

<div class="modal-header">
                <h3 class="modal-title" id="hdrAdd" runat="server"></h3>
            </div>
<div class="modal-body">
<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12">
            <div class="form-group">
                <label class="col-xs-5 control-label">
                    <asp:RadioButton ID="rbPredefinedQues" AutoPostBack="true" runat="server" GroupName="Question" Checked="true" OnCheckedChanged="rbQues_CheckedChanged" />
                    <span class="lbl">Predefined Question</span><span style="color:red">*</span>
                </label>
                <div class="col-xs-7">                    
                    <asp:DropDownList ID="ddlPredefinedQuestion" runat="server" CssClass="form-control selectpicker" data-width="100%"></asp:DropDownList>                   
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-5 control-label">
                    <asp:RadioButton ID="rbCustomQues" runat="server" AutoPostBack="true" GroupName="Question" OnCheckedChanged="rbQues_CheckedChanged"/>
                    <span class="lbl">Custom Question</span><span style="color:red">*</span>
                </label>
                <div class="col-xs-7">
                    <asp:TextBox ID="txtCustQues" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                </div>
            </div>
            <div class="form-group">
                <asp:Label ID="Label5" AssociatedControlID="txtAnswer" required="required" runat="server" Text="Answer" class="col-xs-5 control-label"></asp:Label>
                <div class="col-xs-7">
                    <asp:TextBox ID="txtAnswer" class="form-control" Rows="3" runat="server"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
</div>
</div>
 <div class="modal-footer">
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnAddCancel" Text="Cancel" data-dismiss="modal" OnClick="btnAddCancel_Click"></asp:Button>
                <asp:Button ID="btnAdd" CssClass="add btn btn-primary bordered-button" runat="server"
                    Text="Add" OnClick="btnAdd_Click"></asp:Button>
            </div>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="DI.Chakra.Web.UsrControl.Common.CommentsUserControl" CodeBehind="CommentsUserControl.ascx.cs" %>
<div class="modal-header">
    <h3 class="modal-title" id="H1">Add Comment</h3>
</div>
<div class="modal-body">
    <div class="content-wrapper">
        <div class="form-horizontal left">
            <div class="row-fluid">
                    <div class="form-group">
                        <div class="col-xs-12">
                            <div id="divComments" runat="server" style="overflow: auto; margin-top: 5px; margin-bottom: 5px">
                                <asp:Repeater ID="rptComments" runat="server">
                                    <ItemTemplate>
                                        <div style="margin: 6px;">
                                            <span class="user">
                                                <%# DataBinder.Eval(Container.DataItem, "LastModifiedBy")%></span><span style="padding-right: 3px; padding-bottom: 3px;"><%# DataBinder.Eval(Container.DataItem, "LastModifiedDate")%></span>
                                            <br />

                                            <%# DataBinder.Eval(Container.DataItem, "Comment")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            <asp:TextBox ID="CommentsTextBox" runat="server" TextMode="MultiLine" Rows="5" ValidationGroup="ReviewComment" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="CommentsTextBox" ValidationGroup="ReviewComment"
                                ErrorMessage="Please enter the comment." ForeColor="Red"></asp:RequiredFieldValidator>
                        </div>
                    </div>

                </div>
        </div>
    </div>
</div>
<div class="modal-footer">
    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary transparent-button" data-dismiss="modal" Text="Cancel" OnClick="btnCancel_Click" OnClientClick="closeCommentWindow()"/>
    <asp:Button ID="SaveButton" runat="server"  OnClick="SaveButton_Click" CssClass="add btn btn-primary bordered-button"
        Text="Save" ValidationGroup="ReviewComment"  />
</div>
<asp:HiddenField runat="server" Value="1" ID="hdnIsEditable" />
<script type="text/javascript">
   
    function validateComments() {
       
           $("#UpdateProgressBar").hide();
         setTimeout(function(){
                $(".disableDIv").hide();
            },1000);    
        var hdValue = document.getElementById('<%= hdnIsEditable.ClientID%>').value;
        if (hdValue == "1") {
            if (Page_ClientValidate('ReviewComment')) {
                 
                $(document.getElementById('<%=SaveButton.ClientID  %>')).attr("disabled", false);
            } else {
                $(document.getElementById('<%=SaveButton.ClientID  %>')).attr("disabled", true);
            }
        }
    }
    function enableButtonOnFocus(enable) {
       <%-- if (enable) {
            strValue = document.getElementById('<%= CommentsTextBox.ClientID%>').value;
        if (strValue === "") {
            $(document.getElementById('<%=btnCancel.ClientID  %>')).attr("disabled", false);
            $(document.getElementById('<%=SaveButton.ClientID  %>')).attr("disabled", true);
        } else {
            var da = "<%= IsEditable %>";
            $(document.getElementById('<%=SaveButton.ClientID  %>')).attr("disabled", false);
        }--%>
    }
    
</script>

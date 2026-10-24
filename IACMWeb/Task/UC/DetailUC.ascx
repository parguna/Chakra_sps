<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Task.UC.DetailUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:UpdatePanel ID="UpdateUC" UpdateMode="Conditional" runat="server"><ContentTemplate>
<div class="modal-header">
    <h3 class="modal-title" id="H2">Delegate</h3>
</div>
<div class="modal-body">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-12">
                <div class="form-group required">
                    <label class="col-xs-3 control-label"><%= T("App.Task.DelegateTo") %></label>
                    <div class="col-xs-9">
                        <asp:updatepanel ID="UpdateUsers" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <uc1:UserSearchUC runat="server" ID="delegateTo" Visible="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="delegateTo" />
                            </Triggers>
                        </asp:updatepanel>
                    </div>
                </div>
                <div class="form-group" id="divlblDelegateToValidationMsg" style="display:none">
                    <label class="col-xs-3 control-label">&nbsp;</label>
                    <div class="col-xs-9">
                    <asp:Label runat="server" ID="lblDelegateToValidationMsg" style="display:none;" ForeColor="Red" Text="Delegated To cannot be empty"></asp:Label>
                    </div>    
                </div>
                <div class="form-group required">
                    <label class="col-xs-3 control-label"><%= T("App.Task.Comments") %></label>
                    <div class="col-xs-9">
                        <asp:TextBox ID="Comment" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="5" ValidationGroup="cmtValidationGroup"></asp:TextBox>
                        <span class="lbl"></span>
                        <asp:RequiredFieldValidator ID="rfvComment" runat="server" ControlToValidate="Comment" ErrorMessage="Please enter comments" ForeColor="Red" ValidationGroup="cmtValidationGroup"></asp:RequiredFieldValidator>
                        
                    </div>
                </div>
                <div class="form-group required">
                    <div class="col-xs-12"> 
                        <asp:Label ID="lblDelegationMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
 <div class="modal-footer">
    <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" data-dismiss="modal" OnClick="btnCancel_Click" OnClientClick="Close()"></asp:Button>
    <asp:Button ID="btnSubmit" CssClass="add btn btn-primary bordered-button" runat="server" Text="Add"  OnClick="btnSubmit_Click" ValidationGroup="cmtValidationGroup" ></asp:Button>
</div>
    </ContentTemplate></asp:UpdatePanel>
<script>
    function activateButton() {
        var lblDelegationMessage = document.getElementById("<%= lblDelegationMessage.ClientID %>");
        lblDelegationMessage.value = "";
        var button = document.getElementById("<%= btnSubmit.ClientID %>");
        //var elemt = document.getElementById("ctl00_cplMain_TaskListUC_modalPopup_C_UserDetail_delegateTo_UserNameAutoCompleteBox_Input");
        var elementEntry = document.getElementById("ctl00_cplMain_TaskListUC_modalPopup_C_UserDetail_delegateTo_UserNameAutoCompleteBox_ClientState");
        var delegateToMsg = document.getElementById("<%= lblDelegateToValidationMsg.ClientID %>");
        var allowSubmit = false;
        if (elementEntry && elementEntry.value) {
            var data = JSON.parse(elementEntry.value);
            if (data.logEntries.length > 0) {
                allowSubmit = true;
            }
        }
        var divlblDelegateToValidationMsg = document.getElementById("divlblDelegateToValidationMsg");
        if (divlblDelegateToValidationMsg) {
            divlblDelegateToValidationMsg.style.display = "none";
        }
        if (delegateToMsg) {
            delegateToMsg.style.display = "none";  
        }
        if (allowSubmit === false && delegateToMsg) {
            delegateToMsg.style.display = "block";
             divlblDelegateToValidationMsg.style.display = "block";
        }
        if(Page_ClientValidate('cmtValidationGroup') && allowSubmit){ button.disabled=false; }else{button.disabled=true;}
    }
</script>
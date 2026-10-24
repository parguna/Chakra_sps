<%@ control language="C#" autoeventwireup="true" inherits="DI.Chakra.Web.WorkFlow.UC.ApproversUC" codebehind="ApproversUC.ascx.cs" %>
<%@ register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:panel runat="server" id="MainDiv">
<div class="modal-header">
    <h3 class="modal-title" id="H1">Approver Detail</h3>
</div>
<div class="modal-body">
    <div class="content-wrapper">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group required">
                        <label class="col-sm-4 control-label">Approver Type</label>
                        <div class="col-sm-7">
                            <asp:DropDownList ID="ddlApprovalUserType" runat="server" TabIndex="1" 
                                OnSelectedIndexChanged="ddlApprovalUserType_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control selectpicker" data-width="100%">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group required">
                        <label class="col-sm-4 control-label">Approver</label>
                        <div class="col-sm-7">
                            <asp:DropDownList ID="ddlApprover" runat="server" TabIndex="2" CssClass="form-control selectpicker" data-width="100%"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlApprover_SelectedIndexChanged">
                            </asp:DropDownList>
                            <telerik:RadAutoCompleteBox ID="UserNameAutoCompleteBox" TabIndex="2" EmptyMessage="Enter * to view full list" runat="server"
                                InputType="Token" CssClass="form-control">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="GetActiveAndNonTerminatedUsers" Path="../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                            <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="text-danger"></asp:Label>
                        </div>
                    </div>
                    <div id="divRequiresApprovalFromAll" runat="server" class="form-group">
                        <asp:Label AssociatedControlID="rbRequireAllYes" CssClass="col-sm-4" Text="Requires Approval From All" runat="server"></asp:Label>
                        <div class="col-sm-7">
                            <asp:RadioButton ID="rbRequireAllYes" runat="server" TabIndex="3" GroupName="RequireApprovalFromAll" />
                            <span class="lbl">Yes</span>
                            <asp:RadioButton ID="rbRequireAllNo" runat="server" TabIndex="4" GroupName="RequireApprovalFromAll" />
                            <span class="lbl">No</span>
                        </div>
                    </div>
                    <div id="divIsMockGroup" runat="server" class="form-group">
                        <asp:Label ID="Label1" AssociatedControlID="rbMockGrpYes" CssClass="col-sm-4" Text="Mock Group" runat="server"></asp:Label>
                        <div class="col-sm-7">
                            <asp:RadioButton ID="rbMockGrpYes" runat="server" TabIndex="5" GroupName="MockGroup" />
                            <span class="lbl">Yes</span>
                            <asp:RadioButton ID="rbMockGrpNo" runat="server" TabIndex="6" GroupName="MockGroup" />
                            <span class="lbl">No</span>
                        </div>
                    </div>
                    <div class="form-group required">
                        <label class="col-sm-4 control-label">Response Time (Days)</label>
                        <div class="col-sm-7">
                            <asp:TextBox ID="txtResponseTime" runat="server" CssClass="form-control" TabIndex="7" MaxLength="4" AutoPostBack="true" onKeyPress="return AllowNumeric(event,0,9999)" OnTextChanged="txtResponseTime_TextChanged"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label AssociatedControlID="rbYes" CssClass="col-sm-4" Text="Requires Reminder" runat="server"></asp:Label>
                        <div class="col-sm-7">
                            <asp:RadioButton ID="rbYes" runat="server" AutoPostBack="true" GroupName="RequireReminder" TabIndex="8" OnCheckedChanged="rbYes_CheckedChanged" Visible="true" />
                            <span class="lbl">Yes</span>
                            <asp:RadioButton ID="rbNo" runat="server" AutoPostBack="true" GroupName="RequireReminder" TabIndex="9" OnCheckedChanged="rbNo_CheckedChanged" Visible="true" />
                            <span class="lbl">No</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4 control-label">Reminder Interval (Days)</label>
                        <div class="col-sm-7">
                            <asp:TextBox ID="txtReminderInterval" runat="server" CssClass="form-control" TabIndex="10" MaxLength="2" min="1" max="99" AutoPostBack="true" onKeyPress="return AllowNumeric(event,0,99)" OnTextChanged="txtReminderInterval_TextChanged" />
                        </div>
                    </div>
                    <div id="divRequiresOnlyAction" class="form-group" runat="server">
                        <asp:Label ID="Label2" AssociatedControlID="rbYes" CssClass="col-sm-4" Text="Requires Only Action" runat="server"></asp:Label>
                        <div class="col-sm-7">
                            <asp:RadioButton ID="rbActionYes" runat="server" TabIndex="11" GroupName="RequireAction" />
                            <span class="lbl">Yes</span>
                            <asp:RadioButton ID="rbActionNo" runat="server" TabIndex="12" GroupName="RequireAction" />
                            <span class="lbl">No</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label3" AssociatedControlID="rbYes" CssClass="col-sm-4" Text="If No Action Taken" runat="server"></asp:Label>
                        <div class="col-sm-7">
                            <asp:RadioButton ID="rbEscalate" runat="server" AutoPostBack="true" GroupName="Escalation" TabIndex="13" OnCheckedChanged="rbEscalate_CheckedChanged" Visible="true" />
                            <span class="lbl">Escalate to next level</span>
                            <asp:RadioButton ID="rbTerminate" runat="server" AutoPostBack="true" GroupName="Escalation" TabIndex="14" OnCheckedChanged="rbTerminate_CheckedChanged" Visible="true" />
                            <span class="lbl">Terminate</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4 control-label">Days To Wait After Escalation</label>
                        <div class="col-sm-7">
                            <asp:TextBox ID="txtEscalationDays" runat="server" CssClass="form-control" TabIndex="15" MaxLength="3" onKeyPress="return AllowNumeric(event,0,365)"></asp:TextBox>
                        </div>
                    </div>
                     <div class="form-group">
                         <div class="col-xs-12">
                             <asp:Label runat="server" ID="lblWarningMessage" ForeColor="Red"></asp:Label>
                         </div>
                     </div>
                </div>
            </div>
        </div>
    </div>
</div>
    </asp:panel>
<div class="modal-footer">
    <asp:button runat="server" id="btnCancel" class="btn btn-primary transparent-button" text="Cancel" tabindex="17" onclick="btnCancel_Click" />
    <asp:button runat="server" id="SaveApproverButton" cssclass="add btn btn-primary bordered-button" usesubmitbehavior="false" causesvalidation="false" tabindex="16" text="Save" onclick="SaveApproverButton_Click" />
</div>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowInfo.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.WorkflowInfo" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<%@ Register Src="~/UsrControl/Common/GroupSearchUC.ascx" TagPrefix="uc1" TagName="GroupSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">

    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">

                    <div class="form-group">
                        <asp:Label ID="lblWorkflow" AssociatedControlID="ddlWorkflow" runat="server" Text="Workflow Name" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:DropDownList ID="ddlWorkflow" runat="server" AppendDataBoundItems="True" DataTextField="WorkflowName" DataValueField="Id" required="required"
                                OnSelectedIndexChanged="ddlWorkflow_SelectedIndexChanged"
                                AutoPostBack="True" CssClass="form-control selectpicker">
                                <asp:ListItem Value="">--Select--</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblWorkflowDesc" AssociatedControlID="txtWorkflowDesc" runat="server" Text="Workflow Description"
                            Enabled="False" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtWorkflowDesc" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="4" Columns="60" Enabled="false"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <asp:Label ID="lblExceptionConfiguration" runat="server" AssociatedControlID="chkSendNotification" CssClass="col-xs-4 control-label"><%= T("App.Attestation.ExceptionConfiguration") %></asp:Label>
                        <div class="col-xs-7">
                            <div class="radio">
                                <asp:Label ID="lbl1" runat="server" AssociatedControlID="chkSendNotification">
                                    <asp:CheckBox runat="server" ID="chkSendNotification" TabIndex="2" />
                                    <span class="lbl"><%= T("App.Attestation.SendNotification") %></span>
                                </asp:Label>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblBlankException" runat="server" AssociatedControlID="chkCreateTask" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <div class="radio">
                                <asp:Label ID="lbl2" runat="server" AssociatedControlID="chkCreateTask">
                                    <asp:CheckBox runat="server" ID="chkCreateTask" TabIndex="2" AutoPostBack="true" OnCheckedChanged="chkCreateTask_CheckedChanged" />
                                    <span class="lbl"><%= T("App.Attestation.CreateTask") %></span>
                                </asp:Label>
                            </div>
                        </div>
                    </div>
                    <asp:Panel runat="server" ID="DV_CreateTask">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">Assignee Type</label>
                            <div class="col-sm-7">
                                <asp:DropDownList ID="ddlApprovalUserType" runat="server" TabIndex="1"
                                    OnSelectedIndexChanged="ddlApprovalUserType_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control selectpicker" data-width="100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-4 control-label">Assignee</label>
                            <div class="col-sm-7">
                                <asp:DropDownList ID="ddlApprover" runat="server" TabIndex="2" CssClass="form-control selectpicker" data-width="100%"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlApprover_SelectedIndexChanged">
                                </asp:DropDownList>
                                <telerik:RadAutoCompleteBox ID="UserNameAutoCompleteBox" TabIndex="2" EmptyMessage="Enter * to view full list" runat="server"
                                    InputType="Token" CssClass="form-control">
                                    <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Method="GetActiveAndNonTerminatedUsers" Path="../../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
</div>

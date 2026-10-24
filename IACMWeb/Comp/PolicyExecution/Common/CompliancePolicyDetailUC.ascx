<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompliancePolicyDetailUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.PolicyExecution.Common.CompliancePolicyDetailUC" %>
<%@ Register Src="~/UsrControl/CompliancePolicySearch.ascx" TagPrefix="uc1" TagName="CompliancePolicySearch" %>

<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <asp:Label ID="Label5" AssociatedControlID="CompliancePolicySearch" runat="server" required="required" Text="Policy Code" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <uc1:CompliancePolicySearch runat="server" ID="CompliancePolicySearch" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="conflictCode" AssociatedControlID="txtUniqueCode" runat="server" Text="Unique Code" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtUniqueCode" CssClass="form-control" runat="server" required="required" data-errMessage="Unique Code is required" ReadOnly ="true" MaxLength="30" onKeyPress="return AllowLetterNumber(event);" ></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblWorkflowtype" AssociatedControlID="rblReport" runat="server" Text="Output Type" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <div class="radio">
                                <asp:Label ID="Label1" runat="server" AssociatedControlID="rblReport">
                                    <asp:RadioButton runat="server" ID="rblReport" AutoPostBack="true" GroupName="Map"></asp:RadioButton>
                                    <asp:Label CssClass="lbl" ID="Label2" runat="server" Text="Report"></asp:Label>
                                </asp:Label>
                                <asp:Label ID="Label3" runat="server" AssociatedControlID="rblWorkflow">
                                    <asp:RadioButton runat="server" ID="rblWorkflow" AutoPostBack="true" GroupName="Map"></asp:RadioButton>
                                    <asp:Label CssClass="lbl" ID="Label4" runat="server" Text="Workflow"></asp:Label>
                                </asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
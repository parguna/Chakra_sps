<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutputUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Mitigation.Common.OutputUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Comp/Mitigation/UC/ReviewApprUC.ascx" TagName="ReviewApp" TagPrefix="RvwApp" %>
<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <asp:Label ID="ReviewerType" runat="server" Text="Reviewer Type" AssociatedControlID="RadlblRew" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <div class="radio">
                                <asp:Label ID="RadlblRew" runat="server" CssClass="col-xs-4 control-label" AssociatedControlID="radActive">
                                    <asp:RadioButton ID="radActive" runat="server" AutoPostBack="true" OnCheckedChanged="radActive_CheckedChanged" Checked="True" />
                                    <asp:Label CssClass="lbl" ID="RadlblRew1" runat="server" Text="User"></asp:Label>
                                </asp:Label>
                                <asp:Label ID="RadlblReve" runat="server" CssClass="col-xs-4 control-label" AssociatedControlID="radClosed">
                                    <asp:RadioButton ID="radClosed" runat="server" AutoPostBack="true" OnCheckedChanged="radClosed_CheckedChanged" />
                                    <asp:Label CssClass="lbl" ID="RadlblRew2" runat="server" Text="Group"></asp:Label>
                                </asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="MitigationReviewer" AssociatedControlID="Reviewer" Text="Mitigation Reviewer" runat="server" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <RvwApp:ReviewApp ID="Reviewer" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="ApproverType" runat="server" AssociatedControlID="RadLblApp" Text="Approver Type" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <div class="radio">
                                <asp:Label ID="RadLblApp" runat="server" CssClass="col-xs-4 control-label" AssociatedControlID="RadAppr1">
                                    <asp:RadioButton Checked="true" ID="RadAppr1" runat="server" AutoPostBack="true" OnCheckedChanged="RadAppr1_CheckedChanged" />
                                    <asp:Label CssClass="lbl" ID="RadLblApp2" runat="server" Text="User"></asp:Label>
                                </asp:Label>
                                <asp:Label ID="RadLblAppr" runat="server" CssClass="col-xs-4 control-label" AssociatedControlID="RadAppr2">
                                    <asp:RadioButton Checked="false" ID="RadAppr2" runat="server" AutoPostBack="true" OnCheckedChanged="RadAppr2_CheckedChanged" />
                                    <asp:Label CssClass="lbl" ID="RadLblAppr2" runat="server" Text="Group"></asp:Label>
                                </asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="MitigationApprover" AssociatedControlID="Approver" Text="Mitigation Approver" runat="server" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <RvwApp:ReviewApp ID="Approver" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

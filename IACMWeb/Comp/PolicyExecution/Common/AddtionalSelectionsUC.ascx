<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddtionalSelectionsUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.PolicyExecution.Common.AddtionalSelectionsUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<%@ Register Src="~/UsrControl/Common/GroupSearchUC.ascx" TagPrefix="uc1" TagName="GroupSearchUC" %>
<%@ Register Src="~/UsrControl/RiskLevelSearchUC.ascx" TagPrefix="uc1" TagName="RiskLevelSearchUC" %>



<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                     <div class="form-group">
                                <div class="radio">
                                    <asp:Label ID="Label1" runat="server" AssociatedControlID="rblUsers">
                                        <asp:RadioButton runat="server" ID="rblUsers" AutoPostBack="true" GroupName="Map" OnCheckedChanged ="rblUsers_CheckedChanged"></asp:RadioButton>
                                        <asp:Label CssClass="lbl" ID="Label2" runat="server" Text="Users"></asp:Label>
                                    </asp:Label>
                                      <asp:Label ID="Label3" runat="server" AssociatedControlID="rblGroups">
                                        <asp:RadioButton runat="server" ID="rblGroups" AutoPostBack="true" GroupName="Map" OnCheckedChanged ="rblGroups_CheckedChanged"></asp:RadioButton>
                                        <asp:Label CssClass="lbl" ID="Label4" runat="server" Text="Groups"></asp:Label>
                                    </asp:Label>
                                    <asp:Label ID="Label6" runat="server" AssociatedControlID="rblRisks">
                                        <asp:RadioButton runat="server" ID="rblRisks" AutoPostBack="true" GroupName="Map" OnCheckedChanged ="rblRisks_CheckedChanged"></asp:RadioButton>
                                        <asp:Label CssClass="lbl" ID="Label7" runat="server" Text="Risks"></asp:Label>
                                    </asp:Label>
                                </div>
                            </div>
                           
                    </div>
                   <div class="form-group" id ="UserSection" runat ="server">
                        <asp:Label ID="Label5" AssociatedControlID="UserSearch" runat="server" Text="Select Users" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <uc1:UserSearchUC runat="server" ID="UserSearch" />
                        </div>
                    </div>
                     <div class="form-group" id ="GroupSection" runat ="server">
                        <asp:Label ID="Label8" AssociatedControlID="GroupSearch" runat="server" Text="Select Group" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <uc1:GroupSearchUC runat="server" ID="GroupSearch" />
                        </div>
                    </div>
                     <div class="form-group" id ="RiskSection" runat ="server">
                        <asp:Label ID="Label9" AssociatedControlID="RiskLevelSearch" runat="server" Text="Select Risks" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <uc1:RiskLevelSearchUC runat="server" ID="RiskLevelSearch" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
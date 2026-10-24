<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConflictDetailsUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Conflict.Common.ConflictDetailsUC" %>
<%@ Register Src="~/UsrControl/Common/GenericAutoCompleteBox.ascx" TagName="RskCode" TagPrefix="RiskCode" %>
<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">

                    <div class="form-group">
                        <asp:Label ID="conflictCode" AssociatedControlID="txtConflictCode" runat="server" Text="Code" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtConflictCode" CssClass="form-control" runat="server" required="required" data-errMessage="Conflict Code is required" MaxLength="30" onKeyPress="return AllowLetterNumber(event);"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="description" AssociatedControlID="txtConflictDescription" runat="server" Text="Description" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtConflictDescription" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblriskcode" AssociatedControlID="AutRskCode" runat="server" required="required" Text="Risk Code" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <RiskCode:RskCode ID="AutRskCode" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label5" AssociatedControlID="ddlComplianceType" runat="server" Text="Compliance Type" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:DropDownList ID="ddlComplianceType" runat="server" AppendDataBoundItems="True" CssClass="form-control selectpicker" data-width="100%"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="radio">
                            <asp:Label ID="Label1" runat="server" AssociatedControlID="rblActive">
                                <asp:RadioButton runat="server" ID="rblActive" AutoPostBack="true" GroupName="Map"></asp:RadioButton>
                                <asp:Label CssClass="lbl" ID="Label2" runat="server" Text="Active"></asp:Label>
                            </asp:Label>
                            <asp:Label ID="Label3" runat="server" AssociatedControlID="rblInActive">
                                <asp:RadioButton runat="server" ID="rblInActive" AutoPostBack="true" GroupName="Map"></asp:RadioButton>
                                <asp:Label CssClass="lbl" ID="Label4" runat="server" Text="Inactive"></asp:Label>
                            </asp:Label>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</div>
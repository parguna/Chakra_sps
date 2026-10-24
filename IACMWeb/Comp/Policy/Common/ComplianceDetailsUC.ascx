<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComplianceDetailsUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Policy.Common.ComplianceDetailsUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                   
                    <div class="form-group">
                        <asp:Label ID="conflictCode" AssociatedControlID="txtComplianceCode" runat="server" Text="Policy Code" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtComplianceCode" CssClass="form-control" runat="server" required="required" data-errMessage="Compliance Code is required" MaxLength="30" onKeyPress="return AllowLetterNumber(event);" ></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="description" AssociatedControlID="txtConflictDescription" runat="server" Text="Policy Description" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtConflictDescription" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                     <div class="form-group">
                        <asp:Label ID="lblriskcode" AssociatedControlID="ddlComplianceType" runat="server" Text="Compliance Type" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:DropDownList ID="ddlComplianceType" OnSelectedIndexChanged="ddlComplianceType_SelectedIndexChanged" AutoPostBack="true" runat="server" AppendDataBoundItems="True" CssClass="form-control selectpicker" data-width="100%"></asp:DropDownList>
                        </div>
                    </div>
                     <div class="form-group">
                        <asp:Label ID="Label5" AssociatedControlID="UserSearch" runat="server" required="required" Text="Compliance Owner" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                           <uc1:UserSearchUC runat="server" ID="UserSearch" Visible="true" />
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

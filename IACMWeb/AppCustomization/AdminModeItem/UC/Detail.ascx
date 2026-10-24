<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="DI.Chakra.Web.AppCustomization.AdminModeItem.UC.Detail" %>
<%@ Register Src="~/ACL/Common/UC/ACLDataMethodSearchUC.ascx" TagPrefix="uc1" TagName="ACLDataMethodSearchUC" %>

<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    
                     <div class="form-group">
                        <asp:Label ID="Label3" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtName">Name</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox runat="server" ID="txtName" required="required" CssClass="form-control" MaxLength="255"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label1" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtDescription">Description</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox runat="server" ID="txtDescription" required="required" CssClass="form-control" MaxLength="255"></asp:TextBox>
                        </div>
                    </div>

                     <div class="form-group">
                        <asp:Label ID="Label2" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtIcon">Icon</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox runat="server" ID="txtIcon" required="required" CssClass="form-control" MaxLength="255">/Images/tools-icons/access-request-form.png</asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label5" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtInformation">Information</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox runat="server" ID="txtInformation" required="required" CssClass="form-control" MaxLength="255"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label7" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="ddlAdminType">Type</asp:Label>
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlAdminType" runat="server" DataTextField="Name" CssClass="form-control selectpicker" DataValueField="Id"></asp:DropDownList>
                        </div>
                    </div>

                     <div class="form-group">
                        <asp:Label ID="Label4" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="ACLDataMethodSearchUC">Menu</asp:Label>
                        <div class="col-xs-4">
                            <uc1:ACLDataMethodSearchUC runat="server" DataSourceMethod="GetMenus" ID="ACLDataMethodSearchUC"  />
                        </div>
                    </div>


                     <div class="form-group hide">
                        <asp:Label ID="Label6" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtTableName">Table Name</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox runat="server" ID="txtTableName" required="required" CssClass="form-control" MaxLength="255"></asp:TextBox>
                        </div>
                    </div>

                   
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click"></asp:Button>
</div>

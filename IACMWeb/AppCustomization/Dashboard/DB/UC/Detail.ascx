<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="DI.Chakra.Web.AppCustomization.Dashboard.DB.UC.Detail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/ACL/Common/UC/ACLDataMethodSearchUC.ascx" TagPrefix="uc1" TagName="ACLDataMethodSearchUC" %>


<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:Label ID="lblCategory" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtCategory">Category</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtCategory" runat="server" required="required"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblName" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtName">Name</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtName" runat="server" required="required"></asp:TextBox>
                        </div>
                    </div>
                   
                     <div class="form-group">
                        <asp:Label ID="lblDBType" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="ddlDBType">Type</asp:Label>
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlDBType" runat="server" DataTextField="Name" DataValueField="Name" CssClass="form-control selectpicker">
                            </asp:DropDownList>
                        </div>
                    </div> 

                    <div class="form-group">
                        <asp:Label ID="Label1" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtTargetURL">Target URL</asp:Label>
                        <div class="col-xs-4">
                           <asp:TextBox ID="txtTargetURL" runat="server" required="required"></asp:TextBox>
                        </div>
                    </div> 
                    <div class="form-group">
                        <asp:Label ID="Label2" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtNavigateURL">Navigate URL</asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtNavigateURL" runat="server" required="required"></asp:TextBox>
                        </div>
                    </div> 

                   <%-- <div class="form-group">
                        <asp:Label ID="lblUrl" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="TargetUrlAutoCompleteBox">TargetURL</asp:Label>
                        <div class="col-xs-4">
                            <uc1:ACLDataMethodSearchUC runat="server" id="TargetUrlAutoCompleteBox" DataSourceMethod="GetPageURLs" /> 
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblNavigateURL" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="NavigateUrlAutoCompleteBox">NavigateURL</asp:Label>
                        <div class="col-xs-4">
                           <uc1:ACLDataMethodSearchUC runat="server" id="NavigateUrlAutoCompleteBox" DataSourceMethod="GetPageURLs" /> 
                        </div>
                    </div>--%>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click" />
</div>

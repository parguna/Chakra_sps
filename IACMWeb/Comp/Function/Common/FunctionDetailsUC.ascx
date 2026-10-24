<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionDetailsUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Function.Common.FunctionDetailsUC" %>
<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">

                    <div class="form-group">
                        <asp:Label ID="functionCode" AssociatedControlID="txtFunctionCode" runat="server" Text="Code" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtFunctionCode" CssClass="form-control" runat="server" MaxLength="30" required="required" data-errMessage="Function Code is required" onKeyPress="return AllowLetterNumber(event);"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="description" AssociatedControlID="txtFunctionDescription" runat="server" Text="Description" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtFunctionDescription" CssClass="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
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
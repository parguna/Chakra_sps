<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExceptionDetailsUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Exception.Common.ExceptionDetailsUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="Usr" TagName="UserSearch" %>
<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
        <div class="row-fluid">
            <div class="col-xs-6">
                <div class="form-group">
                    <asp:Label ID="ExceptionCode" AssociatedControlID="txtExceptionCode" runat="server" Text="Code" CssClass="col-xs-4 control-label"></asp:Label>
                    <div class="col-xs-7">
                        <asp:TextBox id="txtExceptionCode" runat="server" CssClass="form-control" required="required" data-errMessage="Exception Code is required" MaxLength="30" onKeyPress="return AllowLetterNumber(event);" ></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label ID="description" AssociatedControlID="txtExceptionDescription" runat="server" Text="Description" CssClass="col-xs-4 control-label"></asp:Label>
                    <div class="col-xs-7">
                    <asp:TextBox ID="txtExceptionDescription" CssClass="form-control" runat="server" ></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label ID="UserSearch" AssociatedControlID="usrSearch" runat="server" Text="Owner" required="required" CssClass="col-xs-4 control-label"></asp:Label>
                    <div class="col-xs-7">
                    <Usr:UserSearch ID="usrSearch" runat="server" />
                    </div>
                </div>

            </div>
        </div>
        </div>
    </div>

</div>

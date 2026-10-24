<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Risk.Code.UC.DetailUC" %>

<%@ Register Src="~/UsrControl/RiskLevelSearchUC.ascx" TagPrefix="uc1" TagName="RiskLevelSearchUC" %>
<%@ Register Src="~/UsrControl/RiskScoreSearchUC.ascx" TagPrefix="uc1" TagName="RiskScoreSearchUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12">

        <div class="form-group">
            <asp:Label ID="lblRiskCode" AssociatedControlID="txtRiskCode"  required="required" runat="server" Text="Risk Code" class="col-xs-5 control-label"></asp:Label>
            <div class="col-xs-7">
                <asp:TextBox ID="txtRiskCode" class="form-control" Rows="3" runat="server" onKeyPress="return AllowLetterNumber(event);"></asp:TextBox>
            </div>
        </div>


        <div class="form-group">
            <asp:Label ID="lblRiskCodeDesc" AssociatedControlID="txtRiskCodeDescription" runat="server" Text="Risk Code Description" class="col-xs-5 control-label"></asp:Label>
            <div class="col-xs-7">
                <asp:TextBox ID="txtRiskCodeDescription" class="form-control" Rows="3" runat="server"></asp:TextBox>
            </div>
        </div>



        <div class="form-group">    
            <asp:Label ID="Label1" AssociatedControlID="RiskScore" required="required" runat="server" Text="Risk Score" class="col-xs-5 control-label"></asp:Label>

                <%--AssociatedControlID="ddlRiskScore" --%>
            <div class="col-xs-7">
                <uc1:RiskScoreSearchUC runat="server" ID="RiskScore" />
                <%--    <asp:DropDownList ID="ddlRiskScore" CssClass="form-control selectpicker" data-width="100%" runat="server"></asp:DropDownList>--%>
            </div>
        </div>


        <div class="form-group">
            <asp:Label ID="Label2"  AssociatedControlID="Risksearch" required="required" runat="server" Text="Risk Level" class="col-xs-5 control-label"></asp:Label>
                <%--AssociatedControlID="ddlRiskLevel"--%>

            <div class="col-xs-7">
                <uc1:RiskLevelSearchUC runat="server" ID="Risksearch" />
            <%--    <asp:DropDownList ID="ddlRiskLevel" CssClass="form-control selectpicker" data-width="100%" runat="server"></asp:DropDownList>--%>
            </div>
        </div>


        <div class="form-group">
            <asp:Label ID="Label3" AssociatedControlID="UserSearch" required="required" runat="server" Text="Risk Owner" class="col-xs-5 control-label"></asp:Label>
                <%--AssociatedControlID="ddlOwner"--%>
                 
            <div class="col-xs-7">
                <uc1:UserSearchUC runat="server" ID="UserSearch" AllowSingleUser ="true" />
                <%--<asp:DropDownList ID="ddlOwner" CssClass="form-control selectpicker" data-width="100%" runat="server"></asp:DropDownList>--%>
            </div>
        </div>

        <div class="form-group">
            <asp:Label ID="Label7" AssociatedControlID="lblradActive" runat="server" Text="Status" class="col-xs-5 control-label"></asp:Label>
            <div class="col-xs-7">
                <div class="radio">
                    <asp:Label ID="lblradActive" runat="server" AssociatedControlID="radActive">
                        <asp:RadioButton runat="server" ID="radActive" AutoPostBack="true" Checked="true" GroupName="Map" OnCheckedChanged="radActive_CheckedChanged"></asp:RadioButton>
                        <asp:Label CssClass="lbl" ID="radActiveTxt" runat="server" Text="Active"></asp:Label>
                    </asp:Label>
                    <asp:Label ID="lblradClosed" runat="server" AssociatedControlID="radClosed">
                        <asp:RadioButton runat="server" ID="radClosed" AutoPostBack="true" Checked="false" GroupName="Map" OnCheckedChanged="radClosed_CheckedChanged"></asp:RadioButton>
                        <asp:Label CssClass="lbl" ID="Label6" runat="server" Text="Inactive"></asp:Label>
                    </asp:Label>
                </div>
            </div>
        </div>
    </div>

    </div>
</div>

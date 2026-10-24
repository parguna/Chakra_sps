<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NUserDetailUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.NUserDetailUC" %>
<%@ Register Src="~/UsrControl/Common/NOrgDetail.ascx" TagPrefix="uc1" TagName="NOrgDetail" %>
<style>
    #idUserInfo div {
        margin-bottom: 0px !important;
    }
</style>
<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12 mBottom15" id="idUserInfo">

            <div class="form-group">
                <label class="col-xs-4">Display Name:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblDisplayName" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>

            <div class="form-group">
                <label class="col-xs-4">Employee ID:</label>
                <div class="col-xs-8">
                    <asp:Label ID="LabelEmployeeID" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4">Email ID:</label>
                <div class="col-xs-8">
                    <asp:Label ID="LabelEmailID" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4">Telephone Number:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblWorkPhNo" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <div class="form-group" id="DIVCMPNY" runat="server">
                <label class="col-xs-4">Company:</label>
                <div class="col-xs-8">
                    <asp:Label ID="LabelCompany" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4">Department:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblDepartment" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <%--<div class="form-group">
                <label class="col-xs-4">Division:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblDevision" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>--%>
            <div class="form-group">
                <label class="col-xs-4">Branch:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblBranch" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4">Section:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblSection" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4">Unit:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblUnit" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4">Cost Center:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblCostCenter" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4">Cost Distribution:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblCostDistribution" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <%--<div class="form-group">
                <label class="col-xs-4">Company:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblPACompany" runat="server" class="text-wrap description" ></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4">Personnel Area:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblPersonnelArea" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4">Personnel Sub Area:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblPersonnelSubArea" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <label class="col-xs-4">OU:</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblOU" runat="server" class="text-wrap description"></asp:Label>
                </div>
            </div>--%>
            <div id="EmployeeDetails" runat="server">
                <div class="form-group">
                    <label class="col-xs-4">Employee AD-ID:</label>
                    <div class="col-xs-8">
                        <asp:Label ID="LabelUserName" runat="server" class="text-wrap description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4">Join Date:</label>
                    <div class="col-xs-8">
                        <asp:Label ID="LabelJoiningDate" runat="server" class="text-wrap description"></asp:Label>
                    </div>
                </div>
                <div class="form-group " id="DIVEMPTYPE" runat="server">
                    <label class="col-xs-4">Employee Type:</label>
                    <div class="col-xs-8">
                        <asp:Label ID="LabelEmployeeType" runat="server" class="text-wrap description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4">Position:</label>
                    <div class="col-xs-8">
                        <asp:Label ID="lblPosition" runat="server" class="text-wrap description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4">Job Title:</label>
                    <div class="col-xs-8">
                        <asp:Label ID="lblJobTitle" runat="server" class="text-wrap description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4">Supervisor Name:</label>
                    <div class="col-xs-8">
                        <asp:Label ID="LabelManagerName" runat="server" class="text-wrap description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4">Supervisor Work Phone:</label>
                    <div class="col-xs-8">
                        <asp:Label ID="lblManagerPhNo" runat="server" class="text-wrap description"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

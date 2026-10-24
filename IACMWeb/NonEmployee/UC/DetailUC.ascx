<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Non_Employee.UC.DetailUC" %>
<%@ Register Src="~/UsrControl/Common/CurrentAccessUC.ascx" TagPrefix="uc" TagName="UserAccountsListUC" %>
<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12">
            <div class="form-group">
                <div class="col-xs-6">

                    <div class="form-group">
                        <label class="col-xs-4">User Name:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="UserName" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Display Name:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblDisplayName" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Employee Id:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="EmployeeID" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Email Id:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="EmailID" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Telephone Number:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="TelNumber" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Position:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblPosition" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Supervisor:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblRO" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Countersigning Officer:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblCO" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Join Date:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="DateOnBoard" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Last Service Date:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="DateOffBoard" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Token Number:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblOneKeyId" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>

                    <%-- <div class="form-group">
                        <label class="col-xs-4">Status:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblStatus" runat="server" class="description"></asp:Label>
                        </div>
                    </div>--%>

                    <div class="form-group" runat="server" id="nonEmployeeDiv">
                        <label class="col-xs-4">Non Employee Company:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblCompanyOfNonEmployee" runat="server" class="description"></asp:Label>
                        </div>
                    </div>

                </div>

                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-4">First Name:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="FName" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Last Name:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="LName" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Organisation:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblOrganisation" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Cost Center:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblCostCenter" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Company:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="LabelCompany" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Department:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblOrgName" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <%-- <div class="form-group">
                        <label class="col-xs-4">Division:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblDivision" runat="server" class="text-wrap description"></asp:Label>
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
                        <label class="col-xs-4">Mobile Number:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="MbNumber" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</div>
<asp:Panel ID="Panel2" runat="server" CssClass="topBuffer" GroupingText="Active Accounts">
    <uc:UserAccountsListUC runat="server" ID="UserAccountsListUC" />
</asp:Panel>
<div class="buttons text-right nav-rectangle topBuffer">
    <asp:Button runat="server" ID="Cancel" Text="Cancel" CssClass="btn btn-default btn-sm cancel" OnClick="Cancel_Click" />
</div>

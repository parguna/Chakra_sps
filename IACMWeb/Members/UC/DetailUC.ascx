<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Members.UC.DetailUC" %>

<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12">
            <div class="form-group">
                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-4">Identification type:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="IdentiType" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Identification Number:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="IdentNumber" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Identification Country:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="IdentCountry" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
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
                        <label class="col-xs-4">Preferred Name:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="PrefName" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
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
                        <label class="col-xs-4">Email:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="EmailID" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Status:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblStatus" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Designation:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblPosition" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Job Code:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblJobCode" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Salary Plan:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblSalPlan" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Business Unit:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblBU" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-4">Employee Id:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="EmployeeID" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Manager Name:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="ManagerName" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Department Name:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblOrgName" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Company:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="LabelCompany" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Grade:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblGrade" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Cost Center:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblCostCenter" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Location:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblLocation" runat="server" class="text-wrap description"></asp:Label>
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
                        <label class="col-xs-4">Telephone Number:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="TelNumber" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Mobile Number:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="MbNumber" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">CSEL:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="csel" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Gold Bar:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="goldBar" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle topBuffer">
    
    <asp:Button runat="server" ID="Cancel" Text="Cancel" CssClass="btn btn-default btn-sm cancel" OnClick="Cancel_Click" />
</div>
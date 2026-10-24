<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Employee.UC.DetailUC" %>

<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12">
            <div class="form-group">
                <div class="col-xs-6">

                    <%--<div class="form-group">
                        <label class="col-xs-4">Identification Number:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="IdentNumber" runat="server" class="description"></asp:Label>
                        </div>
                    </div>--%>

                    <div class="form-group">
                        <label class="col-xs-4">UserName:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblUserName" runat="server" class="description"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4">Display Name:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblDisplayName" runat="server" class="description"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4">Employee Id:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblEmployeeId" runat="server" class="description"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4">Email Id:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblEmailId" runat="server" class="description"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4">Telephone Number:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblTelephone" runat="server" class="description"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4">Position:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblPosition" runat="server" class="description"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4">Supervisor:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblSupervisor" runat="server" class="description"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4">Countersigning Officer:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblCountersigningOfficer" runat="server" class="description"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4">Join Date:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblJoinDate" runat="server" class="description"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4">Last Service Date:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblTerminationDate" runat="server" class="description"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4">Token Number:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblOneKeyId" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-4">First Name:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblFirstName" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Last Name:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblLastName" runat="server" class="description"></asp:Label>
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
                            <asp:Label ID="lblCostCenter" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Company:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblCompany" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Department:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblDepartment" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                   <%-- <div class="form-group" >
                        <label class="col-xs-4">Division:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblDivision" runat="server" class="description"></asp:Label>
                        </div>
                    </div>--%>
                    <div class="form-group">
                        <label class="col-xs-4">Branch:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblBranch" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Section:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblSection" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Unit:</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblUnit" runat="server" class="description"></asp:Label>
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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommonTaskInfoUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.CommonTaskInfoUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/UserSODConflicts.ascx" TagPrefix="uc1" TagName="UserSODConflicts" %>
<%@ Register Src="~/UsrControl/RequestRiskUC.ascx" TagPrefix="uc1" TagName="RequestRiskUC" %>
<%@ Register Src="~/UsrControl/Common/RequestedEntitlementUC.ascx" TagPrefix="uc1" TagName="RequestedEntitlementUC" %>
<%@ Register Src="~/UsrControl/Common/NEmployeeForm.ascx" TagPrefix="uc1" TagName="EmployeeForm" %>
<%@ Register Src="~/UsrControl/Common/AccessRequestEntitlementDetails.ascx" TagPrefix="uc1" TagName="AccessRequestEntitlementDetails" %>
<%@ Register Src="~/UsrControl/Common/ServiceAccountComplianceUC.ascx" TagPrefix="uc1" TagName="ServiceAccountComplianceUC" %>




<div class="table-header clear">
            <h4>Request Details</h4>
        </div>
        <div class="col-xs-12 ">
            <div class="col-xs-4 ">
                <div class="form-group">
                    <label class="col-xs-4">Request Type:</label>
                    <div class="col-xs-7">
                        <asp:Label ID="lblRequestType" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4">Account Type:</label>
                    <div class="col-xs-7">
                        <asp:Label ID="LabelAccountType" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4">Application Name:</label>
                    <div class="col-xs-7">
                        <asp:Label ID="LabelSystemName" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4">Role Name:</label>
                    <div class="col-xs-7">
                        <uc1:RequestedEntitlementUC runat="server" ID="RequestedEntitlementUC" />
                    </div>
                </div>
                 <div class="form-group" id="entitlementInfo" runat="server">
                    <label class="col-xs-4">Entitlements:</label>
                    <div class="col-xs-7">
                        <uc1:AccessRequestEntitlementDetails runat="server" ID="AccessRequestEntitlementDetailsUC" />
                    </div>
                </div>
            </div>
            <div class="col-xs-4 ">
                <div class="form-group">
                    <label class="col-xs-4">Request ID:</label>
                    <div class="col-xs-7">
                        <asp:Label ID="LabelRequestID" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4">Request Reason:</label>
                    <div class="col-xs-7">
                        <asp:Label ID="LabelRequestReason" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4">Valid From:</label>
                    <div class="col-xs-7">
                        <asp:Label ID="LabelValidFrom" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4">Valid To:</label>
                    <div class="col-xs-7">
                        <asp:Label ID="LabelValidTo" runat="server" class="description"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-xs-4 ">

                <uc1:RequestRiskUC runat="server" ID="RequestRiskUC" Visible="false" />
            </div>
        </div>

         <div id="ServiceAccountCompliance"  runat="server" visible="false" >
              <div class="table-header clear">
            <h4>Service Account Compliance</h4>
                 
            <div class="col-xs-9" >
             <uc1:ServiceAccountComplianceUC runat="server" id="ServiceAccountComplianceUC" />
            </div>
         </div>
        </div>

        <div id="EmpDetailsHeader" visible="false" runat="server" class="table-header clear">
            <h4>Employee Detail</h4>
        </div>
        <div class="employeeRequestDetails">
            <%--<div class="disableDiv">&nbsp;</div>--%>
            <uc1:EmployeeForm ID="EMPForm" runat="server" UIMode="Edit" />
        </div>

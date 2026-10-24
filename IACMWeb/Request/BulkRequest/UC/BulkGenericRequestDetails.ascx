<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BulkGenericRequestDetails.ascx.cs" Inherits="DI.Chakra.Web.Request.BulkRequest.UC.BulkGenericRequestDetails" %>

<%@ Register Src="~/UsrControl/Common/UserInfoUC.ascx" TagPrefix="uc1" TagName="UserInfoUC" %> 
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/CurrentAccessUC.ascx" TagPrefix="uc1" TagName="CurrentAccessUC" %>


<style type="text/css">
    .label {
        font-size: 100%;
    }
</style>
<script type="text/javascript">
    function OnClientRequesting(sender, args) {
        var context = args.get_context()
        context["AccessRequestId"] = document.getElementById("<%= hdnADRequest.ClientID %>").value;
        context["IsBulkRequest"] = 1;
    }

    function showWindow() {
        var oWnd = $find("<%= RadWinViewCurrentAccess.ClientID %>");
        oWnd.show();
    }

    function closeWindow() {
        var oWnd = $find("<%= RadWinViewCurrentAccess.ClientID %>");
        oWnd.close();
    }
</script>
<asp:UpdatePanel runat="server">
    <ContentTemplate>

        <div class="content-wrapper">
            <div class="form">
                <div class="form-horizontal left  clearfix">
                    <div class="row-fluid">
                        <div class="table-header clear">
                            <h4>Request Detail(s)</h4>
                        </div>
                        <asp:Panel ID="VPNPanel" Visible="false" runat="server">
                            <div class="col-xs-12 mBottom15">

                                <div class="col-xs-6">

                                    <div class="form-group">
                                        <label class="col-xs-4">Request ID</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="VPNRequestIDLabel" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-xs-4">Request Comments</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="lblAdhoccomments" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <div class="col-md-12">

                                <telerik:RadGrid CssClass="mTop15" ID="VPNApplicationsGrid" runat="server" AllowPaging="false" AllowSorting="false"
                                    CellSpacing="0" GridLines="None">
                                    <GroupingSettings CaseSensitive="false" />
                                    <MasterTableView AutoGenerateColumns="False" Width="100%">
                                        <Columns>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="15%" HeaderText="Application" ItemStyle-HorizontalAlign="Center" UniqueName="MatrixType">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblMatrixType" Text='<%# Eval("MatrixType").ToString() %>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="15%" HeaderText="VPN Type" ItemStyle-HorizontalAlign="Center" UniqueName="VPNType">
                                                <ItemTemplate>
                                                    <h4>
                                                        <asp:Label ID="lblVPNType" CssClass="label label-primary" Text='<%# Convert.ToString(Eval("VPNType")) %>' runat="server"></asp:Label></h4>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Application Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label4" runat="server" Text='<%#Eval("ApplicationName").ToString().Equals("Others",StringComparison.CurrentCultureIgnoreCase) ? 
                                                                    Eval("Remarks").ToString() : Eval("ApplicationName").ToString() %>' />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Production">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" Visible='<%# Convert.ToBoolean(Eval("Production")) %>'>Yes</asp:Label>
                                                    <asp:Label runat="server" Visible='<%# !Convert.ToBoolean(Eval("Production")) %>'>No</asp:Label>

                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="QA">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" Visible='<%# Convert.ToBoolean(Eval("QA")) %>'>Yes</asp:Label>
                                                    <asp:Label runat="server" Visible='<%# !Convert.ToBoolean(Eval("QA")) %>'>No</asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Development">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" Visible='<%# Convert.ToBoolean(Eval("Dev")) %>'>Yes</asp:Label>
                                                    <asp:Label runat="server" Visible='<%# !Convert.ToBoolean(Eval("Dev")) %>'>No</asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>

                                            <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Others">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" Visible='<%# Convert.ToBoolean(Eval("isOthers")) %>'>Yes</asp:Label>
                                                    <asp:Label runat="server" Visible='<%# !Convert.ToBoolean(Eval("isOthers")) %>'>No</asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>

                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>


                                <%--<telerik:RadGrid CssClass="mTop15" ID="OthersGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                    GridLines="None" Width="500px" ShowFooter="false"
                                    MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Others Available">
                                    <GroupingSettings CaseSensitive="false" />
                                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="identity">
                                        <Columns>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="30%" HeaderText="Application Name" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:Label ID="ApplicationNameLabel" runat="server" Text='<%#Eval("ApplicationName").ToString() %>' />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                           <telerik:GridTemplateColumn HeaderStyle-Width="60%" HeaderText="Remarks" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:Label ID="Remarks" runat="server" Text='<%#Eval("Remarks").ToString() %>' />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>--%>
                            </div>

                        </asp:Panel>

                        <asp:Panel ID="GenericPanel" Visible="false" runat="server">
                            <div class="col-xs-12">

                                <div class="col-xs-6">

                                    <div id="trrequest" runat="server" class="form-group">
                                        <label class="col-xs-4">Request ID</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="RequestIDLabel" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="systemname" runat="server" class="form-group">
                                        <label class="col-xs-4">Application Name</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="SystemNameLabel" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="trrequesttype" runat="server" class="form-group">
                                        <label class="col-xs-4">Request Type</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="RequestTypeLabel" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="EntitlementTypetr" runat="server" class="form-group" visible="false">
                                        <label class="col-xs-4">Entitlement Type:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="EntitlementTypeLabel" runat="server" CssClass="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="traccounttype" runat="server" class="form-group">
                                        <label class="col-xs-4">Account Type</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="AccountTypeLabel" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="AccessNametr" runat="server" class="form-group">
                                        <label class="col-xs-4">Access Name</label>
                                        <div class="col-xs-8">
                                             <div class="form-group row">
                                            <div class="col-xs-7" >
                                                <asp:LinkButton ID="AccessNameLabel" runat="server" CssClass="description" ></asp:LinkButton>
                                            </div>
                                            <div class="col-xs-5" >
                                            
                                            <asp:LinkButton CssClass="label label-primary" Text="Technical Role(s)" ID="PositionLinkButton" runat="server" OnClick="PositionLinkButton_Click" Visible="false">
                                            </asp:LinkButton>
                                                </div>
                                                </div>
                                        </div>
                                    </div>

                                </div>

                            </div>




                            <div id="ConfilctsRemarktr" visible="false" runat="server" class="form-group">

                                <label class="col-xs-2" style="padding-left: 25px">Conflicts:</label>


                                <div class="col-xs-8" style="padding-left: 25px;">
                                    <telerik:RadGrid ID="RadGridConflicts" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None">
                                        <GroupingSettings CaseSensitive="false" />
                                        <MasterTableView AutoGenerateColumns="False" Width="100%">
                                            <Columns>
                                                <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="" HeaderText="Remarks" ItemStyle-HorizontalAlign="Left">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>

                                </div>
                            </div>
                            <div id="divEBSMSSLConflictsRemarks" visible="false" runat="server" class="form-group">

                                <label class="col-xs-2" style="padding-left: 25px">Conflicts:</label>


                                <div class="col-xs-12" >
                                    <telerik:RadGrid ID="rgEBSMSSLConflictsRemarks" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" CssClass="RadGrid RadGrid_IACMSkin" Width="100%">
                                        <GroupingSettings CaseSensitive="false" />
                                        <MasterTableView AutoGenerateColumns="False"  Width="100%">
                                            <Columns>
                                                <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="EndpointName" HeaderText="Endpoint Name" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="RequestedRole" HeaderText="Requested Role" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="20%">
                                                </telerik:GridBoundColumn>
                                                 <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="ConflictedRole" HeaderText="Conflicted Role" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="20%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="ConflictRemarks" HeaderText="Conflict Remarks" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="20%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="Source" HeaderText="Source" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%">
                                                </telerik:GridBoundColumn>
                                                 <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="UserWithAccountName" HeaderText="User" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="20%">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                    </MasterTableView>
                                 </telerik:RadGrid>

                                </div>
                            </div>
                            <div id="DivIDMS" runat="server" visible="false" class="col-xs-12 form-group">
                                <br />
                                <div class="col-xs-4" style="width: auto; padding-right: 0px;">
                                    <label class="col-xs-4" style="width: auto;">Entitlement Details:</label>
                                </div>
                                <div class="col-xs-8" style="padding-left: 0px;">
                                    <telerik:RadGrid ID="RadGridIacm" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                        AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                                        Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                                        MasterTableView-EnableNoRecordsTemplate="true" Visible="true">
                                        <GroupingSettings CaseSensitive="false" />
                                        <MasterTableView DataKeyNames="Entitlement" AutoGenerateColumns="False" Width="100%">
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="EntitlementType" HeaderText="Entitlement Type" ItemStyle-HorizontalAlign="Left">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Entitlement" UniqueName="Entitlement" HeaderText="Entitlement" ItemStyle-HorizontalAlign="Left">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>
                                </div>
                                <br />
                            </div>

                            <div id="TRSRMAccessGrid" runat="server" visible="false" class="col-xs-12">
                                <div class="form-group">
                                    <div class="col-xs-2">Entitlement Details:</div>
                                    <div class="col-xs-10">
                                        <telerik:RadGrid ID="SRMObjGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                            GridLines="None" Width="100%" ShowFooter="false"
                                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed" CssClass="RadGridtextWrap" OnDataBound="SRMObjGrid_DataBound">
                                            <GroupingSettings CaseSensitive="false" />
                                            <ClientSettings>
                                                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" ScrollHeight="120px" FrozenColumnsCount="2"></Scrolling>
                                            </ClientSettings>
                                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Entitlement,PurchaseOrg,PurchaseGroup" TableLayout="Fixed" Width="100%">
                                                <Columns>
                                                    <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" SortExpression="Entitlement" HeaderText="Entitlement" UniqueName="Entitlement">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="PurchaseOrg" FilterControlAltText="PurchaseOrg" HeaderText="Purchase Org" SortExpression="PurchaseOrg" UniqueName="PurchaseOrg">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="PurchaseGroup" FilterControlAltText="PurchaseGroup" SortExpression="PurchaseGroup" HeaderText="Purchase Group" UniqueName="PurchaseGroup">
                                                    </telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-12" runat="server" id="divSkypeForBusiness">
                                <div class="form-group">
                                    <div class="col-xs-2">Entitlement Details:</div>
                                    <div class="col-xs-10">
                                        <telerik:RadGrid ID="ResponseGroupGrid" runat="server" AllowPaging="false" AllowSorting="false"
                                            CellSpacing="0" GridLines="None" Visible="false">
                                            <GroupingSettings CaseSensitive="false" />
                                            <MasterTableView AutoGenerateColumns="False" Width="100%">
                                                <Columns>
                                                    <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Response Group Name">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" Text='<%#Eval("Name").ToString() %>' />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Mask">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" Text='<%#Eval("Mask").ToString() %>' />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-12">
                                <div class="col-xs-6">
                                    <div id="SAPEPRequestInfotr" visible="false" runat="server" class="form-group">
                                        <label class="col-xs-4">Request Info</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="RequestInfoLabel" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="LogonUserNametr" runat="server" class="form-group">
                                        <label class="col-xs-4">Logon UserName</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="lblLogonUserName" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="ValidFromRow" runat="server" class="form-group">
                                        <label class="col-xs-4">Valid From</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="lblValidFrom" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="ValidToRow" runat="server" class="form-group">
                                        <label class="col-xs-4">Valid To</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="lblValidTo" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="RequestorCommentsRow" runat="server" visible="false" class="form-group">
                                        <label class="col-xs-4">Reason for Request</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="lblRequestorCommentstxt" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <%-- <div id="ConfilctsRemarktr" runat="server" visible="false" class="form-group">
                                        <label class="col-xs-4">Conflicts Remark</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="ConflictsLabel" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>--%>

                                    <div id="WithdrawCommentsRow" runat="server" visible="false" class="form-group">
                                        <label class="col-xs-4">Withdraw Reason</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="WithdrawReasonLabelLabel" runat="server" CssClass="description label label-danger"></asp:Label>
                                        </div>
                                    </div>
                                    <div id="WithdrawnOnRow" runat="server" visible="false" class="form-group">
                                        <label class="col-xs-4">Withdrawn On</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="WithdrawnOnLabel" runat="server" CssClass="description label label-danger"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="trPortalGrp" runat="server" visible="false" class="form-group">
                                        <label class="col-xs-4">Group</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="lblSelectedGrp" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="trPortalrole" runat="server" visible="false" class="form-group">
                                        <label class="col-xs-4">Role</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="lblSelectedRole" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="trAlloExternalEmail" visible="false" runat="server" class="form-group">
                                        <label class="col-xs-4">Allow External Email</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="lblExternalEmail" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                    <div id="trLicenseAction" visible="false" runat="server" class="form-group">
                                        <label class="col-xs-4">Licenese Action</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="lblLicenseAction" runat="server" class="description"></asp:Label>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <div class="">
                                <div class="col-xs-2">
                                    Request Details:
                                </div>

                                <div id="SMSTR" runat="server" class="col-xs-12">

                                    <telerik:RadGrid ID="SMSObjGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" OnDataBound="SMSObjGrid_DataBound" CellSpacing="0"
                                        GridLines="None" Width="800px" ShowFooter="false"
                                        MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed" CssClass="RadGridtextWrap">
                                        <GroupingSettings CaseSensitive="false" />
                                        <ClientSettings>
                                            <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" ScrollHeight="120px" FrozenColumnsCount="2"></Scrolling>
                                        </ClientSettings>
                                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="TechnicalRole" TableLayout="Fixed" Width="100%">
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="Building" FilterControlAltText="Building" SortExpression="Building" HeaderText="Building" UniqueName="Building" HeaderStyle-Width="100px">
                                                </telerik:GridBoundColumn>

                                                <telerik:GridBoundColumn DataField="Section" FilterControlAltText="Section" HeaderText="Section" SortExpression="Section" UniqueName="Section" HeaderStyle-Width="100px">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="TechnicalRole" FilterControlAltText="TechnicalRole" SortExpression="TechnicalRole" HeaderText="Technical Role" UniqueName="TechnicalRole">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Description" FilterControlAltText="Description" SortExpression="Description" HeaderText="Description" UniqueName="Description">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>

                                </div>
                            </div>

                            <div id="trHRMSFLINS" runat="server" visible="false" class="form-group">
                                <label class="col-xs-2" style="padding-left: 25px">Access Details</label>
                                <div class="col-xs-8">
                                    <telerik:RadGrid ID="HRMSGRID" OnItemDataBound="HRMSGRID_ItemDataBound" OnItemCommand="HRMSGRID_ItemCommand" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None">
                                        <GroupingSettings CaseSensitive="false" />
                                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Entitlement,Company,CostCenter,PurchaseOrg,PurchaseGroup,Plant,StorageLocation,MRPController" Width="100%">
                                            <Columns>
                                                <telerik:GridTemplateColumn HeaderText="Entitlement">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label4" runat="server" Text='<%#Eval("Entitlement") %>' />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Company">
                                                    <ItemTemplate>
                                                        <asp:Label ID="CompanyLabel" runat="server" Text='<%# Eval("Company") %>' />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn UniqueName="IsCompanyRetain" HeaderText="IsCompany Retain">
                                                    <ItemTemplate>
                                                        <asp:Label ID="IsCompanyRetainLabel" runat="server" Text='<%#Convert.ToBoolean(Eval("RetainCompanyRoles")) ? "Yes" : "No" %>' />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Variance">
                                                    <ItemTemplate>
                                                        <asp:Label ID="VarianceLabel" runat="server" />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderStyle-Width="250px" HeaderText="Technical role" ItemStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="ViewTechnicalRolesLinkButton" CommandName="ViewTechicalRole" Text="View Technical Roles" runat="server"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>
                                </div>
                            </div>

                            </br></br>

                                    <div id="trLicenseInfo" runat="server" visible="false">
                                        <label class="col-xs-2">Licenese Info</label>
                                        <div class="col-xs-11">
                                            <telerik:RadGrid ID="ServicePlanGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0" CssClass="RadGridtextWrap"
                                                GridLines="None" ShowFooter="false"
                                                MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed">
                                                <GroupingSettings CaseSensitive="false" />
                                                <MasterTableView AutoGenerateColumns="False" DataKeyNames="AccountSkuId" TableLayout="Fixed" Width="100%">
                                                    <Columns>
                                                        <telerik:GridBoundColumn DataField="AccountSkuId" FilterControlAltText="License" SortExpression="License" HeaderText="License" UniqueName="License" HeaderStyle-Width="300px">
                                                        </telerik:GridBoundColumn>

                                                        <telerik:GridTemplateColumn DataField="IsRemoveLicense" FilterControlAltText="IsRemoveLicense" HeaderText="Is Remove License" SortExpression="IsRemoveLicense" UniqueName="IsRemoveLicense" HeaderStyle-Width="100px">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Label1" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsRemoveLicense"))?"Yes":"No")%>'></asp:Label>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridBoundColumn DataField="ServiceName" FilterControlAltText="ServiceName" SortExpression="ServiceName" HeaderText="Service Plans" UniqueName="ServiceName">
                                                        </telerik:GridBoundColumn>
                                                    </Columns>
                                                </MasterTableView>
                                            </telerik:RadGrid>
                                        </div>
                                    </div>


                            <div id="tblgroup" runat="server" visible="false">

                                <div class="col-xs-12">
                                    <div class="col-xs-6">
                                        <div id="trreqComment" runat="server" visible="false" class="form-group">
                                            <label class="col-xs-4">Comments for Add/Remove Group:</label>
                                            <div class="col-xs-8">
                                                <asp:Label ID="lblRequesterComment" runat="server" CssClass="description"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="trGroups" runat="server" visible="false" class="col-xs-12">
                                    <div class="col-xs-6">
                                        <div class="form-group">
                                            <label class="col-xs-4">Group Name:</label>

                                            <div id="tdgroup" runat="server" visible="false" class="col-xs-8">
                                                <div class="col-xs-10" style="padding-left: 0px !important;">

                                                    <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" CssClass="form-control" runat="server" MinFilterLength="2" EmptyMessage="Please enter at least 2 character to view the list" OnEntryAdded="RolesAutoCompleteBox_EntryAdded" OnClientRequesting="OnClientRequesting" DropDownHeight="200px" DropDownWidth="300px" InputType="Token">
                                                        <WebServiceSettings Method="GetADGroupsByCanRequest" Path="../../../Services/AutoCompleteDataService.svc" />
                                                    </telerik:RadAutoCompleteBox>
                                                    <asp:HiddenField ID="hdnADRequest" runat="server" Value="" />
                                                </div>
                                                <div id="tdbtn" class="col-xs-2" runat="server" visible="false">
                                                    <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click1" Text="Add" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="tdexistinggroup" runat="server" class="col-xs-8 form-group" visible="false">
                                    <telerik:RadComboBox ID="ExistingGroupComboBox" CssClass="form-control" runat="server" CheckBoxes="true" DataTextField="Value" DataValueField="Key" EnableCheckAllItemsCheckBox="true" Width="250px">
                                    </telerik:RadComboBox>
                                </div>



                                <div id="tdgrid" runat="server" visible="false" class="form-group col-xs-12">
                                    <label class="col-xs-4"></label>
                                    <telerik:RadGrid ID="RoleGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None" class="col-xs-5">
                                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Key" Width="100%">
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" HeaderStyle-Width="300px" HeaderText="Role" SortExpression="Value">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridTemplateColumn HeaderStyle-Width="60px">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="DeleteRoleButton" runat="server" CommandArgument='<%#Eval("Key") %>' OnClick="DeleteRoleButton_Click" Text="Delete" ToolTip="Edit" />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>
                                </div>

                                <div id="trDiscoveryGroups" runat="server" visible="false" class="col-xs-12">
                                    <div class="col-xs-6  form-group ">
                                        <label class="col-xs-4"></label>
                                        <div class="col-xs-8">
                                            <asp:Button ID="btnDiscoveryAD" CssClass="btn btn-default btn-sm" runat="server" OnClick="btnDiscoveryAD_Click" Text="Run AD Group Discovery" />
                                            <asp:Button ID="btnRefreshStatus" CssClass="btn btn-default btn-sm" runat="server" OnClick="btnRefreshStatus_Click" Text="Click To Refresh Status" />
                                            <asp:Label CssClass="description label label-danger" ID="lblStatus" runat="server" Text=""></asp:Label>
                                        </div>
                                    </div>
                                </div>

                                <div id="trDiscoveryGroupsNotes" runat="server" visible="false" class="col-xs-12 form-group">
                                    <div class="col-xs-6">
                                        <label class="col-xs-4">Note:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="Label22" runat="server" ForeColor="Red" Text="Run AD Group Discovery Only if New AD Group Is Not Found"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </asp:Panel>

                        <div class="col-xs-12 mTop15">
                            <telerik:RadGrid ID="UserRequestGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                GridLines="None" Width="100%" OnNeedDataSource="UserRequestGrid_NeedDataSource" ShowFooter="false"
                                MasterTableView-EnableNoRecordsTemplate="true" OnItemDataBound="UserRequestGrid_ItemDataBound" MasterTableView-NoDetailRecordsText="No Request Available">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" Width="100%">
                                    <Columns>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="View Current Access" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                 <asp:LinkButton ID="lnkViewCurrentAccess"  Text="View Current Access" runat="server" OnCommand="lnkViewCurrentAccess_Command" CommandName="show" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "User.Id") %>'></asp:LinkButton>                                                                             
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Name" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                 <uc1:UserInfoUC runat="server" ID="UserInfoUC" UserId='<%# Eval("User.Id") %>' />
                                                <%--<asp:Label ID="DisplayName" runat="server" Text='<%# Eval("User.DisplayName") %>' />--%>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="100px" HeaderText="UserName" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="UserName" runat="server" Text='<%# Eval("User.UserName") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="100px" HeaderText="AccountName" ItemStyle-HorizontalAlign="Left" UniqueName="AccountName">
                                            <ItemTemplate>
                                                <asp:Label ID="AccountName" runat="server" Text='<%# Eval("AccountName") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="100px" HeaderText="EmployeeId" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="EmployeeId" runat="server" Text='<%# Eval("User.EmployeeId") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                         <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Position" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="UsrPositionName" runat="server" Text='<%# Eval("User.PositionName") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Company" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="OrgCompany" runat="server" Text='<%# Eval("User.OrgCompany") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Department" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="OrgDepartment" runat="server" Text='<%# Eval("User.OrgDepartment") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                       
                                        <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Branch" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="OrgBranch" runat="server" Text='<%# Eval("User.OrgBranch") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Section" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="OrgSection" runat="server" Text='<%# Eval("User.OrgSection") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Unit" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="OrgUnit" runat="server" Text='<%# Eval("User.Reserved8") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="200px" UniqueName="AccountInfo" HeaderText="Account Info" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="AccountInfo" runat="server" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn UniqueName="OneKeyTokenVPN" HeaderStyle-Width="200px" HeaderText="Token Number" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="OneKeyTokenId" Visible="true" runat="server" Text='<%# Eval("User.OneKeyTokenId") %>' />
                                                <%--  <asp:TextBox ID="txtOneKey" Visible="false" runat="server" Text='<%# Eval("User.OneKeyTokenId") %>' />--%>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn UniqueName="HandPhone" HeaderStyle-Width="75px" HeaderText="Hand Phone" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="HandPhone" runat="server" Text='<%# Eval("HandPhone") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
<telerik:RadWindow runat="server" ID="ViewTechnicalSourceWindow" Modal="true" Height="100%" Width="700px" Title="Techinical Roles"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Technical Roles</h3>
                </div>
                <div class="modal-body">
                    <asp:Panel ID="panelViewTechnicalSource" CssClass="ModalWindow" runat="server">
                        <div class="row">
                        <div class="col-xs-12">
                        <telerik:RadGrid runat="server" ID="ViewTechnicalSource" GridLines="None" AllowAutomaticUpdates="false" Width="100%" EnableAjaxSkinRendering="true" AllowPaging="False" CellSpacing="0">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" FrozenColumnsCount="2"></Scrolling>
                            </ClientSettings>
                            <PagerStyle Mode="NumericPages" />
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" EditFormSettings-PopUpSettings-ScrollBars="Vertical" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Value" HeaderText="Technical Role" HeaderStyle-Width="10%" FilterControlWidth="94%">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                            </div>
                            </div>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlEntitlementRoles">
                        <div class="row"><div class="col-xs-12">&nbsp;</div></div> 
                        <div class="row">
                         <div class="col-xs-12">
                            <telerik:RadGrid ID="grdEntitlements" runat="server">
                            <MasterTableView NoMasterRecordsText="No Records Available" TableLayout="Fixed" AutoGenerateColumns="false" Width="100%">
                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Key" HeaderText="Business Role" UniqueName="Key">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Value" HeaderText="Technical Role" UniqueName="Value">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                       </div>
                        </div>
                    </asp:Panel>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnCancelViewDetails" CssClass="uiboxgray" runat="server" Text="Close" OnClick="btnCancelViewDetails_Click" />
                </div>

            </ContentTemplate>

        </asp:UpdatePanel>

    </ContentTemplate>

</telerik:RadWindow>

 <telerik:RadWindow runat="server" ID="RadWinViewCurrentAccess" Modal="true" Width="900px" Height="100%"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanelViewAccess" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title">Current Access</h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:CurrentAccessUC ID="ucCurrentAccess" runat="server" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnViewAccess" class="btn btn-primary bordered-button" Text="OK" OnClick="btnViewAccess_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

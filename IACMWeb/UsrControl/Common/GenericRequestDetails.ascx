<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GenericRequestDetails.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.GenericRequestDetails" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/ServiceAccountComplianceUC.ascx" TagPrefix="uc1" TagName="ServiceAccountComplianceUC" %>

<style type="text/css">
    .label {
        font-size: 100%;
    }

    .col-xs-9 {
        word-break: break-word !important;
        -ms-word-break: break-word !important;
    }

    .groupWidth {
        min-width: 450px !important;
    }
</style>
<script>
    function OnClientRequesting(sender, args) {
        var context = args.get_context()
        context["AccessRequestId"] = document.getElementById("<%= hdnADRequest.ClientID %>").value;
        context["IsBulkRequest"] = 0;
    }
    function OpenInNewTab(url) {
        var bulkrequest = document.getElementById("<%=bulkrequesthidden.ClientID %>");
        if (bulkrequest != null) {
            url = url + bulkrequest.value;
            var win = window.open(url, '_blank');
            win.focus();
        }
        return false;
    }
</script>

<div class="table-header clear">
    <h4>Request Detail(s)</h4>
</div>
<asp:HiddenField ID="bulkrequesthidden" runat="server" />

<div class="form-horizontal left clearfix">
    <asp:Panel ID="GenericPanel" Visible="false" runat="server">
        <div class="col-xs-12">
            <div class="col-xs-12">
                <div id="trrequest" runat="server" class="form-group row">
                    <label class="col-xs-3">Request ID:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="RequestIDLabel" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="systemname" runat="server" class="form-group row">
                    <label class="col-xs-3">Application Name:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="SystemNameLabel" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="EntitlementTypetr" runat="server" class="form-group row" visible="false">
                    <label class="col-xs-3">Entitlement Type:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="EntitlementTypeLabel" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="AccessNametr" runat="server" class="form-group row">
                    <label class="col-xs-3">Access Name:</label>
                    <div class="col-xs-9">
                        <div class="form-group row">
                        <div class="col-xs-7" >
                            <asp:LinkButton ID="AccessNameLabel" runat="server" CssClass="description" ></asp:LinkButton>
                        </div>
                        <div class="col-xs-5" >
                            <asp:LinkButton CssClass="description" Text="View Technical Role(s)" ID="PositionLinkButton" runat="server" OnClick="PositionLinkButton_Click" Visible="false">
                         </asp:LinkButton>
                        </div>
                        </div>
                        
                    </div>
                </div>
                <div class="form-group row" runat="server" id="divReallocate" visible="false">
                    <label class="col-xs-3">Re-allocate:</label>
                    <div class="col-xs-9">
                        <asp:Label runat="server" ID="lblIsReAllocate" Text="" CssClass="badge badge-secondary"></asp:Label>
                    </div>
                </div>
                <div id="divNPACSection" runat="server" class="form-group row" visible="false">
                    <label class="col-xs-3">Section:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblNPACSection" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="divNPACUnit" runat="server" class="form-group row" visible="false">
                    <label class="col-xs-3">Unit:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblNPACUnit" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="bulkrequestidgeneraltr" runat="server" class="form-group row">
                    <label class="col-xs-3">BulkRequest ID:</label>
                    <div class="col-xs-9">
                        <asp:LinkButton ID="bulkrequestgenerallinkbutton" OnClientClick="return OpenInNewTab('MyBulkRequestDetails.aspx?Id=');" runat="server"></asp:LinkButton>
                    </div>
                </div>

                <div id="traccounttype" runat="server" class="form-group row">
                    <label class="col-xs-3">Account Type:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="AccountTypeLabel" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="trCompanyType" runat="server" visible="false" class="form-group row">
                    <label class="col-xs-3">Company Type:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="CompanyTypeLabel" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="trrequesttype" runat="server" class="form-group row">
                    <label class="col-xs-3">Request Type:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="RequestTypeLabel" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="ValidFromRow" runat="server" class="form-group row">
                    <label class="col-xs-3">Valid From:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblValidFrom" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="ValidToRow" runat="server" class="form-group row">
                    <label class="col-xs-3">Valid To:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblValidTo" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>
                <div id="PositionPrefix" runat="server" class="form-group row" visible="false">
                    <label class="col-xs-3">Prefix:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblPrefix" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel ID="VPNPanel" Visible="false" runat="server">
            <div class="col-xs-12">
                <div class="col-xs-12">


                    <%-- <div id="bulkrequesttr" runat="server" class="form-group row">
                    <label class="col-xs-3">BulkRequest ID:</label>
                    <div class="col-xs-9">
                        <asp:LinkButton ID="BulkRequestIDLinkButton" OnClientClick="return OpenInNewTab('MyBulkRequestDetails.aspx?Id=');" runat="server"></asp:LinkButton>
                    </div>
                </div>--%>
                    <div id="divADID" runat="server" class="form-group row" visible="false">
                        <label class="col-xs-3">AD ID:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblADIDForVPN" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>
                    <div id="ADNametr" runat="server" class="form-group row">
                        <label class="col-xs-3">Token Number:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblADName" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>
                    <div id="divUpdateOneKeyToken" runat="server" class="form-group row" visible="false">
                        <label class="col-xs-3">Update Token Number:</label>
                        <div class="col-xs-9">
                            <h4><span class='<%=UpdateOneKeyToken ?"label label-success":"label label-danger"%>'>
                                <asp:Label ID="lblUpdateOneKeyToken" runat="server" CssClass="description"></asp:Label>
                            </span></h4>
                        </div>
                    </div>
                    <div id="vpnremarkstr" runat="server" class="form-group row">
                        <label class="col-xs-3">Remarks:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="vpnremarks" runat="server" CssClass="description"></asp:Label>

                        </div>
                    </div>
                </div>
            </div>
            <div class="col-xs-12">
                <div id="trSVCVPNQstn" runat="server" visible="false" class="form-group row">
                    <div class="col-xs-3" style="width: auto; padding-right: 0px;">
                        <label class="col-xs-3" style="width: auto;">Service Account Compliance:</label>
                    </div>
                    <div class="col-xs-9">
                        <uc1:ServiceAccountComplianceUC runat="server" ID="svcQstnGridVPN" />
                    </div>
                </div>
                <div class="row" runat="server" id="divVPNEntitlementLabel" visible="true">
                    <div class="col-xs-3">Application/Project</div>
                </div>
                <div style="padding-top: 5px; padding-bottom: 5px;" class="form-group row">
                    <div class="col-xs-12">
                        <telerik:RadGrid ID="VPNApplicationsGrid" runat="server" AllowPaging="false" AllowSorting="false"
                            CellSpacing="0" GridLines="None">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False"  Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Type">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%#Eval("MatrixType").ToString() %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="VPN Type">
                                        <ItemTemplate>
                                            <h4>
                                                <asp:Label CssClass="label label-primary" runat="server" Text='<%#Convert.ToBoolean(Eval("isOthers"))? DI.Chakra.Infrastructure.Enums.VPNType.VDI.ToString():DI.Chakra.Infrastructure.Enums.VPNType.SSLVPNOnly.ToString() %>' /></h4>
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

                                    <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Others" Visible="false" Display="false">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Visible='<%# Convert.ToBoolean(Eval("isOthers")) %>'>Yes</asp:Label>
                                            <asp:Label runat="server" Visible='<%# !Convert.ToBoolean(Eval("isOthers")) %>'>No</asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>

                        <telerik:RadGrid ID="OthersGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" Width="500px" ShowFooter="false"
                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Others Available">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="identity"  Width="100%">
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
                        </telerik:RadGrid>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <div class="col-xs-12">
            <div id="TRSMSAccessGrid" runat="server" visible="false">
                <telerik:RadGrid ID="SMSObjGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                    GridLines="None" Width="100%" ShowFooter="false"
                    MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" OnDataBound="SMSObjGrid_DataBound" TableLayout="Fixed" CssClass="RadGridtextWrap">
                    <GroupingSettings CaseSensitive="false" />
                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" ScrollHeight="80px" FrozenColumnsCount="2"></Scrolling>
                    </ClientSettings>
                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="TechnicalRole" TableLayout="Fixed"  Width="100%">
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

            <div id="TRSRMAccessGrid" runat="server" visible="false">
                <br />
                <telerik:RadGrid ID="SRMObjGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                    GridLines="None" Width="800px" ShowFooter="false" OnItemDataBound="SRMObjGrid_ItemDataBound" OnItemCommand="SRMObjGrid_ItemCommand"
                    MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed" OnDataBound="SRMObjGrid_DataBound">
                    <GroupingSettings CaseSensitive="false" />
                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" ScrollHeight="120px" FrozenColumnsCount="2"></Scrolling>
                    </ClientSettings>
                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="Entitlement,PurchaseOrg,PurchaseGroup,RetainMainRole" TableLayout="Fixed"  Width="100%">
                        <Columns>
                            <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" SortExpression="Entitlement" HeaderText="Entitlement" UniqueName="Entitlement">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="PurchaseOrg" FilterControlAltText="PurchaseOrg" HeaderText="Purchase Org" SortExpression="PurchaseOrg" UniqueName="PurchaseOrg">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="PurchaseGroup" FilterControlAltText="PurchaseGroup" SortExpression="PurchaseGroup" HeaderText="Purchase Group" UniqueName="PurchaseGroup">
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Details" Text="Details" HeaderText="Details"
                                UniqueName="Details" Exportable="false" Visible="false" HeaderStyle-Width="80px" ImageUrl="~/Images/action-buttons/detail.png">
                                <ItemStyle HorizontalAlign="Center" />
                            </telerik:GridButtonColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>

            <div id="TRServiceaAccQstns" runat="server" visible="false" class="col-xs-12">
                <div class="form-group row">
                    <div class="col-xs-12">
                        <label class="form-group">
                            <br />
                            Service Account Compliance:<br />
                        </label>
                    </div>
                </div>
                <div class="form-group row">
                    <uc1:ServiceAccountComplianceUC runat="server" ID="SVCQstnGrid" />
                </div>
            </div>
        </div>
        <div class="col-xs-12 ">
            <br />
        </div>
        <div class="col-xs-12 ">

            <div class="col-xs-12">

                <div id="SAPEPRequestInfotr" runat="server" visible="false" class="form-group row">
                    <label class="col-xs-3">Request Info:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="RequestInfoLabel" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>



                <div id="Remarkstr" runat="server" class="form-group row">
                    <label class="col-xs-3">Remarks:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="RemarksLabel" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="trLicenseAction" runat="server" visible="false" class="form-group row">
                    <label class="col-xs-3">Licenese Action:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblLicenseAction" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="LogonUserNametr" runat="server" class="form-group row">
                    <label class="col-xs-3">Logon UserName:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblLogonUserName" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row" id="divEntitlementType" runat="server" visible="false">
                    <label class="col-xs-3">Entitlement Type:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblEntitlementType" runat="server" CssClass="description label label-primary"></asp:Label>
                    </div>
                </div>
                <div id="trAlloExternalEmail" runat="server" visible="false" class="form-group row">
                    <label class="col-xs-3">Allow External Email:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblExternalEmail" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="trLicenseRmark" runat="server" visible="false" class="form-group row">
                    <label class="col-xs-3">License Remarks:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="LicenseRemarks" runat="server" CssClass="description" ForeColor="Blue"></asp:Label>
                    </div>
                </div>

                <div id="RequestorCommentsRow" runat="server" visible="false" class="form-group row">
                    <label class="col-xs-3">Reason for Request:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblRequestorCommentstxt" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="WithdrawCommentsRow" runat="server" visible="false" class="form-group row">
                    <label class="col-xs-3">Withdraw Reason:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="WithdrawReasonLabelLabel" runat="server" CssClass="description label label-danger"></asp:Label>
                    </div>
                </div>

                <div id="WithdrawnOnRow" runat="server" visible="false" class="form-group row">
                    <label class="col-xs-3">Withdrawn On:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="WithdrawnOnLabel" runat="server" CssClass="description label label-danger"></asp:Label>
                    </div>
                </div>

                <div id="trMailboxConversionType" runat="server" visible="false" class="form-group row">
                    <label id="displayMBCT" runat="server" class="col-xs-3">Mailbox Conversion Type:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblMailboxConversion" runat="server" CssClass="description"></asp:Label>
                    </div>

                </div>

                <div id="trSharedMailboxactionType" runat="server" visible="false" class="form-group row">
                    <label class="col-xs-3">Mailbox Action:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblSharedMailboxAction" runat="server" CssClass="description"></asp:Label>
                    </div>

                </div>

                <%-- <div id="ConfilctsRemarktr" visible="false" runat="server" class="form-group">
                    <label class="col-xs-4">Conflicts Remark:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="ConflictsLabel" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>--%>

                <div id="trPortalGrp" visible="false" runat="server" class="form-group row">
                    <label class="col-xs-3">Group:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblSelectedGrp" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div id="trPortalrole" visible="false" runat="server" class="form-group row">
                    <label class="col-xs-3">Role:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblSelectedRole" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>
                <div id="divPartiallyAutomateInfo" visible="false" runat="server" class="form-group row">
                    <label class="col-xs-3">Groups/Roles info:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblGroupRoleInfo" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>
            </div>
        </div>

        <br />

        <div id="ConfilctsRemarktr" visible="false" runat="server" class="col-xs-12 form-group row">
            <div class="col-xs-3" style="width: auto; padding-right: 75px;">
                <label style="width: auto;">Conflicts:</label>
            </div>

            <div class="col-xs-9" style="padding-left: 35px;">
                <telerik:RadGrid ID="RadGridConflicts" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None">
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView AutoGenerateColumns="False"  Width="100%">
                        <Columns>
                            <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="" HeaderText="Remarks" ItemStyle-HorizontalAlign="Left">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>

            </div>

        </div>
         <div id="divEBSMSSLConflictsRemarks" visible="false" runat="server" class="col-xs-12 form-group row">
            <div class="col-xs-3" style="width: auto; padding-right: 75px;">
                <label style="width: auto;">Conflicts:</label>
            </div>
            <div class="col-xs-12">
                <telerik:RadGrid ID="rgEBSMSSLConflictsRemarks" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" CssClass="RadGrid RadGrid_IACMSkin" Width="100%">
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView AutoGenerateColumns="False"  Width="100%">
                        <Columns>
                            <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="EndpointName" HeaderText="Endpoint Name" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="RequestedRole" HeaderText="Requested Role" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="30%">
                            </telerik:GridBoundColumn>
                             <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="ConflictedRole" HeaderText="Conflicted Role" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="30%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="ConflictRemarks" HeaderText="Conflict Remarks" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="20%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn ItemStyle-ForeColor="Maroon" DataField="Source" HeaderText="Source" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>

        </div>
        <div class="col-xs-12 ">

            <div id="DivIDMS" runat="server" visible="false" class="col-xs-12 form-group row">
                <br />
                <div class="col-xs-3">
                    <label>Entitlement Details:</label>
                </div>
                <div class="col-xs-9" style="padding-left: 0px;">
                    <telerik:RadGrid ID="RadGridIacm" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                        AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                        Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                        MasterTableView-EnableNoRecordsTemplate="true" Visible="true">
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView DataKeyNames="Entitlement" AutoGenerateColumns="False"  Width="100%">
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
        </div>
        <div class="col-xs-12 ">
        <div id="trLicenseInfo" runat="server" visible="false" class="col-xs-12   form-group row">
            <label class="col-xs-3">Licenese Info:</label>
            <div class="col-xs-9">
                <telerik:RadGrid ID="ServicePlanGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                    GridLines="None" ShowFooter="false"
                    MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed">
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="AccountSkuId" TableLayout="Fixed"  Width="100%">
                        <Columns>
                            <telerik:GridBoundColumn DataField="LicenseDescription" FilterControlAltText="LicenseDescription" SortExpression="LicenseDescription" HeaderText="License" UniqueName="LicenseDescription" HeaderStyle-Width="300px">
                            </telerik:GridBoundColumn>

                            <telerik:GridTemplateColumn DataField="IsRemoveLicense" FilterControlAltText="IsRemoveLicense" HeaderText="Is Remove License" SortExpression="IsRemoveLicense" UniqueName="IsRemoveLicense" HeaderStyle-Width="100px">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsRemoveLicense"))?"Yes":"No")%>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="ServiceDescription" FilterControlAltText="ServiceDescription" SortExpression="ServiceDescription" HeaderText="Service Plans" UniqueName="ServiceDescription">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </div>
        </div>
        <br />
        <div id="trMembers" runat="server" visible="false" class="col-xs-12 form-group row">
            <div class="col-xs-3" style="width: auto; padding-right: 75px;">
                <label>Members:</label>
            </div>
            <div class="col-xs-9" style="padding-left: 0px;">
                <telerik:RadGrid ID="MembersGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                    GridLines="None" ShowFooter="false" MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed">
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView AutoGenerateColumns="False" TableLayout="Fixed"  Width="100%">
                        <Columns>
                            <telerik:GridBoundColumn DataField="Members" FilterControlAltText="Members" SortExpression="Members" HeaderText="Members" UniqueName="Members" HeaderStyle-Width="300px">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </div>
        <br />
        <div id="trHRMSFLINS" visible="false" runat="server" class="col-xs-12 form-group row">
            <div class="col-xs-3" style="width: auto; padding-right: 75px;">
                <label>Access Details:</label>
            </div>
            <div class="col-xs-9" style="padding-left: 0px;">
                <telerik:RadGrid ID="HRMSGRID" OnItemDataBound="HRMSGRID_ItemDataBound" OnItemCommand="HRMSGRID_ItemCommand" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None">
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="Entitlement,Company,CostCenter,PurchaseOrg,PurchaseGroup,Plant,StorageLocation,MRPController"  Width="100%">
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

        <asp:Panel ID="pnlShareAccountDetails" runat="server" Visible="false">
            <div class="col-xs-12">
                <div class="col-xs-12">

                    <div id="SAPUserTypediv" runat="server" visible="false" class="form-group row">
                        <label class="col-xs-3">SAP UserType:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="SAPUserTypeLabel" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>

                    <div id="AccountNamediv" runat="server" class="form-group row">
                        <label class="col-xs-3">Account Name:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblAccountName" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>

                    <div id="DisplayNamediv" runat="server" class="form-group row">
                        <label class="col-xs-3">Display Name:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblDisplayName" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>

                    <div id="AliasNamediv" runat="server" visible="false" class="form-group row">
                        <label class="col-xs-3">Alias Name:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblAliasName" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>

                    <div id="Descriptiondiv" runat="server" class="form-group row">
                        <label class="col-xs-3">Description:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblDescription" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>

                    <div runat="server" id="Oudiv" class="form-group row">
                        <label id="lblOUCaption" runat="server" class="col-xs-3">OU:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblOU" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>

                </div>

            </div>
        </asp:Panel>
        <div class="col-xs-12">
            <div runat="server" id="divEntitlements" visible="false">
                <label id="Label2" class="col-xs-3">Entitlements</label>
                <div class="col-xs-9">
                    <telerik:RadGrid ID="ADEntitlements" Width="60%" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None" class="col-xs-3">
                        <MasterTableView AutoGenerateColumns="False"  Width="100%">
                            <Columns>
                                <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" HeaderStyle-Width="300px" HeaderText="Role" SortExpression="Value">
                                </telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </div>
        </div>
        <asp:Panel ID="pnlOffice365" runat="server">
            <table id="Table1" runat="server" class="table_results" visible="false" style="width: 100%">
                <tr>
                    <td></td>
                </tr>

            </table>
        </asp:Panel>
        <div class="col-xs-12" id="pnlSkypeForBusiness" runat="server" visible="false">
            <div class="row">
                <div class="col-xs-12">
                    <div class="col-xs-12">
                        <div class="form-group row">
                            <label class="col-xs-3">Present LineURI:</label>
                            <div class="col-xs-9">
                                <asp:Label ID="userLineURI" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row" id="divPolicy" runat="server" visible="false">
                <div class="col-xs-12">
                    <div>
                        <label class="col-xs-3">Requested LineURI</label>
                        <div class="col-xs-9" style="padding-left: 10px;">
                            <asp:Label ID="lblLineURI" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>
                </div>

            </div>

            <div class="row">
                <div class="col-xs-12">
                    <asp:Panel ID="pnlResponseGroup" runat="server" Visible="false">
                        <telerik:RadGrid ID="ResponseGroupGrid" runat="server" AllowPaging="false" AllowSorting="false" onitemcommand="ResponseGroupGrid_ItemCommand"
                            CellSpacing="0" GridLines="None">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False"  Width="100%" DataKeyNames="Id,Name">
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
                                    <telerik:GridTemplateColumn HeaderText="View Member(s)" ItemStyle-HorizontalAlign="Left"  HeaderStyle-Width="15%">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="ViewGroupNameLinkButton" CommandName="View" Text="View" runat="server"></asp:LinkButton>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                    <%-- <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="ADID">
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%#Eval("ADID") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>--%>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <div class="col-xs-12" id="pnlNova" runat="server" visible="false">
            <div class="row">
                <div class="col-xs-12">
                    <telerik:RadGrid ID="NovaRoleGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                        GridLines="None" ShowFooter="false"
                        MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" tablelayout="Fixed" CssClass="RadGridtextWrap">
                        <GroupingSettings CaseSensitive="false" />
                        <ClientSettings>
                            <Scrolling AllowScroll="true" ScrollHeight="80px" UseStaticHeaders="true" SaveScrollPosition="True" />
                        </ClientSettings>
                        <MasterTableView AutoGenerateColumns="false" DataKeyNames="Id" TableLayout="Fixed"  Width="100%">
                            <Columns>
                                <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="NovaMatrixType" HeaderText="Type" ItemStyle-HorizontalAlign="Center" UniqueName="NovaMatrixType">
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderStyle-Width="15%" DataField="JobDescription" HeaderText="Description" ItemStyle-HorizontalAlign="Center" UniqueName="JobDescription">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblRoleName" Text='<%# Convert.ToString(Eval("JobDescription"))+"("+Convert.ToString(Eval("RoleName"))+")" %>'></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="CompanyCode" HeaderText="CompanyCode" ItemStyle-HorizontalAlign="Center" UniqueName="CompanyCode">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="SectionCode" HeaderText="SectionCode" ItemStyle-HorizontalAlign="Center" UniqueName="SectionCode">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="DepartmentCode" HeaderText="DepartmentCode" ItemStyle-HorizontalAlign="Center" UniqueName="DepartmentCode">
                                </telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </div>
        </div>
        <div id="tblgroup" runat="server" visible="false">

            <div class="col-xs-12">
                <div class="col-xs-12">
                    <div id="trreqComment" runat="server" visible="false" class="form-group row">
                        <label class="col-xs-3">Comments for Add/Remove Group:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblRequesterComment" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>

            <div id="trGroups" runat="server" visible="false" class="col-xs-12">
                <div class="col-xs-12">
                    <div class="form-group row">
                        <label class="col-xs-3">Group Name:</label>

                        <div id="tdgroup" runat="server" visible="false" class="col-xs-9">
                            <div class="col-xs-12" style="padding-left: 0px !important;">
                                <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" MinFilterLength="2" OnClientEntryAdding="restrictDuplicateEntry" HighlightFirstMatch="false" ShowLoadingIcon="true" CssClass="form-control" runat="server" 
                                    EmptyMessage="Please enter at least 2 character to view the list" OnEntryAdded="RolesAutoCompleteBox_EntryAdded" DropDownHeight="200px" DropDownWidth="300px" InputType="Token"
                                    OnClientRequesting="OnClientRequesting"
                                    >
                                    <WebServiceSettings Method="GetADGroupsByCanRequest" Path="../../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                                <asp:HiddenField ID="hdnADRequest" runat="server" Value=""/>
                            </div>
                            <div id="tdbtn" class="col-xs-2" runat="server" visible="false">
                                <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click1" Text="Add" />
                            </div>

                        </div>
                    </div>
                </div>
            </div>
            <div id="tdexistinggroup" runat="server" class="col-xs-12" visible="false">
                <div class="col-xs-12">
                    <div class="form-group row">
                        <label class="col-xs-3">Existing Groups:</label>
                        <div class="col-xs-9">
                            <telerik:RadComboBox ID="ExistingGroupComboBox" CssClass="form-control groupWidth" runat="server" CheckBoxes="true" DataTextField="Value" DataValueField="Key" EnableCheckAllItemsCheckBox="true">
                            </telerik:RadComboBox>
                        </div>
                    </div>
                </div>
            </div>

            <div id="tdgrid" runat="server" visible="false" class="form-group row col-xs-12">
                <label class="col-xs-3"></label>
                <telerik:RadGrid ID="RoleGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None" class="col-xs-3">
                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="Key"  Width="100%">
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
                <div class=" form-group row ">
                    <label class="col-xs-2"></label>
                    <div class="col-xs-6">
                        <asp:Button ID="btnDiscoveryAD" CssClass="btn btn-default btn-sm" runat="server" OnClick="btnDiscoveryAD_Click" Text="Run AD Group Discovery" />
                        <asp:Button ID="btnRefreshStatus" CssClass="btn btn-default btn-sm" runat="server" OnClick="btnRefreshStatus_Click" Text="Click To Refresh Status" />
                        <asp:Label CssClass="description label label-danger" ID="lblStatus" runat="server" Text=""></asp:Label>
                        <%--<asp:Image ID="imgStatus" runat="server" Height="15px" Visible="false" />--%>
                    </div>
                    
                </div>
            </div>

            <div id="trDiscoveryGroupsNotes" runat="server" visible="false" class="col-xs-12 form-group row">
                <div class="col-xs-12">
                    <label class="col-xs-2">Note:</label>
                    <div class="col-xs-10">
                        <asp:Label ID="Label22" runat="server" ForeColor="Red" Text="Run AD Group Discovery Only if New AD Group Is Not Found"></asp:Label>
                    </div>
                </div>
            </div>


        </div>
    </asp:Panel>
    <asp:Panel ID="pnlNonEmployee" Visible="false" runat="server">
        <div class="col-xs-12">
            <div class="col-xs-12">
                <div class="form-group row">
                    <label class="col-xs-3">User Name:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblNonEmployeeName" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-3">Employee Type:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblEmployeeType" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-3">Project/Description:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblProject" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-3">Display Name:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblOldDisplayName" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row" runat="server" id="divNewDisplayName" visible="false">
                    <label class="col-xs-3">New DisplayName:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblNewDisplayName" runat="server" CssClass="description" ForeColor="Red"></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-3">Join Date:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblJoinDate" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>

                <div class="form-group row">
                    <label class="col-xs-3">Last Service Date:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblTerminationDate" runat="server" CssClass="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row" runat="server" id="divNewTerminationDate" visible="false">
                    <label class="col-xs-3">New Last Service Date:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblNewTerminationDate" runat="server" CssClass="description" ForeColor="Red"></asp:Label>
                    </div>
                </div>
                <asp:Panel ID="pnlWReason" runat="server" Visible="false" CssClass="form-group row">
                    <label class="col-xs-3">Withdraw Reason:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblNEWReason" runat="server" CssClass="description label label-danger"></asp:Label>
                    </div>
                </asp:Panel>

                <asp:Panel id="pnlWDate" runat="server" Visible="false" CssClass="form-group row">
                    <label class="col-xs-3">Withdrawn On:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblNPWithDate" runat="server" CssClass="description label label-danger"></asp:Label>
                    </div>
                </asp:Panel>
            </div>
            <asp:Panel runat="server" ID="pnlUserInfo" Visible="false" CssClass="col-xs-12">
                   <div class="form-group row">
                    <label class="col-xs-3">Supervisor:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblRO" runat="server" CssClass="description"></asp:Label>
                    </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-xs-3">Countersigning Officer::</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblCO" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>
                   <div class="form-group row">
                        <label class="col-xs-3">Organisation:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblOrganisation" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-xs-3">Cost Center:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblCostCenter" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                     <div class="form-group row">
                        <label class="col-xs-3">Company:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="LabelCompany" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-xs-3">Department:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblOrgName" runat="server" class="description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-xs-3">Branch:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblBranch" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-xs-3">Section:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblSection" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-xs-3">Unit:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="lblUnit" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                     <div class="form-group row">
                        <label class="col-xs-3">Mobile Number:</label>
                        <div class="col-xs-9">
                            <asp:Label ID="MbNumber" runat="server" class="text-wrap description"></asp:Label>
                        </div>
                    </div>
                </asp:Panel>
        </div>
    </asp:Panel>
</div>
<telerik:RadWindow runat="server" ID="detailWindow" Modal="true" Title="Detail" Width="1000px" Height="200%"
    RenderMode="Lightweight" Behaviors="Close" DestroyOnClose="true" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Details11:</h3>
                </div>

                <div class="modal-body">
                    <div class="row-fluid">
                        <asp:Label ID="lblInformation" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnDetailOk" class="btn btn-primary bordered-button" Text="Close" OnClick="btnDetailOk_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
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
                        <telerik:RadGrid runat="server" ID="ViewTechnicalSource" EnableViewState="true" GridLines="None" AllowAutomaticUpdates="false" Width="100%" EnableAjaxSkinRendering="true" AllowPaging="False" CellSpacing="0">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" FrozenColumnsCount="2"></Scrolling>
                            </ClientSettings>
                            <PagerStyle Mode="NumericPages" />
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" EditFormSettings-PopUpSettings-ScrollBars="Vertical"  Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Value" HeaderText="Technical Role" HeaderStyle-Width="10%" FilterControlWidth="94%">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                             </div>
                             <div class="row">
                        <div class="col-xs-12">&nbsp;
                          </div>
                                 </div>
                    </div>
                    </asp:Panel>
                    
                    <asp:Panel ID="pnlEntitlementRole" runat="server">
                        <div class="row">
                        <div class="col-xs-12">
                        <telerik:RadGrid ID="grdEntitlements" runat="server" >
                            <MasterTableView NoMasterRecordsText="No Records Available" TableLayout="Fixed" AutoGenerateColumns="false"  Width="100%">
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
<telerik:RadWindow runat="server" ID="ViewRoleGroupUserWindow" Modal="true" Height="100%" Width="700px" Title="RoleGroup User"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title"><asp:Label runat="server" ID="lblGrpName"></asp:Label>&nbsp; Member(s)</h3>
                </div>
                <div class="modal-body">
                    <asp:Panel ID="panel1" CssClass="ModalWindow" runat="server">
                        <telerik:RadGrid runat="server" ID="ViewRoleGroupUserSource" GridLines="None" AllowAutomaticUpdates="false" Width="100%" EnableAjaxSkinRendering="true" AllowPaging="False" CellSpacing="0">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" FrozenColumnsCount="2"></Scrolling>
                            </ClientSettings>
                            <PagerStyle Mode="NumericPages" />
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" EditFormSettings-PopUpSettings-ScrollBars="Vertical" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="UserPrincipalName" HeaderText="UserPrincipalName" HeaderStyle-Width="10%" FilterControlWidth="94%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="LastName" HeaderText="Name" HeaderStyle-Width="10%" FilterControlWidth="94%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="LineURI" HeaderText="LineURI" HeaderStyle-Width="10%" FilterControlWidth="94%">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </asp:Panel>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnCancel" CssClass="uiboxgray" runat="server" Text="Close" OnClick="btnCancel_Click" />
                </div>

            </ContentTemplate>

        </asp:UpdatePanel>

    </ContentTemplate>

</telerik:RadWindow>

<script>
    function showSRMWindow() {
        var oWnd = $find("<%= detailWindow.ClientID %>");
        oWnd.show();
    }

    function closeSRMWindow() {
        var oWnd = $find("<%= detailWindow.ClientID %>");
        oWnd.close();
    }
    function restrictDuplicateEntry(sender, eventArgs) {
        var entries = sender.get_entries(),
            count = entries.get_count();

        for (var i = 0; i < count; i++) {
            if (entries.getEntry(i).get_text() == eventArgs.get_entry().get_text()) {
                eventArgs.set_cancel(true);
                return;
            }
        }
    }
</script>

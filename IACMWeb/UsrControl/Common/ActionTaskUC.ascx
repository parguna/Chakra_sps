<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActionTaskUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.ActionTaskUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/Task/InfoUC.ascx" TagPrefix="uc1" TagName="InfoUC" %>
<%@ Register Src="~/UsrControl/Common/CommonTaskInfoUC.ascx" TagPrefix="uc1" TagName="CommonTaskInfoUC" %>
<%@ Register Src="~/UsrControl/Common/ApprverChain.ascx" TagPrefix="uc1" TagName="ApprverChain" %>
<%@ Register Src="~/UsrControl/Common/Task/Applications/InhouseTaskInfoUC.ascx" TagPrefix="uc1" TagName="InhouseTaskInfoUC" %>
<%@ Register Src="~/UsrControl/Common/Task/Applications/InhouseMockTaskInfoUC.ascx" TagPrefix="uc1" TagName="InhouseMockTaskInfoUC" %>
<%@ Register Src="~/UsrControl/Common/SendNotification.ascx" TagPrefix="uc1" TagName="SendNotificationUC" %>
<%@ Register Src="~/UsrControl/Common/ViewNotification.ascx" TagPrefix="uc1" TagName="ViewNotificationUC" %>
<%@ Register Src="~/UsrControl/Common/NoteInfoUC.ascx" TagPrefix="uc2" TagName="NoteInfoUC" %>
<%@ Register Src="~/UsrControl/Common/GenericRequestDetails.ascx" TagPrefix="uc1" TagName="GenericRequestDetails" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/UsrControl/Common/ConfirmBox.ascx" TagPrefix="uc1" TagName="ConfirmMessage" %>

<script type="text/javascript">

    function OnClientRequestingReservedNumber(sender, args) {
        var context = args.get_context()
        context["endpointId"] = "<%= EndpointSystemId %>";
    }
    function OnClientRequesting(sender, args) {
        var context = args.get_context()
        context["endpointId"] = "<%= EndpointSystemId %>";
    }
    function OnClientRequestingForSAPCE(sender, args) {
        var context = args.get_context()
        context["endpointSystemId"] = "<%= EndpointSystemId %>";
         context["endpointUserId"] = document.getElementById("<%= hdnEndpointUserId.ClientID %>").value;
         context["actionType"]= document.getElementById("<%= hdnActionType.ClientID %>").value;
    }
</script>

<style>
     .col-common-width{
        width:140px !important;
    }
    .modal-body .form-group {
        margin-bottom: 0.3em !important;
    }

    .desc-textwrap {
        word-wrap: break-word;
    }

    .RadComboBoxFullWidth {
        width: 100% !important;
    }

    .clear-padding {
        padding-left: 0px !important;
    }

    .clear-margin {
        margin-left: -15px !important;
    }

    .WordWrapTask {
       
        word-wrap: break-word !important;
        min-width:140px !important;
    }

</style>

<asp:UpdatePanel ID="ConfirmMessageUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <uc1:ConfirmMessage ID="ConfirmMessage1" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <script>

            function OnClientValidationFailed(sender, args) {
                var fileExtention = args.get_fileName().substring(args.get_fileName().lastIndexOf('.') + 1, args.get_fileName().length);
                if (args.get_fileName().lastIndexOf('.') != -1) {//this checks if the extension is correct
                    if (sender.get_allowedFileExtensions().indexOf(fileExtention) == -1) {
                        showAlert("Wrong Extension!", "Error");
                    }
                    else {
                        showAlert("Wrong file size!", "Error");
                    }
                }
                else {
                    showAlert("not correct extension!", "Error");
                }
                setTimeout(function () { deleteInvalid(sender) }, 250);//wait for the file to actually be added to the async uploads array before trying to delete it
            }

            function deleteInvalid(sender) {
                var invalidIndexes = [];
                $telerik.$(".ruUploadSuccess, .ruUploadFailure", sender.get_element()).each(function (index) {
                    if ($telerik.$(this).hasClass('ruUploadFailure')) {
                        invalidIndexes.push(index);
                    }
                });
                while (invalidIndexes.length) {
                    sender.deleteFileInputAt(invalidIndexes.pop());
                }
            }
        </script>

        <style>
            /*body
            {
                height: 1950px !important;
            }*/
        </style>

        <div class="table-header clear">
            <h4><%= T("App.Request.TaskDetails") %></h4>
        </div>
        <div class="form-group clear-margin">
            <uc1:InfoUC runat="server" ID="InfoUC" />
        </div>
        <uc1:CommonTaskInfoUC runat="server" ID="CommonTaskInfoUC" canLoadAccessReqItems="false" Visible="false" />
        <uc1:GenericRequestDetails runat="server" ID="GenericRequestDetailsControl" />

        <asp:HiddenField runat="server"  ID="hdnActionType"/>
        <asp:HiddenField runat="server" ID="hdnEndpointUserId" />

        <asp:Panel ID="pnlInhouse" Visible="false" runat="server">
            <div id="InhAccountDetailsHeader" visible="false" runat="server" class="table-header clear">
                <h4>Account Detail</h4>
            </div>
            <uc1:InhouseTaskInfoUC runat="server" ID="InhouseTaskInfoUC" />
        </asp:Panel>

        <asp:Panel ID="pnlMock" Visible="false" runat="server">
            <div id="inhMockHeader" visible="false" runat="server" class="table-header clear">
                <h4>Instruction Detail</h4>
            </div>
            <uc1:InhouseMockTaskInfoUC runat="server" ID="InhouseMockTaskInfoUC" />
        </asp:Panel>

        <uc2:NoteInfoUC runat="server" ID="NoteInfoUC" />

        <div class="table-header clear">
            <h4><%= T("App.Common.Approvals") %></h4>
        </div>

        <div class="col-xs-12">
            <div class="row" style="margin-bottom: 10px;">
                <div class="col-xs-12">
                    <telerik:RadGrid ID="grdApprover" runat="server" AllowPaging="false" AllowSorting="false"  RetainExpandStateOnRebind="true" EnableHierarchyExpandAll="true" OnPreRender="grdApprover_PreRender"
                        CellSpacing="0" GridLines="None" Width="100%" OnDetailTableDataBind="grdApprover_DetailTableDataBind" OnItemCommand="grdApprover_ItemCommand" >
                        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="20" />
                        <ExportSettings
                            HideStructureColumns="true"
                            IgnorePaging="true" ExportOnlyData="true"
                            Excel-Format="ExcelML"
                            OpenInNewWindow="true">
                            <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="297mm" DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                                BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
                            </Pdf>
                        </ExportSettings>
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" TableLayout="Auto" NoMasterRecordsText="No previous approvals" DataKeyNames="ApprovalState,Action"  Width="100%">
                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="false" ShowExportToCsvButton="false"></CommandItemSettings>
                            <DetailTables>
                                <telerik:GridTableView AutoGenerateColumns="false"  EnableHierarchyExpandAll="true" HierarchyDefaultExpanded="true"    ShowHeader="true" Width="100%"
                                    runat="server" Name="Entitlements" NoDetailRecordsText="No records found." AllowPaging="false" AllowFilteringByColumn="false">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="TaskOwner" FilterControlAltText="TaskOwner" SortExpression="TaskOwner" HeaderText="Approver Name" UniqueName="TaskOwner" HeaderStyle-Width="15%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Status"  FilterControlAltText="Status" SortExpression="Status" ItemStyle-CssClass="WordWrapTask" HeaderText="Status" UniqueName="Status" HeaderStyle-Width="5%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="ActionDate" FilterControlAltText="ActionDate" SortExpression="ActionDate" HeaderText="ActionDate"  UniqueName="ActionDate" HeaderStyle-Width="13%" ItemStyle-CssClass="WordWrapTask">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Comments" FilterControlAltText="Comments" SortExpression="Comments" ItemStyle-CssClass="WordWrapTask" HeaderText="Comments" UniqueName="Comments" HeaderStyle-Width="25%">
                                        </telerik:GridBoundColumn>
                                         <telerik:GridTemplateColumn  DataField="Remarks" FilterControlAltText="Remarks" SortExpression="Remarks" HeaderText="System Remarks" UniqueName="Remarks" HeaderStyle-Width="13%"  ItemStyle-CssClass="ApproverCommonWidth">
                                                                    <ItemTemplate>
                                                                <asp:Label runat="server" ID="lblDisplayName" Text="View Remarks" Font-Bold="true" ForeColor="#0066ff" Width="100%" Visible='<%# !string.IsNullOrWhiteSpace ( Convert.ToString( Eval("Remarks")) )  %>'></asp:Label>
                                                                <asp:HoverMenuExtender ID="hoverDisplayName" runat="server" PopupControlID="pnlDescription" PopupPosition="Center" TargetControlID="lblDisplayName"></asp:HoverMenuExtender>
                                                                <asp:Panel ID="pnlDescription" runat="server" CssClass="HoverPopupMsslEBS">
                                                                    <%# Eval("Remarks") %>
                                                                </asp:Panel>
                                                            </ItemTemplate>
                                                                  
                                                                </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="DelegatedFrom" FilterControlAltText="DelegatedFrom" SortExpression="DelegatedFrom" HeaderText="Delegated From" UniqueName="DelegatedFrom" HeaderStyle-Width="10%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DelegatedBy" FilterControlAltText="DelegatedBy" SortExpression="DelegatedBy" ItemStyle-CssClass="WordWrapTask" HeaderText="Delegated By" UniqueName="DelegatedBy" HeaderStyle-Width="10%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DelegationComments" FilterControlAltText="DelegationComments" SortExpression="DelegationComments" ItemStyle-CssClass="WordWrapTask" HeaderText="Delegation Comments" UniqueName="DelegationComments" HeaderStyle-Width="22%">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </telerik:GridTableView>
                            </DetailTables>
                            <Columns>
                                <telerik:GridBoundColumn DataField="ApproverName" FilterControlAltText="ApproverName" SortExpression="ApproverName" HeaderText="ApproverName" UniqueName="ApproverName">
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Approver Role" SortExpression="ApproverType" UniqueName="ApproverType">
                                    <ItemTemplate>
                                        <asp:Label ID="lblApprover" runat="server" Text='<%#Eval("ApproverType") != null && Eval("ApproverType").ToString() == "Group - NPAC Approver"?"Group - NPAC/NMAC/PROTIS/ENIS Approver":Eval("ApproverType") %>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="Type" FilterControlAltText="Type" SortExpression="Type" HeaderText="Type" UniqueName="Type">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status">
                                </telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </div>
        </div>

        <%--<div id="divExecutionerHeader" runat="server" class="table-header clear">
            <h4><%= T("App.Common.Executioner") %></h4>
        </div>

        <div id="divExecutionerGrid" runat="server" class="col-xs-12">
            <div class="row" style="margin-bottom: 10px;">
                <div class="col-xs-12">
                    <telerik:RadGrid ID="grdExecutioner" runat="server" AllowPaging="false" AllowSorting="false"
                        CellSpacing="0" GridLines="None" Width="100%" OnDetailTableDataBind="grdApprover_DetailTableDataBind">
                        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="20" />
                        <ExportSettings
                            HideStructureColumns="true"
                            IgnorePaging="true" ExportOnlyData="true"
                            Excel-Format="ExcelML"
                            OpenInNewWindow="true">
                            <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="297mm" DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                                BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
                            </Pdf>
                        </ExportSettings>
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" NoMasterRecordsText="No previous approvals" DataKeyNames="ApprovalState">
                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="false" ShowExportToCsvButton="false"></CommandItemSettings>
                            <DetailTables>
                                <telerik:GridTableView AutoGenerateColumns="false" EnableHierarchyExpandAll="true" ShowHeader="true" Width="100%"
                                    runat="server" Name="Entitlements" NoDetailRecordsText="No records found." AllowPaging="false" AllowFilteringByColumn="false">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="TaskOwner" FilterControlAltText="TaskOwner" SortExpression="TaskOwner" HeaderText="Approver Name" UniqueName="TaskOwner">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="ActionDate" FilterControlAltText="ActionDate" SortExpression="ActionDate" HeaderText="ActionDate" UniqueName="ActionDate">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Comments" FilterControlAltText="Comments" SortExpression="Comments" HeaderText="Comments" UniqueName="Comments">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DelegatedFrom" FilterControlAltText="DelegatedFrom" SortExpression="DelegatedFrom" HeaderText="Delegated From" UniqueName="DelegatedFrom">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DelegatedBy" FilterControlAltText="DelegatedBy" SortExpression="DelegatedBy" HeaderText="Delegated By" UniqueName="DelegatedBy">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DelegationComments" FilterControlAltText="DelegationComments" SortExpression="DelegationComments" HeaderText="Delegation Comments" UniqueName="DelegationComments">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </telerik:GridTableView>
                            </DetailTables>
                            <Columns>
                                <telerik:GridBoundColumn DataField="ApproverName" FilterControlAltText="ApproverName" SortExpression="ApproverName" HeaderText="ApproverName" UniqueName="ApproverName">
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Approver Role" SortExpression="ApproverType" UniqueName="ApproverType">
                                    <ItemTemplate>
                                        <asp:Label ID="lblApprover" runat="server" Text='<%#Eval("ApproverType") != null && Eval("ApproverType").ToString() == "Group - NPAC Approver"?"Group - NPAC/NMAC/PROTIS/ENIS Approver":Eval("ApproverType") %>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status">
                                </telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </div>
        </div>--%>

        <asp:Panel ID="MainPanel" runat="server" Visible="false">
            <div class="clear">
                <h5 id="MainPanelHeaderSpan" runat="server">Prefix Submission</h5>
            </div>
            <asp:Panel ID="SharedEditSAP" runat="server" Visible="false">
                <div class="col-xs-12 5Bottom15">
                    <div class="col-xs-4">
                        <div id="SAPUserTypeRow" runat="server" class="form-group">
                            <label class="col-xs-4">SAP UserType</label>
                            <div class="col-xs-8">
                                <asp:DropDownList ID="SAPUserTypeDropDown" runat="server">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">Account Name</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtAccountNameSAP" MaxLength="12" runat="server">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">Description</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtDescriptionSAP" TextMode="MultiLine" runat="server" MaxLength="40">
                                </asp:TextBox>
                            </div>
                        </div>

                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="PostionPrefixPanel" runat="server" Visible="false">
                <div class="col-xs-12 5Bottom15">
                    <div class="col-xs-4">
                        <div class="form-group">
                            <label class="col-xs-4">Position Prefix <span style="color: red">*</span></label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtPrefixValue" MaxLength="4" runat="server" CssClass="wizlbl" Width="345px" style="text-transform:uppercase" ValidationGroup="positionValidation"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvPrefixValue"  ValidationGroup="positionValidation" ControlToValidate="txtPrefixValue" runat="server" ErrorMessage="Please enter Prefix" SetFocusOnError="true" ForeColor="Red"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="rgxPrefixValue" ValidationExpression="^[A-Za-z]+$"  ValidationGroup="positionValidation" ForeColor="Red" ControlToValidate="txtPrefixValue" runat="server" ErrorMessage="Please enter only alphabets" SetFocusOnError="true"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="SAPEPEntitlementPanel" runat="server" Visible="true">

                <div class="row col-xs-12 5Bottom15">
                    <div class="col-xs-6">
                        <div id="GroupEntilementtr" runat="server" class="form-group">
                            <label class="col-xs-3">Group</label>
                            <div class="col-xs-6">
                                <telerik:RadComboBox CheckBoxes="true" ID="ExistingGroupAutoCompleteBox" CssClass="form-control RadComboBoxFullWidth" DropDownAutoWidth="Enabled" runat="server" Visible="false" AutoPostBack="true">
                                </telerik:RadComboBox>
                                <telerik:RadAutoCompleteBox ID="GroupsAutoCompleteBox" HighlightFirstMatch="false"  CssClass="form-control" OnClientRequesting="OnClientRequestingForSAPCE" runat="server" Visible="false" EmptyMessage="Enter * to view full list" DropDownHeight="500px" DropDownWidth="500px" InputType="Token">
                                    <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Method="GetPortalGroups" Path="../../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                            <div class="col-xs-2">
                                
                                <asp:Button ID="btnGrpAdd" runat="server" CssClass="btn btn-default" OnClick="btnGrpAdd_Click" Text="Add" />
                            </div>
                        </div>
                    </div>

                    <div class="col-xs-6">
                        <div id="RoleEntilementtr" runat="server" class="form-group">
                            <label class="col-xs-3">Role</label>
                            <div class="col-xs-6">
                                <telerik:RadComboBox CheckBoxes="true" ID="ExistingRoleAutoCompleteBox" CssClass="form-control RadComboBoxFullWidth" DropDownAutoWidth="Enabled" runat="server" Visible="false" AutoPostBack="true">
                                </telerik:RadComboBox>
                                <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" HighlightFirstMatch="false"  OnClientRequesting="OnClientRequestingForSAPCE" CssClass="form-control" runat="server" Visible="false" EmptyMessage="Enter * to view full list" DropDownHeight="200px" DropDownWidth="500px" InputType="Token">
                                    <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Method="GetPortalRoles" Path="../../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                            <div class="col-xs-2">
                                <asp:Button ID="btnroleAdd" runat="server" CssClass="btn btn-default" OnClick="btnroleAdd_Click" Text="Add" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row col-xs-12 5Bottom15">
                    <div id="trGroupGrid" runat="server" visible="false" class="col-xs-6 form-group">
                        <telerik:RadGrid ID="GroupGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None">
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Key"  Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" HeaderStyle-Width="300px" HeaderText="Group" SortExpression="Value">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="60px">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="DeleteGroupButton" runat="server" CommandArgument='<%#Eval("Key") %>' OnClick="DeleteGroupButton_Click" Text="Delete" ToolTip="Edit" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </div>

                    <div id="trRoleGrid" runat="server" class="col-xs-6 form-group">
                        <telerik:RadGrid ID="RoleGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None">
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

                </div>



                <%-- <div class="row 5Bottom15">
                    <div class="col-xs-6">
                        <div id="GroupEntilementtr" runat="server" class="form-group">
                            <label class="col-xs-3">Group</label>
                            <div class="col-xs-9">
                                <div class="col-xs-8">
                                    <telerik:radcombobox checkboxes="true" id="ExistingGroupAutoCompleteBox" runat="server" visible="false" autopostback="true">
                                </telerik:radcombobox>
                                    <telerik:radautocompletebox id="GroupsAutoCompleteBox" cssclass="form-control" runat="server" visible="false" emptymessage="Enter * to view full list" inputtype="Token">
                                    <WebServiceSettings Method="GetPortalGroups" Path="../../Services/AutoCompleteDataService.svc" />
                                </telerik:radautocompletebox>
                                </div>
                                <div class="col-xs-4">
                                    <asp:Button ID="btnGrpAdd" runat="server" CssClass="btn btn-default" OnClick="btnGrpAdd_Click" Text="Add" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div id="trGroupGrid" runat="server" class="form-group" visible="false">
                            <telerik:radgrid id="GroupGrid" runat="server" allowpaging="false" allowsorting="false" cellspacing="0" gridlines="None" width="300px">
                                <MasterTableView AutoGenerateColumns="False" DataKeyNames="Key">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" HeaderStyle-Width="300px" HeaderText="Group" SortExpression="Value">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="70px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="DeleteGroupButton" runat="server" CommandArgument='<%#Eval("Key") %>' OnClick="DeleteGroupButton_Click" Text="Delete" ToolTip="Edit" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:radgrid>
                        </div>

                    </div>

                </div>
                </br>
                <div class="row 5Bottom15">
                    <div class="col-xs-6">
                        <div id="RoleEntilementtr" runat="server" class="form-group">
                            <label class="col-xs-3">Role</label>
                            <div class="col-xs-9">
                                <div class="col-xs-8">
                                    <telerik:radcombobox checkboxes="true" id="ExistingRoleAutoCompleteBox" runat="server" visible="false" autopostback="true">
                                </telerik:radcombobox>
                                    <telerik:radautocompletebox id="RolesAutoCompleteBox" cssclass="form-control" runat="server" visible="false" emptymessage="Enter * to view full list" inputtype="Token">
                                    <WebServiceSettings Method="GetPortalRoles" Path="../../Services/AutoCompleteDataService.svc" />
                                </telerik:radautocompletebox>
                                </div>
                                <div class="col-xs-4">
                                    <asp:Button ID="btnroleAdd" runat="server" CssClass="btn btn-default" OnClick="btnroleAdd_Click" Text="Add" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-xs-6">
                        <div id="trRoleGrid" runat="server" class="form-group" visible="false">
                            <telerik:radgrid id="RoleGrid" runat="server" allowpaging="false" allowsorting="false" cellspacing="0" gridlines="None" width="300px">
                                <MasterTableView AutoGenerateColumns="False" DataKeyNames="Key">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" HeaderStyle-Width="300px" HeaderText="Role" SortExpression="Value">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="70px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="DeleteRoleButton" runat="server" CommandArgument='<%#Eval("Key") %>' OnClick="DeleteRoleButton_Click" Text="Delete" ToolTip="Edit" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:radgrid>
                        </div>
                    </div>
                </div>--%>
            </asp:Panel>

        </asp:Panel>


        <asp:Panel ID="SharedAccountEditPanel" Visible="false" runat="server">

            <div class="table-header clear">
                <h4>Account Details</h4>
            </div>

            <asp:Panel ID="SharedAccountADEditPanel" runat="server" Visible="false">
                <div class="col-xs-12">
                    <div class="col-xs-6">
                        <div class="form-group row">
                            <label class="col-xs-4">Account Name</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtAcntName" CssClass="form-control" runat="server" MaxLength="20" ForeColor="Red"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group  row">
                            <label class="col-xs-4">Display Name</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtDisplayName" CssClass="form-control" runat="server" MaxLength="60">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group row">
                            <label class="col-xs-4">Description</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtDiscription" CssClass="form-control" runat="server" MaxLength="255">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div runat="server" id="trOU" class="form-group row">
                            <label class="col-xs-4">OU</label>
                            <div class="col-xs-8">
                                <telerik:RadAutoCompleteBox ID="OUAutoCompleteBox" CssClass="form-control" runat="server" EmptyMessage="Enter * to view full list" InputType="Text">
                                    <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Method="GetADOrganisationUnits" Path="../../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>

                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>



            <asp:Panel ID="SharedAccountSRMEditPanel" runat="server" Visible="false">
                <div class="col-xs-12 mBottom15">
                    <div class="col-xs-4">
                        <div class="form-group">
                            <label class="col-xs-4">Account Name</label>

                            <div class="col-xs-8">
                                <asp:TextBox ID="txtSRMAccountName" CssClass="form-control" runat="server" MaxLength="12" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="O365SharedEditPanel" runat="server" Visible="false">
                <div class="col-xs-12 mBottom15">
                    <div class="col-xs-12">
                        <div class="form-group row">
                            <label class="col-xs-3">Alias Name</label>
                            <div class="col-xs-8">
                                <asp:Label ID="txtboxAliasName" CssClass="form-control" runat="server">
                                </asp:Label>
                            </div>
                        </div>

                        <div class="form-group row">
                            <label class="col-xs-3">Display Name</label>
                            <div class="col-xs-8">
                                <asp:Label ID="txtboxDisplayName" CssClass="form-control" runat="server">
                                </asp:Label>
                            </div>
                        </div>

                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="sharedAccountO365NewAccountPanel" runat="server" Visible="false">
                <div class="col-xs-12">
                    <div class="col-xs-12">
                        <div class="form-group row">
                            <div class="form-group">
                                <label class="col-xs-3">Requester Cost Center</label>
                                <div class="col-xs-8">
                                    <asp:Label ID="lblRequesterCostCenter" CssClass="form-control" runat="server" Enabled="false"></asp:Label>
                                </div>
                            </div>

                            <%--<div class="form-group">
                                <label class="col-xs-4">Entity Id</label>
                                <div class="col-xs-8">
                                    <asp:TextBox ID="txtEntityId" CssClass="form-control" runat="server" MaxLength="30" Visible="false">
                                    </asp:TextBox>
                                </div>
                            </div>--%>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="SharedAccountGEMSEditPanel" runat="server" Visible="false">
                <div class="col-xs-12 mBottom15">
                    <div class="col-xs-4">

                        <div class="form-group">
                            <label class="col-xs-4">Account Name</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtAccountName" CssClass="form-control" runat="server" MaxLength="20">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">Email Id</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtEmail" CssClass="form-control" runat="server" MaxLength="60">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">Display Name</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtDisplayNameShared" CssClass="form-control" runat="server" MaxLength="60">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">Description</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtDescriptionShared" CssClass="form-control" runat="server" MaxLength="255">
                                </asp:TextBox>
                            </div>
                        </div>

                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="SharedAccountInhouseEditPanel" runat="server" Visible="false">
                <div class="col-xs-12 mBottom15">
                    <div class="col-xs-4">

                        <div class="form-group">
                            <label class="col-xs-4">Account Name</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtAcntNameIHN" CssClass="form-control" runat="server" MaxLength="50">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">Display Name</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtDisplayNameINH" CssClass="form-control" runat="server" MaxLength="60">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">Description</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtDiscriptionINH" CssClass="form-control" runat="server" MaxLength="100">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="SharedAccountLNSPanel" runat="server" Visible="false">
                <div class="col-xs-12 mBottom15">
                    <div class="col-xs-4">

                        <div class="form-group">
                            <label class="col-xs-4">Full Name</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtAccountNameLNS" CssClass="form-control" runat="server" MaxLength="100">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">Short Name</label>
                            <div class="col-xs-8">

                                <asp:TextBox ID="txtEmailLNS" CssClass="form-control" runat="server" MaxLength="30">
                                </asp:TextBox>

                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">Description</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtDescriptionLNS" CssClass="form-control" runat="server" MaxLength="255">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">OU</label>
                            <div class="col-xs-8">
                                <telerik:RadAutoCompleteBox ID="OUCertifierLNS" CssClass="form-control" runat="server" EmptyMessage="Enter * to view full list" DropDownHeight="200px" DropDownWidth="300px" InputType="Text" Width="300px">
                                    <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Method="GetLNSOrganisationUnits" Path="../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                        </div>
                    </div>
                </div>

            </asp:Panel>


        </asp:Panel>

        <div class="table-header clear">
            <h4>Additional Task Information</h4>
        </div>
        <div id="OneKeyPanel" class="col-xs-12 form-group 5Bottom15" runat="server" visible="false">
           
            <div class="form-group col-xs-3 clear-padding">
                <label>Token Number<span style="color: red">*</span> :</label>
            </div>
            <div class="col-xs-9">
                <asp:TextBox ID="txtOneKey" runat="server" MaxLength="24"></asp:TextBox>

            </div>
        </div>
        <div id="SkypeForBusinessPanel" class="col-xs-12 5Bottom15" runat="server" visible="false">
            <div class="table-header clear">
                <h4>MicrosoftTeams Submission</h4>
            </div>
            <div class="col-xs-12" runat="server" id="divSpecialNumberText" visible="false">
                <div class="form-group required">
                    <div class="col-xs-12">
                        <asp:Label ID="lblSpecialNumber" Font-Size="Medium" runat="server" CssClass="description" Font-Bold="true" Text="Request for Special Number"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-xs-12" runat="server" visible="false" id="divDCCResponseGroup">
                <div class="form-group required">
                    <label class="col-xs-3">&nbsp;</label>
                     <div class="col-xs-4">
                        <asp:Label runat="server" ID="dccRemarksForResponseGroupUser" CssClass="label label-danger"></asp:Label>
                     </div>   
                </div>
             </div>   
            <div class="col-xs-12">
                <div class="form-group required">
                    <label class="col-xs-3">Enter Extension<span style="color: red; font-weight: bold; font-size: 12pt;">&nbsp;*</span></label>
                    <div class="col-xs-4">
                        <asp:Label ID="lblLineUrIPrefix" runat="server" Visible="true" CssClass="label label-success" Font-Size="Medium"></asp:Label>
                        <telerik:RadAutoCompleteBox ID="ReservedNumberAutoCompleteBox" OnEntryRemoved="ResponseGroupAutoCompleteBox_EntryRemoved" OnEntryAdded="ResponseGroupAutoCompleteBox_EntryAdded"
                            EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" OnClientRequesting="OnClientRequestingReservedNumber"
                            CssClass="form-control" AutoPostBack="true">
                            <TextSettings SelectionMode="Single" />
                            <WebServiceSettings Method="GetSkypeForBusinessReservedNumber" Path="../../../../Services/AutoCompleteDataService.svc" />
                        </telerik:RadAutoCompleteBox>
                        <asp:TextBox ID="txtDCCExtension"  runat="server" Visible="false" MaxLength="4" Enabled="false"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtDCCExtension" runat="server" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+">
                        </asp:RegularExpressionValidator>
                        <asp:FilteredTextBoxExtender runat="server" ID="fltrNumber" TargetControlID="txtDCCExtension" FilterType="Numbers"></asp:FilteredTextBoxExtender>
                    </div>
                </div>
            </div>
             
        </div>


        <div class="mBottom15">

            <div id="ExceptionRemarkstr" runat="server" class="col-xs-12 form-group" visible="false">
                <label class="col-xs-4">Exception Remarks</label>
                <div class="col-xs-12">
                    <asp:Label ID="ExceptionRemarksValueLabel" runat="server" CssClass="description"></asp:Label>
                </div>
            </div>

            <div id="AdditionalInformationtr" runat="server" class="col-xs-12 form-group" visible="false">
                <label class="col-xs-4">Additional Task Information</label>
                <div class="col-xs-12">
                    <asp:Label ID="MSSLAdditionalInfoValueLabel" runat="server" ForeColor="#627F97" CssClass="description"></asp:Label>
                </div>
            </div>

            <div id="ConflictsRemarktr" runat="server" class="col-xs-12 form-group" visible="false">
                <label class="col-xs-4">Conflict Remarks</label>
                <div class="col-xs-8">
                    <asp:Label ID="ConflictsRemarkLabel" runat="server" ForeColor="#627F97" CssClass="description"></asp:Label>
                    <asp:Image ID="ConflictsRemarkImage" runat="server" Width="20px" Height="20px" src="../../Images/warning32.png" alt="" />
                </div>
            </div>


            <div id="RemarksValuetr" runat="server" class="col-xs-12 form-group" visible="false">
                <label class="col-xs-4">Remarks</label>
                <div class="col-xs-8">
                    <asp:Label ID="RemarksValueLabel" runat="server" CssClass="description"></asp:Label>
                </div>
            </div>
            <div id="RoleNameValuetr" runat="server" class="col-xs-12 form-group" visible="false">
                <label class="col-xs-4">RoleName</label>
                <div class="col-xs-8">
                    <asp:Label ID="RoleNameValueLabel" runat="server" CssClass="description"></asp:Label>
                </div>
            </div>

            <div id="TCodeValueLabeltr" runat="server" class="col-xs-12 form-group" visible="false">
                <label class="col-xs-4">TCode</label>
                <div class="col-xs-8">
                    <asp:Label ID="TCodeValueLabel" runat="server" CssClass="description"></asp:Label>
                </div>
            </div>

            <div id="DelegatedBytr" runat="server" class="col-xs-12 form-group" visible="false">
                <label class="col-xs-4">Delegated By</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblDelegatedBy" CssClass="description desc-textwrap" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <div id="DelegatedFromtr" runat="server" class="col-xs-12  form-group" visible="false">
                <label class="col-xs-4">Delegated From</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblDelegatedFrom" CssClass="description desc-textwrap" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <div id="DelegationCommentstr" runat="server" class="col-xs-12 form-group" visible="false">
                <label class="col-xs-4">Delegation Comments</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblDelegationComments" CssClass="description desc-textwrap" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <div id="Tr3" runat="server" class="col-xs-12 form-group" visible="false">
                <label class="col-xs-4">Updated Termination Date</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblcuurentTerminationDate" CssClass="description" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <div id="Tr4" runat="server" class="col-xs-12 form-group" visible="false">
                <label class="col-xs-4">Updated Termination Date</label>
                <div class="col-xs-8">
                    <asp:Label ID="lblModifiedTerminationDate" CssClass="description" runat="server" Text=""></asp:Label>
                </div>
            </div>
        </div>



        <div class="table-header clear">
            <h4><%= T("App.Common.MoreDetails") %></h4>
        </div>

        <div class="col-xs-12">
            <div class="form-group">
                <label class="col-xs-3 clear-padding"><%= T("App.Common.ApprovalComments") %></label>
                <div class="col-xs-9">
                    <asp:TextBox ID="txtComments" runat="server" CssClass="wizlbl" MaxLength="255" Width="100%"
                        TextMode="MultiLine" Rows="7"></asp:TextBox>
                </div>
                <div class="col-xs-4">
                    <%--<asp:LinkButton runat="server" ID="lnkApproverChain" OnClick="lnkApproverChain_Click" ><%= T("App.Common.ViewApprovalChain") %></asp:LinkButton>
                        <br />
                        <br />--%>
                    <asp:LinkButton ID="lnkViewNotification" runat="server" Visible="false" OnClick="lnkViewNotification_Click"><%= T("App.Common.ViewNotification") %></asp:LinkButton>
                    <%--<br />
                        <br />--%>
                </div>
            </div>
        </div>

        <div class="buttons text-right nav-rectangle">
            <%--<asp:Button ID="btnSendNotification" CssClass="btn btn-default btn-sm" Visible="false" runat="server" Text="Send Notification" OnClick="btnSendNotification_Click" />--%>
            <asp:Button ID="btnSubmit" CssClass="btn btn-default btn-sm" runat="server" Text="Submit" OnClick="SubmitButton_Click" />
            <asp:Button ID="btnApprove" CssClass="btn btn-default btn-sm green" runat="server" Text="Approve" OnClick="btnApprove_Click" />
            <asp:Button ID="btnReject" CssClass="btn btn-default btn-sm red" runat="server" Text="Reject" OnClick="btnReject_Click" />
            <asp:Button ID="BackButton" CssClass="btn btn-default btn-sm" runat="server" Text="Back" OnClick="BackButton_Click" />
            <asp:Button ID="RefreshButton" CssClass="btn btn-default btn-sm" runat="server" Text="Refresh" OnClick="RefreshButton_Click" />
        </div>



        <telerik:RadWindow runat="server" ID="RadWinSendNotification" Modal="true" Width="600px" Height="100%"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title"><%= T("App.Common.SendNotification") %></h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:SendNotificationUC runat="server" ID="UCSendNotification" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnSend" class="btn btn-primary bordered-button" Text="Send" OnClick="btnSend_Click" />
                            <asp:Button runat="server" ID="btnCancelSend" class="btn btn-primary bordered-button" Text="Cancel" OnClick="btnCancelSend_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow runat="server" ID="RadWinViewNotification" Modal="true" Width="900px" Height="100%"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title"><%= T("App.Common.ViewNotification") %></h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:ViewNotificationUC runat="server" ID="ViewNotification" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnOk" class="btn btn-primary bordered-button" OnClick="btnOk_Click" Text="OK" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>


    </ContentTemplate>
</asp:UpdatePanel>



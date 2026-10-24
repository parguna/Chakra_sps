
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserViewUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.UC.UserViewUC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/CommentsUserControl.ascx" TagPrefix="uc1" TagName="CommentsUserControl" %>
<%@ Register Src="~/UsrControl/UserSODConflicts.ascx" TagPrefix="uc1" TagName="UserSODConflicts" %>
<%@ Register Src="~/Admin/Attestation/UC/AttestationReviewHistoryUC.ascx" TagPrefix="uc1" TagName="AttestationReviewHistoryUC" %>

<style type="text/css">
    .col-minimum-width{
        min-width:120px;
    }
    .col-common-width{
        width:140px !important;
    }
    .col-approver-width{
        width:110px !important;
        min-width:110px !important;
    }
    .rgAltRow{
        
    }
    .col-newminimum-width{
        min-width:60px !important;
    }
    .col-accountminimum-width{
        min-width:230px !important;
    }
     .col-descriptionminimum-width{
        min-width:100px !important;
    }
     .badge-custom{
		background-color: #e13304;
	}
.alt-row{
    border:2px solid #f2f2f2;
    background-color:white;
}

div.RadGridCustomClass  .rgRow  td,  
div.RadGridCustomClass  .rgAltRow  td
  
{  
     border-top:1px solid #e7e7ea !important;  
}   
.account-highlight{
color: #000;
background: #81cfdc;
}
.account-highlight{
color: #000;
background: #81cfdc;
border:1px solid #f9f9f9 ;
}
.employeenamehighlight{
    background-color:#146a78;
    color:#fff;
    font-weight:bold;
}

tr.employeenamehighlight td.rgExpandCol
{
    background-color:#c5c5c5 !important;
    border-bottom: 1px solid #c5c5c5 !important;
}
tr.account-highlight td{
    border:1px solid #74bdc9;
}
tr.account-highlight{
    border-bottom:1px solid #fff;
}
    tr.employeenamehighlight td {
        border-bottom:1px solid #007d93 !important
    }
.applicationName{
    color:blue;
}
.username-padding{
    padding: 5px;
}
.col-descriptionminimum-width-withbackground{
    background-color:cornflowerblue;
    min-width:100px !important;
}
</style>

<asp:Panel ID="Panel1" runat="server" CssClass="topBuffer">
    <div style="overflow: hidden;">
        <asp:UpdatePanel ID="ContentUpdatePanel" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <div class="buttons text-right topBuffer" runat="server" id="divUnsavedChanges">
                    <asp:Label runat="server" ID="UnSavedReviews" class="description orange">Unsaved reviews found, Click save to proceed.</asp:Label>
                </div>
                <telerik:RadGrid ID="AttestationReviewGrid" runat="server" AutoGenerateColumns="False" GroupPanelPosition="Top" AllowPaging="true" AllowFilteringByColumn="true" AllowSorting="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin"
                    OnDetailTableDataBind="AttestationReviewGrid_DetailTableDataBind" OnPreRender="AttestationReviewGrid_PreRender"
                    OnNeedDataSource="AttestationReviewGrid_NeedDataSource" OnItemDataBound="AttestationReviewGrid_ItemDataBound"
                    OnItemCommand="AttestationReviewGrid_ItemCommand" RetainExpandStateOnRebind="true" EnableHierarchyExpandAll="true"
                    OnItemCreated="AttestationReviewGrid_ItemCreated" CssClass="RadGridCustomClass" >
                    <HeaderStyle Font-Bold="True" />
                    
                    <ItemStyle Wrap="false"  CssClass="alt-row" />
                    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="50" Height="100%" AlwaysVisible="true" />
                    <GroupingSettings CaseSensitive="false" />
                    <ClientSettings AllowColumnHide="true"  EnableAlternatingItems="false" >
                        <Scrolling UseStaticHeaders="true" AllowScroll="false" />
                    </ClientSettings>
                    <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="true" CommandItemDisplay="None" Width="100%"
                        EnableHeaderContextMenu="false" DataKeyNames="KeyId,SnapshotActionId,Entity,Level,ReviewCategory,Comments,ParentBindingLevel,BindingLevel,EndpointSystemId,EntitlementId,EndpointSystemType,AccountName,AccountType,Type" NoMasterRecordsText="No Records Available"
                        HierarchyDefaultExpanded="false" HierarchyLoadMode="ServerOnDemand" RetainExpandStateOnRebind="true" EnableHierarchyExpandAll="true" TableLayout="Auto"
                        ClientDataKeyNames="EndpointSystemType,Level" >
                        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                        <SelfHierarchySettings ParentKeyName="ParentBindingLevel" KeyName="BindingLevel" />
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="Delta" HeaderText="Is New?" AllowFiltering="false" HeaderStyle-Width="60px" ItemStyle-CssClass="col-newminimum-width" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                        <span class="badge badge-warning badge-custom"><%# Convert.ToBoolean(Eval("IsDelta")) == true ?"New":"" %></span>
                                </ItemTemplate>
                                <ItemStyle CssClass="delta-info" HorizontalAlign="Left" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="Name" FilterControlAltText="Name" ItemStyle-CssClass="col-accountminimum-width"
                                HeaderText="User Name" SortExpression="Name" UniqueName="Name" FilterControlWidth="75%"  >
                                <ItemTemplate>
                                   <span class="username-padding"> <asp:Label runat="server" Text='<%# Eval("Name") %>'></asp:Label></span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="DisplayName" FilterControlAltText="DisplayName" ItemStyle-CssClass="col-descriptionminimum-width" 
                                HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName">
                                <ItemTemplate>

                                    <asp:Label runat="server" ID="lblDisplayName" Text='<%# Eval("DisplayName") %>' Width="100%" style="padding:5px"></asp:Label>
                                    <asp:HoverMenuExtender ID="hoverDisplayName" runat="server" PopupControlID="pnlDescription" PopupPosition="Center" TargetControlID="lblDisplayName"></asp:HoverMenuExtender>
                                    <asp:Panel ID="pnlDescription" runat="server" CssClass="HoverPopup">
                                        <%# Eval("DisplayName") %>
                                    </asp:Panel>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="Type" FilterControlAltText="Type" ItemStyle-CssClass="col-common-width"
                                HeaderText="Entitlement Type" SortExpression="Type" UniqueName="Type">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="AccountType" FilterControlAltText="AccountType" ItemStyle-CssClass="col-approver-width"
                                HeaderText="Account Type" SortExpression="AccountType" UniqueName="AccountType" >
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="LastLogon" FilterControlAltText="LastLogon" ItemStyle-CssClass="col-approver-width"
                                HeaderText="Last Logon" SortExpression="LastLogon" UniqueName="LastLogon" >
                            </telerik:GridBoundColumn>
                            
                            <telerik:GridBoundColumn DataField="Reserved1" FilterControlAltText="Reserved1" ItemStyle-Width="200px"
                                HeaderText="Additional Info" SortExpression="Reserved1" UniqueName="Reserved1" >
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn DataField="Reserved2" FilterControlAltText="Reserved2" ItemStyle-CssClass="col-approver-width"
                                HeaderText="Is Locked" SortExpression="Reserved2" UniqueName="Reserved2" >
                                <ItemTemplate>
                                    <span  style='font-size:10pt;font-weight:bold; visibility: <%# !Convert.ToBoolean(Eval("AccountStatus"))  ? "visible":"hidden" %>'> 
                                        <%# Eval("Reserved2") %></span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                           
                            <telerik:GridBoundColumn DataField="Reserved4" FilterControlAltText="Reserved4"
                                HeaderText="Reserved4" SortExpression="Reserved4" UniqueName="Reserved4" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Reserved5" FilterControlAltText="Reserved5"
                                HeaderText="Reserved5" SortExpression="Reserved5" UniqueName="Reserved5" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Reserved6" FilterControlAltText="Reserved6"
                                HeaderText="Reserved6" SortExpression="Reserved6" UniqueName="Reserved6" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Reserved7" FilterControlAltText="Reserved7"
                                HeaderText="Reserved7" SortExpression="Reserved7" UniqueName="Reserved7" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Reserved8" FilterControlAltText="Reserved8"
                                HeaderText="Reserved8" SortExpression="Reserved8" UniqueName="Reserved8" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Reserved9" FilterControlAltText="Reserved9"
                                HeaderText="Reserved9" SortExpression="Reserved9" UniqueName="Reserved9" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Reserved10" FilterControlAltText="Reserved10"
                                HeaderText="Valid To" SortExpression="Reserved10" UniqueName="Reserved10" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ActionReserved1" FilterControlAltText="ActionReserved1"
                                HeaderText="ActionReserved1" SortExpression="ActionReserved1" UniqueName="ActionReserved1" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ActionReserved2" FilterControlAltText="ActionReserved2"
                                HeaderText="ActionReserved2" SortExpression="ActionReserved2" UniqueName="ActionReserved2" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ActionReserved3" FilterControlAltText="ActionReserved3"
                                HeaderText="ActionReserved3" SortExpression="ActionReserved3" UniqueName="ActionReserved3" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ReviewCategory" FilterControlAltText="ReviewCategory"
                                HeaderText="ReviewCategory" SortExpression="ReviewCategory" UniqueName="ReviewCategory" FilterControlWidth="70%" Display="false" Visible="false">
                            </telerik:GridBoundColumn>
                            
                            <telerik:GridTemplateColumn UniqueName="ApproveGridTemplateColumn" HeaderText="Retain" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"  AllowFiltering="false" ItemStyle-CssClass="col-approver-width">
                                <ItemTemplate>
                                    <asp:RadioButton ID="ApproveRadioButton" runat="server" AutoPostBack="true" GroupName="ApproveRevoke" OnCheckedChanged="ApproveRadioButton_CheckedChanged" CssClass="RadGrid_SkinName"
                                        Checked='<%#Convert.ToBoolean(Eval("Approved")) %>' Visible='<%#Convert.ToBoolean(Eval("IsActionable")) %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="RevokeGridTemplateColumn" HeaderText="Revoke" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" AllowFiltering="false"  ItemStyle-CssClass="col-approver-width">
                                <ItemTemplate>
                                    <asp:RadioButton ID="RevokeRadioButton" runat="server" AutoPostBack="true" GroupName="ApproveRevoke" OnCheckedChanged="RevokeRadioButton_CheckedChanged" CssClass="RadGrid_SkinName"
                                        Checked='<%#Convert.ToBoolean(Eval("Rejected")) %>' Visible='<%#Convert.ToBoolean(Eval("IsActionable")) %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Request Id" UniqueName="RequestId" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" AllowFiltering="true" DataField="ActionReserved1" FilterControlAltText="ActionReserved1" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtRequestId" runat="server" AutoPostBack="true" OnTextChanged="txtRequestId_TextChanged" ToolTip="Remedy Request ID" placeholder="Remedy Request ID" Text='<%#Eval("ActionReserved1") %>'></asp:TextBox>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            
                            <telerik:GridTemplateColumn HeaderText="Comments" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" UniqueName="ReviewCommentsBoundColumn" AllowFiltering="false" ItemStyle-Width="150px" >
                                <ItemTemplate>
                                    <asp:ImageButton ID="AddCommentLinkButton" runat="server" Text="Add Comments" OnClick="AddCommentImageButton_Click"
                                        CommandArgument='<%#Eval("KeyId") %>' CommandName="ReviewComment"></asp:ImageButton>
                                </ItemTemplate>
                                <ItemStyle CssClass="action-comment" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="EffectiveDateTemplateColumn" HeaderText="Effective Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" AllowFiltering="false"  ItemStyle-Width="140px">
                                <ItemTemplate>
                                    <telerik:RadDatePicker runat="server" ID="ValidFromPicker" AutoPostBack="true" DateTimePickerElement="" NullText="No Date" NullDate="DateTime.Now()" DbSelectedDate='<%# Bind("EffectiveDate") %>' EnableTyping="false" Skin="IACMSkin" ShowPopupOnFocus="false" CssClass="form-control" DatePopupButton-Visible="true"
                                        OnSelectedDateChanged="ValidFromPicker_SelectedDateChanged" Calendar-ShowRowHeaders="false"
                                        >
                                    </telerik:RadDatePicker>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="TemplateEditColumn" HeaderText="Risk Info" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ShowWarningIcon" Visible='<%#Convert.ToBoolean(Eval("IsSODConflict")) == true %>' runat="server" CommandName="ShowWarningInfo"></asp:ImageButton>
                                </ItemTemplate>
                                <ItemStyle CssClass="warning-info" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="IsChangedBaseGridTemplateColumn" HeaderText="Changed" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" Display="false" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="IsChangedBaseLabel" runat="server" Text='<%#Convert.ToBoolean(Eval("IsChanged")) ? "Yes" : "" %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn UniqueName="SnapshotActionId" DataField="SnapshotActionId" HeaderText="SnapshotActionId" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" Display="false" Visible="false">
                            </telerik:GridBoundColumn>
                             <telerik:GridBoundColumn DataField="Reserved3" FilterControlAltText="Reserved3" ItemStyle-CssClass="col-approver-width"
                                HeaderText="Valid To" SortExpression="Reserved3" UniqueName="Reserved3" >
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="ReviewHistory" HeaderText="Review History" AllowFiltering="false" ItemStyle-Width="140px"> 
                                <ItemTemplate>
                                    <asp:ImageButton ID="ShowReviewHistoryIcon" Visible='<%#Convert.ToBoolean(Eval("IsChangedInPreviousLevel")) == true %>' runat="server" CommandName="ShowReviewHistory"></asp:ImageButton>
                                </ItemTemplate>
                                <ItemStyle CssClass="history-info" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings AllowExpandCollapse="true">
                        <ClientEvents  OnGridCreated="HierarchyExpanding" />
                    </ClientSettings>
                </telerik:RadGrid>
            </ContentTemplate>
        </asp:UpdatePanel>
        <telerik:RadWindowManager runat="server" ID="CommentsUserControlNew" EnableShadow="true">
            <Windows>
                <telerik:RadWindow ID="CommentsDialog" runat="server" Title="Comments" Height="320px"
                    Width="510px" Left="150px" ReloadOnShow="true" ShowContentDuringLoad="false" Modal="true">
                </telerik:RadWindow>
            </Windows>
        </telerik:RadWindowManager>

        <telerik:RadWindow runat="server" ID="CommentsWindow" Modal="true" Height="300px" Width="500px" Title="Add Your Comments"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <uc1:CommentsUserControl runat="server" ID="AddCommentsUserControl" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow runat="server" ID="SensitiveRoleDetails" Modal="true" Width="700px" Height="100%" Title="Account Detail"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title"><%= T("App.Common.RiskInfo") %></h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:UserSODConflicts runat="server" ID="UserSODConflict" isFromAccessRequest="false" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnSensitiveRoleCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnSensitiveRoleCancel_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow runat="server" ID="ReviewHistory" Modal="true" Height="100%" Width="700px" Title="Review History"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title">Review History</h3>
                        </div>
                        <div class="modal-body">
                            <uc1:AttestationReviewHistoryUC runat="server" ID="AttestationReviewHistoryUC" />
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnHistoryCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnHistoryCancel_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </div>
</asp:Panel>
<script type="text/javascript">
    var attestationType = "<%= CurrentAttestationType %>";
    function HierarchyExpanding(sender, eventArgs) {
        var grid = $find("<%= AttestationReviewGrid.ClientID %>");
        doSomething(grid.MasterTableView);
    }

    function doSomething(masterTable) {
        var gridItems = masterTable.get_dataItems();
        gridItems.forEach(function (gridItem) {

            detailTableViews = gridItem.get_nestedViews();
            detailTableViews.forEach(function (detailView) {

                var entpointSystemID = gridItem.getDataKeyValue("EndpointSystemType");
                var level = gridItem.getDataKeyValue("Level");
                
                //if (entpointSystemID == EndpointSystem.CasinoManagementSystem) {
                //    if ((attestationType == AttestationType.User && level == 2) || (attestationType == AttestationType.Role && level == 1)) {
                //        var rs1 = detailView.getColumnByUniqueName("Reserved1");
                //        if (rs1 && rs1.get_element() != null)
                //            $(rs1.get_element()).text("View");

                //        var rs2 = detailView.getColumnByUniqueName("Reserved2");
                //        if (rs2 && rs2.get_element() != null)
                //            $(rs2.get_element()).text("Create");

                //        var rs3 = detailView.getColumnByUniqueName("Reserved3");
                //        if (rs3 && rs3.get_element() != null)
                //            $(rs3.get_element()).text("Modify");

                //        var rs4 = detailView.getColumnByUniqueName("Reserved4");
                //        if (rs4 && rs4.get_element() != null)
                //            $(rs4.get_element()).text("Delete");

                //        var rs5 = detailView.getColumnByUniqueName("Reserved5");
                //        if (rs5 && rs5.get_element() != null)
                //            $(rs5.get_element()).text("Print");

                //        var rs6 = detailView.getColumnByUniqueName("Reserved6");
                //        if (rs6 && rs6.get_element() != null)
                //            $(rs6.get_element()).text("Export");
                //    }
                //}

                //if (entpointSystemID == EndpointSystem.PSFinance)
                //{
                //    if ((attestationType == AttestationType.User && level == 2) || (attestationType == AttestationType.Role && level == 1)) {
                //        var rs1 = detailView.getColumnByUniqueName("Reserved1");
                //        if (rs1 && rs1.get_element() != null)
                //            $(rs1.get_element()).text("MenuName");

                //        var rs2 = detailView.getColumnByUniqueName("Reserved2");
                //        if (rs2 && rs2.get_element() != null)
                //            $(rs2.get_element()).text("BarName");

                //        var rs3 = detailView.getColumnByUniqueName("Reserved3");
                //        if (rs3 && rs3.get_element() != null)
                //            $(rs3.get_element()).text("ContentReference");

                //        var rs4 = detailView.getColumnByUniqueName("Reserved4");
                //        if (rs4 && rs4.get_element() != null)
                //            $(rs4.get_element()).text("Description");

                //        var rs5 = detailView.getColumnByUniqueName("Reserved5");
                //        if (rs5 && rs5.get_element() != null) {
                //            $(rs5.get_element()).text("IsDisplay");
                //        }
                //    }
                //}
              
                //var r2 = detailView.getColumnByUniqueName("Reserved2");
                //var r3 = detailView.getColumnByUniqueName("Reserved3");
                detailView.hideColumn(detailView.getColumnByUniqueName("LastLogon").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("AccountType").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("Delta").get_element().cellIndex);
                
                
                if (level == 0) {
                    detailView.showColumn(detailView.getColumnByUniqueName("Delta").get_element().cellIndex);
                    //var accountName = detailView.getColumnByUniqueName("Name").get_element();
                    //if (accountName) {
                    //    accountName.InnerText = "AccountName";
                    //}
                    //detailView.showColumn(detailView.getColumnByUniqueName("Reserved3").get_element().cellIndex);
                    detailView.showColumn(detailView.getColumnByUniqueName("LastLogon").get_element().cellIndex);
                    detailView.hideColumn(detailView.getColumnByUniqueName("Type").get_element().cellIndex);
                    detailView.showColumn(detailView.getColumnByUniqueName("AccountType").get_element().cellIndex);

                    if (attestationType == AttestationType.User) {
                        //detailView.showColumn(detailView.getColumnByUniqueName("Delta").get_element().cellIndex);
                    }
                }

                if (level == 1 ) {
                    detailView.hideColumn(detailView.getColumnByUniqueName("LastLogon").get_element().cellIndex);
                
                    //detailView.showColumn(detailView.getColumnByUniqueName("Delta").get_element().cellIndex);
                }
                if (level >= 1 ) {
                    try {
                        detailView.hideColumn(detailView.getColumnByUniqueName("Reserved3").get_element().cellIndex);
                    } catch (e) {
                    }
                    //detailView.showColumn(detailView.getColumnByUniqueName("Delta").get_element().cellIndex);
                }
                if (detailView._hasDetailTables) {
                    try {
                        doSomething(detailView);
                    } catch (e)
                    {
                        console.log(e);
                    }
                }
            });
        });
    }
     function closeCommentWindow() {
         try {
              setTimeout(function () {
                $("#UpdateProgressBar").hide();
            }, 10);
             var oWnd = $find("<%= CommentsWindow.ClientID %>");
             oWnd.Close();
             
         } catch (e) {
             console.log(e);
         }
    }
    function ShowProgress() {
        $("#UpdateProgressBar").show();
    }
</script>
<style>
    .HoverPopup
        {
            background-color: #2077BD;
            color: White;
            padding: 5px;
            display:none;
        }
</style>
<asp:Panel ID="Panel2" runat="server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <div class="buttons text-right topBuffer" runat="server" id="trrbtn">
                            <asp:Button ID="btnIntrimSave" CssClass="btn btn-default btn-sm" OnClientClick="ShowProgress();" runat="server" Text="Save" OnClick="btnIntrimSave_Click" />
                            <asp:Button ID="btnSaveAndSubmit" CssClass="btn btn-default btn-sm"  runat="server" Text="Save & Submit" OnClick="btnSaveAndSubmit_Click" />
                            <asp:Button ID="CancelButton" runat="server" OnClick="CancelButton_Click" Text="Cancel" CssClass="btn btn-default btn-sm" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
    <telerik:RadWindow runat="server" ID="ConfirmServiceAccount" Modal="true" Width="300px" Height="700"
        RenderMode="Lightweight" Behaviors="Close" VisibleOnPageLoad="false" CssClass="windowcss" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="upnlCSRVAc" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title confirmationHeader">Confirm Revoke Access</h3>
                    </div>
                    <div class="modal-body">
                        <div class="row-fluid">
                            <asp:Label ID="lblConfirm" runat="server" Text="Are you sure you want to revoke account?" class="control-label"></asp:Label>
                            <asp:HiddenField runat="server" ID="hdnRevokeItem"/>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancelSrvSubmit" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancelSrvSubmit_Click"></asp:Button>
                        <asp:Button runat="server" ID="btnConfirmSrvSubmit" CssClass="btn btn-primary bordered-button" Text="Ok" OnClick="btnConfirmSrvSubmit_Click" ></asp:Button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

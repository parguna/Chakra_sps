<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RoleViewUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.UC.RoleViewUC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/CommentsUserControl.ascx" TagPrefix="uc1" TagName="CommentsUserControl" %>
<%@ Register Src="~/UsrControl/UserSODConflicts.ascx" TagPrefix="uc1" TagName="UserSODConflicts" %>
<%@ Register Src="~/Admin/Attestation/UC/AttestationReviewHistoryUC.ascx" TagPrefix="uc1" TagName="AttestationReviewHistoryUC" %>

<script type="text/javascript">
    var attestationType = "<%= CurrentAttestationType %>";
    function HierarchyExpanding(sender, eventArgs) {
        var grid = $find("<%= AttestationReviewGrid.ClientID %>");

        $(grid.MasterTableView.getColumnByUniqueName("Type").get_element()).text("Entitlement Type");

        if (attestationType == AttestationType.Privileged) {
            $(grid.MasterTableView.getColumnByUniqueName("Reserved1").get_element()).text("Operator Id");
            $(grid.MasterTableView.getColumnByUniqueName("Reserved1").get_element()).text("Operator Id");
            $(grid.MasterTableView.getColumnByUniqueName("Reserved2").get_element()).text("Activity DateTime");
            $(grid.MasterTableView.getColumnByUniqueName("ActionReserved1").get_element()).text("Request Id");
            $(grid.MasterTableView.getColumnByUniqueName("DisplayName").get_element()).text("Activity");
        }
        else if (attestationType == AttestationType.Role)
        {
            var gridItem = grid.MasterTableView.get_dataItems()[0];
            var entpointSystemID = gridItem.getDataKeyValue("EndpointSystemType");

            $(grid.MasterTableView.getColumnByUniqueName("Name").get_element()).text("Name");
            $(grid.MasterTableView.getColumnByUniqueName("DisplayName").get_element()).text("Description");

            if (entpointSystemID == EndpointSystem.CasinoManagementSystem) {
                var rs1 = grid.MasterTableView.getColumnByUniqueName("Reserved1");
                if (rs1 != null && rs1.get_element() != null)
                    $(rs1.get_element()).text("View");

                var rs2 = grid.MasterTableView.getColumnByUniqueName("Reserved2");
                if (rs2 != null && rs2.get_element() != null)
                    $(rs2.get_element()).text("Create");

                var rs3 = grid.MasterTableView.getColumnByUniqueName("Reserved3");
                if (rs3 != null && rs3.get_element() != null)
                    $(rs3.get_element()).text("Modify");

                var rs4 = grid.MasterTableView.getColumnByUniqueName("Reserved4");
                if (rs4 != null && rs4.get_element() != null)
                    $(rs4.get_element()).text("Delete");

                var rs5 = grid.MasterTableView.getColumnByUniqueName("Reserved5");
                if (rs5 != null && rs5.get_element() != null)
                    $(rs5.get_element()).text("Print");

                var rs6 = grid.MasterTableView.getColumnByUniqueName("Reserved6");
                if (rs6 != null && rs6.get_element() != null)
                    $(rs6.get_element()).text("Export");
            }

            if (entpointSystemID == EndpointSystem.PSFinance) {
                var rs1 = grid.MasterTableView.getColumnByUniqueName("Reserved1");
                if (rs1 != null && rs1.get_element() != null)
                    $(rs1.get_element()).text("MenuName");

                var rs2 = grid.MasterTableView.getColumnByUniqueName("Reserved2");
                if (rs2 != null && rs2.get_element() != null)
                    $(rs2.get_element()).text("BarName");

                var rs3 = grid.MasterTableView.getColumnByUniqueName("Reserved3");
                if (rs3 != null && rs3.get_element() != null)
                    $(rs3.get_element()).text("ContentReference");

                var rs4 = grid.MasterTableView.getColumnByUniqueName("Reserved4");
                if (rs4 != null && rs4.get_element() != null)
                    $(rs4.get_element()).text("Description");

                var rs5 = grid.MasterTableView.getColumnByUniqueName("Reserved5");
                if (rs5 != null && rs5.get_element() != null)
                    $(rs5.get_element()).text("IsDisplay");

            }
        }
        doSomething(grid.MasterTableView);
    }

    function doSomething(masterTable) {
        var gridItems = masterTable.get_dataItems();
        gridItems.forEach(function (gridItem) {

            detailTableViews = gridItem.get_nestedViews();
            detailTableViews.forEach(function (detailView) {
                detailView.hideColumn(detailView.getColumnByUniqueName("Reserved1").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("Reserved2").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("Reserved3").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("Reserved4").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("Reserved5").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("Reserved6").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("Reserved7").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("Reserved8").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("Reserved9").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("Reserved10").get_element().cellIndex);

                detailView.hideColumn(detailView.getColumnByUniqueName("ActionReserved1").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("ActionReserved2").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("ActionReserved3").get_element().cellIndex);


                var entpointSystemID = gridItem.getDataKeyValue("EndpointSystemType");
                $(detailView.getColumnByUniqueName("Type").get_element()).text("Entitlement Type");
                if (detailView.getColumnByUniqueName("TemplateEditColumn") != null) {
                    detailView.hideColumn(detailView.getColumnByUniqueName("TemplateEditColumn").get_element().cellIndex);
                }
                if (detailView.getColumnByUniqueName("RevokeGridTemplateColumn") != null) {
                    detailView.showColumn(detailView.getColumnByUniqueName("RevokeGridTemplateColumn").get_element().cellIndex);
                }
                
                if (attestationType == AttestationType.Privileged) {
                    $(detailView.getColumnByUniqueName("Reserved1").get_element()).text("Operator Id");
                    $(detailView.getColumnByUniqueName("Reserved2").get_element()).text("Activity DateTime");
                    $(detailView.getColumnByUniqueName("ActionReserved1").get_element()).text("Request Id");
                    $(detailView.getColumnByUniqueName("DisplayName").get_element()).text("Activity");
                }
                else if (attestationType == AttestationType.Role) {
                    if (entpointSystemID == EndpointSystem.CasinoManagementSystem) {
                        var rs1 = detailView.getColumnByUniqueName("Reserved1");
                        if (rs1 != null && rs1.get_element() != null)
                            $(rs1.get_element()).text("View");

                        var rs2 = detailView.getColumnByUniqueName("Reserved2");
                        if (rs2 != null && rs2.get_element() != null)
                            $(rs2.get_element()).text("Create");

                        var rs3 = detailView.getColumnByUniqueName("Reserved3");
                        if (rs3 != null && rs3.get_element() != null)
                            $(rs3.get_element()).text("Modify");

                        var rs4 = detailView.getColumnByUniqueName("Reserved4");
                        if (rs4 != null && rs4.get_element() != null)
                            $(rs4.get_element()).text("Delete");

                        var rs5 = detailView.getColumnByUniqueName("Reserved5");
                        if (rs5 != null && rs5.get_element() != null)
                            $(rs5.get_element()).text("Print");

                        var rs6 = detailView.getColumnByUniqueName("Reserved6");
                        if (rs6 != null && rs6.get_element() != null)
                            $(rs6.get_element()).text("Export");
                    }

                    if (entpointSystemID == EndpointSystem.PSFinance) {
                        var rs1 = detailView.getColumnByUniqueName("Reserved1");
                        if (rs1 != null && rs1.get_element() != null)
                            $(rs1.get_element()).text("MenuName");

                        var rs2 = detailView.getColumnByUniqueName("Reserved2");
                        if (rs2 != null && rs2.get_element() != null)
                            $(rs2.get_element()).text("BarName");

                        var rs3 = detailView.getColumnByUniqueName("Reserved3");
                        if (rs3 != null && rs3.get_element() != null)
                            $(rs3.get_element()).text("ContentReference");

                        var rs4 = detailView.getColumnByUniqueName("Reserved4");
                        if (rs4 != null && rs4.get_element() != null)
                            $(rs4.get_element()).text("Description");

                        var rs5 = detailView.getColumnByUniqueName("Reserved5");
                        if (rs5 != null && rs5.get_element() != null)
                            $(rs5.get_element()).text("IsDisplay");

                    }
                }

                var level = gridItem.getDataKeyValue("Level");
                detailView.hideColumn(detailView.getColumnByUniqueName("LastLogon").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("AccountType").get_element().cellIndex);
                detailView.hideColumn(detailView.getColumnByUniqueName("Delta").get_element().cellIndex);

                $(detailView.getColumnByUniqueName("Name").get_element()).text("Name");
                $(detailView.getColumnByUniqueName("DisplayName").get_element()).text("Description");


                if (level - 1 == 1) {
                    detailView.hideColumn(detailView.getColumnByUniqueName("Type").get_element().cellIndex);
                    detailView.showColumn(detailView.getColumnByUniqueName("AccountType").get_element().cellIndex);

                    if (attestationType == AttestationType.User) {
                        detailView.showColumn(detailView.getColumnByUniqueName("Delta").get_element().cellIndex);
                    }

                    $(detailView.getColumnByUniqueName("Name").get_element()).text("User Name");
                    $(detailView.getColumnByUniqueName("DisplayName").get_element()).text("Display Name");
                }

                //if (level == 1 && attestationType == AttestationType.User) {
                //    detailView.showColumn(detailView.getColumnByUniqueName("Delta").get_element().cellIndex);
                //}

                if (detailView._hasDetailTables) {
                    doSomething(detailView);
                }
            });
        });
    }
</script>

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
                    OnItemCreated="AttestationReviewGrid_ItemCreated">
                    <HeaderStyle Font-Bold="True" />
                    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="10" Height="100%" AlwaysVisible="true" />
                    <GroupingSettings CaseSensitive="false" />
                    <ClientSettings>
                        <Scrolling UseStaticHeaders="true" AllowScroll="false" />
                    </ClientSettings>
                    <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="true" CommandItemDisplay="None" Width="100%"
                        EnableHeaderContextMenu="false" DataKeyNames="KeyId,SnapshotActionId,Entity,Level,ReviewCategory,Comments,ParentBindingLevel,BindingLevel,EndpointSystemId,EndpointSystemType,ParentEntity" NoMasterRecordsText="No Records Available"
                        HierarchyDefaultExpanded="false" HierarchyLoadMode="ServerOnDemand" RetainExpandStateOnRebind="true" EnableHierarchyExpandAll="true" TableLayout="Auto"
                        ClientDataKeyNames="EndpointSystemType,Level">
                        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                        <SelfHierarchySettings ParentKeyName="ParentBindingLevel" KeyName="BindingLevel" />
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="Delta" HeaderText="Delta" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:ImageButton ID="DeltaIcon" Visible='<%#Convert.ToBoolean(Eval("IsDelta")) == true %>' runat="server" CommandName="ShowDelta"></asp:ImageButton>
                                </ItemTemplate>
                                <ItemStyle CssClass="delta-info" HorizontalAlign="Left" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Name"
                                HeaderText="Name" SortExpression="Name" UniqueName="Name" FilterControlWidth="86.7%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName"
                                HeaderText="Description" SortExpression="DisplayName" UniqueName="DisplayName" FilterControlWidth="86.7%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Type" FilterControlAltText="Type"
                                HeaderText="Entitlement Type" SortExpression="Type" UniqueName="Type" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="AccountType" FilterControlAltText="AccountType"
                                HeaderText="Account Type" SortExpression="AccountType" UniqueName="AccountType" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="LastLogon" FilterControlAltText="LastLogon"
                                HeaderText="Last Logon" SortExpression="LastLogon" UniqueName="LastLogon" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Reserved1" FilterControlAltText="Reserved1"
                                HeaderText="Reserved1" SortExpression="Reserved1" UniqueName="Reserved1" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Reserved2" FilterControlAltText="Reserved2"
                                HeaderText="Reserved2" SortExpression="Reserved2" UniqueName="Reserved2" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Reserved3" FilterControlAltText="Reserved3"
                                HeaderText="Reserved3" SortExpression="Reserved3" UniqueName="Reserved3" FilterControlWidth="70%">
                            </telerik:GridBoundColumn>
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
                                HeaderText="Reserved10" SortExpression="Reserved10" UniqueName="Reserved10" FilterControlWidth="70%">
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
                                HeaderText="ReviewCategory" SortExpression="ReviewCategory" UniqueName="ReviewCategory" FilterControlWidth="70%" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="TemplateEditColumn" HeaderText="Risk Info" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ShowWarningIcon" Visible='<%#Convert.ToBoolean(Eval("IsSODConflict")) == true %>' runat="server" CommandName="ShowWarningInfo"></asp:ImageButton>
                                </ItemTemplate>
                                <ItemStyle CssClass="warning-info" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="IsChangedBaseGridTemplateColumn" HeaderText="Changed" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" Display="false">
                                <ItemTemplate>
                                    <asp:Label ID="IsChangedBaseLabel" runat="server" Text='<%#Convert.ToBoolean(Eval("IsChanged")) ? "Yes" : "" %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn UniqueName="SnapshotActionId" DataField="SnapshotActionId" HeaderText="SnapshotActionId" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="ApproveGridTemplateColumn" HeaderText="Retain" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:RadioButton ID="ApproveRadioButton" runat="server" AutoPostBack="true" GroupName="ApproveRevoke" OnCheckedChanged="ApproveRadioButton_CheckedChanged" CssClass="RadGrid_SkinName"
                                        Checked='<%#Convert.ToBoolean(Eval("Approved")) %>' Visible='<%#Convert.ToBoolean(Eval("IsActionable")) %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="RevokeGridTemplateColumn" HeaderText="Revoke" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:RadioButton ID="RevokeRadioButton" runat="server" AutoPostBack="true" GroupName="ApproveRevoke" OnCheckedChanged="RevokeRadioButton_CheckedChanged" CssClass="RadGrid_SkinName"
                                        Checked='<%#Convert.ToBoolean(Eval("Rejected")) %>' Visible='<%#Convert.ToBoolean(Eval("IsActionable")) %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="EffectiveDateTemplateColumn" HeaderText="Effective Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" AllowFiltering="false">
                                <ItemTemplate>
                                    <telerik:RadDatePicker runat="server" ID="ValidFromPicker" DateTimePickerElement="" NullText="No Date" NullDate="DateTime.Now()" DbSelectedDate='<%# Bind("EffectiveDate") %>' EnableTyping="false" Skin="IACMSkin" ShowPopupOnFocus="false" CssClass="form-control" DatePopupButton-Visible="true">
                                    </telerik:RadDatePicker>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Comments" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" UniqueName="ReviewCommentsBoundColumn" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:ImageButton ID="AddCommentLinkButton" runat="server" Text="Add Comments" OnClick="AddCommentImageButton_Click"
                                        CommandArgument='<%#Eval("KeyId") %>' CommandName="ReviewComment"></asp:ImageButton>
                                </ItemTemplate>
                                <ItemStyle CssClass="action-comment" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="ReviewHistory" HeaderText="Review History" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ShowReviewHistoryIcon" Visible='<%#Convert.ToBoolean(Eval("IsChangedInPreviousLevel")) == true %>' runat="server" CommandName="ShowReviewHistory"></asp:ImageButton>
                                </ItemTemplate>
                                <ItemStyle CssClass="history-info" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings AllowExpandCollapse="true">
                        <ClientEvents OnGridCreated="HierarchyExpanding" />
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


<asp:Panel ID="Panel2" runat="server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <div class="buttons text-right topBuffer" runat="server" id="trrbtn">
                            <asp:Button ID="CancelButton" runat="server" OnClick="CancelButton_Click" Text="Cancel" CssClass="btn btn-default btn-sm" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>

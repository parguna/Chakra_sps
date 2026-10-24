<%@ Page Title="Attestation Review" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Review.aspx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.Review" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/CommentsUserControl.ascx" TagPrefix="uc1" TagName="CommentsUserControl" %>
<%@ Register Src="~/UsrControl/UserSODConflicts.ascx" TagPrefix="uc1" TagName="UserSODConflicts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">

    <asp:UpdatePanel ID="updSearch" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>

            <asp:Panel ID="tblAttestationReviewPage" runat="server">
                <div class="content-wrapper">
                    <div class="form">
                        <div class="form-horizontal left">
                            <div class="row-fluid">
                                <div class="col-xs-5">
                                    <div class="form-group">
                                        <asp:Label ID="lblAttestationName" Font-Bold="true" AssociatedControlID="lblAttName" runat="server" CssClass="col-xs-2 control-label" Text="Name"></asp:Label>
                                        <asp:Label ID="lblAttName" runat="server" CssClass="col-xs-10 control-label align-left"></asp:Label>
                                    </div>
                                </div>
                                <div class="col-xs-5">
                                    <div class="form-group">
                                        <asp:Label ID="lblAttestationDescription" Font-Bold="true" AssociatedControlID="AttestationDescLabel" runat="server" CssClass="col-xs-2 control-label" Text="Description"></asp:Label>
                                        <asp:Label ID="AttestationDescLabel" runat="server" CssClass="col-xs-10 control-label align-left"></asp:Label>
                                   </div>
                                </div>
                                <div class="col-xs-6">
                                    <div class="form-group">
                                        <asp:Label ID="lblCreatedDate" AssociatedControlID="lblReqDate" Font-Bold="true" runat="server" CssClass="col-xs-4 control-label" Text="Created Date" Visible="false"></asp:Label>
                                        <div class="col-xs-6">
                                            <asp:Label ID="lblReqDate" runat="server" CssClass="form-control" Visible="false"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-6">
                                    <div class="form-group">
                                        <asp:Label ID="lblSubmitBefore" AssociatedControlID="SubmitBeforeLabel" Font-Bold="true" runat="server" CssClass="col-xs-4 control-label" Text="Submit Before" Visible="false"></asp:Label>
                                        <div class="col-xs-6">
                                            <asp:Label ID="SubmitBeforeLabel" runat="server" CssClass="form-control" Visible="false"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-6">
                                    <div class="form-group">
                                        <div id="PreviousApproversDiv" runat="server" style="overflow: auto; margin-top: 5px; margin-bottom: 5px">
                                            <asp:Repeater ID="PreviousApproversRepeater" runat="server">
                                                <ItemTemplate>
                                                    <div style="margin: 6px;">
                                                        <span class="wizlbl" style="font-weight:bold">Reviewed By </span>
                                                        <span class="user" style="padding-left:30px"><%# DataBinder.Eval(Container.DataItem, "UserName")%></span>
                                                        <span class="wizlbl">on </span>
                                                        <span class="user"><%# DataBinder.Eval(Container.DataItem, "ApprovedDate", "{0:yyyy-MM-dd hh:mm:ss}")%></span>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-xs-12">
                                    <div class="col-xs-6">
                                        <div class="form-group">
                                            <div class="buttons text-left topBuffer">
                                                <asp:Button ID="ExpandAllButton" runat="server" OnClick="ExpandAllButton_Click" Text="Expand All" CssClass="btn btn-default btn-sm"></asp:Button>&nbsp;&nbsp;
                                            <asp:Button ID="ExpandAllUptoTCode" runat="server" OnClick="ExpandAllUptoTCode_Click" Text="Expand All Upto TCode" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
                                            <asp:Button ID="CollapseAllButton" runat="server" OnClick="CollapseAllButton_Click" Text="Collapse All" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
                                            <asp:Button ID="ExportExcelButton" Visible="false" runat="server" OnClick="ExportExcelButton_Click" Text="Export To Excel" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-xs-6">
                                        <div class="form-group">
                                            <div class="buttons text-right topBuffer" runat="server" id="tddbtn">
                                                <asp:Button ID="Button1" runat="server" OnClick="RevertButton_Click" Text="Revert" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
                                        <asp:Button ID="SaveButton1" runat="server" OnClick="SaveButton_Click" Text="Save" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
                                        <asp:Button ID="SubmitButton1" runat="server" OnClick="SubmitButton_Click" Text="Submit" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
                                        <asp:Button ID="Button3" runat="server" OnClick="CancelButton_Click" Text="Cancel" CssClass="btn btn-default btn-sm" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
            </asp:Panel>

            <asp:Panel ID="Panel1" runat="server" CssClass="topBuffer">
                <div style="overflow: auto;">
                    <telerik:RadGrid ID="AttestationReviewGrid" ShowStatusBar="True" GridLines="None" CellSpacing="0"
                        runat="server" AutoGenerateColumns="False" PageSize="100" AllowSorting="True" AllowPaging="True"
                        OnDetailTableDataBind="AttestationReviewGrid_DetailTableDataBind" OnPreRender="AttestationReviewGrid_PreRender"
                        OnNeedDataSource="AttestationReviewGrid_NeedDataSource" OnItemDataBound="AttestationReviewGrid_ItemDataBound" OnItemCommand="AttestationReviewGrid_ItemCommand"
                        GroupPanelPosition="Top">
                        <HeaderStyle Font-Bold="True" />
                        <PagerStyle Mode="NumericPages" AlwaysVisible="true"></PagerStyle>
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView EnableHierarchyExpandAll="true" AllowMultiColumnSorting="True" DataKeyNames="ReviewId,AttestationReview.UserId"
                            ShowHeadersWhenNoRecords="false">
                            <DetailTables>
                                <telerik:GridTableView EnableHierarchyExpandAll="false" PagerStyle-AlwaysVisible="false" ShowHeader="true" DataKeyNames="Id" Width="100%"
                                    runat="server" Name="EPSystem" HorizontalAlign="Left" NoDetailRecordsText=""
                                    ShowHeadersWhenNoRecords="false">
                                    <DetailTables>
                                        <telerik:GridTableView EnableHierarchyExpandAll="true" PagerStyle-AlwaysVisible="false" DataKeyNames="L2DetailId,Id,AccessName" Width="100%"
                                            runat="server" Name="EPSystemL2" NoDetailRecordsText=""
                                            ShowHeadersWhenNoRecords="false">
                                            <DetailTables>
                                                <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="L3DetailId" Width="100%"
                                                    runat="server" Name="EPSystemL3" AllowPaging="false" NoDetailRecordsText="" ShowHeadersWhenNoRecords="false">
                                                    <DetailTables>
                                                        <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="Id" Width="100%" runat="server" Name="SAPOnlyDetails" AllowPaging="false" NoDetailRecordsText=""
                                                            ShowHeadersWhenNoRecords="false">
                                                            <Columns>
                                                                <telerik:GridBoundColumn SortExpression="sapLevel1Detail" HeaderText="TCodes"
                                                                    DataField="sapLevel1Detail" UniqueName="DetailSapLevel1GridBoundColumn">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn SortExpression="sapLevel1DetailDesc" HeaderText="Description"
                                                                    DataField="sapLevel1DetailDesc" UniqueName="DetailTypeSapLevel1GridBoundColumn">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridTemplateColumn UniqueName="IsChangedSAPGridTemplateColumn" HeaderText="Changed" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right" Visible="false">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="IsChangedSAPLabel" runat="server" Text='<%#Convert.ToBoolean(Eval("IsChanged")) ? "Yes" : "" %>' />
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                            </Columns>
                                                        </telerik:GridTableView>
                                                    </DetailTables>
                                                    <Columns>
                                                        <telerik:GridTemplateColumn Display="false">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="HiddenField1" runat="server" Value='<%#Eval("Id") %>' />
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridBoundColumn SortExpression="AccessName" HeaderText="AccessName"
                                                            DataField="AccessName" UniqueName="AccessNameL3GridBoundColumn">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn SortExpression="AccessType" HeaderText="AccessType"
                                                            DataField="AccessType" UniqueName="AccessTypeL3GridBoundColumn">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridTemplateColumn UniqueName="ApproveL3GridTemplateColumn" HeaderText="Approve" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right">
                                                            <ItemTemplate>
                                                                <asp:RadioButton ID="ApproveL3RadioButton" runat="server" AutoPostBack="true" GroupName="L3" OnCheckedChanged="ApproveL3RadioButton_CheckedChanged" CssClass="RadGrid_SkinName"
                                                                    Checked='<%#Convert.ToBoolean(Eval("Approve")) %>'/>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn UniqueName="RevokeL3GridTemplateColumn" HeaderText="Revoke" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:RadioButton ID="RevokeL3RadioButton" runat="server" AutoPostBack="true" GroupName="L3" OnCheckedChanged="RevokeL3RadioButton_CheckedChanged" CssClass="RadGrid_SkinName"
                                                                    Checked='<%#Convert.ToBoolean(Eval("Revoke")) %>'/>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn UniqueName="HiddenL3GridTemplateColumn" HeaderText="Hidden" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right">
                                                            <ItemTemplate>
                                                                <asp:RadioButton ID="HiddenL3RadioButton" runat="server" AutoPostBack="true" GroupName="L3" OnCheckedChanged="HiddenL3RadioButton_CheckedChanged"
                                                                    Checked='<%#Convert.ToBoolean(Eval("Hidden")) %>'/>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn UniqueName="IsChangedL3GridTemplateColumn" HeaderText="Changed" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="IsChangedL3Label" runat="server" Text='<%#Convert.ToBoolean(Eval("IsChanged")) ? "Yes" : "" %>' />
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridBoundColumn SortExpression="PreviousAccess" HeaderText="Current Access" DataField="PreviousAccess" UniqueName="PreviousAccessL3GridBoundColumn" HeaderStyle-Width="105px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right" Visible="false">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridTemplateColumn HeaderText="Comments" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right" UniqueName="CommentsL3GridBoundColumn">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="AddCommentLinkButton" runat="server" Text="Add Comments" OnClick="AddCommentImageButton_Click"
                                                                    CommandArgument='<%#Eval("Id") %>' CommandName="EndpointSystemL3Comment"></asp:ImageButton>
                                                            </ItemTemplate>
                                                            <ItemStyle CssClass="action-comment" HorizontalAlign="Center" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </telerik:GridTemplateColumn>
                                                    </Columns>
                                                </telerik:GridTableView>
                                            </DetailTables>
                                            <Columns>
                                                <telerik:GridTemplateColumn Display="false">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="HiddenField2" runat="server" Value='<%#Eval("L2DetailId") %>' />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridBoundColumn SortExpression="AccessName" HeaderText="Permission"
                                                    DataField="AccessName" UniqueName="AccessNameL2GridBoundColumn">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn SortExpression="AccessDescription" HeaderText="Access Description"
                                                    DataField="AccessDescription" UniqueName="AccessDescriptionL2GridBoundColumn">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridTemplateColumn UniqueName="AccessTypeL2GridBoundColumn" HeaderText="Access Type" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label ID="AccessTypeLabel" runat="server" Text='<%#DI.Chakra.Infrastructure.UtilityMethods.GetEnumAttribute<Description>((DI.Chakra.Infrastructure.Enums.EntitlementType)Eval("AccessType"))%>' />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn UniqueName="ResourceCompletePathL2GridBoundColumn" HeaderText="Page Detail" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label2" runat="server" Text='<%#Eval("ResourceType") %>'></asp:Label><br />
                                                        <asp:Label ID="Label1" runat="server" Text='<%#Eval("ResourceCompletePath") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>

                                                <telerik:GridTemplateColumn UniqueName="ApproveL2GridTemplateColumn" HeaderText="Retain" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:RadioButton ID="ApproveL2RadioButton" runat="server" AutoPostBack="true" GroupName="L2" OnCheckedChanged="ApproveL2RadioButton_CheckedChanged" CssClass="RadGrid_SkinName"
                                                            Checked='<%#Convert.ToBoolean(Eval("Approve")) %>'/>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn UniqueName="RevokeL2GridTemplateColumn" HeaderText="Revoke" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:RadioButton ID="RevokeL2RadioButton" runat="server" AutoPostBack="true" GroupName="L2" OnCheckedChanged="RevokeL2RadioButton_CheckedChanged" CssClass="RadGrid_SkinName"
                                                            Checked='<%#Convert.ToBoolean(Eval("Revoke")) %>'/>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn UniqueName="IsChangedL2GridTemplateColumn" HeaderText="Changed" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="IsChangedL2Label" runat="server" Text='<%#Convert.ToBoolean(Eval("IsChanged")) ? "Yes" : "" %>' />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridBoundColumn SortExpression="PreviousAccess" HeaderText="Current Access" DataField="PreviousAccess" UniqueName="PreviousAccessL2GridBoundColumn" HeaderStyle-Width="105px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right" Visible="false">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridTemplateColumn HeaderText="Comments" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="AddCommentLinkButton" runat="server" Text="Add Comments" OnClick="AddCommentImageButton_Click"
                                                            CommandArgument='<%#Eval("Id") %>' CommandName="EndpointSystemL2Comment"></asp:ImageButton>
                                                    </ItemTemplate>
                                                    <ItemStyle CssClass="action-comment" HorizontalAlign="Center" />
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </telerik:GridTableView>
                                    </DetailTables>
                                    <%--End EPSysytem Section--%>
                                    <Columns>
                                        <telerik:GridTemplateColumn Display="false">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="HiddenField3" runat="server" Value='<%#Eval("Id") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn SortExpression="EndpointUserName" HeaderText="Endpoint User Name"
                                            DataField="EndpointUserName" UniqueName="EndpointUserName">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn SortExpression="HostName" HeaderText="Endpoint System Name"
                                            DataField="HostName" UniqueName="HostName">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn SortExpression="EndpointSystemTypeName" HeaderText="Endpoint System Type Name"
                                            DataField="EndpointSystemTypeName" UniqueName="EndpointSystemTypeName" Visible="false">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn UniqueName="AccountTypeTemplateColumn" HeaderText="Account Type" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="AccountTypeLabel" runat="server" Text='<%#DI.Chakra.Infrastructure.UtilityMethods.GetEnumAttribute<Description>((DI.Chakra.Infrastructure.Enums.AccountType)Eval("AccountType"))%>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn SortExpression="IsActive" HeaderText="Active"
                                            DataField="IsActive" UniqueName="IsActive">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn HeaderText="Retain" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:RadioButton ID="ApproveRadioButton" runat="server" AutoPostBack="true" GroupName="L1" OnCheckedChanged="ApproveRadioButton_CheckedChanged" CssClass="RadGrid_SkinName"
                                                    Checked='<%#Convert.ToBoolean(Eval("Approve")) %>'/>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Revoke" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:RadioButton ID="RevokeRadioButton" runat="server" AutoPostBack="true" GroupName="L1" OnCheckedChanged="RevokeRadioButton_CheckedChanged" CssClass="RadGrid_SkinName"
                                                    Checked='<%#Convert.ToBoolean(Eval("Revoke"))%>'/>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn UniqueName="IsChangedGridTemplateColumn" HeaderText="Changed" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="IsChangedLabel" runat="server" Text='<%#Convert.ToBoolean(Eval("IsChanged")) ? "Yes" : "" %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn SortExpression="PreviousAccess" HeaderText="Current Access" DataField="PreviousAccess" UniqueName="PreviousAccessGridBoundColumn" HeaderStyle-Width="105px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right" Visible="false">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn HeaderText="Comments" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="AddCommentLinkButton" runat="server" Text="Add Comments" OnClick="AddCommentImageButton_Click"
                                                    CommandArgument='<%#Eval("Id") %>' CommandName="EndpointSystemComment"></asp:ImageButton>
                                                <%--OnClientClick='<%# String.Format("javascript: return ShowInfo(\"{0}\",\"{1}\");", Eval("ReviewEndpointId"),"EndpointSystemComment") %>'>--%>
                                            </ItemTemplate>
                                            <ItemStyle CssClass="action-comment" HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </telerik:GridTableView>
                            </DetailTables>
                            <Columns>
                                <telerik:GridBoundColumn DataField="AttestationReview.User.UserName" FilterControlAltText="AttestationReview.User.UserName"
                                    HeaderText="User Name" SortExpression="AttestationReview.User.UserName" UniqueName="AttestationReview.User.UserName">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="AttestationReview.User.DisplayName" FilterControlAltText="AttestationReview.User.DisplayName"
                                    HeaderText="Display Name" SortExpression="AttestationReview.User.DisplayName" UniqueName="AttestationReview.User.DisplayName">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="AttestationReview.User.EmailId" FilterControlAltText="AttestationReview.User.EmailId"
                                    HeaderText="EmailId" SortExpression="AttestationReview.User.EmailId" UniqueName="AttestationReview.User.EmailId">
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn UniqueName="TemplateEditColumn"  HeaderText="Risk Info" HeaderStyle-Width="7%">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ShowWarningIcon" Visible='<%#Convert.ToBoolean(Eval("AttestationReview.HasRisk")) == true %>' runat="server" CommandName="ShowWarningInfo"></asp:ImageButton>
                                        <%--<asp:ImageButton ID="ShowWarningIcon" Visible="true" runat="server" CommandName="ShowWarningInfo"></asp:ImageButton>--%>
                                    </ItemTemplate>
                                    <ItemStyle CssClass="warning-info" HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn UniqueName="IsChangedBaseGridTemplateColumn" HeaderText="Changed" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="IsChangedBaseLabel" runat="server" Text='<%#Convert.ToBoolean(Eval("AttestationReview.IsChanged")) ? "Yes" : "" %>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
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
                    <h3 class="modal-title">Risk Information</h3>
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

                </div>
            </asp:Panel>

            <asp:Panel ID="Panel2" runat="server">
                <div class="content-wrapper">
                    <div class="form">
                        <div class="form-horizontal left">
                            <div class="row-fluid">
                                <div class="col-xs-12">
                                    <div class="buttons text-right topBuffer" runat="server" id="trrbtn">
                                        <asp:Button ID="RevertButton" runat="server" OnClick="RevertButton_Click" Text="Revert" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
                                        <asp:Button ID="SaveButton" runat="server" OnClick="SaveButton_Click" Text="Save" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
                                        <asp:Button ID="SubmitButton" runat="server" OnClick="SubmitButton_Click" Text="Submit" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
                                        <asp:Button ID="CancelButton" runat="server" OnClick="CancelButton_Click" Text="Cancel" CssClass="btn btn-default btn-sm" />
                                    </div>
                                </div>

                                <div class="col-xs-6">
                                    <div class="buttons text-right topBuffer" runat="server" id="trrback" visible="false">
                                        <asp:Button ID="btnBack" runat="server" OnClick="btnBack_Click" Text="Cancel" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
                                        <div id="hiddenDiv" style="display: none; visibility: hidden;">
                                            <asp:Button runat="server" ID="hiddenButton" />
                                        </div>
                                        <asp:ModalPopupExtender ID="CommentsModalPopupExtender" runat="server" TargetControlID="hiddenButton"
                                            PopupControlID="CommentsPanel" BackgroundCssClass="modalBackground"
                                            BehaviorID="mpeReUser">
                                        </asp:ModalPopupExtender>
                                        <asp:Panel ID="CommentsPanel" CssClass="ModalWindow" runat="server">
                                            <uc1:CommentsUserControl runat="server" ID="CommentsUserControl" />
                                        </asp:Panel>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>


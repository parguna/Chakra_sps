<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.ProvRS.ProvisioningFields.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Src="~/ProvRS/ProvisioningFields/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">

    <telerik:RadWindow ID="AddUpdateModalPopup" RenderMode="Lightweight" runat="server" Width="800px" Height="200px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" VisibleStatusbar="false">

        <ContentTemplate>
            <div class="modal-header">
                <h3 class="modal-title" runat="server" id="H1">Add Provisioning Field</h3>
            </div>
            <div class="modal-body">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <uc1:DetailUC runat="server" ID="DetailUC" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div class="modal-footer">
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" data-dismiss="modal" OnClick="btnCancel_Click"></asp:Button>
                <asp:Button ID="btnSubmit" CssClass="add btn btn-primary bordered-button" runat="server"
                    Text="Add" OnClick="btnSubmit_Click"></asp:Button>
            </div>

        </ContentTemplate>
    </telerik:RadWindow>
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-0">
                        <div style="font-weight: bold">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Applications</div>
                    </div>
                    <div class="form-group">
                    </div>
                    <div class="col-xs-2">
                        <telerik:RadTreeView ID="RadTreeViewProvField" DataKeyNames="EndPointSystemId,EndPointSystemTypeId" RenderMode="Lightweight" Visible="true" runat="server" DataTextField="Name" DataValueField="Id" DataFieldID="RowID" DataFieldParentID="ParentRowID"
                            OnNodeClick="RadTreeViewProvField_NodeClick" OnNodeCreated="RadTreeViewProvField_NodeCreated" OnNodeDataBound="RadTreeViewProvField_NodeDataBound" ShowLineImages="true" Style="overflow: auto" HorizontalAlignment="Left">
                            <DataBindings>
                                <telerik:RadTreeNodeBinding Expanded="False"></telerik:RadTreeNodeBinding>
                            </DataBindings>
                            <NodeTemplate>
                                <asp:Label ID="lblNodeText" runat="server" Text='<%# DataBinder.Eval(Container, "Text") %>'></asp:Label>
                                <asp:HiddenField ID="hdnEndpointHasValues" runat="server" Visible="false" Value='<%# Eval("EndPointHasValues") %>'></asp:HiddenField>
                                <asp:ImageButton ID="imgNode" Height="20px" Width="30px" runat="server" OnClientClick="return confirm('Are you sure want to delete?')" ImageUrl="~/Images/Treeview/treedelete.png" />
                            </NodeTemplate>
                        </telerik:RadTreeView>
                    </div>
                    <div class="form-group" id="divGenerate" runat="server" visible="false">
                        <div class="col-xs-7">
                            No provisioning Field(s) configured for
                            <asp:Label ID="lblEndpointSystem" Font-Bold="true" runat="server" Text=""></asp:Label>
                            System,click here to
                            <asp:LinkButton ID="lnkGenerates" Text="generate" runat="server" OnClientClick="return confirm('Are you sure want to generate?')" OnClick="lnkGenerates_Click"></asp:LinkButton>
                            from Base type.
                        </div>
                    </div>
                    <div class="col-xs-10">
                        <cc1:IACMRadGridControl ID="gvProvisioningField" runat="server" OnNeedDataSource="gvProvisioningField_NeedDataSource" OnSelectedIndexChanged="gvProvisioningField_SelectedIndexChanged">
                            <MasterTableView DataKeyNames="Id"  Width="100%"
                                NoMasterRecordsText="No Data Found">

                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>

                                <Columns>

                                    <telerik:GridTemplateColumn DataField="FieldName" FilterControlAltText="FieldName" HeaderText="Field Name" SortExpression="FieldName" UniqueName="FieldName" HeaderStyle-Width="200px" FilterControlWidth="70%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFieldName" runat="server" Text='<%# Bind("FieldName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridTemplateColumn>

                                   <%-- <telerik:GridTemplateColumn DataField="PrimaryIdentifier" FilterControlAltText="PrimaryIdentifier" HeaderText="Primary Identifier" SortExpression="PrimaryIdentifier" UniqueName="PrimaryIdentifier" HeaderStyle-Width="158px" FilterControlWidth="80%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPrimaryIdentifier" runat="server" Text='<%#Convert.ToBoolean( Eval("PrimaryIdentifier")) == true ? "Yes" : "No" %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridTemplateColumn>--%>

                                    <telerik:GridTemplateColumn DataField="AccountType" FilterControlAltText="AccountType" HeaderText="Account Type" SortExpression="AccountType" UniqueName="AccountType" HeaderStyle-Width="140px" FilterControlWidth="49.5%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAccountType" runat="server" Text='<%# Bind("AccountType") %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridTemplateColumn>

                                    <%--<telerik:GridTemplateColumn DataField="ResolverType" FilterControlAltText="ResolverType" HeaderText="Resolver Type" SortExpression="ResolverType" UniqueName="ResolverType" HeaderStyle-Width="160px" FilterControlWidth="64.5%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblResolverType" runat="server" Text='<%# Bind("ResolverType") %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridTemplateColumn>--%>

                                   <%-- <telerik:GridTemplateColumn DataField="FieldScope" FilterControlAltText="FieldScope" HeaderText="Field Scope" SortExpression="FieldScope" UniqueName="FieldScope" HeaderStyle-Width="135px" FilterControlWidth="49%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFieldScope" runat="server" Text='<%# Bind("FieldScope") %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridTemplateColumn>--%>

                                    <%--<telerik:GridTemplateColumn DataField="CanDerive" FilterControlAltText="CanDerive" HeaderText="Can Derive" SortExpression="CanDerive" UniqueName="CanDerive" HeaderStyle-Width="95px" FilterControlWidth="50.5%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCanDerive" runat="server" Text='<%#Convert.ToBoolean( Eval("CanDerive")) == true ? "Yes" : "No" %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridTemplateColumn>--%>

                                    <telerik:GridTemplateColumn DataField="ValueDisplayName" FilterControlAltText="Value" HeaderText="Value" SortExpression="ValueDisplayName" UniqueName="ValueDisplayName" HeaderStyle-Width="180px" FilterControlWidth="70%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblValue" runat="server" Text='<%# Bind("ValueDisplayName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn DataField="IsActive" FilterControlAltText="IsActive" HeaderText="IsActive" SortExpression="IsActive" UniqueName="IsActive" HeaderStyle-Width="85px" FilterControlWidth="93.5%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIsActive" runat="server" Text='<%#Convert.ToBoolean( Eval("IsActive")) == true ? "Yes" : "No" %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridDateTimeColumn DataField="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" FilterControlWidth="85%" HeaderStyle-Width="100px">
                                    </telerik:GridDateTimeColumn>
                                    <telerik:GridBoundColumn DataField="CreatedBy" HeaderText="Created By" SortExpression="CreatedBy" UniqueName="CreatedBy" FilterControlWidth="60%" HeaderStyle-Width="100px">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridDateTimeColumn DataField="LastModifiedDate" HeaderText="Last Modified Date" SortExpression="LastModifiedDate" UniqueName="LastModifiedDate" FilterControlWidth="85%" HeaderStyle-Width="100px">
                                    </telerik:GridDateTimeColumn>
                                    <telerik:GridBoundColumn DataField="LastModifiedBy" HeaderText="Last Modified By" SortExpression="LastModifiedBy" UniqueName="LastModifiedBy" FilterControlWidth="60%" HeaderStyle-Width="100px">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </cc1:IACMRadGridControl>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

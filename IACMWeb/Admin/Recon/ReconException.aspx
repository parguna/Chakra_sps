<%@ Page Title="ReconException" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="ReconException.aspx.cs" Inherits="DI.Chakra.Web.Admin.Recon.ReconException" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
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
    <asp:Panel runat="server" ID="tblMainList">

        <div class="content-wrapper">
            <div class="form">
                <div class="form-horizontal left clearfix">
                    <div class="row-fluid">
                        <div class="col-xs-6">
                            <uc1:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="EndpointUC_EndpointChanged" />
                            <div class="form-group">
                                <label class="col-xs-4 control-label">Exception Type</label>
                                <div class="col-xs-7">
                                    <asp:DropDownList ID="ExceptionTypesField" runat="server" CssClass="form-control selectpicker" data-width="100%"
                                         AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ExceptionTypesField_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label AssociatedControlID="IsResolvedField" CssClass="col-xs-4" Text="Action Type" runat="server"></asp:Label>
                                <div class="col-xs-7">
                                    <asp:CheckBox runat="server" ID="IsResolvedField"></asp:CheckBox>

                                    <asp:Label ID="Label2" runat="server" CssClass="lbl" Text="Resolved"></asp:Label>

                                    <asp:CheckBox runat="server" ID="IsDeletedField" Visible ="false"></asp:CheckBox>
                                    <asp:Label ID="Label3" runat="server" CssClass="lbl" Text="Deleted" Visible ="false"></asp:Label>
                                </div>
                                <%--   <div class="form-group" runat="server" visible="false">
                                        <label class="col-xs-4">IsDeleted</label>
                                        <div class="col-xs-2">
                                            <asp:RadioButton ID="chkEntitlementReconcilation" AutoPostBack="true" GroupName="ReconcilationType" runat="server" />
                                            <asp:Label ID="Label6" runat="server" CssClass="lbl" Text="Entitlement"></asp:Label>
                                        </div>
                                    </div>
                                --%>
                            </div>
                            <div class="form-group">
                                <asp:Label AssociatedControlID="chkAccount" CssClass="col-xs-4" Text="Reconcilation Type" runat="server"></asp:Label>
                                <div class="col-xs-7">
                                    <asp:RadioButton ID="chkAccount" AutoPostBack="true" GroupName="ReconcilationType" Checked="true" runat="server" />
                                    <asp:Label ID="Account" runat="server" CssClass="lbl" Text="Account"></asp:Label>

                                    <asp:RadioButton ID="chkEntitlement" AutoPostBack="true" GroupName="ReconcilationType" runat="server" />
                                    <asp:Label ID="Label1" runat="server" CssClass="lbl" Text="Entitlement"></asp:Label>

                                    <asp:RadioButton ID="chkAll" AutoPostBack="true" GroupName="ReconcilationType" runat="server" />
                                    <asp:Label ID="Label5" runat="server" CssClass="lbl" Text="All"></asp:Label>
                                </div>
                            </div>


                            <div class="form-group">
                                <div class="col-xs-4">
                                </div>
                                <div class="col-xs-7 text-right">
                                    <asp:Button ID="search" runat="server" Text="Search" CssClass="btn btn-default btn-sm" OnClick="search_Click" />
                                </div>
                            </div>

                        </div>
                        <asp:HiddenField ID="hf1" runat="server" />


                        <asp:Panel ID="grdPanelEntitlement" Visible="true" runat="server" CssClass="inlineBlock topBuffer">
                            <cc1:IACMRadGridControl ID="grdEntitlement" OnNeedDataSource="grdEntitlement_NeedDataSource" AllowAutomaticUpdates="false" FilterMenu-CssClass="RadMenu_IACMFilterSkin"
                                runat="server" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" AllowMultiRowSelection="true"
                                OnItemDataBound="grdEntitlement_ItemDataBound" OnSelectedIndexChanged="grdEntitlement_SelectedIndexChanged"
                                OnItemCommand="grdEntitlement_ItemCommand">
                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
                                <ClientSettings EnablePostBackOnRowClick="true">
                                    <Scrolling UseStaticHeaders="true" />
                                    <Selecting AllowRowSelect="true" />
                                    <%-- <ClientEvents OnGridCreated="GridCreated" OnRowSelected="RowSelected" OnRowDeselected="RowDeselected" />--%>
                                </ClientSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" NoMasterRecordsText="No Records Available" EnableHeaderContextMenu="false"
                                    DataKeyNames="ReconciliationId,ReconcilationType">
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                    <Columns>
                                        <%--   <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                </telerik:GridClientSelectColumn>--%>
                                        <telerik:GridTemplateColumn UniqueName="CheckTemp" HeaderText="CheckTemp">
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkEntitlement" AutoPostBack="true" runat="server" OnCheckedChanged="chkEntitlement_CheckedChanged" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ClientSelectEntitlementColumn" runat="server" AutoPostBack="true" OnCheckedChanged="ClientSelectEntitlementColumn_CheckedChanged" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="AccountName" FilterControlAltText="AccountName" HeaderText="Account Name" SortExpression="AccountName" UniqueName="AccountName" HeaderStyle-Width="15%" FilterControlWidth="69%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="15%" FilterControlWidth="69%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" HeaderText="Entitlement" SortExpression="Entitlement" UniqueName="Entitlement" HeaderStyle-Width="20%" FilterControlWidth="78%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="EntitlementType" FilterControlAltText="EntitlementType" HeaderText="Entitlement Type" SortExpression="EntitlementType" UniqueName="EntitlementType" HeaderStyle-Width="15%" FilterControlWidth="71%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="ExceptionType" FilterControlAltText="ExceptionType" HeaderText="Exception Type" SortExpression="ExceptionType" UniqueName="ExceptionType" HeaderStyle-Width="15%" FilterControlWidth="70%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Comments" FilterControlAltText="Comments" HeaderText="Resolve Comments" SortExpression="Comments" UniqueName="Comments" HeaderStyle-Width="15%" FilterControlWidth="71%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="CreatedDate" FilterControlAltText="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" HeaderStyle-Width="15%" FilterControlWidth="85%">
                                        </telerik:GridDateTimeColumn>
                                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Attachment" HeaderStyle-Width="15%" UniqueName="Attachment">
                                            <ItemTemplate>
                                                <asp:UpdatePanel runat="server" ID="UpdatePanelDownload" UpdateMode="Conditional" ChildrenAsTriggers="True" OnUnload="UpdatePanelDownload_Unload">
                                                    <ContentTemplate>
                                                        <asp:ImageButton ID="AttachmentImage" runat="server" CommandName="file" />
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:PostBackTrigger ControlID="AttachmentImage" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </ItemTemplate>
                                            <ItemStyle CssClass="action-attachment" HorizontalAlign="Center" />
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                            </cc1:IACMRadGridControl>
                        </asp:Panel>
                        <div class="col-xs-12">
                            <div id="Tr1" runat="server" visible="false" class="form-group topBuffer">
                                <asp:Label ID="Label7" runat="server" AssociatedControlID="comments" CssClass="col-xs-2 control-label" Text="Comments"></asp:Label>
                                <div class="col-xs-4">
                                    <asp:TextBox runat="server" ID="comments" TextMode="MultiLine" CssClass="form-control" required="required" data-errMessage="Comment is required"></asp:TextBox>
                                </div>
                                <div class="form-group required col-xs-6">
                                    <asp:Label ID="lblAttachFile" AssociatedControlID="AsyncUploadAttach" runat="server" Text="Attach Files" CssClass="col-xs-4 control-label"></asp:Label>
                                    <div class="col-xs-4">
                                        <telerik:RadAsyncUpload RenderMode="Lightweight" runat="server" ID="AsyncUploadAttach" OnClientValidationFailed="OnClientValidationFailed"
                                            HideFileInput="true" MaxFileSize="10485760" MaxFileInputsCount="1" MultipleFileSelection="Disabled" AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.csv,.pdf,.txt,.zip"
                                            Width="100%" EnableInlineProgress="false" />
                                        <span style="display: none" class="allowed-attachments">Allowed File Formats <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUploadAttach.AllowedFileExtensions ) %>)</span></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="buttons text-right nav-rectangle">
            <asp:Button ID="btnResolveManullay" Visible="false" runat="server" OnClick="btnResolveManullay_Click" Text="Resolve" CssClass="btn btn-default btn-sm" />
            <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
        </div>
    </asp:Panel>
</asp:Content>

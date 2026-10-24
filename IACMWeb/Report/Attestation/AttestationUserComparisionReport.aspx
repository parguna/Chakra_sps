<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="AttestationUserComparisionReport.aspx.cs" Inherits="DI.Chakra.Web.Report.Attestation.AttestationUserComparisionReport" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
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


    <telerik:RadWindow runat="server" Modal="true" ID="usrModal" Width="1050px" Height="100%" Title="Cart Items"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <div class="modal-header">
                <h3 class="modal-title" id="H1">Schedule Report</h3>
            </div>
            <div class="modal-body">
                <%--<ReportScheduler:RptSchd ID="RptSchd" runat="server" />--%>
            </div>

            --%><div class="modal-footer">
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>

    <asp:Panel ID="Panel1" runat="server">

        <cc1:IACMRadGridControl ID="CampaignReportGrid" AllowPaging="True" AllowCustomPaging="True" AutoGenerateColumns="False"  runat="server" OnItemCommand="EmployeeGrid_ItemCommand" OnSelectedIndexChanged="CampaignReportGrid_SelectedIndexChanged" OnNeedDataSource="CampaignReportGrid_NeedDataSource"  Width="100%"
            PageSize="50">
            <MasterTableView TableLayout="fixed" Width="100%" PageSize="50"
                DataKeyNames="AttestationCampaignId" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                <CommandItemSettings ShowAddNewRecordButton="false"
                    ShowRefreshButton="true"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn SortExpression="AttestationCampaignId" HeaderText="CampaignId" HeaderButtonType="TextButton" DataField="AttestationCampaignId" HeaderStyle-Width="15%" FilterControlWidth="85%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CampaignName" HeaderText="Campaign Name" HeaderButtonType="TextButton" DataField="CampaignName" HeaderStyle-Width="28%" FilterControlWidth="85%">
                    </telerik:GridBoundColumn>
                    <%--<telerik:GridBoundColumn SortExpression="Name" HeaderText="Campaign Name" HeaderButtonType="TextButton" DataField="Name" HeaderStyle-Width="28%" FilterControlWidth="85%">
                    </telerik:GridBoundColumn>--%>
                    <telerik:GridBoundColumn SortExpression="AttestationName" HeaderText="Attestation Name" HeaderButtonType="TextButton" DataField="AttestationName" HeaderStyle-Width="28%" FilterControlWidth="85%">
                    </telerik:GridBoundColumn> 
                    <telerik:GridDateTimeColumn SortExpression="CreatedDate" DataFormatString="{0:dd MMM yyyy hh:mm tt}" HeaderText="Start Date" HeaderButtonType="TextButton" DataField="CreatedDate" HeaderStyle-Width="13%">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn SortExpression="CreatedBy" Visible="false" HeaderText="CreatedBy" HeaderButtonType="TextButton" DataField="CreatedBy" HeaderStyle-Width="25%">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn SortExpression="EndDate" DataFormatString="{0:dd MMM yyyy hh:mm tt}" HeaderText="End Date" HeaderButtonType="TextButton" DataField="EndDate" HeaderStyle-Width="13%">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn SortExpression="SnapshotFileName" Display="false" HeaderText="Report File Name" HeaderButtonType="TextButton" DataField="SnapshotFileName" HeaderStyle-Width="20%">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn UniqueName="TemplateDownloadPdf" HeaderText="pdf" HeaderStyle-Width="7%" AllowFiltering="false" HeaderStyle-HorizontalAlign="Center" Display="false">
                        <ItemTemplate>
                            <asp:UpdatePanel runat="server" ID="UpdatePanelDownloadpdf" UpdateMode="Conditional" ChildrenAsTriggers="True" OnUnload="UpdatePanelDownloadPdf_Unload">
                                <ContentTemplate>
                                    <asp:ImageButton ID="showpdf" CssClass="action-attachment" runat="server" CommandName="DownloadPdf" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="showpdf" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </ItemTemplate>
                        <ItemStyle CssClass="action-pdf" HorizontalAlign="center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="TemplateDownloadExcelFlat" HeaderText="Excel Flat" HeaderStyle-Width="7%" AllowFiltering="false" HeaderStyle-HorizontalAlign="Center" Display="false">
                        <ItemTemplate>
                            <asp:UpdatePanel runat="server" ID="UpdatePanelDownloadExl" UpdateMode="Conditional" ChildrenAsTriggers="True" OnUnload="UpdatePanelDownloadExl_Unload">
                                <ContentTemplate>
                                    <asp:ImageButton ID="ShowExcelF" CssClass="action-attachment"  runat="server" CommandName="DownloadExcelFlat" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="ShowExcelF" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </ItemTemplate>
                        <ItemStyle CssClass="action-excel" HorizontalAlign="Center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="TemplateDownloadExcelHierarchy" HeaderText="User Profile Comparision" HeaderStyle-Width="7%" AllowFiltering="false" HeaderStyle-HorizontalAlign="Center" Display="true">
                        <ItemTemplate>
                            <asp:UpdatePanel runat="server" ID="UpdatePanelDownloadExH" UpdateMode="Conditional" ChildrenAsTriggers="True" OnUnload="UpdatePanelDownloadExH_Unload">
                                <ContentTemplate>
                                    <asp:ImageButton ID="ShowExcelH" CssClass="action-attachment"  runat="server" CommandName="DownloadExcelHierarchy" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="ShowExcelH" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </ItemTemplate>
                        <ItemStyle CssClass="action-excel" HorizontalAlign="Center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="TemplateDownloadCsv" HeaderText="Csv" HeaderStyle-Width="5%" AllowFiltering="false" HeaderStyle-HorizontalAlign="Center" Display="false">
                        <ItemTemplate>
                            <asp:UpdatePanel runat="server" ID="UpdatePanelDownloadCsv" UpdateMode="Conditional" ChildrenAsTriggers="True" OnUnload="UpdatePanelDownloadCsv_Unload">
                                <ContentTemplate>
                                    <asp:ImageButton ID="ShowCsv" CssClass="action-attachment"  runat="server" CommandName="DownloadCsv" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="ShowCsv" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </ItemTemplate>
                        <ItemStyle CssClass="action-Csv" HorizontalAlign="Center" />
                    </telerik:GridTemplateColumn>
                </Columns>

            </MasterTableView>
        </cc1:IACMRadGridControl>

        <div class="buttons text-right nav-rectangle topBuffer">
            <asp:Button ID="btnPageCancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" PostBackUrl="~/Home/Home.aspx" UseSubmitBehavior="false" OnClick="btnPageCancel_Click" />
        </div>       

    </asp:Panel>
</asp:Content>
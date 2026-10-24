<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Risk.Code.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Risk/Code/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

<%--<style>
    .rwTitleRow {
        display: none;
    }

    .modal-footer {
        background-color: #383f46;
        padding: 8px 15px;
    }

    .rwStatusbarRow {
        display: none;
    }

    .modal-header {
        text-align: center;
        color: #ff2700;
        border-bottom: 1px solid #383f46;
        font-size: 16px;
    }

        .modal-header h3 {
            font-size: 1em;
        }
</style>--%>

<div class="content-wrapper">
    <telerik:RadWindow ID="modalPopup" runat="server" Width="460px" Height="187px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" VisibleStatusbar="false">

        <ContentTemplate>
            <div class="modal-header">
                <h3 class="modal-title" runat="server" id="H1">Add Risk Code</h3>
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
    <cc1:IACMRadGridControl ID="gvRiskCode" runat="server" OnNeedDataSource="gvRiskCode_NeedDataSource"  OnSelectedIndexChanged="gvRiskCode_SelectedIndexChanged">
    
        <MasterTableView  DataKeyNames="Id"
            NoMasterRecordsText="There are no Risk Codes to display. Please click on Add button to add a Risk Code.">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>

            <Columns>

                <telerik:GridTemplateColumn DataField="RiskCode" FilterControlAltText="RiskCode" HeaderText="Risk Code" SortExpression="RiskCode" UniqueName="RiskCode" HeaderStyle-Width="13%" FilterControlWidth="64.5%">
                    <ItemTemplate>

                        <asp:Label ID="lblRiskCode" runat="server" Text='<%# Bind("RiskCode") %>'></asp:Label>
                    </ItemTemplate>

                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="RiskDescription" FilterControlAltText="RiskDescription" HeaderText="Risk Code Description" SortExpression="RiskDescription" UniqueName="RiskDescription" HeaderStyle-Width="25%" FilterControlWidth="82.5%">

                    <ItemTemplate>
                        <asp:Label ID="lblRiskCodeDesc" runat="server" Text='<%# Bind("RiskDescription") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="sodrisklevel.RiskLevelDescription" FilterControlAltText="sodrisklevel.RiskLevelDescription" HeaderText="Risk Level" SortExpression="sodrisklevel.RiskLevelDescription" UniqueName="sodrisklevel.RiskLevelDescription" HeaderStyle-Width="12%" FilterControlWidth="61%">

                    <ItemTemplate>
                        <asp:Label ID="lblRiskCodeLevel" runat="server" Text='<%# Bind("sodrisklevel.RiskLevelDescription") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="sodriskscore.RiskScoreDescription" FilterControlAltText="sodriskscore.RiskScoreDescription" HeaderText="Risk Score" SortExpression="sodriskscore.RiskScoreDescription" UniqueName="sodriskscore.RiskScoreDescription" HeaderStyle-Width="12%" FilterControlWidth="61%">

                    <ItemTemplate>
                        <asp:Label ID="lblRiskCodeScore" runat="server" Text='<%# Bind("sodriskscore.RiskScoreDescription") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="user.DisplayName" FilterControlAltText="user.DisplayName" HeaderText="Risk Code Owner" SortExpression="user.DisplayName" UniqueName="user.DisplayName" HeaderStyle-Width="25%" FilterControlWidth="82.5%">

                    <ItemTemplate>
                        <asp:Label ID="lblRiskCodeOwner" runat="server" Text='<%# Bind("user.DisplayName") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="Status" FilterControlAltText="Status" HeaderText="Risk Code Status" SortExpression="Status" UniqueName="Status" HeaderStyle-Width="13%" FilterControlWidth="64.5%">

                    <ItemTemplate>
                        <asp:Label ID="lblRiskCodeStatus" runat="server" Text='<%# Eval("Status").ToString().Equals("1") ? "Active" : "Inactive" %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>
            </Columns>

        </MasterTableView>
   </cc1:IACMRadGridControl>

    <%--<div class="buttons text-right nav-rectangle">
        <asp:Button ID="btn_Cancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btn_Cancel_Click" />
    </div>--%>
</div>
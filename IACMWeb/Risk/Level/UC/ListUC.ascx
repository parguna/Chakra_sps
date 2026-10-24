<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Risk.Level.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Src="~/Risk/Level/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<style>
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
</style>

<div class="content-wrapper">
    <telerik:RadWindow ID="modalPopup" runat="server" Width="450px" Height="187px" Modal="true" OffsetElementID="main" VisibleTitlebar="false">

        <ContentTemplate>
            <div class="modal-header">
                <h3 class="modal-title" runat="server" id="H1">Add Risk Level</h3>
            </div>
            <div class="modal-body">
                <uc1:DetailUC runat="server" ID="DetailUC" />
            </div>

            <div class="modal-footer">
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" data-dismiss="modal" OnClick="btnCancel_Click"></asp:Button>
                <asp:Button ID="btnSubmit" CssClass="add btn btn-primary bordered-button" runat="server"
                    Text="Add" OnClick="btnSubmit_Click"></asp:Button>
            </div>

        </ContentTemplate>

    </telerik:RadWindow>
    <cc1:IACMRadGridControl ID="gvRiskLevel" runat="server" OnNeedDataSource="gvRiskLevel_NeedDataSource" OnSelectedIndexChanged="gvRiskLevel_SelectedIndexChanged">
        <MasterTableView  DataKeyNames="Id"
            NoMasterRecordsText="There are no Risk Levels to display. Please click on Add button to add a Risk Level.">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>

            <Columns>

                <telerik:GridTemplateColumn DataField="RiskLevelCode" FilterControlAltText="RiskLevelCode" HeaderText="Risk Level Code" SortExpression="RiskLevelCode" UniqueName="RiskLevelCode" HeaderStyle-Width="35%" FilterControlWidth="87.5%">
                    <ItemTemplate>

                        <asp:Label ID="lblRiskLevel" runat="server" Text='<%# Bind("RiskLevelCode") %>'></asp:Label>
                    </ItemTemplate>

                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="RiskLevelDescription" FilterControlAltText="RiskLevelDescription" HeaderText="Risk Level Description" SortExpression="RiskLevelDescription" UniqueName="RiskLevelDescription" HeaderStyle-Width="65%" FilterControlWidth="93.5%">

                    <ItemTemplate>
                        <asp:Label ID="lblRiskLevelDesc" runat="server" Text='<%# Bind("RiskLevelDescription") %>'></asp:Label>
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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Risk.Score.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Risk/Score/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

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
<script type ="text/javascript">
    function test()
    {
        var clientId = $("[id$=_txtRiskScore]").attr("id");
        if (document.getElementById(clientId).value == "")
        {
            alert("Please enter RiskCode");
            return false;
        }
        else
        {
            return true;
        }
            
    }
    
</script>
<div class="content-wrapper">
    <telerik:RadWindow ID="modalPopup" runat="server" Width="360px" Height="187px" Modal="true" OffsetElementID="main" VisibleTitlebar="false">

        <ContentTemplate>
            <div class="modal-header">
                <h3 class="modal-title" runat="server" id="H1">Add Risk Score</h3>
            </div>
            <div class="modal-body">
                <uc1:DetailUC runat="server" ID="DetailUC" />
            </div>

            <div class="modal-footer">
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" data-dismiss="modal" OnClick="btnCancel_Click"></asp:Button>
                <asp:Button ID="btnSubmit" CssClass="add btn btn-primary bordered-button" runat="server" OnClientClick ="return test()"
                    Text="Add" OnClick="btnSubmit_Click"></asp:Button>
            </div>
        </ContentTemplate>

    </telerik:RadWindow>
    <cc1:IACMRadGridControl ID="gvRiskScore" runat="server" OnInit="gvRiskScore_Init" OnNeedDataSource="gvRiskScore_NeedDataSource" OnSelectedIndexChanged="gvRiskScore_SelectedIndexChanged">
        <MasterTableView DataKeyNames="Id"
            NoMasterRecordsText="There are no Risk Scores to display. Please click on Add button to add a Risk Score.">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>

            <Columns>

                <telerik:GridTemplateColumn DataField="RiskScore" FilterControlAltText="RiskScore" HeaderText="Risk Score" SortExpression="RiskScore" UniqueName="RiskScore" HeaderStyle-Width="35%" FilterControlWidth="87.5%">
                    <ItemTemplate>
                        <asp:Label ID="lblRiskScore" runat="server" Text='<%# Bind("RiskScore") %>'></asp:Label>
                    </ItemTemplate>

                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="RiskScoreDescription" FilterControlAltText="RiskScoreDescription" HeaderText="Risk Score Description" SortExpression="RiskScoreDescription" UniqueName="RiskScoreDescription" HeaderStyle-Width="65%" FilterControlWidth="93.5%">

                    <ItemTemplate>
                        <asp:Label ID="lblRiskScoreDesc" runat="server" Text='<%# Bind("RiskScoreDescription") %>'></asp:Label>
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
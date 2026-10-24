<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.PrivateQuestion.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/PasswordMgmt/PrivateQuestion/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<%@ Register Src="~/PasswordMgmt/PrivateQuestion/UC/AddUC.ascx" TagPrefix="uc1" TagName="AddUC" %>

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
    <telerik:RadWindow ID="modalPopupEdit" runat="server" Width="500px" Height="200px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleStatusbar="false">

        <ContentTemplate>
            <uc1:DetailUC runat="server" ID="DetailUC" OnCancelButtonClicked="btnEditCancel_Click" OnEditButtonClicked="btnEdit_Click" />
        </ContentTemplate>

    </telerik:RadWindow>

    <telerik:RadWindow ID="modalPopupAdd" runat="server" Width="600px" Height="200px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleStatusbar="false">

        <ContentTemplate>
            <uc1:AddUC runat="server" ID="AddUC" OnAdd_SubmitButtonClicked="btnAdd_Click" OnAdd_CancelButtonClicked="btnAddCancel_Click" />
        </ContentTemplate>

    </telerik:RadWindow>
    <cc1:IACMRadGridControl ID="gvQuestions" runat="server" OnNeedDataSource="gvQuestions_NeedDataSource"  OnSelectedIndexChanged="gvQuestions_SelectedIndexChanged">
        <MasterTableView DataKeyNames="Id" TableLayout="Fixed" ShowHeadersWhenNoRecords="true"
            NoMasterRecordsText="There are no questions to display. Please click on Add Question button to add a question.">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridTemplateColumn DataField="Question" FilterControlAltText="Question" HeaderText="Security Question" SortExpression="Question" UniqueName="Question" HeaderStyle-Width="50%" FilterControlWidth="91%">
                    <ItemTemplate>
                        <asp:Label ID="lblQuestion" runat="server" Text='<%# Bind("Question") %>'></asp:Label>
                    </ItemTemplate>

                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="Answer" FilterControlAltText="Answer" HeaderText="Answer" SortExpression="Answer" UniqueName="Answer" HeaderStyle-Width="50%" FilterControlWidth="91%">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="PlainAnswer" runat="server" Text='<%# Bind("Answer") %>'></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

            </Columns>
        </MasterTableView>
   </cc1:IACMRadGridControl>
</div>
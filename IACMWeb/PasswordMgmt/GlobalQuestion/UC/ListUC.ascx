<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.Global_Question.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Src="~/PasswordMgmt/GlobalQuestion/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadWindow ID="modalPopup" runat="server" Width="360px" Height="179px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <uc1:DetailUC runat="server" ID="DetailUC" OnAdd_SubmitButtonClicked="btnSubmit_Click" OnAdd_CancelButtonClicked="btnCancel_Click" />
    </ContentTemplate>
</telerik:RadWindow>
<cc1:IACMRadGridControl ID="gvQuestions" runat="server" OnNeedDataSource="gvQuestions_NeedDataSource" runat="server" OnSelectedIndexChanged="gvQuestions_SelectedIndexChanged">
  <MasterTableView  DataKeyNames="Id"
        NoMasterRecordsText="There are no questions to display. Please click on Add Question button to add a question.">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="Question"
                FilterControlAltText="Question" HeaderText="Global Question"
                SortExpression="Question" UniqueName="Question"
                HeaderStyle-Width="62%" FilterControlWidth="95.5%">
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>
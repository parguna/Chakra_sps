<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecurityQuestionUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.SecurityQuestionUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<style>
    .full-width-yes-no {
        width: 100% !important;
        white-space: normal;
    }
</style>

<div>

    <asp:LinkButton ID="securityQstnsBtn" Text="<%# LinkText %>" runat="server" OnClick="securityQstnsBtn_Click"></asp:LinkButton>

</div>

<telerik:RadWindow runat="server" ID="SQDetails" Modal="true" Width="1000px" Height="200%" Title="Security Questions"
    AutoSizeBehaviors="Width, Height" AutoSize="false" KeepInScreenBounds="true" VisibleOnPageLoad="false"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">

    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Security Compliance:</h3>
                </div>

                <div class="modal-body" style="height:510px">
                    <%--<asp:HiddenField ID="hdnScrollControlId" runat="server" />--%>
                    <div class="row-fluid">
                        <div class="col-xs-12 form-group">
                            <div class="col-xs-1 description">SL/No</div>
                            <div class="col-xs-5 description">Question</div>
                            <div class="col-xs-2 description">Compliance (Y/N)</div>
                            <div class="col-xs-4 description">Comments</div>
                        </div>
                    </div>
                    <telerik:RadListView runat="server" RenderMode="Lightweight" ID="securityQuestionListView" Width="100%" PageSize="10" AllowPaging="True"
                        ItemPlaceholderID="itemPlaceholder">
                        <ItemTemplate>
                            <div class="row-fluid">
                                <div class="col-xs-12 form-group">
                                    <div class="col-xs-1">
                                        <asp:Label ID="lblQstnID" runat="server" Text='<%# Eval("QuestionId") %>'></asp:Label>
                                    </div>
                                    <div class="col-xs-5">
                                        <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Question") %>'></asp:Label>
                                    </div>
                                    <div class="col-xs-2">
                                        <telerik:RadComboBox CssClass="full-width-yes-no" ID="radCmbCompliance" AutoPostBack="true" OnSelectedIndexChanged="radCmbCompliance_SelectedIndexChanged" runat="server" DataSource='<%# Eval("ComplianceData") %>' DataTextField="value" DataValueField="key" SelectedValue='<%# Eval("Compliance") %>' />
                                    </div>
                                    <div class="col-xs-4">
                                        <asp:TextBox ID="txtComments" AutoPostBack="true" OnTextChanged="txtComments_TextChanged" TextMode="MultiLine" runat="server" Width="100%" Text='<%# Eval("Comments") %>'></asp:TextBox>
                                        <asp:Label ID="lblComments" runat="server" ForeColor="Red" Visible="false">Please enter the comments</asp:Label>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </telerik:RadListView>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="SecqstnsbtnSave" runat="server" CssClass="btn btn-default btn-sm" OnClick="SecqstnsbtnSave_Click" Text="Save" />
                    <asp:Button runat="server" CssClass="btn btn-default btn-sm" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
<script>
    function showWindowSQ() {
        var oWnd = $find("<%= SQDetails.ClientID %>");
        oWnd.show();
    }

    function closeWindowSQ() {
        var oWnd = $find("<%= SQDetails.ClientID %>");
        oWnd.close();
    }


</script>



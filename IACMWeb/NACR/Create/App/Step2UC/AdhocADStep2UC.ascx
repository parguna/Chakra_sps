<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocADStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocADStep2UC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxCtrl" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>




<%--<style type="text/css">
    .auto-style1 {
        text-align: right;
    }

    .headertitle {
        color: #1c2a47;
        font-size: 13px;
        padding-left: 5px;
        text-align: left;
    }
</style>--%>

<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-6">
                <asp:Panel ID="SharedPanelAD" runat="server" Visible="false">
                    <div class="form-group required">
                        <label class="col-xs-4 control-label">Account Name</label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtAcntName" runat="server" MaxLength="20" CssClass="col-xs-12">
                            </asp:TextBox>

                        </div>
                    </div>

                    <div class="form-group required">
                        <label class="col-xs-4 control-label">Display name</label>
                        <div class="col-xs-7">

                            <asp:TextBox ID="txtDisplayName" runat="server" MaxLength="60" CssClass="col-xs-12">
                            </asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group required">
                        <label class="col-xs-4 control-label">Description</label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtDiscription" runat="server" MaxLength="255" CssClass="col-xs-12">
                            </asp:TextBox>
                        </div>
                    </div>
                    <asp:Panel ID="OU_panel" runat="server">
                        <div class="form-group required">
                            <label class="col-xs-4 control-label">OU</label>
                            <div class="col-xs-7">
                                <telerik:RadAutoCompleteBox ID="OUAutoCompleteBox" runat="server" EmptyMessage="Enter * to view full list" DropDownHeight="200px" DropDownWidth="300px" InputType="Text">
                                    <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Method="GetADOrganisationUnits" Path="../../../../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>

                            </div>

                        </div>
                    </asp:Panel>

                </asp:Panel>

            </div>
        </div>
    </div>

    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-6">

                <asp:Panel ID="GroupPanelAD" runat="server">
                    <asp:Panel ID="pnlEndPointUserName" runat="server">
                        <div class="form-group required">
                            <label class="col-xs-4 control-label">Endpoint User Name</label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="ddlEndpointUser" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlEndpointUser_SelectedIndexChanged1">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="groupComments" runat="server">
                        <div id="divComments" runat="server" class="form-group">
                            <label class="col-xs-4 control-label">Comments for Group Add/Remove</label>
                            <div class="col-xs-7">

                                <asp:TextBox ID="txtRequesterComments" runat="server" TextMode="MultiLine" Height="150px" CssClass="col-xs-12">
                                </asp:TextBox>
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="form-group">
                        <div class="col-xs-4"></div>

                        <asp:Panel ID="pnlSecurityQstns" CssClass="col-xs-7" runat="server">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>
                                    <asp:Panel ID="pnlSecurityPopup" runat="server">
                                        <uc1:SecurityQuestionUC runat="server" ID="SecurityQuestionUC" OnSecurityQuestionPopUp="SecurityQuestionUC_SecurityQuestionPopUp" OnSecurityResponseSaveClicked="SecurityQuestion_SecurityResponseSaveClicked" LinkText="Service account compliance check" />
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>


                    </div>

                </asp:Panel>

            </div>
        </div>

    </div>
    <div class="row-fluid">
        <div class="form-group">
            <div class="col-xs-6">
                <div class="form-group">
                    <label class="col-xs-4 control-label"></label>
                    <div class="col-xs-7">
                        <asp:Button ID="saveRequest" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>



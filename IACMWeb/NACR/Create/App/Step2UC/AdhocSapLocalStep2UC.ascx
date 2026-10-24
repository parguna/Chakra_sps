<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSapLocalStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocSapLocalStep2UC" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxCtrl" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>


<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">

            <asp:Panel ID="SharedPanelAD" runat="server" Visible="false" class="form-group required">
                <div class="col-xs-6">
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
                            <asp:TextBox ID="txtDiscription" runat="server" MaxLength="255" TextMode="MultiLine" Height="100px" CssClass="col-xs-12">
                            </asp:TextBox>
                        </div>
                    </div>

                    <div id="ouTR" runat="server" class="form-group">
                        <label class="col-xs-4 control-label">OU</label>
                        <div class="col-xs-7">
                            <telerik:RadAutoCompleteBox ID="OUAutoCompleteBox" runat="server" EmptyMessage="Enter * to view full list" DropDownHeight="200px" DropDownWidth="300px" InputType="Text" Width="305px">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="GetADOrganisationUnits" Path="../../../../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="GroupPanelAD" runat="server">

                <div class="col-xs-12">
                    <div class="col-xs-6">
                        <div id="trEndPointUser" runat="server" visible="false" class="form-group">
                            <label class="col-xs-4 control-label">Endpoint User Name</label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="ddlEndpointUser" OnSelectedIndexChanged="ddlEndpointUser_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="form-control" Width="350px">
                                </asp:DropDownList>

                            </div>
                        </div>

                        <div class="form-group" id="trSecurityQstns" runat="server" visible="false">
                            <div class="col-xs-4">
                            </div>
                            <div class="col-xs-7">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="Panel2" runat="server">
                                            <uc1:SecurityQuestionUC runat="server" ID="SecurityQuestionUC1" OnSecurityQuestionPopUp="SecurityQuestionUC_SecurityQuestionPopUp" OnSecurityResponseSaveClicked="SecurityQuestion_SecurityResponseSaveClicked" LinkText="Service account compliance check" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>

                <%-- <div id="Div2">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <asp:Panel ID="pnlSecurityPopup" runat="server">
                                <uc:securityquestion runat="server" id="SecurityQuestionUC"   />
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>--%>
            </asp:Panel>

            <%--<asp:Panel ID="groupComments" runat="server">
                <div class="col-xs-12">
                    <div class="col-xs-6">
                        <div id="divComments" runat="server" class="form-group">
                            <label class="col-xs-4 control-label">Comments for Group Add/Remove</label>
                            <div class="col-xs-7">

                                <asp:TextBox ID="txtRequesterComments" runat="server" TextMode="MultiLine" Height="100px" CssClass="col-xs-12">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>--%>

            <div class="col-xs-12">
                <div class="col-xs-6">
                    <div class="form-group">
                        <div class="col-xs-4"></div>
                        <div class="col-xs-7">
                            <asp:Button ID="saveRequest" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

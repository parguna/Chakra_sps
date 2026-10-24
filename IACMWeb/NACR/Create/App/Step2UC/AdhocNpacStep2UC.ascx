<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocNpacStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocNpacStep2UC" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>


<div class="form">
    <div class="form-horizontal left clearfix">
        <asp:HiddenField ID="Mandatory1HiddenField" runat="server" />
        <asp:HiddenField ID="Mandatory2HiddenField" runat="server" />
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6">

                    <div id="trEndPointUser" runat="server" class="form-group required">
                        <label class="col-xs-3 control-label">Account Name</label>
                        <div class="col-xs-8">
                            <asp:DropDownList ID="EndPointUserDropDownList" runat="server" CssClass="form-control selectpicker" AutoPostBack="true" OnSelectedIndexChanged="EndPointUserDropDownList_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <asp:Panel ID="OtherControls" runat="server">
                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Level</label>
                            <div class="col-xs-8">
                                <telerik:RadComboBox ID="LevelRadComboBox" runat="server" CssClass="selectpicker AccountsPageComoboWidth radbox100Width" AutoPostBack="true" OnSelectedIndexChanged="LevelRadComboBox_SelectedIndexChanged" data-width="100%">
                                </telerik:RadComboBox>
                            </div>
                        </div>
                          <div class="form-group" id="divSection" runat="server">
                            <label class="col-xs-3 control-label">Section</label>
                            <div class="col-xs-8">
                                <telerik:RadComboBox ID="ddlSections" runat="server"  AutoPostBack="true"  CssClass="selectpicker AccountsPageComoboWidth radbox100Width"  OnSelectedIndexChanged="ddlSections_SelectedIndexChanged" data-width="100%">
                                </telerik:RadComboBox>
                            </div>
                        </div>
                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Email</label>
                            <div class="col-xs-8">
                                <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control" MaxLength="100"></asp:TextBox>
                            </div>
                        </div>
                         <div class="form-group">
                            <label class="col-xs-3 control-label">Hand Phone</label>
                            <div class="col-xs-8">
                                <asp:TextBox runat="server" ID="txtPhoneNumber" CssClass="form-control" MaxLength="8"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" id="divUnit" runat="server">
                            <label class="col-xs-3 control-label required">Unit/Zone</label>
                            <div class="col-xs-8">
                              <telerik:RadComboBox ID="ddlUnit" runat="server" CssClass="selectpicker AccountsPageComoboWidth radbox100Width" data-width="100%">
                                </telerik:RadComboBox>
                            </div>
                        </div>
                        <div class="form-group" id="trSecurityQstns" runat="server" visible="false">
                            <div class="col-xs-3">
                            </div>
                            <div class="col-xs-8">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="Panel2" runat="server">
                                            <uc1:SecurityQuestionUC runat="server" ID="SecurityQuestionUC1" OnSecurityQuestionPopUp="SecurityQuestionUC_SecurityQuestionPopUp" OnSecurityResponseSaveClicked="SecurityQuestion_SecurityResponseSaveClicked" LinkText="Service account compliance check" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </asp:Panel>

                    <div class="form-group">
                        <label class="col-xs-3 control-label"></label>
                        <div class="col-xs-8">
                            <asp:Button ID="SaveButton" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

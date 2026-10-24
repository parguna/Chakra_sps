<%@ Page Title="Application Settings" Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="ApplicationSetting.aspx.cs" Inherits="DI.Chakra.Web.Admin.AppSetting.ApplicationSetting" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Admin/AppSetting/UC/ProvEngineSetupUC.ascx" TagPrefix="uc1" TagName="ProvEngineSetupUC" %>
<%@ Register Src="~/Admin/AppSetting/UC/ServiceMonitorUC.ascx" TagPrefix="uc1" TagName="ServiceMonitorUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">

    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <asp:Label ID="Label1" AssociatedControlID="AuthenticationModeField" CssClass="col-xs-4 control-label" Text="Authentication Mode" runat="server"></asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="AuthenticationModeField" runat="server" CssClass="form-control selectpicker" data-width="100%" OnSelectedIndexChanged="AuthenticationModeField_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <asp:Label ID="Label9" AssociatedControlID="AuthenticationDomainField" CssClass="col-xs-4 control-label" Text="Authentication Domain" runat="server"></asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="AuthenticationDomainField" runat="server" CssClass="form-control selectpicker" data-width="100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <asp:Label ID="Label2" AssociatedControlID="PasswordResetOptionField" CssClass="col-xs-4 control-label" Text="Password Reset" runat="server"></asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="PasswordResetOptionField" runat="server" CssClass="form-control selectpicker" data-width="100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <asp:Label ID="Label3" AssociatedControlID="NoofSecurityQuestion" CssClass="col-xs-4 control-label" Text="No. of Security Questions" runat="server"></asp:Label>
                            <div class="col-xs-7">
                                <asp:TextBox ID="NoofSecurityQuestion" runat="server" TextMode="SingleLine" CssClass="form-control" Rows="4" Columns="60"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <asp:Label ID="Label4" AssociatedControlID="ddlCultures" CssClass="col-xs-4 control-label" Text="Culture" runat="server"></asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="ddlCultures" runat="server" CssClass="form-control selectpicker"></asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="col-xs-6">
                        <div class="form-group">
                            <asp:Label ID="Label8" AssociatedControlID="OTPOptionfield" CssClass="col-xs-4 control-label" Text="OTP Option" runat="server"></asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="OTPOptionfield" runat="server" CssClass="form-control selectpicker"></asp:DropDownList>
                            </div>
                        </div>
                    </div>


                    <div class="col-xs-12">
                        <div class="form-group">
                        </div>
                    </div>
                    <div class="col-xs-6">
                       <%-- <div class="form-group">
                            <asp:Label ID="Label5" runat="server" CssClass="col-xs-4 control-label1" Text="Meta Data"></asp:Label>
                            <div class="col-xs-7 control-label2">
                                <asp:LinkButton ID="lbltest" runat="server" Text="Edit" PostBackUrl="~/Metadata/MetadataEditor.aspx"></asp:LinkButton>
                            </div>
                        </div>--%>
                        <div class="form-group">
                            <asp:Label ID="Label6" runat="server" CssClass="col-xs-4 control-label1" Text="Provisioning Setup"></asp:Label>
                            <div class="col-xs-7 control-label2">
                                <asp:LinkButton ID="lblSetup" runat="server" Text="Change" OnClick="lblSetup_Click"></asp:LinkButton>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label7" runat="server" CssClass="col-xs-4 control-label1" Text="Service Monitor"></asp:Label>
                            <div class="col-xs-7 control-label2">
                                <asp:LinkButton ID="lblServiceMonitor" runat="server" Text="Open" OnClick="lblServiceMonitor_Click"></asp:LinkButton>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="lblEditAppText" runat="server" CssClass="col-xs-4 control-label1" Text="App Text Configuration"></asp:Label>
                            <div class="col-xs-7 control-label2">
                                <asp:LinkButton ID="lbEditAppText" runat="server" Text="Edit Text" PostBackUrl="~/Admin/AppText/List.aspx"></asp:LinkButton>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>

    <div class="buttons text-right nav-rectangle">
        <asp:Button ID="Button1" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btnCancel1_Click" />
    </div>

    </div>

    <telerik:RadWindow runat="server" ID="ProvisioningSetup" Modal="true" Width="500px" Height="90%" Title="Provisioning Setup"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title">Provisioning Setup</h3>
                    </div>
                    <div class="modal-body">
                        <uc1:ProvEngineSetupUC runat="server" ID="ProvEngineSetupUC" />
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" ID="btnSave" class="btn btn-primary bordered-button" Text="Save" OnClick="btnSave_Click" />
                        <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="ServiceMonitor" Modal="true" Width="500px" Height="90%" Title="Service Monitor"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title">Service Monitor</h3>
                    </div>
                    <div class="modal-body">
                        <uc1:ServiceMonitorUC runat="server" ID="ServiceMonitorUC" />
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" ID="btnRefresh" class="btn btn-primary bordered-button" Text="Refresh" OnClick="btnRefresh_Click" />
                        <asp:Button runat="server" ID="btnSMCancel" class="btn btn-primary bordered-button" Text="Cancel" OnClick="btnSMCancel_Click" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>

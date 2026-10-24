<%@ Page Title="User Preference" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="UserPreference.aspx.cs" Inherits="DI.Chakra.Web.Preferences.UserPreference" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Master/UC/PersonalControlsUC.ascx" TagPrefix="uc1" TagName="PersonalControlsUC" %>

<asp:Content ID="Content3" ContentPlaceHolderID="cplMain" runat="server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="col-xs-6 hide">
                        <div class="form-group">
                            <asp:Label ID="Label4" AssociatedControlID="Cultures" CssClass="col-xs-2 control-label" Text="Culture" runat="server"></asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="Cultures" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCultures_SelectedIndexChanged" CssClass="form-control selectpicker"></asp:DropDownList>
                            </div>
                            <div class="col-xs-1">
                                <div class="whats-this bottom showTooltip">
                                    <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                                    <span style="display: none" role="tooltip" class="tooltip right info">
                                        <h5>Culture</h5>
                                        <p>The culture selected applies only to date and time values. </p>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6 clearRight hide">
                        <div class="form-group">
                            <asp:Label ID="Label1" AssociatedControlID="ShortDatePatterns" CssClass="col-xs-2 control-label" Text="Short Date " runat="server"></asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="ShortDatePatterns" runat="server" CssClass="form-control selectpicker"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label2" AssociatedControlID="ShortTimePatterns" CssClass="col-xs-2 control-label" Text="Short Time" runat="server"></asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="ShortTimePatterns" runat="server" CssClass="form-control selectpicker"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label3" AssociatedControlID="LongDatePatterns" CssClass="col-xs-2 control-label" Text="Long Date" runat="server"></asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="LongDatePatterns" runat="server" CssClass="form-control selectpicker"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label5" AssociatedControlID="LongTimePatterns" CssClass="col-xs-2 control-label" Text="Long Time" runat="server"></asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="LongTimePatterns" runat="server" CssClass="form-control selectpicker"></asp:DropDownList>
                            </div>
                        </div>

                    </div>
                    <div class="col-xs-6 clearLeft">


                        <div class="col-xs-12">&nbsp;</div>
                        <div class="col-xs-12">&nbsp;</div>

                        <div class="form-group">
                            <h5 style="color: #2077bd;">DashBoard Chart Preference Layout:</h5>
                            <div class="col-xs-12">
                                <telerik:RadDockLayout runat="server" ID="RadDockLayout1">
                                    <telerik:RadDockZone runat="server" BorderStyle="None" ID="RadDockZone1" Style="float: left;" Orientation="Vertical">
                                        <telerik:RadDock ID="RadDock1" runat="server" EnableDrag="false">
                                            <ContentTemplate>
                                                <uc1:PersonalControlsUC runat="server" ID="dashboardLayout1" DashBoardType="1" />
                                            </ContentTemplate>
                                        </telerik:RadDock>
                                        <telerik:RadDock ID="RadDock3" runat="server" EnableDrag="false">
                                            <ContentTemplate>
                                                <uc1:PersonalControlsUC runat="server" ID="dashboardLayout3" DashBoardType="1" />
                                            </ContentTemplate>
                                        </telerik:RadDock>
                                    </telerik:RadDockZone>
                                </telerik:RadDockLayout>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6 clearRight">
                        <div class="form-group">
                            <h5 style="color: #2077bd;">DashBoard Detail Preference Layout:</h5>
                            <div class="col-xs-12">
                                <telerik:RadDockLayout runat="server" ID="RadDockLayout2">
                                    <telerik:RadDockZone runat="server" BorderStyle="None" ID="RadDockZone2" Style="float: left;" Orientation="Vertical">
                                        <telerik:RadDock ID="RadDock2" runat="server" EnableDrag="false">
                                            <ContentTemplate>
                                                <uc1:PersonalControlsUC runat="server" ID="dashboardLayout2" DashBoardType="2" />
                                            </ContentTemplate>
                                        </telerik:RadDock>
                                    </telerik:RadDockZone>
                                </telerik:RadDockLayout>
                            </div>
                        </div>

                        <asp:Panel runat="server" Visible="false" CssClass="col-xs-12">
                            <h4 class="table-header">Script Execution</h4>
                            <div class="form-group">
                                <asp:Label ID="lblExecute" AssociatedControlID="txtWeb" CssClass="col-xs-4 control-label" Text="Execute Script" runat="server"></asp:Label>
                                <div class="col-xs-7">
                                    <asp:Button ID="btnExecute" runat="server" Text="Execute Script" OnClick="btnExecute_Click" />
                                    <asp:TextBox ID="txtWeb" runat="server" Text="D:\DI\V5\Source\SOURCE\DI.Chakra.Model\SqlScripts\PS\Core\Test.ps1" Width="500px"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-xs-5">&nbsp;</div>
                                <div class="col-xs-7">
                                    <asp:Label ID="ScriptError" runat="server" ForeColor="Red" Text="Test" Visible="false"></asp:Label>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                    <div class="col-xs-6 clearLeft">
                        <div class="col-xs-12">&nbsp;</div>
                        <div class="col-xs-12">&nbsp;</div>
                        <div class="form-group">
                            <h5 style="color: #2077bd;">Landing Page Preference:</h5>
                            <div class="col-xs-12">
                                <div class="form-group">
                                    <asp:Label ID="Label6" AssociatedControlID="ddlPageList" CssClass="col-xs-3 control-label" Text="Landing Page:" runat="server"></asp:Label>
                                    <div class="col-xs-7">
                                        <asp:DropDownList ID="ddlPageList" runat="server" CssClass="form-control selectpicker"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</asp:Content>

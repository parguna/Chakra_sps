<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSsidsStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocSsidsStep2UC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-6">

                <asp:Panel ID="pnlEndPointUserName" runat="server" class="form-group required" Visible="false">
                    <label class="col-xs-4 control-label">Endpoint User Name</label>
                    <div class="col-xs-7">
                        <asp:DropDownList ID="ddlEndpointUser" CssClass="form-control selectpicker" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlEndpointUser_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlSMS" runat="server" Visible="false">
                    <div class="form-group required">
                        <label class="col-xs-4 control-label">Hand Phone Number</label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtSMS" runat="server" TextMode="SingleLine" onkeypress='validate(event)' MaxLength="8">
                            </asp:TextBox>


                            <%-- <asp:RegularExpressionValidator ID="SMSValidator" runat="server" ErrorMessage="Enter Valid Hand Phone Number"
                                ControlToValidate="txtSMS" ValidationExpression="^[89]\d{7}$"></asp:RegularExpressionValidator>--%>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlGUI" runat="server" Visible="false">
                    <div class="form-group required">
                        <label class="col-xs-4 control-label">Role</label>
                        <div class="col-xs-7">
                            <asp:DropDownList ID="ddlRole" CssClass="form-control selectpicker" runat="server">
                            </asp:DropDownList>
                        </div>
                    </div>
                </asp:Panel>

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

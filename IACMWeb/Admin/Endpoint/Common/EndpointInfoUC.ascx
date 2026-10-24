<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EndpointInfoUC.ascx.cs" Inherits="DI.Chakra.Web.EndPoints.EndpointSystem.Common.EndpointInfoUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>

<div class="content-wrapper" runat="server" id="contentwrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <uc1:EndpointUC runat="server" ID="EndpointUC" DisplayControl="ApplicationType" OnApplicationChanged="EndpointTypeUC_EndpointTypeChanged" />

                    <div class="form-group" runat="server" id="divApplication" visible="true">
                        <asp:Label ID="applicationType" AssociatedControlID="ddlApplicationTypes" runat="server" Text="Application" CssClass="col-xs-4 control-label required"></asp:Label>
                        <div class="col-xs-7">
                            <asp:DropDownList ID="ddlApplicationTypes" runat="server" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="True"
                                  DataTextField="DisplayName" DataValueField="Id">
                            </asp:DropDownList>
                        </div>
                    </div>


                    <div class="form-group">
                        <asp:Label ID="hostname" AssociatedControlID="txtName" runat="server" Text="Name" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtName" CssClass="form-control numeric" runat="server" required="required" onKeyPress="return AllowLetterNumber(event)" data-required-msg="Name is required"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label1" AssociatedControlID="txtApplicationCode" runat="server" Text="Code" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtApplicationCode" CssClass="form-control numeric" runat="server" required="required" data-required-msg="Name is required" onKeyPress="return AllowAlphaNumber(event);"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="owner" runat="server" AssociatedControlID="UserSearchUC" Text="Owner" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <uc1:UserSearchUC runat="server" IsRequired="true" AllowSingleUser="true" DataSourceMethod="GetUsers" ID="UserSearchUC" />
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="endpointdesc" AssociatedControlID="txtEndPointDesc" runat="server" Text="Description" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtEndPointDesc" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="4" Columns="60"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6">
                    <div class="form-group" id="IsTrustedSourceDiv" visible="false" runat="server">
                        <asp:Label ID="Label2" runat="server" AssociatedControlID="chkIsTrustedSource">
                            <asp:CheckBox ID="chkIsTrustedSource" runat="server" AutoPostBack="true" OnCheckedChanged="chkIsTrustedSource_CheckedChanged" />
                            <span class="lbl">Trusted Source</span>
                        </asp:Label>
                    </div>
                    <div class="form-group" id="attestationDiv" runat="server">
                        <asp:Label ID="Label3" runat="server" AssociatedControlID="chkIncludeInAttestation">
                            <asp:CheckBox ID="chkIncludeInAttestation" runat="server" />
                            <span class="lbl">Enable Attestation</span>
                        </asp:Label>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label5" runat="server" AssociatedControlID="chkIsActive">
                            <asp:CheckBox ID="chkIsActive" runat="server" />
                            <span class="lbl">Active</span>
                        </asp:Label>
                    </div>
                    <div class="form-group" id="resetform" runat="server">
                        <asp:Label ID="Label6" runat="server" AssociatedControlID="chkAllowPaswordReset">
                            <asp:CheckBox ID="chkAllowPaswordReset" runat="server" />
                            <span class="lbl">Allow Password Reset</span>
                        </asp:Label>
                    </div>
                      <div class="form-group" id="unlockDiv" runat="server">
                        <asp:Label ID="Label10" runat="server" AssociatedControlID="chkUnlockAccount">
                            <asp:CheckBox ID="chkunlockAccount" runat="server" />
                            <span class="lbl">Allow Account unlock</span>
                        </asp:Label>
                    </div>
                    <div class="form-group" id="SODApplicableDiv" runat="server">
                        <asp:Label ID="Label4" runat="server" AssociatedControlID="chkIsSODApplicable">
                            <asp:CheckBox ID="chkIsSODApplicable" runat="server" />
                            <span class="lbl">Is SOD Applicable</span>
                        </asp:Label>
                    </div>
                    <div class="form-group" id="SecondaryAccountDiv" runat="server">
                        <asp:Label ID="Label7" runat="server" AssociatedControlID="chkSupportSecondaryAccount">
                            <asp:CheckBox ID="chkSupportSecondaryAccount" runat="server" />
                            <span class="lbl">Support Secondary Account</span>
                        </asp:Label>
                    </div>
                    <div class="form-group" id="divCanAutomate" runat="server" visible="false">
                        <asp:Label ID="Label9" runat="server" AssociatedControlID="chkCanAutomate">
                            <asp:CheckBox ID="chkCanAutomate" runat="server" OnCheckedChanged="chkCanAutomate_CheckedChanged" AutoPostBack="True" />
                            <span class="lbl">Can Automate</span>
                        </asp:Label>
                    </div>
                    <div class="form-group" id="divFunctionLevel" runat="server" visible="false">
                        <asp:Label ID="Label8" runat="server" AssociatedControlID="chkFunctionLevel">
                            <asp:CheckBox ID="chkFunctionLevel" runat="server" />
                            <span class="lbl">Function Level</span>
                        </asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

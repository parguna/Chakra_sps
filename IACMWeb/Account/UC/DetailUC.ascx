<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Account.UC.DetailUC" %>
<%@ Register Src="~/Account/UC/SAP/SAPAccessListUC.ascx" TagPrefix="uc1" TagName="SAPAccessListUC" %>
<%@ Register Src="~/Account/UC/AccessListUC.ascx" TagPrefix="uc2" TagName="AccessListUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>

<asp:Panel runat="server" ID="ContentWrapper">
    <div class="form">
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-xs-4">Account Name</label>
                            <div class="col-xs-7">
                                <asp:Label runat="server" ID="AccountName"></asp:Label>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label1" runat="server" CssClass="col-xs-4 control-label" AssociatedControlID="TypeofAccounts">Account Type</asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList runat="server" ID="TypeofAccounts" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="true" OnSelectedIndexChanged="TypeofAccount_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <div class="col-xs-6">
                                <asp:CheckBox Visible="false" ID="chkPrimaryAccount" Text="Primary Account" TextAlign="Right" runat="server" />
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label2" runat="server" CssClass="col-xs-4 control-label" AssociatedControlID="ddlOwnerOffboard">When Owner off-boarding</asp:Label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="ddlOwnerOffboard" runat="server" CssClass="form-control selectpicker" data-width="100%">
                                </asp:DropDownList>

                            </div>
                            <div id="" class="whats-this bottom showTooltip">
                                <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                                <span style="display: none" role="tooltip" class="tooltip right info">
                                    <h5>When mapped employee resigns</h5>
                                    <p>
                                        This is applicable only for Shared/Service accounts.
                                    </p>
                                </span>
                            </div>
                        </div>
                    </div>

                    <div class="col-xs-6">

                        <div class="form-group">
                            <label class="col-xs-4">Display Name</label>
                            <div class="col-xs-7">
                                <asp:Label runat="server" ID="DisplayName"></asp:Label>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4 control-label">
                                <asp:RadioButton runat="server" ID="MapGlobalUser" GroupName="MapGlobalUser" AutoPostBack="true" OnCheckedChanged="Map_CheckedChanged" />
                                <span class="lbl">Map Employee</span>
                            </label>
                            <div class="col-xs-7">
                                <uc1:UserSearchUC runat="server" ID="UserSearchUC" AllUsers="true" />
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">
                                <asp:RadioButton runat="server" ID="UnmapGlobalUser" Checked="false" GroupName="MapGlobalUser" AutoPostBack="true" OnCheckedChanged="Map_CheckedChanged" />
                                <span class="lbl">Un-Map Employee</span>
                            </label>
                            <div class="col-xs-7">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
<uc2:AccessListUC runat="server" Visible="false" ID="AccessListUC" />
<uc1:SAPAccessListUC runat="server" ID="SAPAccessListUC" Visible="false" />
<div class="buttons text-right nav-rectangle topBuffer">
    <asp:Button runat="server" ID="Cancel" Text="Cancel" CssClass="btn btn-default btn-sm cancel" OnClick="Cancel_Click" />
</div>

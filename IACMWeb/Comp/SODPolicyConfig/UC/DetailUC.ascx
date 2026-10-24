<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.SODPolicyConfig.UC.DetailUC" %>
<asp:Panel ID="Mainpanel" runat="server">
    <div class="content-wrapper" runat="server" id="contentwrapper">
        <div class="form">
            <div class="form-horizontal left readonly">
                <div class="row-fluid">
                    <div class="col-xs-6">
                        <div class="form-group required">
                            <label id="lbl_risklevel" runat="server" class="col-xs-4 control-label">Risk Level</label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="drprisklevel" runat="server" CssClass="form-control selectpicker" data-width="100%" AppendDataBoundItems="true" AutoPostBack="True">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group">
                            <label id="lbl_riskoperator" runat="server" class="col-xs-4 control-label"></label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="drpriskoperator" runat="server" CssClass="form-control selectpicker" data-width="100%" AppendDataBoundItems="true" AutoPostBack="True">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group required">
                            <label id="lbl_riskscore" runat="server" class="col-xs-4 control-label">Risk Score Operator</label>
                            <div class="col-xs-4">
                                <asp:DropDownList ID="drpriskScoreOperator" runat="server" CssClass="form-control selectpicker" data-width="100%" AppendDataBoundItems="true" AutoPostBack="True">
                                </asp:DropDownList>
                            </div>
                            <div class="col-xs-3">
                                <asp:TextBox runat="server" ID="txtriskscore" TextMode="SingleLine" CssClass="form-control" onKeyPress="return AllowNumeric(event,0,-1)"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>

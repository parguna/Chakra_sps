<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProvEngineSetupUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.AppSetting.UC.ProvEngineSetupUC" %>

<asp:Panel runat="server" ID="ContentPanel" Enabled="true">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-12">
                <div class="form-group">
                    <asp:Label ID="lblonbtrigger" runat="server" CssClass="col-xs-6" AssociatedControlID="CanRunOnBoardTrigger">
                        <asp:CheckBox runat="server" ID="CanRunOnBoardTrigger" TabIndex="2"></asp:CheckBox>
                        <span class="lbl">CanRunOnBoardTrigger</span>
                    </asp:Label>

                    <div class="whats-this bottom showTooltip">
                        <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                        <span style="display: none" role="tooltip" id="Txtonboard" runat="server" class="tooltip right info">
                            <asp:Label ID="lblrunon" runat="server"> 
                            </asp:Label>
                        </span>
                    </div>

                </div>

                <div class="form-group">

                    <asp:Label ID="lbloffbtrigger" runat="server" CssClass="col-xs-6" AssociatedControlID="CanRunOffBoardTrigger">
                        <asp:CheckBox runat="server" ID="CanRunOffBoardTrigger" TabIndex="2"></asp:CheckBox>
                        <span class="lbl">CanRunOffBoardTrigger</span>
                    </asp:Label>

                    <div class="whats-this bottom showTooltip">
                        <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                        <span style="display: none" role="tooltip" id="Txtoffboard"  runat="server" class="tooltip right info">
                            <asp:Label ID="lbloffbtrigg" runat="server"> 
                            </asp:Label>
                        </span>
                    </div>

                </div>

                <div class="form-group">

                    <asp:Label ID="Label1" runat="server" CssClass="col-xs-6" AssociatedControlID="CanRunRehireTrigger">
                        <asp:CheckBox runat="server" ID="CanRunRehireTrigger" TabIndex="2"></asp:CheckBox>
                        <span class="lbl">CanRunRehireTrigger</span>
                    </asp:Label>

                    <div class="whats-this bottom showTooltip">
                        <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                        <span style="display: none" role="tooltip" id="TxtReHire"  runat="server" class="tooltip right info">
                            <asp:Label ID="lblRehiretrigg" runat="server"> 
                            </asp:Label>
                        </span>
                    </div>

                </div>


                <div class="form-group">

                    <asp:Label ID="lbltransfertrigger" runat="server" CssClass="col-xs-6" AssociatedControlID="CanRunTransferTrigger">
                        <asp:CheckBox runat="server" ID="CanRunTransferTrigger" TabIndex="2"></asp:CheckBox>
                        <span class="lbl">CanRunTransferTrigger</span>
                    </asp:Label>

                    <div class="whats-this bottom showTooltip">
                        <a class="di_tooltip" data-toggle="tooltip"  href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                        <span style="display: none" role="tooltip" id="Txttransfer" runat="server" class="tooltip right info">
                            <asp:Label ID="lbltranstrigg" runat="server" Text='<%# Bind("Description")%>'> 
                            </asp:Label>
                        </span>
                    </div>

                </div>
                <div class="form-group">

                    <asp:Label ID="lblrunorgtrigger" runat="server" CssClass="col-xs-6" AssociatedControlID="CanRunReOrgTrigger">
                        <asp:CheckBox runat="server" ID="CanRunReOrgTrigger" TabIndex="2"></asp:CheckBox>
                        <span class="lbl">CanRunReOrgTrigger</span>
                    </asp:Label>

                    <div class="whats-this bottom showTooltip">
                        <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                        <span style="display: none" role="tooltip" id="Txtrunorg" runat="server" class="tooltip right info">
                            <asp:Label ID="lblrunreorg" runat="server" Text='<%# Bind("Description")%>'> 
                            </asp:Label>
                        </span>
                    </div>

                </div>
                <div class="form-group">

                    <asp:Label ID="lblrunprotrigger" runat="server" CssClass="col-xs-6" AssociatedControlID="CanRunPropagationTrigger">
                        <asp:CheckBox runat="server" ID="CanRunPropagationTrigger" TabIndex="2"></asp:CheckBox>
                        <span class="lbl">CanRunPropagationTrigger</span>
                    </asp:Label>

                    <div class="whats-this bottom showTooltip">
                        <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                        <span style="display: none" role="tooltip" id="Txtprop" runat="server" class="tooltip right info">
                            <asp:Label ID="lblrunprop" runat="server" Text='<%# Bind("Description")%>'> 
                            </asp:Label>
                        </span>
                    </div>

                </div>

                <div class="form-group">

                    <asp:Label ID="Label2" runat="server" CssClass="col-xs-6" AssociatedControlID="CanRunPromotionTrigger">
                        <asp:CheckBox runat="server" ID="CanRunPromotionTrigger" TabIndex="2"></asp:CheckBox>
                        <span class="lbl">CanRunPromotionTrigger</span>
                    </asp:Label>

                    <div class="whats-this bottom showTooltip">
                        <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                        <span style="display: none" role="tooltip" id="TxtPromotion"  runat="server" class="tooltip right info">
                            <asp:Label ID="lblPromotionTrigg" runat="server"> 
                            </asp:Label>
                        </span>
                    </div>

                </div>

                <div class="form-group">

                    <asp:Label ID="lblrunAttRemediation" runat="server" CssClass="col-xs-6" AssociatedControlID="runAttestationRemediation">
                        <asp:CheckBox runat="server" ID="runAttestationRemediation" TabIndex="2"></asp:CheckBox>
                        <span class="lbl">RunAttestationRemediation</span>
                    </asp:Label>

                    <div class="whats-this bottom showTooltip">
                        <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                        <span style="display: none" role="tooltip" id="Txtrunatt" runat="server" class="tooltip right info">
                            <asp:Label ID="lblrunatt" runat="server" Text='<%# Bind("Description")%>'> 
                            </asp:Label>
                        </span>



                    </div>
                </div>

            </div>
        </div>
    </div>

</asp:Panel>


<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AttestationUserComparision.aspx.cs" Title="User Entitlement Comparision" Inherits="DI.Chakra.Web.Admin.Attestation.AttestationUserComparision" MasterPageFile="~/Master/ProdAttestation.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <div class="container-fluid">
        <div class="row" style="margin-top: 5px; margin-bottom: 5px;">
            <div class="col-lg-12 col-sm-12 col-xs-12 col-md-12" style="padding-left: 2px;">
                
            </div>
        </div>
    </div>

    <div class="container-fluid">

        <!-- Nav tabs -->
        <ul class="nav nav-tabs" role="tablist">
            <li id="tbLinkUserAccount">
                <a href="#userAccountTab" role="tab" data-toggle="tab">
                    <i class="fa-solid fa-user"></i>
                    &nbsp;User Account(s)
                </a>
            </li>
            <li id="tbLinkSecondaryAccount">
                <a href="#secondaryAccountTab" role="tab" data-toggle="tab">
                    <i class="fa-solid fa-user-group"></i>&nbsp;Secondary Account(s)
                </a>
            </li>

            <li id="tbLinkSharedAccount">
                <a href="#sharedAccountTab" role="tab" data-toggle="tab">
                    <i class="fa-solid fa-users"></i>&nbsp;Shared Account(s)
                </a>
            </li>
            <li id="tbLinkServiceAccount">
                <a href="#serviceAccountTab" role="tab" data-toggle="tab">
                    <i class="fa-solid fa-desktop"></i>&nbsp;Service Account(s)
                </a>
            </li>

            <li style="float: right;" id="actionButtonData">
                <div class="col-lg-12">
                    <div class="btn-right float-end" style="margin: 5px;">
                        <label id="unSavedChanges" style="color: orange;">Unsaved reviews found, Click save to proceed.</label>&nbsp;&nbsp;
                    
                    </div>
                </div>
            </li>
        </ul>

        <!-- Tab panes -->
        <div class="tab-content" style="min-height: 85%;border-bottom-width: 0px;">
            <div class="tab-pane fade" id="userAccountTab">
                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6">
                        <div class="input-group">
                            <label class="input-group-addon" for="userAccountEndpoint"><b>Application Name:</b></label>
                            <select id="userAccountEndpoint" multiple class="form-control selectpicker" data-live-search="true">
                            </select>
                            <span class="input-group-addon" onclick="clearUserAccountFilterByElement('#userAccountEndpoint','#userAccountEntitlement',1)"><i class="fa-solid fa-filter-circle-xmark"></i></span>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6">
                        <div class="input-group">
                            <label class="input-group-addon" for="userAccountEntitlement"><b>Role/Group:</b></label>
                            <select id="userAccountEntitlement" multiple class="form-control selectpicker" data-live-search="true">
                            </select>
                            <span class="input-group-addon" onclick="clearUserAccountFilterByElement('#userAccountEntitlement',null,1)"><i class="fa-solid fa-filter-circle-xmark"></i></span>
                        </div>
                    </div>
                </div>
                 <div class="row" style="padding-top:10px;padding-bottom:5px;">
                    <div class="col-sm-12 col-md-12 col-lg-12">
                        <div style="float: left;padding-left:15px;" >
                    <asp:Label ID="entitlementNotes" CssClass="lblNotes" runat="server" ForeColor="Red"></asp:Label>
                </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="mb-12">
                            <div id="userAccountGrid" style="width: 100%; height: 600px;" class="ag-theme-balham"></div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="tab-pane fade" id="serviceAccountTab">

                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6">
                        <div class="input-group">
                            <label class="input-group-addon" for="serviceAccountEndpoint"><b>Application Name:</b></label>
                            <select id="serviceAccountEndpoint" multiple class="form-control selectpicker" data-live-search="true">
                            </select>
                            <span class="input-group-addon" onclick="clearUserAccountFilterByElement('#serviceAccountEndpoint','#serviceAccountEntitlement',2)"><i class="fa-solid fa-filter-circle-xmark"></i></span>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6">
                        <div class="input-group">
                            <label class="input-group-addon" for="serviceAccountEntitlement"><b>Role/Group:</b></label>
                            <select id="serviceAccountEntitlement" multiple class="form-control selectpicker" data-live-search="true">
                            </select>
                            <span class="input-group-addon" onclick="clearUserAccountFilterByElement('#serviceAccountEntitlement',null,2)"><i class="fa-solid fa-filter-circle-xmark"></i></span>
                        </div>
                    </div>
                </div>
				
                <div class="row" style="padding-top:10px;padding-bottom:5px;">
                    <div class="col-sm-12 col-md-12 col-lg-12">
                        <div style="float: left;padding-left:15px;" >
                    <label Id="entitlementNotesSrv" class="lblNotes" style="color:red;font-weight: lighter;"/>
                </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12">
                        <div id="serviceAccountGrid" style="width: 100%; height: 600px;" class="ag-theme-balham"></div>
                    </div>
                </div>

            </div>
            <div class="tab-pane fade" id="sharedAccountTab">

                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6">
                        <div class="input-group">
                            <label class="input-group-addon" for="sharedAccountEndpoint" data-live-search="true"><b>Application Name:</b></label>
                            <select id="sharedAccountEndpoint" multiple class="form-control selectpicker">
                            </select>
                            <span class="input-group-addon" onclick="clearUserAccountFilterByElement('#sharedAccountEntitlement','#sharedAccountEntitlement',3)"><i class="fa-solid fa-filter-circle-xmark"></i></span>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6">
                        <div class="input-group">
                            <label class="input-group-addon" for="sharedAccountEntitlement" data-live-search="true"><b>Role/Group:</b></label>
                            <select id="sharedAccountEntitlement" multiple class="form-control selectpicker">
                            </select>
                            <span class="input-group-addon" onclick="clearUserAccountFilterByElement('#sharedAccountEntitlement',null,3)"><i class="fa-solid fa-filter-circle-xmark"></i></span>
                        </div>
                    </div>
                </div>
               <div class="row" style="padding-top:10px;padding-bottom:5px;">
                    <div class="col-sm-12 col-md-12 col-lg-12">
                        <div style="float: left;padding-left:15px;" >
                    <label Id="entitlementNotesShd" class="lblNotes" style="color:Red;font-weight: lighter;"/>
                </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12">
                        <div id="sharedAccountGrid" style="width: 100%; height: 600px;" class="ag-theme-balham"></div>
                    </div>
                </div>

            </div>
            <div class="tab-pane fade" id="secondaryAccountTab">
                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6">
                        <div class="input-group">
                            <label class="input-group-addon" for="secondaryAccountEndpoint" data-live-search="true"><b>Application Name:</b></label>
                            <select id="secondaryAccountEndpoint" multiple class="form-control selectpicker">
                            </select>
                            <span class="input-group-addon" onclick="clearUserAccountFilterByElement('#secondaryAccountEndpoint','#secondaryAccountEntitlement',4)"><i class="fa-solid fa-filter-circle-xmark"></i></span>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6">
                        <div class="input-group">
                            <label class="input-group-addon" for="sharedAccountEntitlement" data-live-search="true"><b>Role/Group:</b></label>
                            <select id="secondaryAccountEntitlement" multiple class="form-control selectpicker">
                            </select>
                            <span class="input-group-addon" onclick="clearUserAccountFilterByElement('#secondaryAccountEntitlement',null,4)"><i class="fa-solid fa-filter-circle-xmark"></i></span>
                        </div>
                    </div>
                </div>
                <div class="row" style="padding-top:10px;padding-bottom:5px;">
                    <div class="col-sm-12 col-md-12 col-lg-12">
                        <div style="float: left;padding-left:15px;" >
                    <label Id="entitlementNotesSec" class="lblNotes" style="color:red;font-weight: lighter;"/>
                </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12">
                        <div id="secondaryAccountGrid" style="width: 100%; height: 600px;" class="ag-theme-balham"></div>
                    </div>
                </div>
            </div>
        </div>
		<div class="row">
		<div class="mt-12">
           <div class="text-right" style="background-color:#fff;padding:5px 10px 10px 10px;border-bottom: 1px solid #e8f0fe;border-right: 1px solid #e8f0fe;border-left: 1px solid #e8f0fe;margin-left: -2px;
    margin-right: 2px;">
                   
                    <a id="btnSave" class="btn btn-primary btn-sm" onclick="savedChanges(this);" style="display: inline-block;">Save</a>
                    <a id="btnRevert" class="btn btn-default btn-sm" onclick="openRevertChanges(this);" data-toggle="modal" data-keyboard="false" style="display: inline-block;">Revert</a>
                    <a id="btnSaveAndSubmit" class="btn btn-default btn-sm" onclick="saveAndSubmit();" data-toggle="modal" style="display: inline-block;">Save & Submit</a>
                    <a id="btnInfo" class="btn btn-default btn-sm" onclick="showTaskInfo();" data-toggle="modal" data-keyboard="false" style="display: none;">Details</a>
                    <a id="btnDownloadAll" class="btn btn-default btn-sm" runat="server" onclick="downloadReport();" data-toggle="modal" style="display: inline-block;">Export</a>
                    <a id="btnAttestationComparision" class="btn btn-default btn-sm" data-toggle="modal" style="display: inline-block;">Cancel</a>
                </div>
        </div></div>
    </div>
    <div class="container-fluid">
        <div class="row">
            <div class="col-auto">
            </div>
        </div>

    </div>

    <div class="container-fluid-att">
        <div class="row">
            <div class="col-md-12 col-lg-12">
            </div>
        </div>
        <div class="row">
            <asp:HiddenField ID="hdnTaskId" runat="server" />
            <asp:HiddenField ID="hdnUsername" runat="server" />
            <asp:HiddenField ID="hdnIsEditable" runat="server" />
            <asp:HiddenField ID="hdnActionUserId" runat="server" />
            <asp:HiddenField ID="hdnBackUrl" runat="server" />
            <asp:HiddenField ID="hdnCampaignName" runat="server" />
        </div>
    </div>
    <div class="container">
        <div class="modal" id="mdwindowTaskInfo" tabindex="-1" role="dialog" aria-labelledby="mdTaskInfo" data-keyboard="false" data-backdrop="static">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <table style="width: 100%;">
                            <tr>
                                <td>
                                    <div style="color: #207bbd;">
                                        <h4 class="modal-title" id="mdTaskInfo">Task Information</h4>
                                        <div class="badge badge-warning" style="display: math;">
                                            <label id="lblStatus" class="description "></label>
                                        </div>
                                    </div>
                                </td>
                                <td>
                                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>

                                </td>
                            </tr>

                        </table>
                    </div>
                    <div class="modal-body container-fluid">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <div class="form-group">
                                    <label for="lblTaskId" class="col-xs-4 control-label">TaskId:</label>
                                    <div class="col-xs-8">
                                        <label id="lblTaskId" class="description"></label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="lblAttName" class="col-xs-4 control-label"><%= T("App.Attestation.Name") %>:</label>
                                    <div class="col-xs-8">
                                        <label id="lblAttName" class="description "></label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="lblAttestationDesc" class="col-xs-4 control-label"><%= T("App.Attestation.Description") %></label>
                                    <div class="col-xs-8">
                                        <label id="lblAttestationDesc" class="description "></label>
                                    </div>

                                </div>
                                <div class="form-group">
                                    <label for="lblType" class="col-xs-4 control-label"><%= T("App.Attestation.Type") %></label>
                                    <div class="col-xs-8">
                                        <label id="lblType" class="description "></label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="lblCampaignName" class="col-xs-4 control-label"><%= T("App.Campaign.Name") %></label>
                                    <div class="col-xs-8">
                                        <label id="lblCampaignName" class="description "></label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="lblReqDate" class="col-xs-4 control-label"><%= T("App.Common.RequestDate") %></label>
                                    <div class="col-xs-8">
                                        <label id="lblReqDate" class="description "></label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="lblCurrentReviewer" class="col-xs-4 control-label"><%= T("App.Attestation.CurrentReviewer") %></label>
                                    <div class="col-xs-8">
                                        <label id="lblCurrentReviewer" class="description "></label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="lblCurrentLevel" class="col-xs-4 control-label"><%= T("App.Attestation.CurrentLevel") %></label>
                                    <div class="col-xs-8">
                                        <label id="lblCurrentLevel" class="description "></label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="lblSubmitBefore" class="col-xs-4 control-label"><%= T("App.Attestation.SubmitBefore") %></label>
                                    <div class="col-xs-8">
                                        <label id="lblSubmitBefore" class="description "></label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row-fluid">
                            <div class="col-xs-4">&nbsp;</div>
                            <div class="col-xs-8">
                                <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="container">
        <div class="modal" id="mdwindowSaveandSubmit" tabindex="-1" role="dialog" aria-labelledby="mdSaveandSubmit" aria-hidden="true" data-keyboard="false" data-backdrop="static">
            <div class="modal-dialog modal-center" role="document">
                <div class="modal-content ">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <div style="color: #207bbd;">
                            <h5 class="modal-title" id="mdSaveandSubmit">Access Review - Submit Confirmation</h5>
                        </div>
                    </div>
                    <div class="modal-body container">
                        <div class="row">&nbsp;</div>
                        <div class="row">
                            <div class="col-xs-1">
                            </div>
                            <div class="col-xs-12">
                                <div style="display: inline-block">
                                    <div style="display: inline-block; vertical-align: middle;">
                                        <img id="imgCommit" src="../../Images/action-buttons/submit.png" class="img-responsive">
                                    </div>
                                    <div style="display: inline-block; vertical-align: middle;">
                                        <label id="lblConfirm" runat="server" text="Are you sure you want to submit review changes ?" class="control-label">Are you sure you want to submit review changes ?</label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">&nbsp;</div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-success" id="btnConfirmSubmit">Confirm</button>&nbsp;
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Cancel</button>

                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="container">
        <div class="modal" id="mdwindowRevert" tabindex="-1" role="dialog" aria-labelledby="mdRevert" data-keyboard="false" data-backdrop="static">
            <div class="modal-dialog modal-center" role="document">
                <div class="modal-content" style="margin: auto;">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <div style="color: #207bbd;">
                            <%--                            <h4 class="modal-title" id="mdRevert">Access Review -  Revert Confirmation?</h4>--%>
                        </div>
                    </div>
                    <div class="modal-body container">
                        <div class="row-fluid">
                            <asp:Label ID="Label1" runat="server" Text="Do you want to revert the changes made since last saved?" class="control-label"></asp:Label>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-success" data-dismiss="modal" onclick="revertChanges()">Yes</button>
                        &nbsp;
                        <button type="button" class="btn btn-danger" data-dismiss="modal" onclick="cancelRevert()">No</button>

                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="container">
        <div class="modal" id="mdwindowPendingReview" tabindex="-1" role="dialog" aria-labelledby="mdSaveandSubmit" aria-hidden="true" data-keyboard="false" data-backdrop="static">
            <div class="modal-dialog modal-center" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <div style="color: #207bbd;">
                            <h4 class="modal-title" id="mdPendingReview">Access Review - Pending Review</h4>
                        </div>
                    </div>
                    <div class="modal-body" style="background-color: #e8f0fe !important; color: #337ab7; text-decoration: none;">
                        <div class="row-fluid" style="display: inline-block; vertical-align: middle;" id="lblPendingReviewNotification">
                            <div>Please complete the following review: </div>
                            <div>
                                <ul class="pending-list">
                                    <li id="liUsrAc"><i class="fa-solid fa-user"></i>&nbsp;User Account(s)</li>
                                    <li id="liSecAc"><i class="fa-solid fa-user-group"></i>&nbsp;Secondary Account(s)</li>
                                    <li id="liShdAc"><i class="fa-solid fa-users"></i>&nbsp;Shared Account(s)</li>
                                    <li id="liSvcAc"><i class="fa-solid fa-desktop"></i>&nbsp;Service Account(s)</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Cancel</button>

                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="container">
        <div class="modal" id="mdwindowAccounEndpoint" tabindex="-1" role="dialog" aria-labelledby="mdAccountApprove" data-keyboard="false" data-backdrop="static">
            <div class="modal-dialog modal-center" role="document">
                <div class="modal-content" style="margin: auto;">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <div style="color: #207bbd;">
                            <h4 class="modal-title" id="mdAccountRevoke">
                                <i id="iconAccountType"></i>&nbsp;<label id="lblAccountCategory" class="control-label"></label>
                                &nbsp; - Retain/Revoke?</h4>
                        </div>
                    </div>
                    <div class="modal-body ">
                        <div class="row-fluid">
                            <label id="lblAccountType" class="control-label" style="display: none;"></label>
                        </div>
                        <div class="row-fluid">
                            <label id="lblEmployeeName" class="control-label"></label>
                        </div>
                        <div class="row-fluid">
                            <div class="container-fluid" style="padding: 0px; width: 100%;">
                                <div id="accountGrid" style="width: 100%; height: 350px;" class="ag-theme-balham"></div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal" id="mdConfirmSpecialAccount" tabindex="-1" role="dialog" aria-labelledby="hdSpecAccount" data-keyboard="false" data-backdrop="static">
            <div class="modal-dialog" role="document">
                <div class="modal-content" style="margin: auto; width: 600px; margin-top: 50%; margin-left: 2%;">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <div style="color: #207bbd;">
                            <h4 class="modal-title" id="hdSpecAccount">Confirm Revoke Access</h4>
                        </div>
                    </div>
                    <div class="modal-body ">
                        <asp:Label ID="lblConfirmMessageSpecialAccount" runat="server" Text="Are you sure you want to revoke account?" class="control-label"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnSpecialAccountConfirmation" class="btn btn-success" data-dismiss="modal">Confirm</button>
                        &nbsp;
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Cancel</button>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="container">
        <div class="modal" id="mdConfirmSpecialAccountEntitlement" tabindex="-1" role="dialog" aria-labelledby="hdSpecEntitlement" data-keyboard="false" data-backdrop="static">
            <div class="modal-dialog" role="document">
                <div class="modal-content" style="margin: auto; width: 600px; margin-top: 50%; margin-left: 2%;">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <div style="color: #207bbd;">
                            <h4 class="modal-title" id="hdSpecEntitlement">Confirm Revoke Access</h4>
                        </div>
                    </div>
                    <div class="modal-body ">
                        <asp:Label ID="lblConfirmMessageSpecialAccountEntitlement" runat="server" Text="Are you sure you want to revoke access?" class="control-label"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnSpecialAccountEntitlementConfirmation" class="btn btn-success" data-dismiss="modal">Confirm</button>
                        &nbsp;
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Cancel</button>

                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal" style="top: 20%;" id="mdPleasewaitAfterSubmit" tabindex="-1" role="dialog" aria-labelledby="hdPleasewaitAfterSubmit" data-keyboard="false" data-backdrop="static">
        <div class="message-box">
            <div id="SqCo">
                <p>
                    Please wait&nbsp;
                                <span class="point one">.</span>
                    <span class="point two">.</span>
                    <span class="point three">.</span>
                    <span class="point four">.</span>
                    <span class="point five">.</span>
                </p>
                <span class="bloc one"></span>
                <span class="bloc two"></span>
                <span class="bloc three"></span>
                <span class="bloc four"></span>
                <span class="bloc five"></span>
            </div>

        </div>
    </div>
    <div class="floatingusr-menu-right">
        <div id="fbtnSave" class="floating-icon  expand-link" onclick="savedChanges(this);" title="Save" style="display: none;">
            <i class="fas fa-save"></i>
        </div>
        <div id="fbtnRevert" class="floating-icon  expand-link" onclick="openRevertChanges(this);" data-toggle="modal" data-keyboard="false" title="Revert" style="display: none;">
            <i class="fa fa-undo"></i>
        </div>
        <div id="fbtnSaveAndSubmit" class="floating-icon  expand-link" onclick="saveAndSubmit();" data-toggle="modal" title="Save & Submit" style="display: none;">
            <i class="fa fa-paper-plane"></i>
        </div>
        <div id="fbtnExport" class="floating-icon  expand-link" onclick="downloadReport();" data-toggle="modal" data-keyboard="false" title="Export">
            <i class="fa fa-download"></i>
        </div>
        <div class="floating-icon  expand-link" onclick="showTaskInfo();" data-toggle="modal" title="Details">
            <i class="fa-solid fa-circle-info"></i>
        </div>
        <div class="floating-icon  expand-link" title="Go User Review">
            <div onclick="gotoUserReviewPage()"><i class="fa-solid fa-eye"></i>&nbsp;</div>
        </div>
    </div>
    <style>
	.btn {
    background: #eeeeee none repeat scroll 0 0;
    border: 1px solid #ddd;
    color: #2077bd;
    font-size: 14px;
    font-weight: 700;
    padding: 5px 15px;
    line-height: 1.5;
    height: auto;
    border-radius: 3px;
}
        .float-icon{
            width:24px;
            height:24px;
            border-radius:5px;
        }
        .floating-icon {
    clear: both;
    display: flex; /* Change display to flex */
    justify-content: center; /* Center align horizontally */
    align-items: center; /* Center align vertically */
    height: 25px;
    width: 25px;
    margin-bottom: 12px;
    background-color: #2077bd;
    text-align: center;
    color: #fff;
    border-radius: 5px;
    cursor:pointer;
}
        .floatingusr-menu-right {
    border-top-left-radius: 5px;
    border-bottom-left-radius: 5px;
    position: fixed !important;
    z-index: 999;
    right: 0px;
    top: 33%;
    width: 35px;
    border: 1px solid #DDD;
    padding: 10px 7px 0px 5px;
    /* position: fixed; */
    /* top: 33%; */
    right: 0;
    display: -webkit-box;
    display: -ms-flexbox;
    display: flex;
    flex-direction: column;
}
        @media (min-width: 768px) {
            .modal-dialog {
                width: 600px;
                margin: 10% auto;
            }
        }

        .modal-footer {
            background-color: #2077BD;
            padding: 8px 15px;
            border-top: 0;
        }

        .close {
            opacity: 1;
        }

        .message-box {
            position: absolute;
            top: 35%;
            left: 50%;
            width: 350px;
            transform: translate(-50%, -50%);
            background-color: #fff;
            padding: 10px;
            border-radius: 5px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.3);
        }

        @keyframes bounceIn {
            0% {
                transform: scale(0.1);
                opacity: 0;
            }

            60% {
                transform: scale(1.2);
                opacity: 1;
            }

            100% {
                transform: scale(1);
            }
        }

        @-webkit-keyframes bounceIn {
            0% {
                transform: scale(0.1);
                opacity: 0;
            }

            60% {
                transform: scale(1.2) rotate(90deg);
                opacity: 1;
            }

            100% {
                transform: scale(1);
            }
        }

        div#load {
            animation: bounceIn 2s;
            -webkit-animation: bounceIn 2s ease-in-out;
            width: 150px;
            height: 150px;
        }

        @keyframes fadeIn {
            0% {
                opacity: 0;
                border-radius: 0;
            }

            100% {
                opacity: 1;
                border-radius: 50px;
            }
        }

        @-webkit-keyframes fadeIn {
            0% {
                opacity: 0;
                border-radius: 0;
            }

            100% {
                opacity: 1;
                border-radius: 50px;
            }
        }

        @keyframes rotateR {
            from {
                transform: rotate(0deg);
            }

            to {
                transform: rotate(360deg);
            }
        }

        @-webkit-keyframes rotateR {
            from {
                transform: rotate(0deg);
            }

            to {
                transform: rotate(360deg);
            }
        }

        div#SqCo {
            text-align: center;
            background-color: #fff;
            width: 100%;
            height: 70px;
            padding: 14.5px;
        }


            div#SqCo .bloc {
                background: #073e6b;
                display: inline-block;
                width: 20px;
                height: 20px;
                margin: 0 10px;
                opacity: 0;
            }



                div#SqCo .bloc.one {
                    -webkit-animation: fadeIn 1s linear 0s infinite alternate, rotateR 1s linear 0s infinite;
                    animation: fadeIn 1s linear 0s infinite alternate, rotateR 1s linear 0s infinite;
                }

                div#SqCo .bloc.two {
                    -webkit-animation: fadeIn 1s linear 0.5s infinite alternate, rotateR 1s linear 0.5s infinite;
                    animation: fadeIn 1s linear 0.5s infinite alternate, rotateR 1s linear 0.5s infinite;
                }

                div#SqCo .bloc.three {
                    -webkit-animation: fadeIn 1s linear 1s infinite alternate, rotateR 1s linear 1s infinite;
                    animation: fadeIn 1s linear 1s infinite alternate, rotateR 1s linear 1s infinite;
                }

                div#SqCo .bloc.four {
                    -webkit-animation: fadeIn 1s linear 1.5s infinite alternate, rotateR 1s linear 1.5s infinite;
                    animation: fadeIn 1s linear 1.5s infinite alternate, rotateR 1s linear 1.5s infinite;
                }

                div#SqCo .bloc.five {
                    -webkit-animation: fadeIn 1s linear 2s infinite alternate, rotateR 1s linear 2s infinite;
                    animation: fadeIn 1s linear 2s infinite alternate, rotateR 1s linear 2s infinite;
                }

            div#SqCo p {
                color: #2077BD;
                font-size: 18px;
                font-weight: bold;
                margin-bottom: 10px;
            }


                div#SqCo p .point {
                    opacity: 0;
                }

                    div#SqCo p .point.one {
                        -webkit-animation: fadeIn 1s linear 0s infinite alternate;
                        animation: fadeIn 1s linear 0s infinite alternate;
                    }

                    div#SqCo p .point.two {
                        -webkit-animation: fadeIn 1s linear 0.5s infinite alternate;
                        animation: fadeIn 1s linear 0.5s infinite alternate;
                    }

                    div#SqCo p .point.three {
                        -webkit-animation: fadeIn 1s linear 1s infinite alternate;
                        animation: fadeIn 1s linear 1s infinite alternate;
                    }

                    div#SqCo p .point.four {
                        -webkit-animation: fadeIn 1s linear 1s infinite alternate;
                        animation: fadeIn 1s linear 1s infinite alternate;
                    }

                    div#SqCo p .point.five {
                        -webkit-animation: fadeIn 1s linear 1s infinite alternate;
                        animation: fadeIn 1s linear 1s infinite alternate;
                    }


        ul.pending-list {
            list-style-type: none; /* Remove bullets */
            padding: 10px 10px 10px 10px; /* Remove padding */
            margin: 5px; /* Remove margins */
        }

            ul.pending-list li {
                padding: 5px;
            }

        .row {
            margin-right: 2px;
            margin-left: 2px;
        }

        .col-lg-12 {
            padding: 0px;
        }

        .control-label {
            font-weight: bold !important;
        }

        .tab-content > .tab-pane {
            padding: 0px 0px;
        }

        .tab-content > .tab-pane {
            padding: 5px 5px 5px 2px !important;
        }

        .modal-content {
            min-width: 450px !important;
        }

        .modal.in .modal-dialog {
            min-width: 650px;
        }


        img.not-exist {
            width: 21px !important;
            height: 21px !important;
            cursor: default;
        }

        .defaultaccess {
            width: 24px !important;
            height: 24px !important;
            cursor: not-allowed;
        }

        .ag-header-group-text {
            color: #fff !important;
            text-decoration: underline;
        }

        .badge {
            background-color: #fff !important;
        }

        span.ag-header-group-text:empty {
            display: none;
        }
        .badge {
    padding: 1px 9px 2px;
    font-size: 12.025px;
    font-weight: bold;
    white-space: nowrap;
    color: #ffffff;
    background-color: #70b9c5;
    -webkit-border-radius: 9px;
    -moz-border-radius: 9px;
    border-radius: 9px;
}

    .badge:hover {
        color: #ffffff;
        text-decoration: none;
        cursor: pointer;
    }

.badge-error {
    background-color: #b94a48;
}

    .badge-error:hover {
        background-color: #953b39;
    }

.badge-warning {
    background-color: #f89406 !important;
}

    .badge-warning:hover {
        background-color: #c67605  !important;
    }

.badge-success {
    background-color: ##5cc45e;
}

    .badge-success:hover {
        background-color: #356635;
    }

.badge-info {
    background-color: #3a87ad;
}

    .badge-info:hover {
        background-color: #2d6987;
    }

.badge-inverse {
    background-color: #333333;
}

    .badge-inverse:hover {
        background-color: #1a1a1a;
    }
    .imgTick {
    width: 24px;
    height: 24px;
}
    </style>

    <script type="text/javascript">
        jQuery.noConflict();

        var sessionTimeout;
        function openRevertChanges(selectedElement) {
            if (jQuery(selectedElement).is(":visible") === false) {
                // Prevent default click behavior
                event.preventDefault();
                // Stop event propagation
                event.stopPropagation();
                // Optionally, return false to also prevent default behavior and stop propagation
                return false;
            }
            jQuery("#mdwindowRevert").modal("show");
        }
        function canShowActionButton(show) {
            if (show === true) {
                jQuery("#btnSave").show();
                jQuery("#fbtnSave").show();
                jQuery("#fbtnRevert").show();
                jQuery("#btnRevert").show();
            } else {
                jQuery("#btnSave").hide();
                jQuery("#fbtnSave").hide();
                jQuery("#fbtnRevert").hide();
                jQuery("#btnRevert").hide();
            }

        }

        const resizeTimeoutDuration = 5;
        const USRACC = "USRAC";
        const SERVICEACC = "SVCAC";
        const SHAREDACC = "SHDAC";
        const SECONDARYACC = "SECAC";
        var isSavedChangesFound = false;
        var taskId = 0;
        var isEditable = 0;
        var actionUserId = 0;
        var hdnUsername = "";
        //UAC,SVCAC,SHDAC,SECAC
        var activeTab = USRACC;
        let userAgGridApi;
        var userGridOptions;
        var isUserGridInitialized = false;

        //Tab Click Validation
        var isUserTabClicked = false;
        var isSharedTabClicked = false;
        var isServiceTabClicked = false;
        var isSecondaryTabClicked = false;

        let serviceAgGridApi;
        var serviceGridOptions;
        var isServiceGridInitialized = false;

        let sharedAgGridApi;
        var sharedGridOptions;
        var isSharedGridInitialized = false;


        let secondaryAgGridApi;
        var secondaryGridOptions;
        var isSecondaryGridInitialized = false;
        let secondaryGridApi;

        let isTaskInfoOpened = false;
        let previousPage;
        jQuery('#unSavedChanges').hide();
        //jQuery(".dt-search").hide();
        var pageUrlDomain = location.protocol + "//" + location.host;
        let isResizing = false;
        let resizeTimeout;
        var columnNameTemplate = `<div class="ag-cell-label-container"  role="presentation">
													  <span ref="eMenu" class="ag-header-icon ag-header-cell-menu-button"></span>
													  <span ref="eFilterButton" class="ag-header-icon ag-header-cell-filter-button"></span>
													  <div ref="eLabel" style="color:#0000f4 !important;text-decoration:underline;" class="ag-header-cell-label" role="presentation">
														<span ref="eSortOrder" class="ag-header-icon ag-sort-order"></span>
														<span ref="eSortAsc" class="ag-header-icon ag-sort-ascending-icon"></span>
														<span ref="eSortDesc" class="ag-header-icon ag-sort-descending-icon"></span>
														<span ref="eSortNone" class="ag-header-icon ag-sort-none-icon"></span>
														<span ref="eText" style="color:#0000f4 !important;text-decoration:underline;" class="ag-header-cell-text badge badge-default" role="columnheader"></span>
														<span ref="eFilter" class="ag-header-icon ag-filter-icon"></span>
													  </div>
													</div>`;

        function gotoUserReviewPage() {
            var url = `${pageUrlDomain}/Admin/Attestation/NReview.aspx?Id=${taskId}`;
            window.location.href = url;
        }
        class AccountActionRenderer {

            // Optional: Params for rendering. The same params that are passed to the cellRenderer function.
            init(params) {

                var tObj = params.value;
                var imeg = "";
                var imgTitle = "";
                if (tObj && tObj.length > 0) {
                    const blockImg = "block.png";
                    var accessType = "";
                    var objAcc = JSON.parse(tObj);
                    if (objAcc.AccountStatus === true) {
                        imeg = "Retain.png";
                        imgTitle = "To be Retained";
                        accessType = "Retain";
                    } else {
                        imeg = "Revoked.png";
                        imgTitle = "To be Revoked";
                        accessType = "Revoked";
                    }

                    var dataObj = params.data;
                    let icon = document.createElement("img")
                    icon.title = imgTitle;
                    icon.dataset.status = objAcc.AccountStatus;
                    icon.dataset.accessType = accessType;
                    icon.dataset.endpointId = dataObj.EndpointId;
                    icon.dataset.userId = objAcc.Id;
                    icon.dataset.employeeid = objAcc.AdditionalInfo;
                    icon.dataset.username = objAcc.UserName;
                    icon.dataset.entitlementType = "Account";
                    icon.dataset.employee = objAcc.EndpointId + "|" + objAcc.UserName + "|" + objAcc.Id;
                    icon.setAttribute("class", "missionIcon ");
                    icon.dataset.entitlement = objAcc.UserName;
                    if (objAcc.IsActionable && objAcc.IsActionable === true) {
                        icon.className = "imgTick";
                        icon.onclick = accountRevoke;
                        icon.src = `../../assets/DataTables/${imeg}`;
                    } else {
                        icon.setAttribute("class", "defaultaccess")
                        icon.title = "Default Access";
                        icon.src = `../../assets/DataTables/${blockImg}`;
                    }
                    this.eGui = document.createElement("span")
                    this.eGui.setAttribute("class", "missionSpan")
                    this.eGui.appendChild(icon)
                } else {
                    let icon = document.createElement("img")
                    icon.title = "Not Exists";
                    icon.src = `../../assets/DataTables/wrong.png`;
                    icon.setAttribute("class", "not-exist")
                    this.eGui = document.createElement("span")
                    this.eGui.setAttribute("class", "missionSpan")
                    this.eGui.appendChild(icon)
                }


            }

            // Required: Return the DOM element of the component, this is what the grid puts into the cell
            getGui() {
                return this.eGui
            }

            // Required: Get the cell to refresh.
            refresh(params) {
                return false
            }
        }
        const columnAccountDefs = [
            {
                headerName: "Application Name", field: "EndpointName", resizable: false, tooltipValueGetter: function (params) {
                    // Return the tooltip text based on the cell value
                    return params.value;
                }
            },
            {
                headerName: "UserName", field: "UserName", resizable: false, tooltipValueGetter: function (params) {
                    // Return the tooltip text based on the cell value
                    return params.value;
                }
            },
            {
                width: 80, headerName: "Action", field: "AccountStatus", cellRenderer: AccountActionRenderer, editable: false, resizable: false, sortable: false,
                valueGetter: function (params) {
                    if (params && params.data) {

                        return JSON.stringify(params.data);
                    } return "";
                }
            }
        ];

        const accountGridOptions = {
            columnDefs: columnAccountDefs,
            rowData: [], tooltipShowDelay: 0,
        };

        function loadbyAccountTypeEndpoint(taskid, accountType, epElementId) {
            jQuery.ajax({
                type: "POST",
                url: `${pageUrlDomain}/Services/AttestationDataService.svc/GetTaskFilterData`,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({ taskFilter: { TaskId: taskid, FilterType: 3, EpIds: null, UserIds: null, AccountType: accountType } }), // Adjust length as needed
                success: function (data) {
                    var jsonObjectArray = data.GetTaskFilterDataResult
                    if (jsonObjectArray) {
                        jQuery(epElementId).selectpicker();
                        var $multiselectElement = jQuery(epElementId);
                        jQuery.each(jsonObjectArray, function (index, value) {
                            var $newOption = jQuery("<option>").val(value.Id).text(value.Name);
                            $newOption =
                                $multiselectElement.append($newOption);
                        });
                        $multiselectElement.selectpicker({
                            noneSelectedText: 'All'
                        });
                        $multiselectElement.selectpicker('refresh');
                    }
                }
            });
        }
        function showunsavedChanges() {
            try {
                var jsonObject = {
                    "taskId": taskId
                };
                jQuery.ajax({
                    type: "POST",
                    url: `${pageUrlDomain}/Services/AttestationDataService.svc/GetTaskHasUnsavedChanges`,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify(jsonObject),
                    success: function (data) {
                        if (data && data.GetTaskHasUnsavedChangesResult === true) {
                            //toggleAlert('Success',`${entitlement} ${accessType} successfully`);
                            jQuery('#unSavedChanges').show();
							if(parseInt(isEditable) === 1){
								isSavedChangesFound = true;
								canShowActionButton(true);
							}else{
								canShowActionButton(false);
							}
                        } else if (data && data.GetTaskHasUnsavedChangesResult === false) {
                            jQuery('#unSavedChanges').hide();
                            isSavedChangesFound = false;
                            canShowActionButton(false);
                        }
                    },
                    error: function (errormsg) {
                        alert(errormsg);
                    }
                });
            } catch (e) {
                alert(e);
            }
        }

        function getAccountTypeSelectedEndPoints(element) {
            var selectedEndpoints = jQuery(`${element} option:selected`).map(function () { return parseInt(jQuery(this).val()); }).get();
            var endpointIds = [];
            if (selectedEndpoints && selectedEndpoints.length > 0) {
                endpointIds = selectedEndpoints;
            }
            return endpointIds;
        }

        function getAccountTypeSelectedEntitlement(element) {
            var selectedValues = jQuery(element + ' option:selected').map(function () {
                return jQuery(this).text();
            }).get().join(',');
            return selectedValues;
        }

        function clearUserAccountFilterByElement(elementId, dependentElement, accountType) {
            jQuery(elementId + ' option:selected').prop("selected", false);
            jQuery(`${elementId}`).selectpicker('refresh');
            if (dependentElement && dependentElement != null) {
                jQuery(dependentElement + ' option:selected').prop("selected", false);
                jQuery(`${dependentElement}`).selectpicker('refresh');
            }
            if (accountType && accountType === 1)
                initializeOrReloadUserAccountGrid();

            if (accountType && accountType === 2)
                initializeOrReloadServiceAccountGrid();

            if (accountType && accountType === 3)
                initializeOrReloadSharedAccountGrid();

            if (accountType && accountType === 4)
                initializeOrReloadSecondaryAccountGrid();

        }

        function loadAccountTypeEntitlement(taskid, accountType, entitlementId, epElementId) {

            jQuery.ajax({
                type: "POST",
                url: `${pageUrlDomain}/Services/AttestationDataService.svc/GetTaskFilterData`,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({ taskFilter: { TaskId: taskid, FilterType: 2, UserIds: null, EpIds: getAccountTypeSelectedEndPoints(epElementId), AccountType: accountType } }), // Adjust length as needed
                success: function (data) {
                    var jsonObjectArray = data.GetTaskFilterDataResult
                    if (jsonObjectArray) {
                        jQuery(entitlementId).empty();
                        jQuery(entitlementId).selectpicker();
                        var $multiselectElement = jQuery(entitlementId);
                        var EndpointName = "";
                        var group = null;
                        var epNames = jsonObjectArray.map(function (obj) { return obj.EndpointName; });
                        epNames = epNames.filter(function (v, i) { return epNames.indexOf(v) == i; });
                        for (i = 0; i < epNames.length; i++) {
                            EndpointName = epNames[i];
                            group = jQuery('<optgroup label="' + EndpointName + '" />');
                            var selectEpEntitlements = jsonObjectArray.filter(v => v.EndpointName === EndpointName);
                            for (j = 0; j < selectEpEntitlements.length; j++) {
                                var $newOption = jQuery("<option>").val(selectEpEntitlements[j].Id).text(selectEpEntitlements[j].Name);
                                $newOption =
                                    group.append($newOption);
                            }
                            $multiselectElement.append(group);
                        }
                        $multiselectElement.selectpicker({
                            noneSelectedText: 'All'
                        });
                        $multiselectElement.selectpicker('refresh');
                    }
                }
            });
        }

        function savedChanges(selectedElement) {
            try {
                if (jQuery(selectedElement).is(":visible") === false) {
                    // Prevent default click behavior
                    event.preventDefault();
                    // Stop event propagation
                    event.stopPropagation();
                    // Optionally, return false to also prevent default behavior and stop propagation
                    return false;
                }
                jQuery(selectedElement).hide();

                hdnUsername = document.getElementById('<%= hdnUsername.ClientID %>').value;
                  var jsonObject = {
                      "taskId": taskId,
                      "userName": hdnUsername
                  };
                  jQuery.ajax({
                      type: "POST",
                      url: `${pageUrlDomain}/Services/AttestationDataService.svc/SavedChanges`,
                      contentType: "application/json; charset=utf-8",
                      dataType: "json",
                      data: JSON.stringify(jsonObject),
                      success: function (data) {
                          if (data && data.SavedChangesResult === true) {
                              //toggleAlert('Success',`${entitlement} ${accessType} successfully`);
                              jQuery('#unSavedChanges').hide();
                              showMessage("success", "changes saved successfully");
                              canShowActionButton(false);
                              refreshTab();
                          } else if (data && data.SavedChangesResult === false) {
                              jQuery('#unSavedChanges').show();
                              showMessage("error", "Error occured while saving");
                              canShowActionButton(true);
                          }
                          jQuery(selectedElement).hide();
                      },
                      error: function (errormsg) {
                          showMessage("error", "Error occured while saving");
                          jQuery(selectedElement).show();
                      }
                  });
              }
              catch (e) {
                  jQuery(selectedElement).show();
                  showMessage("error", "Error occured while saving");
              }
          }

          function refreshTab() {
              if (activeTab === USRACC && isUserGridInitialized && isUserGridInitialized === true) initializeOrReloadUserAccountGrid();
              else if (activeTab === SERVICEACC && isServiceGridInitialized && isServiceGridInitialized === true) initializeOrReloadServiceAccountGrid();
              else if (activeTab === SHAREDACC && isSharedGridInitialized && isSharedGridInitialized === true) initializeOrReloadSharedAccountGrid();
              else if (activeTab === SECONDARYACC && isSecondaryGridInitialized && isSecondaryGridInitialized === true) initializeOrReloadSecondaryAccountGrid();
          }

          function getAccountType() {
              let accType = "User Account";
              if (activeTab === USRACC) {
                  accType = "User Account";
              }
              if (activeTab === SERVICEACC) {
                  accType = "Service Account";
              }
              if (activeTab === SHAREDACC) {
                  accType = "Shared Account";
              }
              if (activeTab === SECONDARYACC) {
                  accType = "Secondary Account";
              }
              return accType;
          }
          function getAccountTypeId() {
              let accType = 1;
              if (activeTab === USRACC) {
                  accType = 1;
              }
              if (activeTab === SERVICEACC) {
                  accType = 2;
              }
              if (activeTab === SHAREDACC) {
                  accType = 3;
              }
              if (activeTab === SECONDARYACC) {
                  accType = 4;
              }
              return accType;
          }
          function setAccountTypeIcon() {
              let accType = "fa-solid fa-user";
              if (activeTab === USRACC) {
                  accType = "fa-solid fa-user";
              }
              if (activeTab === SERVICEACC) {
                  accType = "fa-solid fa-desktop";
              }
              if (activeTab === SHAREDACC) {
                  accType = "fa-solid fa-users";
              }
              if (activeTab === SECONDARYACC) {
                  accType = "fa-solid fa-user-group";
              }
              jQuery("#iconAccountType").addClass(accType);
          }

          function showMessage(status, message) {
              if (status === 'success') { toastr.success(message); }
              else if (status === 'error') { toastr.error(message); }
              else if (status === 'warning') { toastr.warning(message); }
          }

          function accountRevoke() {
              if (parseInt(isEditable) === 1) {
                  selectedElement = jQuery(this);

                  //UserId
                  var accountId = jQuery(this).data('userId');
                  var entitlement = jQuery(this).data('entitlement');
                  var userName = jQuery(this).data('username');
                  var endpointId = jQuery(this).data('endpointId');
                  var status = jQuery(this).data('status');
                  var accessType = status === true ? "Retain" : "Revoke";
                  var accountType = parseInt(jQuery("#lblAccountType").text());
                  var action = accessType === "Retain" ? "Revoked" : "Retain";
                  if (accessType && accessType != 'NotExists') {
                      var jsonObject = {
                          "dTORequest": {
                              "TaskId": taskId,
                              "EndpointId": endpointId,
                              "UserId": accountId,
                              "AccessType": action,
                              "Entitlement": entitlement,
                              "UserName": userName,
                              "AccountType": accountType,
                              "ActionUserId": actionUserId,
                              "Level": 1,
                              "EntitlementType": "Account"
                          }
                      };
                      if ((accountType === 2 || accountType === 3) && status === true) {
                          jQuery('#mdConfirmSpecialAccountEntitlement').modal('show');

                          // Confirm button click event
                          jQuery('#btnSpecialAccountEntitlementConfirmation').off().on('click', function () {
                              confirmAccountRevoke(jsonObject, selectedElement, status);
                              jQuery('#mdConfirmSpecialAccountEntitlement').modal('hide');
                          });
                      }
                      else {
                          //jQuery('#loading-overlay').show();
                          jQuery('#mdPleasewaitAfterSubmit').modal('show');
                          confirmAccountRevoke(jsonObject, selectedElement, status);
                      }

                  }
              }
          }
          function confirmAccountRevoke(dataObj, selectedElement, status) {
              jQuery.ajax({
                  type: "POST",
                  url: `${pageUrlDomain}/Services/AttestationDataService.svc/UpdateAttestationTaskActionByUser`,
                  contentType: "application/json; charset=utf-8",
                  dataType: "json",
                  data: JSON.stringify(dataObj),

                  success: function (data) {
                      if (data && data.UpdateAttestationTaskActionByUserResult === true) {
                          dataObj = dataObj.dTORequest;
                          selectedElement.attr("src", `${pageUrlDomain}/assets/DataTables/` + dataObj.AccessType + ".png");
                          selectedElement.data("status", !status);
                          selectedElement.data("title", dataObj.AccessType);
                          jQuery('#unSavedChanges').show();
                          showMessage("success", `Account: ${dataObj.UserName} status changed to ${dataObj.AccessType}`);
                          isSavedChangesFound = true;
                          canShowActionButton(true);
                          if (dataObj.AccountType === 1) {
                              initializeOrReloadUserAccountGrid();
                          }
                          if (dataObj.AccountType === 2) {
                              initializeOrReloadServiceAccountGrid();
                          }
                          if (dataObj.AccountType === 3) {
                              initializeOrReloadSharedAccountGrid();
                          }
                          if (dataObj.AccountType === 4) {
                              initializeOrReloadSecondaryAccountGrid();
                          }
                      } else {
                          canShowActionButton(false);
                          //jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                          jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                          showMessage("error", `Account: ${userName} ${dataObj.AccessType} action not done`);
                      }
                      jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                      //jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                  },
                  error: function (error) {
                      //jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                      jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                      showMessage("error", `Error occured while saving ${entitlement} ${accessType}`);
                      canShowActionButton(false);
                  }
              });
          }
          function iconRenderer(params) {

              if (params.column.instanceId > 2) {
                  if (params.data && params.data.EmployeeNames) {

                      const employee = params.data.EmployeeNames.find(emp => emp.Value.UserName === params.colDef.headerName);
                      if (employee && employee.Value.Approve == true) {
                          return '<i class="fa fa-check-circle" style="color: green;"></i>';
                      } else if (employee && employee.Value.Approve == false) {
                          return '<i class="fa fa-times-circle" style="color: red;"></i>';
                      } else {
                          return '';
                      }
                  } else {
                      params.value;
                  }
              }
              return params.value;
          }

          function centerAlignedRenderer(params) {
              return '<div style="text-align: center;">' + params.value + '</div>';
          }

        class MissionResultRenderer {
            init(params) {
            this.params = params;
                this.eGui = document.createElement('span');
                this.updateImage(params);

                // Attach click event
                //this.eGui.querySelector('img').addEventListener('click', this.onClick.bind(this));
            }

            getGui() {
                return this.eGui;
            }

            updateImage(params) {
                let tObj = params.value;
                let imeg = "";
                let imgTitle = "";
                let accessType = "";
                const blockImg = "block.png";

                if (tObj && tObj.length > 0) {
                    const objAcc = JSON.parse(tObj);
                    if (objAcc.Value.Approve === true) {
                        imeg = "Retain.png";
                        imgTitle = "To be Retained";
                        accessType = "Retain";
                    } else {
                        imeg = "Revoked.png";
                        imgTitle = "To be Revoked";
                        accessType = "Revoked";
                    }

                    const dataObj = this.params.data;
                    let icon = document.createElement("img");
                    icon.title = imgTitle;
                    icon.className = "imgTick";
                    icon.dataset.accessType = accessType;
                    icon.dataset.endpointId = dataObj.EndpointId;
                    icon.dataset.accountid = objAcc.Value.AccountId;
                    icon.dataset.employeeid = objAcc.Value.EmployeeNo;
                    icon.dataset.entitlement = dataObj.Entitlement;
                    icon.dataset.entitlementType = dataObj.EntitlementType;
                    icon.dataset.username = objAcc.Value.UserName;
                    icon.dataset.employee = `${objAcc.Key}|${objAcc.Value.UserName}|${objAcc.Value.AccountId}`;
                    icon.setAttribute("class", "missionIcon");

                    if (objAcc.Value.IsActionable && objAcc.Value.IsActionable === true) {
                        icon.className = "imgTick";
                        icon.addEventListener("click", function () {
                            approveorrevoke(this, params)
                        });
                        icon.src = `../../assets/DataTables/${imeg}`;
                    } else {
                        icon.setAttribute("class", "missionIcon defaultaccess");
                        icon.title = "Default Access";
                        if (accessType == "Revoked") {
                            icon.src = `../../assets/DataTables/${imeg}`;
                        } else {
                            icon.src = `../../assets/DataTables/${blockImg}`;
                        }
                    }

                    this.eGui.innerHTML = "";
					this.eGui = document.createElement("span");
                    this.eGui.setAttribute("class", "missionSpan");
                    this.eGui.appendChild(icon);
                } else {
                    let icon = document.createElement("img");
                    icon.title = "No Access";
                    icon.src = `../../assets/DataTables/wrong.png`;
                    icon.setAttribute("class", "not-exist");
                    this.eGui.innerHTML = "";
					this.eGui = document.createElement("span");
                    this.eGui.setAttribute("class", "missionSpan");
                    this.eGui.appendChild(icon);
                }
            }

            //onClick(event) {
            //    event.stopPropagation();
            //    this.approveorrevoke(event, this.params);
            //}



            refresh(params) {
                if (params.value !== this.params.value) {
                    this.params = params;
                    this.updateImage(params);
                }
                return true;
            }
        }
        function approveorrevoke(event, params) {
            if (event && parseInt(isEditable) === 1) {
                var selectedElement = jQuery(event);
                var accessType = selectedElement.data('accessType');
                var accountId = selectedElement.data('accountid');
                var entitlement = selectedElement.data('entitlement');
                var entitlementType = selectedElement.data('entitlementType');
                var userName = selectedElement.data('username');
                var endpointId = selectedElement.data('endpointId');
                var newStatus = accessType === "Retain" ? "Revoked" : "Retain";
                var changedImgTitle = accessType === "Retain" ? "To be Revoked" : "To be Retained";
                var message = accessType === "Retain" ? "Change to ‘Revoke’" : "Revert to ‘Retain’";
                
                var accountType;
                if (activeTab === USRACC) accountType = 1;
                else if (activeTab === SERVICEACC) accountType = 2;
                else if (activeTab === SHAREDACC) accountType = 3;
                else if (activeTab === SECONDARYACC) accountType = 4;

                if (accessType && accessType != 'NotExists') {
                    var jsonObject = {
                        "dTORequest": {
                            "TaskId": taskId,
                            "EndpointId": endpointId,
                            "UserId": accountId,
                            "AccessType": newStatus,
                            "Entitlement": entitlement,
                            "UserName": userName,
                            "AccountType": accountType,
                            "ActionUserId": actionUserId,
                            "Level": 2,
                            "EntitlementType": entitlementType
                        }
                    };

                    if ((accountType === 2 || accountType === 3) && accessType === "Retain") {
                        jQuery('#mdConfirmSpecialAccount').modal('show');
                        jQuery('#btnSpecialAccountConfirmation').off().on('click', function () {
                            confirmAccountRevoke(jsonObject, selectedElement, newStatus);
                            jQuery('#mdConfirmSpecialAccount').modal('hide');
                        });
                    } else {
                        confirmEntitlementAction(jsonObject, selectedElement, userName, newStatus, entitlement, changedImgTitle, message, params);
                    }
                }
            }
        }
          function idmsagGridColumnResized(event) {
              if (event.source == "uiColumnResized") {
                  isResizing = true;
                  clearTimeout(resizeTimeout);
                  resizeTimeout = setTimeout(function () {
                      isResizing = false;
                  }, resizeTimeoutDuration);
                  return;
              } else {
                  isResizing = false;
              }
          }
          function idmsagGridColumnClicked(event) {
              if (isResizing) {
                  isResizing = false;
                  return;
              } else {

                  openPopupWithCustomData(event.column, getAccountTypeId());
              }
          }
        
          function initializeOrReloadUserAccountGrid() {
              activeTab = USRACC;
              isUserTabClicked = true;
              //Already initialized
              if (isUserGridInitialized && isUserGridInitialized === true) {
                  //jQuery('#loading-overlay').show();
                  jQuery('#mdPleasewaitAfterSubmit').modal('show');
                  var dataSource = {
                      getRows: function (params) {
                          var pageSize = params.pageSize || 30;
                          var startRow = 0;
                          // Calculate page number based on startRow and page size
                          var pageNumber = Math.floor(startRow / pageSize) + 1;
                          //console.log("asking for " + startRow + " to " + params.endRow);
                          // Mock API call to fetch data
                          var selectedEndPoints = getAccountTypeSelectedEndPoints('#userAccountEndpoint');
                          var selectedEntitlements = getAccountTypeSelectedEntitlement('#userAccountEntitlement');
                          var searchObj = { taskId: taskId, draw: 1, start: params.startRow, length: params.endRow, search: { Value: selectedEntitlements, regex: false, EndpointIds: selectedEndPoints, AccountType: 1 } };

                          fetch(`${pageUrlDomain}/Services/AttestationDataService.svc/GetAttestationUserComparisionData`, {
                              method: 'post',
                              body: JSON.stringify(searchObj),
                              headers: { 'Content-Type': 'application/json; charset=utf-8' }
                          })
                              .then(httpResponse => httpResponse.json())
                              .then(function (data) {

                                  var jsonObjectArray = data.data
                                  params.successCallback(jsonObjectArray, data.recordsTotal);
                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                              })
                              .catch(function (error) {
                                  console.error('Error fetching data:', error);
                                  params.failCallback();
                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                              });
                      }
                  };
                  window.userGridApi.updateGridOptions({ datasource: dataSource });
              }
              else {

                  var selectedEndPoints = getAccountTypeSelectedEndPoints('#userAccountEndpoint');
                  //create grid
                  var data = { taskId: taskId, search: { value: "", regex: false, EndpointIds: selectedEndPoints, AccountType: 1 } };
                  jQuery.ajax({
                      type: "POST",
                      url: `${pageUrlDomain}/Services/AttestationDataService.svc/GetEmployeeInformation`,
                      contentType: "application/json; charset=utf-8",
                      dataType: "json",
                      data: JSON.stringify(data), // Adjust length as needed
                      success: function (data) {

                          const columnDefs = [
                              {
                                  headerName: "Application Name", field: "SystemName", width: 200, sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value
                                      return params.value;
                                  }
                              },
                              {
                                  headerName: "Role/Group", field: "Entitlement", width: 200, sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value
                                      return params.value;
                                  }
                              },
                              {
                                  headerName: "Description", field: "EntitlementDescription", width: 200, sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value
                                      if (params && params.value && params.value.length > 0 && params.value.match(/^ *$/) === null) {
                                          return params.value;
                                      }
                                      return null;
                                  }
                              }
                          ];

                          var jsonObjectArray = data;
                          //Get EmployeeName
                          var uniqueEmployeeNo = [];
                          for (const jsonObjectArray of data) {
                              var empName = jsonObjectArray.Key.split('_');

                              uniqueUsernames = [];
                              var childrenAccount = [];
                              const usernameMap = new Map();
                              for (const item of jsonObjectArray.Value) {
                                  const lowercaseUsername = item.UserName.toUpperCase();
                                  if (!usernameMap.has(lowercaseUsername)) {
                                      usernameMap.set(lowercaseUsername, item.UserName); // Store original case for display
                                      uniqueUsernames.push(lowercaseUsername);
                                      childrenAccount.push({
                                          headerTooltip: lowercaseUsername,
                                          headerComponentParams: {
                                              template: columnNameTemplate
                                          },
                                          customData: jsonObjectArray.Value, parentHeader: false,
                                          headerName: lowercaseUsername, resizable: true, field: `${empName[0]}-${lowercaseUsername}`, cellRenderer: MissionResultRenderer, editable: false, sortable: false,
                                          valueGetter: function (params) {
                                              if (params && params.data) {

                                                  const employee = params.data.EmployeeNames.find(emp => emp.Value.UserName === params.colDef.headerName);
                                                  return JSON.stringify(employee);
                                              } return "";
                                          }
                                      })
                                  }
                              }
                              columnDefs.push({ headerName: empName[0], parentHeader: true, customData: jsonObjectArray.Value, headerTooltip: empName[0], field: "", children: childrenAccount, resizable: true, sortable: false, editable: false, headerClass: "center-aligned-header" });
                          }
                          userGridOptions = {
                              onColumnResized: idmsagGridColumnResized,
                              onColumnHeaderClicked: idmsagGridColumnClicked,
                              columnDefs: columnDefs,
                              pagination: true,
                              rowModelType: 'infinite',
                              tooltipShowDelay: 0,
                              enableColumnMove: false,
                              defaultColDef: {
                                  flex: 1,
                                  minWidth: 130,
                                  editable: false,
                                  resizable: true,
                                  minHeight: 650,
                                  cellStyle: { borderRight: '1px solid rgb(217 221 223)' }
                              }, tooltipShowDelay: 0,
                              rowBuffer: 0,
                              getRowNodeId: function (data) {
                                  return data.id; // Ensure this matches the unique identifier in your row data
                              },
                              maxConcurrentDatasourceRequests: 1,
                              paginationPageSize: 30, // Number of rows to display per page
                              animateRows: true,
                              cacheBlockSize: 30, // Number of rows per request
                              infiniteInitialRowCount: 30,
                              maxBlocksInCache: 30,
                              onGridReady: function (params) {
                                  var gridApi = params.api;
                                  //jQuery('#loading-overlay').show();
                                  jQuery('#mdPleasewaitAfterSubmit').modal('show');
                                  // Callback function to initialize the grid
                                  var dataSource = {
                                      getRows: function (params) {
                                          var selectedEndPoints = getAccountTypeSelectedEndPoints('#userAccountEndpoint');
                                          //console.log("asking for " + params.startRow + " to " + params.endRow);
                                          // Mock API call to fetch data
                                          fetch(`${pageUrlDomain}/Services/AttestationDataService.svc/GetAttestationUserComparisionData`, {
                                              method: 'post',
                                              body: JSON.stringify({ taskId: taskId, draw: 1, start: params.startRow, length: params.endRow, search: { value: "", regex: false, endpointIds: [], AccountType: 1 } }),
                                              headers: { 'Content-Type': 'application/json; charset=utf-8' }
                                          })
                                              .then(httpResponse => httpResponse.json())
                                              .then(function (data) {
                                                  var jsonObjectArray = data.data
                                                  params.successCallback(jsonObjectArray, data.recordsTotal);
                                                  //jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');

                                              })
                                              .catch(function (error) {
                                                  console.error('Error fetching data:', error);
                                                  params.failCallback();
                                              });
                                      }
                                  };
                                  params.api.updateGridOptions({ datasource: dataSource });
                                  window.userGridApi = params.api;
                              }
                          };
                          jQuery(document).ready(function () {
                              // Assuming you have a div with id 'userAccountGrid' to render the grid
                              const gridDiv = document.querySelector('#userAccountGrid');
                              userAgGridApi = new agGrid.Grid(gridDiv, userGridOptions);
                              isUserGridInitialized = true;
                              loadAccountTypeEntitlement(taskId, 1, '#userAccountEntitlement', '#userAccountEndpoint')
                              //loadbyAccountTypeEndpoint(taskId, 1, '#userAccountGrid');
                              //Trigger filter by Entitlement for service account

                          });
                      }
                  });
              }
          }
        function confirmEntitlementAction(jsonObject, selectedElement, userName, newStatus, entitlement, changedImgTitle, message, params) {
              jQuery('#mdPleasewaitAfterSubmit').modal('show');
              jQuery.ajax({
                  type: "POST",
                  url: `${pageUrlDomain}/Services/AttestationDataService.svc/UpdateAttestationTaskActionByUser`,
                  contentType: "application/json; charset=utf-8",
                  dataType: "json",
                  data: JSON.stringify(jsonObject),
                  success: function (data) {
                      if (data && data.UpdateAttestationTaskActionByUserResult === true) {
                          jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                          selectedElement.attr("src", `${pageUrlDomain}/assets/DataTables/` + newStatus + ".png");
                          selectedElement.attr("title", changedImgTitle);
                          selectedElement.data("accessType", newStatus);
                          var dataA = JSON.parse(params.value);
                          if (newStatus == "Revoked") {
                              dataA.Value.Approve = false;
                              dataA.Value.Revoke = true;
                          }
                          else {
                              dataA.Value.Revoke = false;
                              dataA.Value.Approve = true;
                          }
                          var rowNode = userAgGridApi.gridOptions.api.getRowNode(params.node.id);
                          if (rowNode) {
                              var mainData = rowNode.data.EmployeeNames.findIndex(function (employee)
                              {
                                  return employee.Key === dataA.Key;
                              });
                              if (mainData !== -1) {
                                  rowNode.data.EmployeeNames[mainData] = dataA;
                              }
                          }

                          jQuery('#unSavedChanges').show();
                          canShowActionButton(true);
                          showMessage("success", `<b>Account:</b> ${userName} <br/> <b>Role/Group:</b> ${entitlement} <br/> <b class="${newStatus}">${message}</b>`);
                      } else {
                          canShowActionButton(false);
                          jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                          showMessage("error", `Account: ${userName} Role/Group: ${entitlement} ${newStatus} action not done`);
                      }
                  },
                  error: function (errormsg) {
                      showMessage("error", `Error occured while saving`);
                      jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                  }
              });
          }
         

          function initializeOrReloadServiceAccountGrid() {
              activeTab = SERVICEACC;
              isServiceTabClicked = true;
              //Already initialized
              if (isServiceGridInitialized && isServiceGridInitialized === true) {
                  jQuery('#mdPleasewaitAfterSubmit').modal('show');
                  var dataSource = {
                      getRows: function (params) {
                          var pageSize = params.pageSize || 30;
                          var startRow = 0;
                          // Calculate page number based on startRow and page size
                          var pageNumber = Math.floor(startRow / pageSize) + 1;
                          //console.log("asking for " + startRow + " to " + params.endRow);
                          // Mock API call to fetch data
                          var selectedEndPoints = getAccountTypeSelectedEndPoints('#serviceAccountEndpoint');
                          var selectedEntitlements = getAccountTypeSelectedEntitlement('#serviceAccountEntitlement');
                          var searchObj = { taskId: taskId, draw: 1, start: params.startRow, length: params.endRow, search: { Value: selectedEntitlements, regex: false, EndpointIds: selectedEndPoints, AccountType: 2 } };

                          fetch(`${pageUrlDomain}/Services/AttestationDataService.svc/GetAttestationUserComparisionData`, {
                              method: 'post',
                              body: JSON.stringify(searchObj),
                              headers: { 'Content-Type': 'application/json; charset=utf-8' }
                          })
                              .then(httpResponse => httpResponse.json())
                              .then(function (data) {

                                  var jsonObjectArray = data.data
                                  //console.log(jsonObjectArray);
                                  params.successCallback(jsonObjectArray, data.recordsTotal);
                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                              })
                              .catch(function (error) {
                                  console.error('Error fetching data:', error);
                                  params.failCallback();
                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                              });
                      }
                  };
                  window.serviceGridApi.updateGridOptions({ datasource: dataSource })
              }
              else {

                  var selectedEndPoints = getAccountTypeSelectedEndPoints('#serviceAccountEndpoint');
                  //create grid
                  var data = { taskId: taskId, search: { value: "", regex: false, EndpointIds: selectedEndPoints, AccountType: 2 } };
                  jQuery.ajax({
                      type: "POST",
                      url: `${pageUrlDomain}/Services/AttestationDataService.svc/GetEmployeeInformation`,
                      contentType: "application/json; charset=utf-8",
                      dataType: "json",
                      data: JSON.stringify(data), // Adjust length as needed
                      success: function (data) {

                          const columnDefs = [
                              {
                                  headerName: "Application Name", field: "SystemName", sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value
                                      return params.value;
                                  }
                              },
                              {
                                  headerName: "Role/Group", field: "Entitlement", sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value

                                      return params.value;
                                  }
                              },
                              {
                                  headerName: "Description", field: "EntitlementDescription", sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value
                                      if (params && params.value && params.value.length > 0 && params.value.match(/^ *$/) === null) {
                                          return params.value;
                                      }
                                      return null;
                                  }
                              }
                          ];

                          var jsonObjectArray = data;
                          //Get EmployeeName
                          var uniqueEmployeeNo = [];
                          for (const jsonObjectArray of data) {
                              var empName = jsonObjectArray.Key.split('_');

                              uniqueUsernames = [];
                              var childrenAccount = [];
                              const usernameMap = new Map();
                              for (const item of jsonObjectArray.Value) {
                                  const lowercaseUsername = item.UserName.toUpperCase();
                                  if (!usernameMap.has(lowercaseUsername)) {
                                      usernameMap.set(lowercaseUsername, item.UserName); // Store original case for display
                                      uniqueUsernames.push(lowercaseUsername);
                                      childrenAccount.push({
                                          headerTooltip: lowercaseUsername,
                                          headerComponentParams: {
                                              template: columnNameTemplate
                                          },
                                          customData: jsonObjectArray.Value, parentHeader: false,
                                          headerName: lowercaseUsername, field: `${empName[0]}-${lowercaseUsername}`, cellRenderer: MissionResultRenderer, editable: false, sortable: false,
                                          valueGetter: function (params) {
                                              if (params && params.data) {

                                                  const employee = params.data.EmployeeNames.find(emp => emp.Value.UserName === params.colDef.headerName);
                                                  return JSON.stringify(employee);
                                              } return "";
                                          }
                                      })
                                  }
                              }
                              columnDefs.push({ headerName: empName[0], parentHeader: true, customData: jsonObjectArray.Value, headerTooltip: empName[0], field: "", children: childrenAccount, sortable: false, editable: false, headerClass: "center-aligned-header" });
                          }
                          serviceGridOptions = {
                              onColumnResized: idmsagGridColumnResized,
                              onColumnHeaderClicked: idmsagGridColumnClicked,
                              columnDefs: columnDefs,
                              pagination: true,
                              rowModelType: 'infinite',
                              defaultColDef: {
                                  flex: 1,
                                  minWidth: 130,
                                  editable: false,
                                  resizable: true,
                                  minHeight: 650,
                                  cellStyle: { borderRight: '1px solid rgb(217 221 223)' }
                              }, tooltipShowDelay: 0,
                              rowBuffer: 0,
                              getRowNodeId: function (data) {
                                  return data.id; // Ensure this matches the unique identifier in your row data
                              },
                              maxConcurrentDatasourceRequests: 1,
                              paginationPageSize: 30, // Number of rows to display per page
                              animateRows: true,
                              cacheBlockSize: 30, // Number of rows per request
                              infiniteInitialRowCount: 30,
                              maxBlocksInCache: 30,
                              // how many pages to store in cache. default is undefined, which allows an infinite sized cache,
                              // pages are never purged. this should be set for large data to stop your browser from getting
                              // full of data
                              maxBlocksInCache: 30,
                              onGridReady: function (params) {
                                  jQuery('#mdPleasewaitAfterSubmit').modal('show');
                                  var gridApi = params.api;

                                  // Callback function to initialize the grid
                                  var dataSource = {
                                      getRows: function (params) {
                                          var selectedEndPoints = getAccountTypeSelectedEndPoints('#serviceAccountEndpoint');
                                          //console.log("asking for " + params.startRow + " to " + params.endRow);
                                          // Mock API call to fetch data
                                          fetch(`${pageUrlDomain}/Services/AttestationDataService.svc/GetAttestationUserComparisionData`, {
                                              method: 'post',
                                              body: JSON.stringify({ taskId: taskId, draw: 1, start: params.startRow, length: params.endRow, search: { value: "", regex: false, endpointIds: [], AccountType: 2 } }),
                                              headers: { 'Content-Type': 'application/json; charset=utf-8' }
                                          })
                                              .then(httpResponse => httpResponse.json())
                                              .then(function (data) {
                                                  var jsonObjectArray = data.data
                                                  params.successCallback(jsonObjectArray, data.recordsTotal);
                                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                                              })
                                              .catch(function (error) {
                                                  console.error('Error fetching data:', error);
                                                  params.failCallback();
                                              });
                                      }
                                  };
                                  params.api.updateGridOptions({ datasource: dataSource });
                                  window.serviceGridApi = params.api;
                              }
                          };
                          jQuery(document).ready(function () {
                              // Assuming you have a div with id 'serviceAccountGrid' to render the grid
                              const gridDiv = document.querySelector('#serviceAccountGrid');
                              serviceAgGridApi = new agGrid.createGrid(gridDiv, serviceGridOptions);
                              isServiceGridInitialized = true;
                              loadbyAccountTypeEndpoint(taskId, 2, '#serviceAccountEndpoint');
                              loadAccountTypeEntitlement(taskId, 2, '#serviceAccountEntitlement', '#serviceAccountEndpoint');
                          });
                      }
                  });
              }
          }

          function initializeOrReloadSharedAccountGrid() {
              activeTab = SHAREDACC;
              isSharedTabClicked = true;
              //Already initialized
              if (isSharedGridInitialized && isSharedGridInitialized === true) {
                  jQuery('#mdPleasewaitAfterSubmit').modal('show');
                  var dataSource = {
                      getRows: function (params) {
                          var pageSize = params.pageSize || 30;
                          var startRow = 0;
                          // Calculate page number based on startRow and page size
                          var pageNumber = Math.floor(startRow / pageSize) + 1;
                          //console.log("asking for " + startRow + " to " + params.endRow);
                          // Mock API call to fetch data
                          var selectedEndPoints = getAccountTypeSelectedEndPoints('#sharedAccountEndpoint');
                          var selectedEntitlements = getAccountTypeSelectedEntitlement('#sharedAccountEntitlement');
                          var searchObj = { taskId: taskId, draw: 1, start: params.startRow, length: params.endRow, search: { Value: selectedEntitlements, regex: false, EndpointIds: selectedEndPoints, AccountType: 3 } };

                          fetch(`${pageUrlDomain}/Services/AttestationDataService.svc/GetAttestationUserComparisionData`, {
                              method: 'post',
                              body: JSON.stringify(searchObj),
                              headers: { 'Content-Type': 'application/json; charset=utf-8' }
                          })
                              .then(httpResponse => httpResponse.json())
                              .then(function (data) {

                                  var jsonObjectArray = data.data
                                  params.successCallback(jsonObjectArray, data.recordsTotal);
                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                              })
                              .catch(function (error) {
                                  console.error('Error fetching data:', error);
                                  params.failCallback();
                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                              });
                      }
                  };
                  window.sharedGridApi.updateGridOptions({ datasource: dataSource })
              }
              else {

                  var selectedEndPoints = getAccountTypeSelectedEndPoints('#sharedAccountEndpoint');
                  //create grid
                  var data = { taskId: taskId, search: { value: "", regex: false, EndpointIds: selectedEndPoints, AccountType: 3 } };
                  jQuery.ajax({
                      type: "POST",
                      url: `${pageUrlDomain}/Services/AttestationDataService.svc/GetEmployeeInformation`,
                      contentType: "application/json; charset=utf-8",
                      dataType: "json",
                      data: JSON.stringify(data), // Adjust length as needed
                      success: function (data) {

                          const columnDefs = [
                              {
                                  headerName: "Application Name", field: "SystemName", sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value
                                      return params.value;
                                  }
                              },
                              {
                                  headerName: "Role/Group", field: "Entitlement", sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value
                                      return params.value;
                                  }
                              },
                              {
                                  headerName: "Description", field: "EntitlementDescription", sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value
                                      if (params && params.value && params.value.length > 0 && params.value.match(/^ *$/) === null) {
                                          return params.value;
                                      }
                                      return null;
                                  }
                              }
                          ];

                          //Get EmployeeName
                          for (const jsonObjectArray of data) {
                              var empName = jsonObjectArray.Key.split('_');

                              uniqueUsernames = [];
                              var childrenAccount = [];
                              const usernameMap = new Map();
                              for (const item of jsonObjectArray.Value) {
                                  const lowercaseUsername = item.UserName.toUpperCase();
                                  if (!usernameMap.has(lowercaseUsername)) {
                                      usernameMap.set(lowercaseUsername, item.UserName); // Store original case for display
                                      uniqueUsernames.push(lowercaseUsername);
                                      childrenAccount.push({
                                          headerTooltip: lowercaseUsername,
                                          headerComponentParams: {
                                              template: columnNameTemplate
                                          },
                                          customData: jsonObjectArray.Value, parentHeader: false,
                                          headerName: lowercaseUsername, field: `${empName[0]}-${lowercaseUsername}`, cellRenderer: MissionResultRenderer, editable: false, sortable: false,
                                          valueGetter: function (params) {
                                              if (params && params.data) {

                                                  const employee = params.data.EmployeeNames.find(emp => emp.Value.UserName === params.colDef.headerName);
                                                  return JSON.stringify(employee);
                                              } return "";
                                          }
                                      })
                                  }
                              }
                              columnDefs.push({ headerName: empName[0], parentHeader: true, customData: jsonObjectArray.Value, headerTooltip: empName[0], field: "", children: childrenAccount, sortable: false, editable: false, headerClass: "center-aligned-header" });
                          }
                          sharedGridOptions = {
                              onColumnResized: idmsagGridColumnResized,
                              onColumnHeaderClicked: idmsagGridColumnClicked,
                              columnDefs: columnDefs,
                              pagination: true,
                              rowModelType: 'infinite',
                              defaultColDef: {
                                  flex: 1,
                                  minWidth: 130,
                                  editable: false,
                                  resizable: true,
                                  minHeight: 650,
                                  cellStyle: { borderRight: '1px solid rgb(217 221 223)' }
                              }, tooltipShowDelay: 0,
                              rowBuffer: 0,
                              getRowNodeId: function (data) {
                                  return data.id; // Ensure this matches the unique identifier in your row data
                              },
                              maxConcurrentDatasourceRequests: 1,
                              paginationPageSize: 30, // Number of rows to display per page
                              animateRows: true,
                              cacheBlockSize: 30, // Number of rows per request
                              infiniteInitialRowCount: 30,
                              maxBlocksInCache: 30,
                              onGridReady: function (params) {
                                  jQuery('#mdPleasewaitAfterSubmit').modal('show');
                                  var gridApi = params.api;

                                  // Callback function to initialize the grid
                                  var dataSource = {
                                      getRows: function (params) {
                                          var selectedEndPoints = getAccountTypeSelectedEndPoints('#sharedAccountEndpoint');
                                          //console.log("asking for " + params.startRow + " to " + params.endRow);
                                          // Mock API call to fetch data
                                          fetch(`${pageUrlDomain}/Services/AttestationDataService.svc/GetAttestationUserComparisionData`, {
                                              method: 'post',
                                              body: JSON.stringify({ taskId: taskId, draw: 1, start: params.startRow, length: params.endRow, search: { value: "", regex: false, endpointIds: [], AccountType: 3 } }),
                                              headers: { 'Content-Type': 'application/json; charset=utf-8' }
                                          })
                                              .then(httpResponse => httpResponse.json())
                                              .then(function (data) {
                                                  var jsonObjectArray = data.data
                                                  params.successCallback(jsonObjectArray, data.recordsTotal);
                                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                                              })
                                              .catch(function (error) {
                                                  console.error('Error fetching data:', error);
                                                  params.failCallback();
                                              });
                                      }
                                  };
                                  params.api.updateGridOptions({ datasource: dataSource });
                                  window.sharedGridApi = params.api;
                              }
                          };
                          jQuery(document).ready(function () {
                              // Assuming you have a div with id 'sharedAccountGrid' to render the grid
                              const gridDiv = document.querySelector('#sharedAccountGrid');
                              sharedAgGridApi = new agGrid.createGrid(gridDiv, sharedGridOptions);
                              isSharedGridInitialized = true;
                              loadbyAccountTypeEndpoint(taskId, 3, '#sharedAccountEndpoint');
                              loadAccountTypeEntitlement(taskId, 3, '#sharedAccountEntitlement', '#sharedAccountEndpoint');
                          });
                      }
                  });
              }
          }

          function initializeOrReloadSecondaryAccountGrid() {
              activeTab = SECONDARYACC;
              isSecondaryTabClicked = true;
              //Already initialized
              if (isSecondaryGridInitialized && isSecondaryGridInitialized === true) {
                  jQuery('#mdPleasewaitAfterSubmit').modal('show');
                  var dataSource = {
                      getRows: function (params) {
                          var pageSize = params.pageSize || 30;
                          var startRow = 0;
                          // Calculate page number based on startRow and page size
                          var pageNumber = Math.floor(startRow / pageSize) + 1;
                          //console.log("asking for " + startRow + " to " + params.endRow);
                          // Mock API call to fetch data
                          var selectedEndPoints = getAccountTypeSelectedEndPoints('#secondaryAccountEndpoint');
                          var selectedEntitlements = getAccountTypeSelectedEntitlement('#secondaryAccountEntitlement');
                          var searchObj = { taskId: taskId, draw: 1, start: params.startRow, length: params.endRow, search: { Value: selectedEntitlements, regex: false, EndpointIds: selectedEndPoints, AccountType: 4 } };

                          fetch(`${pageUrlDomain}/Services/AttestationDataService.svc/GetAttestationUserComparisionData`, {
                              method: 'post',
                              body: JSON.stringify(searchObj),
                              headers: { 'Content-Type': 'application/json; charset=utf-8' }
                          })
                              .then(httpResponse => httpResponse.json())
                              .then(function (data) {

                                  var jsonObjectArray = data.data
                                  params.successCallback(jsonObjectArray, data.recordsTotal);
                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                              })
                              .catch(function (error) {
                                  console.error('Error fetching data:', error);
                                  params.failCallback();
                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                              });
                      }
                  };
                  window.secondaryGridApi.updateGridOptions({ datasource: dataSource })
              }
              else {

                  var selectedEndPoints = getAccountTypeSelectedEndPoints('#secondaryAccountEndpoint');
                  //create grid
                  var data = { taskId: taskId, search: { value: "", regex: false, EndpointIds: selectedEndPoints, AccountType: 4 } };
                  jQuery.ajax({
                      type: "POST",
                      url: `${pageUrlDomain}/Services/AttestationDataService.svc/GetEmployeeInformation`,
                      contentType: "application/json; charset=utf-8",
                      dataType: "json",
                      data: JSON.stringify(data), // Adjust length as needed
                      success: function (data) {

                          const columnDefs = [
                              {
                                  headerName: "Application Name", field: "SystemName", sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value
                                      return params.value;
                                  }
                              },
                              {
                                  headerName: "Role/Group", field: "Entitlement", sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value
                                      return params.value;
                                  }
                              },
                              {
                                  headerName: "Description", field: "EntitlementDescription", sortable: false, editable: false, resizable: true, pinned: 'left', tooltipValueGetter: function (params) {
                                      // Return the tooltip text based on the cell value
                                      if (params && params.value && params.value.length > 0 && params.value.match(/^ *$/) === null) {
                                          return params.value;
                                      }
                                      return null;
                                  }
                              }
                          ];

                          var jsonObjectArray = data;
                          //Get EmployeeName
                          var uniqueEmployeeNo = [];
                          for (const jsonObjectArray of data) {
                              var empName = jsonObjectArray.Key.split('_');

                              uniqueUsernames = [];
                              var childrenAccount = [];
                              const usernameMap = new Map();
                              for (const item of jsonObjectArray.Value) {
                                  const lowercaseUsername = item.UserName.toUpperCase();
                                  if (!usernameMap.has(lowercaseUsername)) {
                                      usernameMap.set(lowercaseUsername, item.UserName); // Store original case for display
                                      uniqueUsernames.push(lowercaseUsername);
                                      childrenAccount.push({
                                          headerTooltip: lowercaseUsername,
                                          headerComponentParams: {
                                              template: columnNameTemplate
                                          },
                                          customData: jsonObjectArray.Value, parentHeader: false,
                                          headerName: lowercaseUsername, field: `${empName[0]}-${lowercaseUsername}`, cellRenderer: MissionResultRenderer, editable: false, sortable: false,
                                          valueGetter: function (params) {
                                              if (params && params.data) {

                                                  const employee = params.data.EmployeeNames.find(emp => emp.Value.UserName === params.colDef.headerName);
                                                  return JSON.stringify(employee);
                                              } return "";
                                          }
                                      })
                                  }
                              }
                              columnDefs.push({ headerName: empName[0], parentHeader: true, customData: jsonObjectArray.Value, headerTooltip: empName[0], field: "", children: childrenAccount, sortable: false, editable: false, headerClass: "center-aligned-header" });
                          }
                          secondaryGridOptions = {
                              onColumnResized: idmsagGridColumnResized,
                              onColumnHeaderClicked: idmsagGridColumnClicked,
                              columnDefs: columnDefs,
                              pagination: true,
                              rowModelType: 'infinite',
                              defaultColDef: {
                                  flex: 1,
                                  minWidth: 130,
                                  editable: false,
                                  resizable: true,
                                  minHeight: 650,
                                  cellStyle: { borderRight: '1px solid rgb(217 221 223)' }
                              },
                              tooltipShowDelay: 0,
                              rowBuffer: 0,
                              getRowNodeId: function (data) {
                                  return data.id; // Ensure this matches the unique identifier in your row data
                              },
                              maxConcurrentDatasourceRequests: 1,
                              paginationPageSize: 30, // Number of rows to display per page
                              animateRows: true,
                              cacheBlockSize: 30, // Number of rows per request
                              infiniteInitialRowCount: 30,
                              maxBlocksInCache: 30,
                              onGridReady: function (params) {
                                  jQuery('#mdPleasewaitAfterSubmit').modal('show');
                                  var gridApi = params.api;

                                  // Callback function to initialize the grid
                                  var dataSource = {
                                      getRows: function (params) {
                                          var selectedEndPoints = getAccountTypeSelectedEndPoints('#secondaryAccountEndpoint');
                                          //console.log("asking for " + params.startRow + " to " + params.endRow);
                                          // Mock API call to fetch data
                                          fetch(`${pageUrlDomain}/Services/AttestationDataService.svc/GetAttestationUserComparisionData`, {
                                              method: 'post',
                                              body: JSON.stringify({ taskId: taskId, draw: 1, start: params.startRow, length: params.endRow, search: { value: "", regex: false, endpointIds: [], AccountType: 4 } }),
                                              headers: { 'Content-Type': 'application/json; charset=utf-8' }
                                          })
                                              .then(httpResponse => httpResponse.json())
                                              .then(function (data) {
                                                  var jsonObjectArray = data.data
                                                  params.successCallback(jsonObjectArray, data.recordsTotal);
                                                  jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                                              })
                                              .catch(function (error) {
                                                  console.error('Error fetching data:', error);
                                                  params.failCallback();
                                              });
                                      }
                                  };
                                  params.api.updateGridOptions({ datasource: dataSource });
                                  window.secondaryGridApi = params.api;
                              }
                          };
                          jQuery(document).ready(function () {
                              // Assuming you have a div with id 'serviceAccountGrid' to render the grid
                              const gridDiv = document.querySelector('#secondaryAccountGrid');
                              secondaryAgGridApi = new agGrid.createGrid(gridDiv, secondaryGridOptions);
                              isSecondaryGridInitialized = true;
                              loadbyAccountTypeEndpoint(taskId, 4, '#secondaryAccountEndpoint');
                              loadAccountTypeEntitlement(taskId, 4, '#secondaryAccountEntitlement', '#secondaryAccountEndpoint');
                          });
                      }
                  });
              }
          }

          function showTaskInfo() {

              if (isTaskInfoOpened === false) {

                  try {

                      hdnUsername = document.getElementById('<%= hdnUsername.ClientID %>').value;
                      var jsonObject = {
                          "taskId": taskId,
                          "userName": hdnUsername
                      };
                      jQuery.ajax({
                          type: "POST",
                          url: `${pageUrlDomain}/Services/AttestationDataService.svc/GetAttestationTaskInformation`,
                          contentType: "application/json; charset=utf-8",
                          dataType: "json",
                          data: JSON.stringify(jsonObject),
                          success: function (data) {


                              if (data && data.GetAttestationTaskInformationResult) {
                                  var objData = data.GetAttestationTaskInformationResult;
                                  jQuery("#lblTaskId").html(taskId);
                                  jQuery("#lblAttName").html(objData.AttestationName);
                                  jQuery("#lblAttestationDesc").html(objData.AttestationDescription??"");
                                  jQuery("#lblType").html(objData.AttestationType);
                                  jQuery("#lblCampaignName").html(objData.CampaignName);
                                  jQuery("#lblReqDate").html(objData.RequestedDate);
                                  jQuery("#lblCurrentReviewer").html(objData.Approver);
                                  jQuery("#lblCurrentLevel").html(objData.Level);
                                  jQuery("#lblSubmitBefore").html(objData.SubmittedDate);
                                  jQuery("#lblStatus").html(objData.Status);
                                  jQuery("#mdwindowTaskInfo").modal("show");
                                  isTaskInfoOpened = true;
                              } else {
                                  showMessage("error", "Error occured while geting task information");
                                  jQuery("#mdwindowTaskInfo").modal("hide");
                              }
                          },
                          error: function (errormsg) {
                              showMessage("error", "Error occured while saving22222");
                              jQuery("#mdwindowTaskInfo").modal("hide");
                          }
                      });
                  } catch (e) {
                      console.log(e);
                      jQuery("#mdwindowTaskInfo").modal("hide");
                      showMessage("error", "Error occured while saving");

                  }
              } else {
                  jQuery("#mdwindowTaskInfo").modal("show");
              }
          }
        function cancelRevert() {
            jQuery("#btnRevert").show();
        }
          function revertChanges() {
              try {

                  hdnUsername = document.getElementById('<%= hdnUsername.ClientID %>').value;
                  var jsonObject = {
                      "taskId": taskId,
                      "userName": hdnUsername
                  };
                  jQuery.ajax({
                      type: "POST",
                      url: `${pageUrlDomain}/Services/AttestationDataService.svc/RevertTaskInInterimStore`,
                      contentType: "application/json; charset=utf-8",
                      dataType: "json",
                      data: JSON.stringify(jsonObject),
                      success: function (data) {
                          if (data && data.RevertTaskInInterimStoreResult === true) {
                              //toggleAlert('Success',`${entitlement} ${accessType} successfully`);
                              jQuery('#unSavedChanges').hide();
                              showMessage("success", "changes reverted successfully");
                              canShowActionButton(false);
                              refreshTab();
                          } else if (data && data.SavedChangesResult === false) {
                              jQuery('#unSavedChanges').show();
                              showMessage("error", "Error occured while saving");
                              canShowActionButton(true);
                          }
                          jQuery("#mdwindowRevert").modal("hide");
                      },
                      error: function (errormsg) {
                          showMessage("error", "Error occured while saving");
                          jQuery("#mdwindowRevert").modal("hide");
                          canShowActionButton(true);
                      }
                  });
              } catch (e) {
                  console.log(e);
                  jQuery("#mdwindowRevert").modal("hide");
                  showMessage("error", "Error occured while saving");

              }
          }
          function accountTypes() {

              var jsonObject = {
                  "taskId": taskId
              };
              jQuery.ajax({
                  type: "POST",
                  url: `${pageUrlDomain}/Services/AttestationDataService.svc/GetAccountTypes`,
                  contentType: "application/json; charset=utf-8",
                  dataType: "json",
                  data: JSON.stringify(jsonObject),
                  success: function (result) {
                      if (result && result.GetAccountTypesResult && result.GetAccountTypesResult.length > 0) {
                          var accTypes = result.GetAccountTypesResult;
                          // Replace this with your logic to check if data exists for each tab
                          var userAccountDataExists = false; // Example: true if data exists, false otherwise
                          var serviceAccountDataExists = false; // Example: true if data exists, false otherwise
                          var sharedAccountDataExists = false; // Example: true if data exists, false otherwise
                          var secondaryAccountDataExists = false; // Example: true if data exists, false otherwise



                          for (var i = 0; i < accTypes.length; i++) {
                              if (accTypes[i] === 1) {
                                  userAccountDataExists = true;
                              } else if (accTypes[i] === 2) {
                                  serviceAccountDataExists = true;
                              } else if (accTypes[i] === 3) {
                                  sharedAccountDataExists = true;
                              } else if (accTypes[i] === 4) {
                                  secondaryAccountDataExists = true;
                              }
                          }
                          jQuery(".nav-tabs li").hide();
                          jQuery("#actionButtonData").show();
                          // Show the tabs based on the response
                          if (serviceAccountDataExists === true) {
                              jQuery("#tbLinkServiceAccount").show();
                          }
                          if (sharedAccountDataExists === true) {
                              jQuery("#tbLinkSharedAccount").show();
                          }
                          if (userAccountDataExists === true) {
                              jQuery("#tbLinkUserAccount").show();
                          }
                          if (secondaryAccountDataExists === true) {
                              jQuery("#tbLinkSecondaryAccount").show();
                          }

                          jQuery(".nav-tabs li:visible:first a").tab('show');

                      }
                  },
                  error: function (xhr, status, error) {
                      console.log(error);
                      reject(error);
                  }
              });
          }

          function saveAndSubmit() {
              if (isAllTabClicked()) {
                  jQuery('#mdwindowSaveandSubmit').modal("show");
              }
          }
          function isAllTabClicked() {
              var isVisibleUAC = jQuery('#tbLinkUserAccount').is(':visible');
              var isVisibleSERAC = jQuery('#tbLinkServiceAccount').is(':visible');
              var isVisibleSHDAC = jQuery('#tbLinkSharedAccount').is(':visible');
              var isVisibleSECAC = jQuery('#tbLinkSecondaryAccount').is(':visible');

              var showpop = false;

              if ((isVisibleUAC && !isUserTabClicked)) {
                  jQuery("#liUsrAc").show();
                  showpop = true;
              } else {
                  jQuery("#liUsrAc").hide();
              }
              if (isVisibleSERAC && !isServiceTabClicked) {
                  jQuery("#liSvcAc").show();
                  showpop = true;
              } else {
                  jQuery("#liSvcAc").hide();
              }
              if (isVisibleSHDAC && !isSharedTabClicked) {
                  jQuery("#liShdAc").show();
                  showpop = true;
              } else {
                  jQuery("#liShdAc").hide();
              }
              if (isVisibleSECAC && !isSecondaryTabClicked) {
                  jQuery("#liSecAc").show();
                  showpop = true;
              } else {
                  jQuery("#liSecAc").hide();
              }
              if (showpop) {
                  jQuery('#mdwindowPendingReview').modal("show");
              } else { return true; }


          }
          function redirectreview() {
              var redirectUrl = `${pageUrlDomain}/Home/Home.aspx`; //or grab it by tagname etc
              document.getElementById("btnAttestationComparision").href = redirectUrl;
              document.getElementById("btnAttestationComparision").click()
          }
          function openPopupWithCustomData(colData, accountType) {

              let dataSource = null;
              let accountName = null;
              let userId = 0;
              if (colData.colGroupDef) {
                  dataSource = colData.colGroupDef;
              }
              else if (colData.colDef) {
                  dataSource = colData.colDef;
              }
              if (dataSource && dataSource.customData) {
                  let empName = "";
                  var data = dataSource.customData[0];
                  userId = data.Id;

                  let selectedAccounts = [];
                  if (dataSource.parentHeader === false) {
                      accountName = dataSource.headerName;
                      //selectedAccounts = data.filter(i => i.UserName.toUpperCase() === dataSource.headerName);
                      //empName = `${data[0].Name} - ${data[0].AdditionalInfo}`;
                  } else if (dataSource.parentHeader === true) {
                      //selectedAccounts = dataSource.customData;
                      //empName = `${data[0].Name} - ${data[0].AdditionalInfo}`;
                  }

                  //selectedAccounts.sort(function (a, b) {
                  //    return a.EndpointName.localeCompare(b.EndpointName);
                  //});

                  jQuery("#lblEmployeeName").text(empName);

                  var data = { taskId: taskId, userId: userId, search: { Value: accountName, regex: false, EndpointIds: [], AccountType: accountType } };

                  jQuery.ajax({
                      type: "POST",
                      url: `${pageUrlDomain}/Services/AttestationDataService.svc/GetAccountStatusInformation`,
                      contentType: "application/json; charset=utf-8",
                      dataType: "json",
                      data: JSON.stringify(data), // Adjust length as needed
                      success: function (data) {
                          setAccountTypeIcon();
                          jQuery("#lblAccountCategory").text(getAccountType());
                          jQuery("#lblAccountType").text(accountType);
                          accountGridOptions.api.setRowData(data);
                          accountGridOptions.api.sizeColumnsToFit();
                          jQuery("#lblEmployeeName").text(`${data[0].Name} - ${data[0].AdditionalInfo}`);
                          jQuery("#mdwindowAccounEndpoint").modal("show");

                      },
                      error: function (errormsg) {
                          showMessage("error", `Error occured while saving`);
                      }
                  });
              }
        }
        function downloadReport() {
            jQuery('#mdPleasewaitAfterSubmit').modal('show');
            var data = { taskId: taskId };
            jQuery.ajax({
                type: "POST",
                url: `${pageUrlDomain}/Services/AttestationDataService.svc/DownloadUserComparisionData`,
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                responseType: "arraybuffer",
                success: function (response) {
                    if (response) {
                        var campaignName = document.getElementById('<%= hdnCampaignName.ClientID %>').value;
                        // Handle the response
                        var excelData = response;
                        // Use Blob to create a binary large object and initiate a file download
                        var uint8Array = new Uint8Array(excelData); // Convert the Uint8Array to a Blobvar
                        var blob = new Blob([uint8Array]); // Use FileSaver.js to save the Blob as a 
                        saveAs(blob, `${campaignName}.xlsx`);
                        jQuery('#mdPleasewaitAfterSubmit').modal('hide');
                    }

                },
                error: function (msg) {
                    console.log(error);
                    alert("There's some error in Download." + pageUrlDomain + msg.errormsg);
                }
            });
            return false;
        }
          jQuery(document).ready(function () {
              jQuery.sessionTimeout({
                  keepAliveUrl: `${pageUrlDomain}/Admin/Attestation/ResetSession.aspx`,
                  logoutUrl: `${pageUrlDomain}/login.aspx`,
                  redirUrl: `${pageUrlDomain}/login.aspx`,
                  warnAfter: ((sessionTimeout - 1) * 60 * 1000),
                  redirAfter: sessionTimeout * 60 * 1000,
                  countdownBar: true,
                  countdownSmart: true,
                  ignoreUserActivity: false
              });

              var val = document.getElementById('<%= hdnBackUrl.ClientID %>').value;
			  document.getElementById("entitlementNotesSrv").innerHTML=document.getElementById('<%= entitlementNotes.ClientID %>').innerHTML;
			  document.getElementById("entitlementNotesShd").innerHTML=document.getElementById('<%= entitlementNotes.ClientID %>').innerHTML;
			  document.getElementById("entitlementNotesSec").innerHTML=document.getElementById('<%= entitlementNotes.ClientID %>').innerHTML;
            var a = document.getElementById('btnAttestationComparision'); //or grab it by tagname etc
            a.href = val;
            /*
             * Initialise the account grid for account revoke
             */
            new agGrid.Grid(document.querySelector('#accountGrid'), accountGridOptions);
              taskId = document.getElementById('<%= hdnTaskId.ClientID %>').value;
              isEditable = document.getElementById('<%= hdnIsEditable.ClientID %>').value;
            actionUserId = document.getElementById('<%= hdnActionUserId.ClientID %>').value;
            hdnUsername = document.getElementById('<%= hdnUsername.ClientID %>').value;
              //For Notification
              toastr.options = {
                  "closeButton": true,
                  "debug": false,
                  "newestOnTop": false,
                  "progressBar": false,
                  "positionClass": "toast-top-right",
                  "preventDuplicates": false,
                  "onclick": null,
                  "showDuration": "200",
                  "hideDuration": "1000",
                  "timeOut": "2000",
                  "extendedTimeOut": "1000",
                  "showEasing": "swing",
                  "hideEasing": "linear",
                  "showMethod": "fadeIn",
                  "hideMethod": "fadeOut"

              };


              // Load data when a tab is shown
              jQuery('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                  var targetTabId = jQuery(e.target).attr("href");
                  loadData(targetTabId);

              });

              // Example function to load data for a tab
              function loadData(tabId) {
                  // Replace this with your logic to load data for each tab
                  switch (tabId) {
                      case "#userAccountTab":
                          initializeOrReloadUserAccountGrid();
                          break;
                      case "#serviceAccountTab":
                          initializeOrReloadServiceAccountGrid();
                          break;
                      case "#sharedAccountTab":
                          initializeOrReloadSharedAccountGrid();
                          break;
                      case "#secondaryAccountTab":
                          initializeOrReloadSecondaryAccountGrid();
                          break;
                  }
              }



              accountTypes();
              showunsavedChanges();

              loadbyAccountTypeEndpoint(taskId, 1, '#userAccountEndpoint');
              //initializeOrReloadUserAccountGrid();


              jQuery('#btnConfirmSubmit').on('click', function () {
                  var jsonObject = {
                      "taskId": taskId,
                      "userName": hdnUsername,
                      "isSaved": false
                  };

                  jQuery('#btnConfirmSubmit').prop('disabled', true);
                  // Make AJAX call
                  jQuery.ajax({
                      type: "POST",
                      url: `${pageUrlDomain}/Services/AttestationDataService.svc/SubmitAttestationTask`,
                      contentType: "application/json; charset=utf-8",
                      dataType: "json",
                      data: JSON.stringify(jsonObject),
                      success: function (response) {
                          jQuery('#mdwindowSaveandSubmit').modal('hide');
                          jQuery('#btnConfirmSubmit').prop('disabled', true);
                          if (response && response.SubmitAttestationTaskResult === true) {
                              jQuery('#mdPleasewaitAfterSubmit').modal('show');
                              setTimeout(redirectreview, 7000);
                          } else {
                              showMessage("error", "Action not completed. Please contact administrator for more information.");
                          }
                      },
                      error: function (xhr, status, error) {
                          jQuery('#btnConfirmSubmit').prop('disabled', false);
                          showMessage("error", "Action not completed. Please contact administrator for more information.");
                          console.error(xhr.responseText);
                      }

                  });
              });

              jQuery('#userAccountEndpoint').on('change', function () {
                  initializeOrReloadUserAccountGrid();
                  loadAccountTypeEntitlement(taskId, 1, '#userAccountEntitlement', '#userAccountEndpoint')
              });
              jQuery('#userAccountEntitlement').on('change', function () {
                  initializeOrReloadUserAccountGrid();
              });

              jQuery('#serviceAccountEndpoint').on('change', function () {
                  initializeOrReloadServiceAccountGrid();
                  loadAccountTypeEntitlement(taskId, 2, '#serviceAccountEntitlement', '#serviceAccountEndpoint')
              });
              jQuery('#serviceAccountEntitlement').on('change', function () {
                  initializeOrReloadServiceAccountGrid();
              });

              jQuery('#sharedAccountEndpoint').on('change', function () {
                  initializeOrReloadSharedAccountGrid();
                  loadAccountTypeEntitlement(taskId, 3, '#sharedAccountEntitlement', '#sharedAccountEndpoint')
              });
              jQuery('#sharedAccountEntitlement').on('change', function () {
                  initializeOrReloadSharedAccountGrid();
              });

              jQuery('#secondaryAccountEndpoint').on('change', function () {
                  initializeOrReloadSecondaryAccountGrid();
                  loadAccountTypeEntitlement(taskId, 4, '#secondaryAccountEntitlement', '#secondaryAccountEndpoint')
              });
              jQuery('#secondaryAccountEntitlement').on('change', function () {
                  initializeOrReloadSecondaryAccountGrid();
              });
          });
    </script>


    <script type="text/javascript" src="<%=Page.ResolveClientUrl("~/assets/agGrid/ag-grid-community.min.js")%>"></script>
</asp:Content>

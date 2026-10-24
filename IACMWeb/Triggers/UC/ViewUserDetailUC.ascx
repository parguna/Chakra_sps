<%@ Control Language="C#" AutoEventWireup="true" Inherits="DI.Chakra.Web.Triggers.UC.ViewUserDetailUC" CodeBehind="ViewUserDetailUC.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../../UsrControl/Common/OrgDetailUC.ascx" TagName="OrgDetailUC" TagPrefix="uc1" %>
<style>
    .label-margin {
        margin-bottom: 15px !important;
    }
</style>
<div class="modal-header">
    <h3 class="modal-title" id="H1">Detail</h3>
</div>
<div class="modal-body" >
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">

            <div class="col-xs-6">
                <label class="label-margin">User Current Details:</label>

                <div>
                    <div class="form-group">

                        <uc1:OrgDetailUC ID="OrgDetailUCNew" runat="server" />

                    </div>

                </div>
            </div>
            <div class="col-xs-6">
                <label class="label-margin">User Old Details:</label>

                <div>
                    <div class="form-group">

                        <uc1:OrgDetailUC ID="OrgDetailUCCurrent" runat="server" />

                    </div>
                </div>
            </div>
        </div>
    </div>
<div class="modal-footer">
    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="Cancel" OnClientClick="return HideModalPopup();" />
</div>

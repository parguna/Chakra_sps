<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnterprisePasswordPolicy.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.EnterprisePasswordPolicy" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/UsrControl/Common/PasswordPolicy.ascx" TagPrefix="uc1" TagName="PasswordPolicy" %>

<div class="modal-header">
    <h3 class="modal-title" id="myModalLabel" runat="server">Policies     
    </h3>
</div>

<div class="modal-body">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <uc1:PasswordPolicy runat="server" ID="PasswordPolicy1" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>



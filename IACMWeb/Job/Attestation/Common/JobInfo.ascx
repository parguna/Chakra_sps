<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JobInfo.ascx.cs" Inherits="DI.Chakra.Web.Job.Attestation.Common.JobInfo" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">

                    <div class="form-group">
                        <asp:Label ID="processname" AssociatedControlID="txtProcessName" runat="server" Text="Process Name" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtProcessName" CssClass="form-control" runat="server" required="required" data-errMessage="Process Name is required"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="description" AssociatedControlID="txtDescription" runat="server" Text="Description" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtDescription" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label3" AssociatedControlID="ddlAttestationProcess" runat="server" Text="Attestation Name" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:DropDownList ID="ddlAttestationProcess" DataTextField="AttestationName" DataValueField="Id" runat="server" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
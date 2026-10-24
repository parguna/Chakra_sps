<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.ProvRS.ProvisioningFields.UC.DetailUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="form">
    <div class="form-horizontal left">
        <div class="row-fluid clearfix">
            <div class="col-xs-12">
                <div class="form-group">
                    <asp:Label ID="lblFieldName" AssociatedControlID="txtFieldName"  runat="server" Text="Field Name" class="col-xs-2 required control-label"></asp:Label>
                    <div class="col-xs-4">
                        <asp:TextBox ID="txtFieldName" class="form-control" runat="server" ></asp:TextBox>
                    </div>
                    <asp:Label ID="Label5" AssociatedControlID="txtDescription" runat="server" Text="Description" class="col-xs-2 control-label"></asp:Label>
                    <div class="col-xs-4">
                        <asp:TextBox ID="txtDescription" class="form-control" Rows="2" TextMode="MultiLine" runat="server"></asp:TextBox>

                    </div>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblAccountType" AssociatedControlID="ddlAccounType" required="required" runat="server" Text="Account Type" class="col-xs-2 control-label"></asp:Label>
                    <div class="col-xs-4">
                        <asp:DropDownList ID="ddlAccounType" runat="server" Rows="3" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlAccountType_SelectedIndexChanged" data-width="100%" AutoPostBack="True">
                        </asp:DropDownList>
                    </div>
               <%-- </div>
                <div class="form-group">--%>
                    
                    <div id="divValue" runat="server">
                        <asp:Label ID="lblValue" AssociatedControlID="txtValue" runat="server" Text="Value" class="col-xs-2 control-label"></asp:Label>
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtValue" class="form-control" Rows="3" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    
                </div>
                <div class="form-group clearfix">
                    <div class="col-xs-2">
                    </div>
                    <div class="col-xs-4"  style="margin-left: 10px;">>
                        <asp:Label ID="Label3" CssClass="form-group" runat="server" AssociatedControlID="chkIsActive">
                            <asp:CheckBox runat="server" ID="chkIsActive" Checked="true" />
                            <span class="lbl">&nbsp;&nbsp;IsActive</span>
                        </asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

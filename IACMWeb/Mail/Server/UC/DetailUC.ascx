<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Mail.Server.UC.DetailUC" %>

<asp:Panel runat="server" ID="ContentWrapper" CssClass="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <asp:Label ID="lblEmailId" AssociatedControlID="txtEmailId" runat="server" Text="Email ID" CssClass="col-xs-4 control-label"></asp:Label>
	                    <div class="col-xs-7">
		                    <asp:TextBox ID="txtEmailId" CssClass="form-control" rows="3" runat="server" required="required"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblUserName" AssociatedControlID="txtUserName" runat="server" Text="User Name" CssClass="col-xs-4 control-label"></asp:Label>
	                    <div class="col-xs-7">
		                    <asp:TextBox ID="txtUserName" CssClass="form-control" rows="3" runat="server"></asp:TextBox>
	                    </div>
                    </div>   
                    <div class="form-group">
                        <asp:Label ID="lblPassword" AssociatedControlID="txtPassword" runat="server" Text="Password" CssClass="col-xs-4 control-label"></asp:Label>
	                    <div class="col-xs-7">
		                    <asp:TextBox ID="txtPassword" CssClass="form-control" rows="3" runat="server" TextMode="Password"></asp:TextBox>
	                    </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblHostName" AssociatedControlID="txtHostName" runat="server" Text="Host Name" CssClass="col-xs-4 control-label"></asp:Label>
	                    <div class="col-xs-7">
		                    <asp:TextBox ID="txtHostName" CssClass="form-control" rows="3" runat="server" required="required"></asp:TextBox>
	                    </div>
                   </div>
                    <div class="form-group">
                        <asp:Label ID="lblPortNo" AssociatedControlID="txtPortNo" runat="server" Text="Port Number" CssClass="col-xs-4 control-label"></asp:Label>
	                    <div class="col-xs-7">
		                    <asp:TextBox ID="txtPortNo" CssClass="form-control" rows="3" runat="server" pattern="[0-9]{2,4}" title="valid numeric" required="required"></asp:TextBox>
	                    </div>
                    </div>      
                    <div class="form-group">     
                        <div class="checkbox">
                            <label class="col-xs-4 control-label">Enable SSL</label>
                            <div class="col-xs-1">
                                <asp:Label ID="Label5" AssociatedControlID="chkEnableSSL" runat="server">
                                    <asp:CheckBox ID="chkEnableSSL" runat="server"/> 
                                    <span class="lbl"></span> 
                                </asp:Label> 
                            </div> 
                        </div>    
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>

<div class="buttons text-right nav-rectangle">
    <asp:Button ID="Cancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="Cancel_Click" UseSubmitBehavior="false"/>
</div>
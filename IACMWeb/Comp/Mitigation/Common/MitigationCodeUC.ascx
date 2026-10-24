<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MitigationCodeUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Mitigation.Common.MitigationCodeUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <asp:Label ID="MitigationCode" AssociatedControlID="txtMitigationCode" runat="server" Text="Code" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox id="txtMitigationCode" runat="server" CssClass="form-control" required="required" data-errMessage="Mitigation Code is required" MaxLength="30" onKeyPress="return AllowLetterNumber(event);" ></asp:TextBox>                            
                        </div>
                    </div>
                    <div class="form-group">
                    <asp:Label ID="MitigationDesc" AssociatedControlID="txtMitigationDesc" runat="server" Text="Description" CssClass="col-xs-4 control-label"></asp:Label>
                    <div class="col-xs-7">
                        <asp:TextBox id="txtMitigationDesc" runat="server" CssClass="form-control" ></asp:TextBox>
                    </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Steps" AssociatedControlID="txtSteps" runat="server" Text="Steps" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                              <asp:TextBox ID="txtSteps" TextMode="MultiLine" runat="server" required="required" CssClass="form-control" Rows="12" Width="305px"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">

                        <asp:Label ID="Status" runat="server" Text="Status" AssociatedControlID="acInc" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7" id="Checkboxes">
                            <label id="acInc" runat="server" CssClass="col-xs-4 control-label"  AssociatedControlID="radActive">
                            <asp:RadioButton  ID="radActive" runat="server" AutoPostBack="true" OnCheckedChanged="radActive_CheckedChanged" Checked="True" />
                                <asp:Label CssClass="lbl" ID="acLbl1" runat="server" Text="Active"></asp:Label>
                            <asp:RadioButton Checked="false" ID="radClosed" runat="server" AutoPostBack="true" OnCheckedChanged="radClosed_CheckedChanged" />
                                <asp:Label CssClass="lbl" ID="acLbl2" runat="server" Text="Inactive"></asp:Label>
                            </label>
                            </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="DocName" AssociatedControlID="DocumentName" runat="server" Text="Document Name" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="DocumentName" runat="server" CssClass="form-control" required="required" data-errMessage="Document Name is required"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="DocPath" AssociatedControlID="FileUpload" runat="server" Text="Document Path" CssClass="col-xs-4 control-label"></asp:Label>
                       <div class="col-xs-7">
                           <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                               <ContentTemplate>
                                   <asp:FileUpload ID="FileUpload" runat="server"/>
                              </ContentTemplate>  
                           </asp:UpdatePanel>
                           
                        </div>
                        </div>
                        
                    
                </div>
            </div>
        </div>
    </div>
</div>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComplianceinfoUC.ascx.cs" Inherits="DI.Chakra.Web.Job.Compliance.Common.ComplianceinfoUC" %>
<%@ Register Src="~/UsrControl/Common/GenericAutoCompleteBox.ascx" TagPrefix="uc1" TagName="GenericAutoCompleteBox" %>

<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                   
                    <div class="form-group">
                        <asp:Label ID="processname" AssociatedControlID="txtProcessName" runat="server" Text="Process Name" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtProcessName" CssClass="form-control" runat="server" required="required" data-errMessage="Process Name is required" ></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="description" AssociatedControlID="txtDescription" runat="server" Text="Description" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtDescription" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group required">
                          <asp:Label ID="lblObject" AssociatedControlID="PolicyDefinitionAutoCompleteBox" runat="server" CssClass="col-xs-4 control-label" Text ="Select Policy"></asp:Label>    
                        <div class="col-xs-7">
                            <uc1:GenericAutoCompleteBox runat="server" ID="PolicyDefinitionAutoCompleteBox" ServiceMethodName ="GetSODPolicyDefinitions" ServicePath ="../../../Services/AutoCompleteDataService.svc"/>
                            </div> 
                        
                    </div>
                </div>
               
                </div>
            </div>
        </div>
    </div>
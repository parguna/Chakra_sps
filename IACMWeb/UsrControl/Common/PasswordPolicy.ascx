<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PasswordPolicy.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.PasswordPolicy" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<div class="form-group"><h4>Password Policy</h4></div>                   
<asp:Repeater ID="Repeater1" runat="server" >                        
    <ItemTemplate> 
        <div class="form-group">
            <label class="col-xs-6" style="font-weight: normal">
                <%#DataBinder.Eval(Container.DataItem, "name") %>:
            </label>        
            <div class="col-xs-6" style="word-wrap: break-word">
                <label id="PolicyName" runat="server" style="font-weight: normal">
                    <%#DataBinder.Eval(Container.DataItem, "value") %>
                </label>  
            </div>
        </div>                            
    </ItemTemplate>                           
</asp:Repeater>
<%@ Page Title="My Notifications" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Notification.List" %>
<%@ Register Src="~/Notification/UC/ListUC.ascx" TagPrefix="uc1" TagName="ListUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
       <asp:UpdatePanel ID="UpdatePanel2" runat="server">
      <ContentTemplate>
    <uc1:ListUC runat="server" id="ListUC" /> 
            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" ID="Button1" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>          
             
            </div>   
          </ContentTemplate>    
               </asp:UpdatePanel>
</asp:Content>





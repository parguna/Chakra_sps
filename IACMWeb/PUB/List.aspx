<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.PUB.List" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/PUB/UC/ListUC.ascx" TagPrefix="uc1" TagName="ListUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server"> 
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                   
                             <uc1:ListUC runat="server" id="ListUC" />
                            
                            <div class="buttons text-right nav-rectangle">
                                <asp:Button runat="server" ID="RefreshButton" CssClass="btn btn-default btn-sm" Text="Refresh" OnClick="RefreshButton_Click"></asp:Button>
                                <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
                            </div>

                   
                </div>
            </div>
        </div>
    </div> 
</asp:Content>

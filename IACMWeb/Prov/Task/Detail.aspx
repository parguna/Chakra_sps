<%@ Page Title="Task" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Prov.Task.Detail" %>

<%@ Register Src="~/UsrControl/Common/ActionTaskUC.ascx" TagPrefix="uc1" TagName="ActionTask" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="content-wrapper">
                <div class="form">
                    <div class="form-horizontal left">
                        <div class="row-fluid">
                            <uc1:ActionTask runat="server" ID="ActionTask" IsAdminMode="Yes" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


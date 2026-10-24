<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NoteInfoUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.NoteInfoUC" %>

<asp:UpdatePanel ID="UpdatePanel2" runat="server">
    <ContentTemplate>
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="col-xs-12" runat="server" id="userInfoMainDiv">
                    <div class="form-group">
                        <div class="table-header mTop15">
                            <h4><%= T("App.Request.Info") %></h4>
                        </div>
                        <div class="col-xs-12" runat="server" id="userInfoDiv">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

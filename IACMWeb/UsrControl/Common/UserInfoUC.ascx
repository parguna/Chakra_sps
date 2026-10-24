<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserInfoUC.ascx.cs" Inherits="DI.Chakra.Web.AccessRequest.Common.UserInfoUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/NUserDetailUC.ascx" TagPrefix="uc1" TagName="UserDetailUC" %>
<script>
    function Show() {
    }
        </script>

<asp:LinkButton ID="LinkButtonRequestRequestorName" OnClientClick="return Show();" Text="Show Info" runat="server" OnClick="LinkButtonRequestRequestorName_Click"></asp:LinkButton>

 <telerik:radwindow runat="server" id="UserInfo" modal="true" width="500px" height="100%" title="User Info"
            rendermode="Lightweight" behaviors="Close" enableviewstate="true" visibletitlebar="false" visiblestatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title" id="lblUserName" runat="server">User Info</h3>
                        </div>
                         <div class="modal-body">
                    <uc1:UserDetailUC runat="server" ID="UserDetailUC"  />
                </div>
                        <div class="modal-footer">
                    <asp:Button ID="btnOk" runat="server" CssClass="btn btn-primary bordered-button" data-dismiss="modal" Text="OK" Width="60px" />
                      </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="LinkButtonRequestRequestorName" />
                    </Triggers>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:radwindow>

      
            

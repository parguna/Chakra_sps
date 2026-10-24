<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecurityQA.ascx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.Forgot.UC.SecurityQA" %>
<%@ Register Src="~/UsrControl/Common/AlertMessage.ascx" TagPrefix="uc1" TagName="AlertMessage" %>
<uc1:AlertMessage runat="server" ID="AlertMessage" />
<asp:Repeater ID="loadQuestion" runat="server">
    <ItemTemplate>
         <asp:Panel ID="group" runat="server" Visible='<%# (Container.ItemIndex==0) ? true : false  %>'>
        <div class="form-group">
            <asp:Label ID="Label1" AssociatedControlID="txtQuestions" runat="server" Text="Question" class="control-label"></asp:Label>
            <div>
                <asp:HiddenField runat="server" ID="Id"/>
                <asp:TextBox ID="txtQuestions" class="form-control" Rows="3" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <asp:Label ID="Label2" AssociatedControlID="txtAnswer" runat="server" Text="Answer" class="control-label"></asp:Label>
            <div>
                <asp:TextBox ID="txtAnswer" class="form-control numeric" Rows="3" runat="server" required="required" TextMode="Password" ></asp:TextBox>
            </div>
        </div>
    </asp:Panel>
   </ItemTemplate>
</asp:Repeater>
 <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-default btn-primary btn-block" Text="Submit" OnClick="btnSubmit_Click" />
    <div class="forget-password topBuffer">
         <a href="/Login.aspx">&#8592; Login</a>
    </div>
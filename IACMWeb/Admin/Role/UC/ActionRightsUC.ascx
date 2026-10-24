<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActionRightsUC.ascx.cs" Inherits="DI.Chakra.Web.Configuration.Role.UC.ActionRightsUC" %>

<div class="CheckboxList">
    <asp:Repeater ID="ActionRepeater" runat="server">
        <ItemTemplate>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                 <asp:CheckBox ID="CheckBox1" CssClass='<%# Eval("Id") %> ' runat="server" AutoPostBack="true" OnCheckedChanged="Action_CheckedChanged" Checked='<%# CurrentAccessRights.Where(lq => lq.PagePermission.PermissionId == Convert.ToInt32(Eval("Id"))).Select(lq => lq.PagePermissionId).FirstOrDefault() > 0 ? true : false  %>' />
                    <span class="lbl"><%# Eval("Name") %></span>
                </ContentTemplate>
                <Triggers>
					<asp:AsyncPostBackTrigger ControlID="CheckBox1" EventName="CheckedChanged" />
				</Triggers>
            </asp:UpdatePanel>
        </ItemTemplate>
    </asp:Repeater>
</div>

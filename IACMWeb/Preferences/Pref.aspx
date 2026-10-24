<%@ Page Title="Preferences" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Pref.aspx.cs" Inherits="DI.Chakra.Web.Preferences.Pref" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <div class="row-fluid"></div>
    <asp:Repeater ID="rptParent" runat="server" OnItemDataBound="rptParent_ItemDataBound">
        <ItemTemplate>

            <div class="col-xs-4">
                <div class="preference-box">
                    <div class="preference-list-box" >
                        <h4 class="list-title"><%# DataBinder.Eval(Container.DataItem, "Category") %></h4>
                        <ul class="preference-list-items">
                            <asp:Repeater ID="rptChild" runat="server" OnItemDataBound="rptChild_ItemDataBound">
                                <ItemTemplate>
                                    <li class="disabled">
                                        <asp:LinkButton ID='lnkitem' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Name") %>' OnClick="lnkItem_Click" runat="server"><%# DataBinder.Eval(Container.DataItem, "Name") %></asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                </div>
            </div>
        </ItemTemplate>

    </asp:Repeater>

</asp:Content>


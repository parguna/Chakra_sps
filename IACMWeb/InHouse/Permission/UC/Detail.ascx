<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="DI.Chakra.Web.InHouse.Permission.UC.Detail" %>

<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<script type="text/javascript">

    function requesting(sender, eventArgs) {
        if (sender.get_text().length < 3) {
            eventArgs.set_cancel(true);
        }
    }

    function EntryAdding(sender, eventArgs) {
        var entries = sender.get_entries();
        var count = entries.get_count();
        for (var i = 0; i < count; i++) {
            if (entries.getEntry(i)._text == eventArgs.get_entry()._text) {
                var entry1 = sender.get_entries().getEntry(i);
                sender.get_entries().remove(entry1);
                break;
            }
        }
    }

</script>

<asp:Panel runat="server" ID="ContentPanel">
    <div class="content-wrapper" id="ContentWrapper" runat="server">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-6">
                        <uc1:EndpointUC runat="server" ID="EndpointUC" OnApplicationChanged="EndpointUC_ApplicationChanged" OnEndpointChanged="EndpointUC_EndpointChanged" />
                        <div class="form-group required">
                            <asp:Label ID="lblPermission" runat="server" Text="InHouse System Code" CssClass="col-xs-4 control-label" AssociatedControlID="txtInHouseSystemCode"></asp:Label>
                            <div class="col-xs-5">
                                <telerik:RadAutoCompleteBox ID="txtInHouseSystemCode" runat="server" Enabled="false" AutoPostBack="false" Width="320px" EmptyMessage="Enter * to view full list"
                                    DropDownWidth="320px" InputType="Token" CssClass="form-control">
                                    <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetInhouseSystemCodeByEndPointID" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                        </div>
                        <div class="form-group required">
                            <asp:Label ID="Label4" AssociatedControlID="txtName" runat="server" class="col-xs-4 control-label">Name</asp:Label>
                            <div class="col-xs-7">
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label16" AssociatedControlID="txtDescription" runat="server" class="col-xs-4 control-label">Description</asp:Label>
                            <div class="col-xs-7">
                                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-4">Is Active</label>
                            <div class="col-xs-7">
                                <asp:CheckBox ID="IsActive" runat="server" Checked="true"></asp:CheckBox>
                                <span class="lbl"></span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="buttons text-right nav-rectangle topBuffer">
        <asp:Button runat="server" ID="CancelButton" Text="Cancel" CausesValidation="false" UseSubmitBehavior="false" CssClass="btn btn-default btn-sm cancel" OnClick="CancelButton_Click" />
    </div>
</asp:Panel>

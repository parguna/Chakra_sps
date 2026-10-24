<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DiscoveryInfoUC.ascx.cs" Inherits="DI.Chakra.Web.Job.Discovery.Common.DiscoveryInfoUC" %>

<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>

<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <uc1:EndpointUC runat="server" ID="EndpointUC" ElementLengthCSS="col-xs-7" OnEndpointChanged="EndpointUC_EndpointChanged" OnApplicationChanged="EndpointUC_ApplicationChanged" />
                    <div class="form-group">
                        <asp:Label ID="processname" AssociatedControlID="txtProcessName" runat="server" Text="Process Name" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtProcessName" CssClass="form-control" runat="server" required="required" data-errMessage="Process Name is required"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="description" AssociatedControlID="txtDescription" runat="server" Text="Description" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtDescription" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group required">
                        <asp:Label ID="lblIsQUeryProcess" AssociatedControlID="txtDescription" runat="server" Text="Process Type" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-md-7">
                            <asp:DropDownList ID="ddlIsQueryProcess" runat="server" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="True"
                                >
                                <asp:ListItem Text="Non SAP Instance" Value="1"></asp:ListItem>
                                <asp:ListItem Text="SAP Instance" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <label>
                            <input id="chkDeltaDiscovery" type="checkbox" runat="server">
                            <span class="lbl">Disable Delta Discovery</span>
                        </label>
                    </div>
                </div>
                <div class="col-xs-6">
                    <div class="form-group">
                        <asp:Panel ID="pnlEndPointObjects" runat="server">
                            <asp:Repeater ID="rptEndPointObject" runat="server" OnItemDataBound="rptEndPointObject_ItemDataBound">
                                <ItemTemplate>
                                    <div class="form-group">
                                        <div class="col-xs-8">
                                            <input id="Checkbox1" type="checkbox" value='<%# DataBinder.Eval(Container.DataItem, "ObjectName") %>' runat="server">
                                            <span class="lbl"><%# DataBinder.Eval(Container.DataItem, "ObjectName") %> </span>
                                        </div>
                                        <div class="col-xs-4">
                                            <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationObject.ascx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.Common.AttestationObject" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="modal-content">
    <div class="modal-header">
        <h3 class="modal-title" id="H1">Edit Hierarchy</h3>
    </div>
    <div class="modal-body">
        <div class="content-wrapper">

            <div class="form clearfix">
                <div class="form-horizontal left">
                    <div class="row-fluid">
                        <div class="form-group">
                            <div class="col-xs-12">
                                <asp:Repeater ID="AttestationObjectRepeater" runat="server">
                                    <HeaderTemplate>
                                        <table>
                                            <tr>
                                                <td style="width: 100px">Object</td>
                                                <td style="width: 100px">Level</td>                                                
                                                <td style="width: 100px">Visible</td>
                                                <td style="width: 100px">Actionable</td>
                                                <td style="width: 100px">Extendable</td>
                                                <td style="width: 100px">Default Action</td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div style="display:flex; flex-flow:row; flex-wrap:wrap">
                                        <%--<table>
                                            <tr style="height:200px;">--%>
                                                <div class="hierachyTbl">
                                                    <asp:Label ID="lblObject" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "EndpointHierarchy.EntitlementType") %>'></asp:Label></div>
                                                <div class="hierachyTbl">
                                                    <asp:Label ID="lblLevel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "EndpointHierarchy.Level") %>' ></asp:Label></div>
                                                <div class="hierachyTbl">
                                                    <asp:CheckBox ID="chkVisible" runat="server" AutoPostBack="true" Checked='<%# DataBinder.Eval(Container.DataItem, "IsVisible") %>'  OnCheckedChanged="chkVisible_CheckedChanged" /></div>
                                                <div class="hierachyTbl">
                                                    <asp:CheckBox ID="chkAction" runat="server" AutoPostBack="true" Checked='<%# DataBinder.Eval(Container.DataItem, "IsActionAllowed") %>'  OnCheckedChanged="chkAction_CheckedChanged" /></div>                                                
                                                <div class="hierachyTbl">
                                                    <asp:CheckBox ID="chkExtendable" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "IsActionExtendable") %>' /></div>
                                                <div class="hierachyTbl">
                                                    <asp:DropDownList ID="defaultaction" runat="server"></asp:DropDownList></div>
                                            <%--</tr>
                                        </table>--%>
                                            </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>                            
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal-footer">
        <asp:Button runat="server" ID="btnCancel" CausesValidation="true" class="btn btn-primary transparent-button" Text="Cancel" OnClick="btnCancel_Click" />
        <asp:Button ID="SaveButton" runat="server" CausesValidation="true" OnClick="btnSubmit_Click" CssClass="btn btn-primary bordered-button validation"
            Text="Save" />
    </div>
</div>




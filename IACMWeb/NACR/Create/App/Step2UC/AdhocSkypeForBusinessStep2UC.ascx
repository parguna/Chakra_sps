<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSkypeForBusinessStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocSkypeForBusinessStep2UC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style type="text/css">
    .auto-style1 {
        font-size: xx-small;
    }
    .floatleft{
        padding-left:0px !important;
    }
    .label {
        font-size: 100%;
    }
</style>
<script type="text/javascript">
      function isNumberKey(evt)
      {
         var charCode = (evt.which) ? evt.which : evt.keyCode;
         if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;    
         return true;
      }
      function OnClientRequesting(sender, args) {
          var epUser = document.getElementById("<%= hiddenEndpointUser.ClientID%>").value;
        var context = args.get_context()
        context["AdhocActionType"] = "<%= AdhocActionType %>";
          context["endPointUserId"] = epUser;
        context["endpointId"] = "<%= EndpointSystemId %>";
    }
</script>
<asp:HiddenField ID="hiddenEndpointUser" runat="server" Value="0" />
<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="form-group row" runat="server" visible="false" id="divWarningMessage" style="margin-left:-30px">
              <div class="col-xs-7">
                  <div class="form-group required">
                        <div class="col-xs-12" >
                           <asp:Label runat="server" ID="lblWarning" ForeColor="red" Text="Reserved number can be requested only if no additional access has been applied before"></asp:Label>
                        </div>
                    </div> 
                  </div> 
            </div> 
         <div class="form-group row" runat="server" visible="false" id="divReservedNumberWarning" style="margin-left:-30px">
              <div class="col-xs-7">
                  <div class="form-group required">
                        <div class="col-xs-12" >
                           <asp:Label runat="server" ID="lblReservedNumberWarning" ForeColor="red" Text="Please make sure, you entered the Reserved Number in the Step 1 comments field"></asp:Label>
                        </div>
                    </div> 
                  </div> 
            </div> 
        <div class="form-group row" runat="server" id="divEndpointUser" visible="false">
                <div class="col-xs-7">
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">EndPoint User Name</label>
                        <div class="col-xs-9">
                            <asp:DropDownList ID="ddlEndpointUser" CssClass="form-control selectpicker" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEndpointUser_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
        </div>
        <div class="form-group row" runat="server" id="divLineURI" visible="false">
                <div class="col-xs-7">
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">LineURI</label>
                        <div class="col-xs-9">
                          <asp:Label ID="lblLineURI" runat="server" Text=""></asp:Label> &nbsp;<asp:Label ID="lblDCCWarningMessage" runat="server" Text="" ForeColor="red"></asp:Label>
                        </div>
                    </div>
                </div>
        </div>
        <div class="form-group row" runat="server" id="newAccountMessage" visible="false">
                <div class="col-xs-7">
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">&nbsp;</label>
                        <div class="col-xs-9">
                          <asp:Label ID="lblInfoMessage" runat="server" ForeColor="Blue"></asp:Label>
                        </div>
                    </div>
                </div>
        </div>
        <div class="form-group row">
                <div class="col-xs-7" runat="server" visible="true" id="divEntitlementType">
                    <div class="form-group required" runat="server" id="divEntTypeRequFieldIndicator">
                        <label class="col-xs-3 control-label">Type</label>
                        <div class="col-xs-9">
                            <asp:DropDownList ID="ddlEntitlementType" OnSelectedIndexChanged="ddlEntitlementType_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
        </div>
        
        <div class="form-group row" runat="server" id="divNotes" visible="true">
                <div class="col-xs-7">
                    <div class="form-group">
                        <label class="col-xs-3 control-label" style="font-weight:bold;color:red;">Note</label>
                        <div class="col-xs-9">
                           <b>DCC  :</b> Assigning the new LineURI,change in VoicePolicy,VoiceRoutingPolicy and DialPlan and move the User to Online<br />
                           <%--<b>DCCS :</b> Change in VoicePolicy, VoiceRoutingPolicy, DialPlan and move the User to Online--%>
                           <b>IDD  :</b> Change in VoicePolicy, VoiceRoutingPolicy and DialPlan
                        </div>
                    </div>
                </div>
            </div>
        <asp:Panel runat="server" ID="pnlMoveOption" Visible="false">
             <div class="row form-group">
                <div class="col-lg-7">
                     <div class="form-group required">
                         <div class="col-xs-3">&nbsp;</div>
                         <div class="col-xs-9">
                             <asp:RadioButton ID="rdoOnLine" runat="server" GroupName="Move" AutoPostBack="true" OnCheckedChanged="Move_CheckedChanged" />
                            <span class="lbl">OnLine</span>
                            <asp:RadioButton ID="rdoOnPrem" runat="server" GroupName="Move" AutoPostBack="true" OnCheckedChanged="Move_CheckedChanged" Visible="false"/>
                            <%--<span class="lbl">On-Premises </span>--%>
                         </div>
                     </div>
                </div>
             </div>         
             <div class="row form-group" runat="server" visible="false" id="divRegistrarPool">
                <div class="col-lg-7">
                     <div class="form-group required">
                         <label class="col-xs-3 control-label">Registrar Pool:</label>
                         <div class="col-xs-9">
                            <asp:DropDownList ID="ddlRegistrarPool" AutoPostBack="true" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                         </div>
                     </div>
                </div>
             </div>           
        </asp:Panel>
        <asp:Panel ID="pnlResponseGroupSelection" runat="server" Visible="false">
            <div class="form-group row">
                    <div class="col-xs-7">
                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Response Group</label>
                            <div class="col-xs-9">
                                <telerik:radautocompletebox id="ResponseGroupAutoCompleteBox" onentryremoved="ResponseGroupAutoCompleteBox_EntryRemoved" onentryadded="ResponseGroupAutoCompleteBox_EntryAdded"
                                    EmptyMessage="Enter * to view full list" runat="server" dropdownheight="200px" dropdownwidth="300px" inputtype="Token" onclientrequesting="OnClientRequesting"
                                    cssclass="form-control" autopostback="true">
                                    <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Method="GetSkypeForBusinessResponseGroup" Path="../../../../Services/AutoCompleteDataService.svc" />
                                </telerik:radautocompletebox>
                            </div>
                           
                        </div>
                    </div>
            </div>
            <div class="form-group row" runat="server" id="divMask">
                <div class="col-xs-7">
                    <div class="form-group">
                        <asp:Label ID="lblMask" runat="server" CssClass="col-xs-3 floatleft" Text="Mask"></asp:Label>
                        <div class="col-xs-6">
                           <asp:CheckBox ID="chkMask" runat="server" Checked="true" Enabled="false"/>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" Visible="false" ID="pnlDCCExtension">
            <div class="row form-group">
                <div class="col-lg-7">
                     <div class="form-group required">
                            <label class="col-xs-3 control-label">Extension Number</label>
                            <div class="col-xs-8">
                                <asp:Label ID="lblLineUrIPrefix" runat="server" Visible="true" CssClass="label label-success col-lg-7" Font-Size="Medium"></asp:Label>
                                <div class="col-lg-4">
                                    <asp:TextBox ID="txtDCCExtension" onkeypress="OnClientRequesting" runat="server" Visible="true" MaxLength="4" Enabled="true"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtDCCExtension" runat="server" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+">
                                    </asp:RegularExpressionValidator>
                                    <asp:FilteredTextBoxExtender runat="server" ID="fltrNumber" TargetControlID="txtDCCExtension" FilterType="Numbers"></asp:FilteredTextBoxExtender>
                                </div>
                                
                            </div>
                     </div>    
                </div>
            </div>
        </asp:Panel>
         <div class="form-group row">
                    <div class="col-xs-7">
                    <div class="form-group">
                        <label class="col-xs-3 control-label"></label>
                        <div class="col-xs-9">
                        <asp:Button ID="btnAddRequest" runat="server" Text="Add" Visible="false" CssClass="btn btn-default" OnClick="btnAddRequest_Click"></asp:Button>
                            &nbsp;&nbsp;   
                            <asp:Button ID="btnSave" runat="server" OnClick="AddToCart" Text="Save Request" CssClass="btn btn-default" Visible="false"/>
                    </div>
                    <div class="col-xs-3">&nbsp;
                    </div>
                    </div>
                    </div>
        </div>
        <div class="form-group row">
                <div class="col-xs-10">
                    <asp:Panel ID="pnlResponseGroup" runat="server" Visible="false">
                        <telerik:radgrid id="ResponseGroupGrid" runat="server" visible="false" allowpaging="false" allowsorting="false" cellspacing="0"
                            gridlines="None" showfooter="false" onitemcommand="ResponseGroupGrid_ItemCommand" Width="100%"
                            mastertableview-enablenorecordstemplate="true" mastertableview-nodetailrecordstext="No Records Available" tablelayout="Fixed" cssclass="RadGridtextWrap">
                                <GroupingSettings CaseSensitive="false" />
                                <ClientSettings>
                                    <Scrolling AllowScroll="true" ScrollHeight="80px" UseStaticHeaders="true" SaveScrollPosition="True" />
                                </ClientSettings>
                                <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id,Name" TableLayout="Fixed"  Width="100%">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Name" SortExpression="Name" HeaderText="Response Group Name" UniqueName="Name" HeaderStyle-Width="30%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Mask" FilterControlAltText="Mask" SortExpression="Mask" HeaderText="Mask" UniqueName="Mask"  HeaderStyle-Width="5%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn HeaderText="View Member(s)" ItemStyle-HorizontalAlign="Left"  HeaderStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ViewGroupNameLinkButton" CommandName="View" Text="View" runat="server"></asp:LinkButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                                        <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" HeaderStyle-Width="2%" HeaderText="">
                                        </telerik:GridButtonColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:radgrid>
                    </asp:Panel>
                </div>
        </div>
    </div>
</div>
<telerik:RadWindow runat="server" ID="ViewRoleGroupUserWindow" Modal="true" Height="100%" Width="700px" Title="RoleGroup User"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title"><asp:Label runat="server" ID="lblGrpName"></asp:Label>&nbsp; Member(s)</h3>
                </div>
                <div class="modal-body">
                    <asp:Panel ID="panelViewTechnicalSource" CssClass="ModalWindow" runat="server">
                        <telerik:RadGrid runat="server" ID="ViewRoleGroupUserSource" GridLines="None" AllowAutomaticUpdates="false" Width="100%" EnableAjaxSkinRendering="true" AllowPaging="False" CellSpacing="0">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" FrozenColumnsCount="2"></Scrolling>
                            </ClientSettings>
                            <PagerStyle Mode="NumericPages" />
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" EditFormSettings-PopUpSettings-ScrollBars="Vertical" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="UserPrincipalName" HeaderText="UserPrincipalName" HeaderStyle-Width="10%" FilterControlWidth="94%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="LastName" HeaderText="Name" HeaderStyle-Width="10%" FilterControlWidth="94%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="LineURI" HeaderText="LineURI" HeaderStyle-Width="10%" FilterControlWidth="94%">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </asp:Panel>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnCancel" CssClass="uiboxgray" runat="server" Text="Close" OnClick="btnCancel_Click" />
                </div>

            </ContentTemplate>

        </asp:UpdatePanel>

    </ContentTemplate>

</telerik:RadWindow>
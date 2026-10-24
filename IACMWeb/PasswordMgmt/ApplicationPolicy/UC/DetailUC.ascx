<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.ApplicationPolicy.UC.DetailUC" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxCtrl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Panel runat="server" ID="ContentWrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <%-- <uc1:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="EndpointUC_EndpointChanged" OnApplicationChanged="EndpointUC_ApplicationChanged" />--%>
                    <div class="form-group required">
                        <asp:Label ID="Label1" AssociatedControlID="txtPolicyName" runat="server" Text="Policy Name" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtPolicyName" CssClass="form-control numeric" runat="server" ></asp:TextBox>
                        </div>
                         <div class="col-xs-4">
                            <asp:Label ID="Label3" AssociatedControlID="chkPwdUsername" runat="server" CssClass="col-xs-12">
                                <asp:CheckBox ID="chkPwdUsername" runat="server"  />
                                <span class="lbl">Username Not Allowed in Password</span>
                            </asp:Label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-12">
                    <div class="form-group required">
                        <asp:Label ID="Label2" AssociatedControlID="txtPasswordLength" runat="server" Text="Password Length" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtPasswordLength" pattern="^[1-9]$|^0[1-9]$|^1[0-9]$|^20$" title="[1-20]" CssClass="form-control numeric" runat="server"></asp:TextBox>
                        </div>
                       
                    </div>
                </div>
                <div class="col-xs-12">
                    <div class="form-group required">
                        <asp:Label ID="Label4" AssociatedControlID="txtMaximumLength" runat="server" Text="Maximum Length" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtMaximumLength" pattern="^[1-9]$|^0[1-9]$|^1[0-9]$|^20$" title="[1-20]" CssClass="form-control numeric" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group required">
                        <asp:Label ID="Label5" AssociatedControlID="txtMinimumLength" runat="server" Text="Minimum Length" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtMinimumLength" pattern="^[1-9]$|^0[1-9]$|^1[0-9]$|^20$" title="[1-20]" CssClass="form-control numeric"  runat="server"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">
                       <ContentTemplate>
                          
                               <asp:Label ID="Label6" AssociatedControlID="txtNoofSpecialChar" runat="server" Text="Minimum Number Of Special Characters" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtNoofSpecialChar" pattern="[0-5]{1}" title="[0-5]" CssClass="form-control numeric" runat="server"></asp:TextBox>
                       
                             </div>
                       </ContentTemplate>

                       <Triggers>
                           <asp:AsyncPostBackTrigger ControlID="chkNoOfSplCharNotAllowed" EventName  ="CheckedChanged" />
                       </Triggers>
                    </asp:UpdatePanel>
                        <div class="col-xs-3">
                            <asp:Label ID="Label7" AssociatedControlID="chkNoOfSplCharNotAllowed" runat="server" CssClass="col-xs-12">
                                <asp:CheckBox ID="chkNoOfSplCharNotAllowed" runat="server" AutoPostBack="true" OnCheckedChanged="CheckboxCheckChanged" />
                                <span class="lbl">Special characters not allowed</span>
                            </asp:Label>
                        </div>

                    </div>
                </div>
                <div class="col-xs-12">
                    <div class="form-group">
                          <asp:UpdatePanel runat="server" id="UpdatePanel2" updatemode="Conditional">
                       <ContentTemplate>
                          
                               <asp:Label ID="Label8" AssociatedControlID="txtNoOfUCAlphabets" runat="server" Text="Minimum Number Of Upper Case Alphabets" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtNoOfUCAlphabets" pattern="[0-9]{1}|10{1}" title="[0-9]" CssClass="form-control numeric" runat="server"></asp:TextBox>
                        </div>
                       </ContentTemplate>

                       <Triggers>
                           <asp:AsyncPostBackTrigger ControlID="chkNoOfAlphabets" EventName  ="CheckedChanged" />
                       </Triggers>
                    </asp:UpdatePanel>
                        
                        <div class="col-xs-4">
                            <asp:Label ID="Label9" AssociatedControlID="chkNoOfAlphabets" runat="server" CssClass="col-xs-12">
                                <asp:CheckBox ID="chkNoOfAlphabets" runat="server" AutoPostBack="true"  OnCheckedChanged="CheckboxCheckChanged" />
                                <span class="lbl">Alphabets not allowed</span>
                            </asp:Label>
                        </div>
                    </div>
                </div>

                <div class="col-xs-12">
                    <div class="form-group">
                          <asp:UpdatePanel runat="server" id="UpdatePanel3" updatemode="Conditional">
                       <ContentTemplate>
                          
                               <asp:Label ID="Label15" AssociatedControlID="txtNoOfLCAlphabets" runat="server" Text="Minimum Number Of Lower Case Alphabets" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtNoOfLCAlphabets" pattern="[0-9]{1}|10{1}" title="[0-9]" CssClass="form-control numeric" runat="server"></asp:TextBox>
                        </div>
                       </ContentTemplate>

                       <Triggers>
                           <asp:AsyncPostBackTrigger ControlID="chkNoOfAlphabets" EventName  ="CheckedChanged" />
                       </Triggers>
                    </asp:UpdatePanel>
                       
                    </div>
                </div>
                <div class="col-xs-12">
                    <div class="form-group">
                         <asp:UpdatePanel runat="server" id="UpdatePanel4" updatemode="Conditional">
                       <ContentTemplate>
                          
                              <asp:Label ID="Label10" AssociatedControlID="txtNoOfNumeric" runat="server" Text="Minimum Number Of Numeric" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtNoOfNumeric" pattern="[0-9]{1}|10{1}" title="[0-10]" CssClass="form-control numeric" runat="server"></asp:TextBox>
                        </div>
                       </ContentTemplate>

                       <Triggers>
                           <asp:AsyncPostBackTrigger ControlID="chkNoOfNumeric" EventName  ="CheckedChanged" />
                       </Triggers>
                    </asp:UpdatePanel>
                       
                        <div class="col-xs-3">
                            <asp:Label ID="Label11" AssociatedControlID="chkNoOfNumeric" runat="server" CssClass="col-xs-12">
                                <asp:CheckBox ID="chkNoOfNumeric" runat="server" AutoPostBack="true"   OnCheckedChanged="CheckboxCheckChanged" />
                                <span class="lbl">Numeric not allowed</span>
                            </asp:Label>
                        </div>
                    </div>
                </div>
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:Label ID="Label12" AssociatedControlID="txtNoOfSplCharNotAllowed" runat="server" Text="Not Allowed Special Characters" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtNoOfSplCharNotAllowed" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label13" AssociatedControlID="txtNoOfDaysPasswordChangeAllowed" runat="server" Text="Minimum Password Age" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtNoOfDaysPasswordChangeAllowed" title="[0-365]" CssClass="form-control numeric" runat="server"></asp:TextBox>
                            <asp:RangeValidator ID="RangeValidator1" Type="Integer" MinimumValue="0" MaximumValue="365" ControlToValidate="txtNoOfDaysPasswordChangeAllowed" runat="server" ErrorMessage="Enter only numbers between 0 and 365" ForeColor="Red"></asp:RangeValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label14" AssociatedControlID="txtNotAllowedKeywords" runat="server" Text="Not Allowed Keywords" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtNotAllowedKeywords" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblApplications" runat="server" Text="Applicable Applications" CssClass="col-xs-3 control-label" AssociatedControlID="ApplicationAutoCompleteBox"></asp:Label>
                        <div class="col-xs-3">
                            <telerik:RadAutoCompleteBox ID="ApplicationAutoCompleteBox" runat="server" AutoPostBack="true" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single"
                                DropDownWidth="320px" InputType="Token" CssClass="form-control">
                       <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetPasswordPolicyApplications" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                        <div class="col-xs-1">
                            <asp:Button ID="AddApplicationButton" runat="server" Text="Add" CssClass="btn btn-default btn-sm" formnovalidate="true" ValidationGroup="AddForm" OnClick="AddApplicationButton_Click" OnClientClick="return validateAutoCompleteBox(this,'ApplicationAutoCompleteBox','Please enter at least one EndPoint')" />
                        </div>
                    </div>
                    <div class="form-group">
                    <div class="col-xs-3"></div>
                    <div class="col-xs-3">
                        <telerik:RadGrid ID="ApplicationGrid" runat="server" AllowPaging="false" AllowSorting="false"
                            CellSpacing="0" GridLines="None" OnDeleteCommand="ApplicationGrid_DeleteCommand">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Key" ShowHeadersWhenNoRecords="false" NoMasterRecordsText="No Records Available">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" SortExpression="Value" HeaderText="Application" HeaderStyle-Width="300px">
                                    </telerik:GridBoundColumn>
                                       <telerik:GridButtonColumn ImageUrl="~/images/action-buttons/delete.png" ButtonType="ImageButton" CommandName="Delete" Text="Delete" HeaderText="Delete" ConfirmText="Are you sure you want to delete ?"
                                        UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="10%" ConfirmDialogType ="RadWindow">
                                        <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                                    </telerik:GridButtonColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </asp:Panel>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="Button1" Text="Cancel" OnClick="Button1_Click" UseSubmitBehavior="false" ></asp:Button>
</div>

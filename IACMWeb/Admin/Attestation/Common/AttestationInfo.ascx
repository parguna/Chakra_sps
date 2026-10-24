<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationInfo.ascx.cs" Inherits="DI.Chakra.Web.Attestation.Common.AttestationInfo" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>
<%@ Register Src="~/Admin/Attestation/Common/AttestationObject.ascx" TagPrefix="uc1" TagName="AttestationObject" %>

<script type="text/javascript">

    $(document).ready(function() {           
            $('#<%=txtAttestationName.ClientID %>').keyup(function() {
                $('#<%=lblFirstPart.ClientID %>').text(this.value);
            });
        });
</script>


<div class="content-wrapper">



    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-8">
                    <div class="form-group">
                        <asp:Label ID="lblAttestationName" AssociatedControlID="txtAttestationName" runat="server" CssClass="col-xs-4 control-label"><%= T("App.Attestation.AttestationName") %></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtAttestationName" runat="server" CssClass="form-control" Rows="1" required="required" onKeyPress="return AllowLetterNumberDot(event)"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblAttestationDesc" AssociatedControlID="txtAttestationDesc" runat="server" CssClass="col-xs-4 control-label"><%= T("App.Attestation.AttestationDescription") %></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtAttestationDesc" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="4" Columns="60"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblCampaignName" AssociatedControlID="txtCampaignName" runat="server" CssClass="col-xs-4 control-label" Visible="false"><%= T("App.Attestation.CampaignName") %></asp:Label>
                        <div class="col-xs-5">
                            <asp:Label ID="lblFirstPart" AssociatedControlID="txtCampaignName" runat="server" Visible="false" CssClass="col-xs-12 control-label"></asp:Label>
                        </div>
                        <div class="col-xs-2">
                            <asp:TextBox ID="txtCampaignName" runat="server" CssClass="form-control" Rows="1" required="required" Visible="false"></asp:TextBox>
                        </div>
                        <%-- <div class="col-xs-1">
                            <div class="whats-this bottom showTooltip" style="margin-top: -2px">
                                <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                               <span style="display: none;" role="tooltip" class="tooltip right info">
                                    <h5><%= T("App.Attestation.CampaignFormat") %></h5>
                                    <p><%= T("App.Attestation.CampaignFormatMessage") %></p>
                                </span>
                            </div>
                        </div>--%>
                    </div>
                    <div class="form-group" style="visibility:hidden">
                        <asp:Label ID="lblAttestationType" AssociatedControlID="AttestationTypeField" runat="server" CssClass="col-xs-4 control-label"><%= T("App.Attestation.AttestationType") %></asp:Label>
                        <div class="col-xs-7">
                            <asp:DropDownList ID="AttestationTypeField" runat="server" CssClass="form-control selectpicker" data-width="100%" AppendDataBoundItems="true" AutoPostBack="True"
                                DataTextField="DisplayName" DataValueField="Key" OnSelectedIndexChanged="AttestationTypeField_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <uc1:EndpointUC runat="server" ID="EndpointUC" DisplayControl="ApplicationType" OnApplicationChanged="EndpointTypeUC_EndpointTypeChanged" />
                    <div class="form-group">
                        <div class="col-xs-4">
                        </div>

                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblEndPoint" AssociatedControlID="EndPointAutoCompleteBox" runat="server" CssClass="col-xs-4 control-label"><%= T("App.Common.Endpoint") %></asp:Label>
                        <div class="col-xs-7">
                            <telerik:RadAutoCompleteBox ID="EndPointAutoCompleteBox" runat="server" ToolTip="Enter * to view full list" TextSettings-SelectionMode="Single"
                                InputType="Token" CssClass="form-control" Rows="4" required="required" EmptyMessage="No Records found">
                                <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetAllAttestationEndPointsBySelecetedSystemTypes" />
                            </telerik:RadAutoCompleteBox>
                            <asp:Label ID="Label2" runat="server" ForeColor="Red" CssClass="errormsg"></asp:Label>
                        </div>
                        <div class="col-xs-1">
                            <asp:Button ID="AddEndPointButton" ValidationGroup="AddEndPoint" runat="server" Text="Add" OnClick="AddEndPointButton_Click" CssClass="btn btn-default btn-sm" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-4">
                        </div>
                        <div class="col-xs-7">
                            <telerik:RadGrid ID="EndPointSystem_FormGrid" runat="server" AllowPaging="true" AllowSorting="false" Width="100%"
                                OnItemCommand="EndPointSystem_ItemCommand" OnItemDataBound="EndPointSystem_ItemDataBound" OnPreRender="EndPointSystem_FormGrid_PreRender">
                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="false" />
                                <ClientSettings>
                                    <Scrolling UseStaticHeaders="true" />
                                    <Selecting AllowRowSelect="false" />
                                </ClientSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView DataKeyNames="Key" AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" CommandItemDisplay="None" EnableHeaderContextMenu="false" TableLayout="Auto"  Width="100%" >
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" SortExpression="Value" HeaderText="EndPoint System" HeaderStyle-Width="70%" UniqueName="EndPointSystemName">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="Delete" ConfirmText="Are you sure you want to delete ?" HeaderText="Delete" ConfirmDialogType="RadWindow"
                                            UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="20%">
                                            <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                                        </telerik:GridButtonColumn>
                                        <telerik:GridButtonColumn ButtonType="LinkButton" CommandName="Hierarchy" Text="Edit" HeaderText="Hierarchy" ConfirmDialogType="RadWindow"
                                            UniqueName="Hierarchy" Exportable="false" HeaderStyle-Width="20%">
                                            <ItemStyle HorizontalAlign="Center" CssClass="action-button" />
                                        </telerik:GridButtonColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-4"></div>
                        <div class="col-xs-7">
                            <asp:Label ID="lbl1" runat="server" AssociatedControlID="chkRemediate">
                                <asp:CheckBox runat="server" ID="chkRemediate" TabIndex="2" Checked="true"/>
                                <span class="lbl"><%= T("App.Attestation.EnableRemediation") %></span>
                            </asp:Label>
                            <div class="whats-this bottom showTooltip" style="margin-top: -2px">
                                <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                                <span style="display: none;" role="tooltip" class="tooltip right info">
                                    <h5><%= T("App.Attestation.Remediation") %></h5>
                                    <p><%= T("App.Attestation.RemediationMessage") %></p>
                                </span>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-4"></div>
                        <div class="col-xs-7">
                            <asp:Label ID="lblUnmappedAccount" runat="server" AssociatedControlID="chkUnmappedAccount">
                                <asp:CheckBox runat="server" ID="chkUnmappedAccount" TabIndex="2" />
                                <span class="lbl"><%= T("App.Attestation.IncludeUnmappedAccount") %></span>
                            </asp:Label>
                        </div>
                    </div>

                    <div class="form-group" runat="server" visible="false">
                        <div class="col-xs-4"></div>
                        <div class="col-xs-7">
                            <asp:Label ID="lblDormantAccount" runat="server" AssociatedControlID="chkDormantAccount">
                                <asp:CheckBox runat="server" ID="chkDormantAccount" AutoPostBack="true" OnCheckedChanged="chkDormantAccount_CheckedChanged" TabIndex="2" />
                                <span class="lbl"><%= T("App.Attestation.IncludeDormantAccount") %></span>
                            </asp:Label>
                        </div>
                    </div>

                    <div class="form-group" runat="server" visible="false">
                        <asp:Label ID="lblNoOfDormantDays" AssociatedControlID="txtDormantDays" runat="server" CssClass="col-xs-4 control-label"><%= T("App.Attestation.DormantDays") %></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtDormantDays" runat="server" CssClass="form-control" Rows="1" onKeyPress="return AllowNumeric(event, 0, 400)"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%--    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnEditText">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="modalPopup" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>--%>

    <%--    <div id="hiddenDiv" style="display: none; visibility: hidden;">
        <asp:Button runat="server" ID="hiddenButton" />
    </div>--%>
    <%--         <asp:ModalPopupExtender ID="AttestationObjectModalPopupExtender" runat="server" TargetControlID="hiddenButton"
                                PopupControlID="AttestationObjectPanel" BackgroundCssClass="modalBackground"
                                BehaviorID="mpeReUser">
                            </asp:ModalPopupExtender>--%>


    <telerik:RadWindow runat="server" ID="modalAttestationPopup" Modal="true" Width="800px" Height="479px" Title="Account Detail"
        EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>

                    <uc1:AttestationObject ID="AttestationObject" runat="server" OnAdd_SubmitButtonClicked="AttestationObject_Add_SubmitButtonClicked" OnAdd_CancelButtonClicked="AttestationObject_Add_CancelButtonClicked" />

                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

</div>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationListUC.ascx.cs" Inherits="DI.Chakra.Web.Attestation.List.AttestationListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<div class="content-wrapper">
    <asp:Panel ID="pnlApplicationType" runat="server">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-6">
                        <div class="form-group required" id="AttestationApplicationType" runat="server" visible="true">
                            <label class="col-xs-4 control-label"><%= T("App.Common.EndpointType") %></label>
                            <div id="DivApplicationTypeDropDown" class="col-xs-7" runat="server">
                                <asp:DropDownList ID="ddlAttestationAppTypeField"  runat="server" CssClass="form-control selectpicker" data-width="100%" AppendDataBoundItems="true" AutoPostBack="True"
                                    OnSelectedIndexChanged="ddlAttestationAppTypeField_SelectedIndexChanged"  DataTextField="DisplayName" DataValueField="Id">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</div>

<div class="content-wrapper">
    <asp:Panel ID="pnlAttestations" runat="server">
        <cc1:IACMRadGridControl ID="grdAttestation" runat="server" OnNeedDataSource="grdAttestation_NeedDataSource" OnItemCommand="grdAttestation_ItemCommand"
            OnSelectedIndexChanged="grdAttestation_SelectedIndexChanged" Width="100%" PageSize="50">
            <MasterTableView PagerStyle-AlwaysVisible="true" NoMasterRecordsText="No Records Available" DataKeyNames="AttestationId" Width="100%"  PageSize="50">
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn DataField="AttestationName" FilterControlAltText="Filter AttestationName column" HeaderText="Attestation Name" SortExpression="AttestationName" UniqueName="AttestationName" HeaderStyle-Width="25%" FilterControlWidth="82%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="AttestationDescription" FilterControlAltText="Filter AttestationDescription column" HeaderText="Description" SortExpression="AttestationDescription" UniqueName="AttestationDescription" HeaderStyle-Width="30%" FilterControlWidth="85%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="EndPointSystem" FilterControlAltText="Filter EndPointSystem column" HeaderText="Endpoint Systems" SortExpression="EndPointSystem" UniqueName="EndPointSystem" HeaderStyle-Width="25%" FilterControlWidth="82%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="AttestationRemediation" FilterControlAltText="Filter AttestationRemediation column" HeaderText="Remediation Enabled" SortExpression="AttestationRemediation" UniqueName="AttestationRemediation" HeaderStyle-Width="20%" FilterControlWidth="75%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="SpilitCampaignScope" FilterControlAltText="Filter SplitCampaignUsing column" HeaderText="Split Campaign By" SortExpression="SpilitCampaignScope" UniqueName="SpilitCampaignScope" HeaderStyle-Width="30%" FilterControlWidth="75%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="CreatedDate" HeaderStyle-Width="30%"  FilterControlAltText="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlWidth="90px">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="CreatedBy" FilterControlAltText="CreatedBy" SortExpression="CreatedBy" HeaderText="Created By" UniqueName="CreatedBy" HeaderStyle-Width="30%" FilterControlWidth="100%">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="LastModifiedDate" HeaderStyle-Width="30%"  FilterControlAltText="LastModifiedDate" HeaderText="Last Modified Date" SortExpression="LastModifiedDate" UniqueName="LastModifiedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlWidth="90px">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="LastModifiedBy" FilterControlAltText="LastModifiedBy" SortExpression="LastModifiedBy" HeaderText="Last Modified By" UniqueName="LastModifiedBy" HeaderStyle-Width="30%" FilterControlWidth="100%">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </cc1:IACMRadGridControl>
    </asp:Panel>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
</div>

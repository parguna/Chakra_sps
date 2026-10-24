<%@ Page Title="OU Report" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="OU.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.ADS.OU" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>



<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="col-xs-7">
                        <uc1:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="EndpointUC_EndpointChanged" DisplayControl="Application" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="Panel1" runat="server">
        <cc1:IACMRadGridControl ID="grdUser" runat="server" OnNeedDataSource="grdUser_NeedDataSource">
            <MasterTableView TableLayout="fixed"
                DataKeyNames="OuId" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                <CommandItemSettings ShowAddNewRecordButton="false"
                    ShowRefreshButton="false"></CommandItemSettings>

                <Columns>
                    <telerik:GridBoundColumn DataField="name" HeaderStyle-Font-Bold="true" FilterControlAltText="Name" HeaderText="Name" SortExpression="Name" UniqueName="Name" HeaderStyle-Width="15%" FilterControlWidth="75%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="description" FilterControlAltText="description" HeaderText="Description" SortExpression="Description" UniqueName="description" HeaderStyle-Width="25%" FilterControlWidth="85%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn DataField="whenCreated" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="whenCreated" HeaderText="Created On" SortExpression="whenCreated" UniqueName="whenCreated" HeaderStyle-Width="7%" FilterControlWidth="80%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn DataField="whenChanged" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="whenChanged" HeaderText="Changed On" SortExpression="whenChanged" UniqueName="whenChanged" HeaderStyle-Width="7%" FilterControlWidth="80%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridDateTimeColumn>
                </Columns>
            </MasterTableView>
        </cc1:IACMRadGridControl>

    </asp:Panel>

</asp:Content>

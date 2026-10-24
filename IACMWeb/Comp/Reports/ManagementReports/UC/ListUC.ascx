<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Reports.ManagementReports.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-4 control-label">Select Policy Code</label>
                        <div id="DivReportDropDown" class="col-xs-7">
                            <asp:DropDownList ID="ReportIdCom" runat="server" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="True" OnSelectedIndexChanged="ReportIdCom_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<cc1:IACMRadGridControl ID="ComplianceReportGrid" OnNeedDataSource="ComplianceReportGrid_NeedDataSource" runat="server">
    <MasterTableView ShowGroupFooter="true" DataKeyNames="Id">
        <GroupByExpressions>
            <telerik:GridGroupByExpression>
                <SelectFields>
                    <telerik:GridGroupByField HeaderText="Conflict Code" FieldName="ConflictCode"></telerik:GridGroupByField>
                </SelectFields>
                <GroupByFields>
                    <telerik:GridGroupByField FieldName="ConflictCode" SortOrder="None"></telerik:GridGroupByField>
                </GroupByFields>
            </telerik:GridGroupByExpression>
        </GroupByExpressions>

        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="true" ShowExportToPdfButton="true" ShowExportToCsvButton="true"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="UniqueRunCode" FilterControlAltText="RunCode" HeaderText="RunCode" SortExpression="UniqueRunCode" UniqueName="RunCode"
                Aggregate="Count" FooterText="Total Conflicts: ">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridDateTimeColumn DataField="RunDate" FilterControlAltText="RunDate" HeaderText="Execution Date" SortExpression="RunDate" UniqueName="RunDate"
                Aggregate="CountDistinct" FooterText="Total Dates: ">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridDateTimeColumn>

            <telerik:GridBoundColumn DataField="CompliancePolicyCode" FilterControlAltText="CompliancePolicyCode" HeaderText="Compliance Code" SortExpression="CompliancePolicyCode"
                Aggregate="CountDistinct" FooterText="CompliancePolicy Codes: " UniqueName="CompliancePolicyCode">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="SODConflictCode" FilterControlAltText="SODConflictCode" HeaderText="Conflict Code" SortExpression="SODConflictCode"
                Aggregate="CountDistinct" FooterText="Conflict Codes: " UniqueName="ConflictCode">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="UserName" SortExpression="UserName" UniqueName="UserName"
                Aggregate="CountDistinct" FooterText="Users: ">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="RiskCode" FilterControlAltText="RiskCode" HeaderText="Risk Code" SortExpression="RiskCode"
                Aggregate="CountDistinct" FooterText="Risk Codes: " UniqueName="RiskCode">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="MitigationCode" FilterControlAltText="MitigationCode" HeaderText="Mitigation Code" SortExpression="MitigationCode"
                Aggregate="CountDistinct" FooterText="Mitigations: " UniqueName="MitigationCode">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="SodFunctionCode1" FilterControlAltText="SodFunctionCode1" HeaderText="Function1" SortExpression="SodFunctionCode1"
                Aggregate="CountDistinct" FooterText="Function1: " UniqueName="SodFunctionCode1">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Role1" FilterControlAltText="Role1" HeaderText="Function Value 1" SortExpression="Role1"
                Aggregate="CountDistinct" FooterText="Function Value 1: " UniqueName="Role1">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="SodFunctionCode2" FilterControlAltText="SodFunctionCode2" HeaderText="Function2" SortExpression="SodFunctionCode2"
                Aggregate="CountDistinct" FooterText="Function2: " UniqueName="SodFunctionCode2">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Role2" FilterControlAltText="Role2" HeaderText="Function Value 2" SortExpression="Role2"
                Aggregate="CountDistinct" FooterText="Function Value 2: " UniqueName="Role2">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="ExceptionCode" FilterControlAltText="ExceptionCode" HeaderText="Exception Code" SortExpression="ExceptionCode"
                Aggregate="CountDistinct" FooterText="Exception Codes: " UniqueName="ExceptionCode">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>
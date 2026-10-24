<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Reports.OperationDetailedReports.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-4 control-label">Select Run Code</label>
                        <div id="DivReportDropDown" class="col-xs-7">
                            <asp:DropDownList ID="ReportIdCom" runat="server" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<cc1:IACMRadGridControl ID="ComplianceReportGrid" runat="server" OnNeedDataSource="ComplianceReportGrid_NeedDataSource" OnItemCommand="ComplianceReportGrid_ItemCommand">
    <MasterTableView  DataKeyNames="SODReportId">
        <GroupByExpressions>
            <telerik:GridGroupByExpression>
                <SelectFields>
                    <telerik:GridGroupByField HeaderText="Conflict Code" FieldName="conflict.SODConflictCode"></telerik:GridGroupByField>
                </SelectFields>
                <GroupByFields>
                    <telerik:GridGroupByField FieldName="conflict.SODConflictCode" SortOrder="None"></telerik:GridGroupByField>
                </GroupByFields>
            </telerik:GridGroupByExpression>
            <telerik:GridGroupByExpression>
                <SelectFields>
                    <telerik:GridGroupByField HeaderText="UserName" FieldName="UserName"></telerik:GridGroupByField>
                </SelectFields>
                <GroupByFields>
                    <telerik:GridGroupByField FieldName="UserName" SortOrder="None"></telerik:GridGroupByField>
                </GroupByFields>
            </telerik:GridGroupByExpression>
        </GroupByExpressions>

        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="true" ShowExportToPdfButton="true" ShowExportToCsvButton="true"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="RunPolicy.UniqueRunCode" FilterControlAltText="RunCode" HeaderText="RunCode" SortExpression="RunPolicy.UniqueRunCode" UniqueName="RunCode"
                Aggregate="Count" FooterText="Total Conflicts: ">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="RunPolicy.RunDate" FilterControlAltText="RunDate" HeaderText="Execution Date" SortExpression="RunPolicy.RunDate" UniqueName="RunDate"
                Aggregate="CountDistinct" FooterText="Total Dates: ">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="RunPolicy.compliancepolicy.CompliancePolicyCode" FilterControlAltText="CompliancePolicyCode" HeaderText="Compliance Code" SortExpression="RunPolicy.compliancepolicy.CompliancePolicyCode" UniqueName="CompliancePolicyCode">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="Conflict.SODConflictCode" FilterControlAltText="Conflict.SODConflictCode" HeaderText="Conflict Code" SortExpression="Conflict.SODConflictCode"
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

            <telerik:GridBoundColumn DataField="conflict.SodRiskId.RiskCode" FilterControlAltText="conflict.SodRiskId.RiskCode" HeaderText="Risk Code" SortExpression="conflict.SodRiskId.RiskCode"
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

            <telerik:GridBoundColumn DataField="function1.SodFunctionCode" FilterControlAltText="function1.SodFunctionCode" HeaderText="Function1" SortExpression="function1.SodFunctionCode"
                Aggregate="CountDistinct" FooterText="Functions: " UniqueName="SodFunctionCode1">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Role1" FilterControlAltText="Role1" HeaderText="Function Value 1" SortExpression="Role1"
                Aggregate="CountDistinct" FooterText="Values: " UniqueName="Role1">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="AuthorizationCode1" FilterControlAltText="AuthorizationCode1" HeaderText="Authorization Code 1" SortExpression="AuthorizationCode1"
                Aggregate="CountDistinct" FooterText="Authorizations: " UniqueName="AuthorizationCode1">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="ProfileCode1" FilterControlAltText="ProfileCode1" HeaderText="Profile Code 1" SortExpression="ProfileCode1"
                Aggregate="CountDistinct" FooterText="Profiles: " UniqueName="ProfileCode1">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RoleCode1" FilterControlAltText="RoleCode1" HeaderText="Role Code 1" SortExpression="RoleCode1"
                Aggregate="CountDistinct" FooterText="Roles: " UniqueName="RoleCode1">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="FunctionObjItem1.ObjectCode" FilterControlAltText="FunctionObjItem1.ObjectCode1" HeaderText="ObjectCode 1" SortExpression="FunctionObjItem1.ObjectCode1"
                Aggregate="CountDistinct" FooterText="Objects: " UniqueName="ObjectCode1">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="FunctionObjItem1.FieldCode" FilterControlAltText="FieldCode1" HeaderText="Field Code 1" SortExpression="FunctionObjItem1.FieldCode"
                Aggregate="CountDistinct" FooterText="Field Codes: " UniqueName="FieldCode1">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="FunctionObjItem1.FromValue" FilterControlAltText="Value1" HeaderText="Value 1" SortExpression="FunctionObjItem1.Value1"
                Aggregate="CountDistinct" FooterText="Field Values: " UniqueName="Value1">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="function2.SodFunctionCode" FilterControlAltText="function2.SodFunctionCode" HeaderText="Function2" SortExpression="function2.SodFunctionCode"
                Aggregate="CountDistinct" FooterText="Function Codes: " UniqueName="SodFunctionCode2">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Role2" FilterControlAltText="Role2" HeaderText="Function Value 2" SortExpression="Role2"
                Aggregate="CountDistinct" FooterText="Function Values: " UniqueName="Role2">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="AuthorizationCode2" FilterControlAltText="AuthorizationCode2" HeaderText="Authorization Code 2" SortExpression="AuthorizationCode2"
                Aggregate="CountDistinct" FooterText="Authorizations: " UniqueName="AuthorizationCode2">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="ProfileCode2" FilterControlAltText="ProfileCode2" HeaderText="Profile Code 2" SortExpression="ProfileCode2"
                Aggregate="CountDistinct" FooterText="Profiles: " UniqueName="ProfileCode2">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RoleCode2" FilterControlAltText="RoleCode2" HeaderText="Role Code 2" SortExpression="RoleCode2"
                Aggregate="CountDistinct" FooterText="Roles: " UniqueName="RoleCode2">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="FunctionObjItem2.ObjectCode" FilterControlAltText="ObjectCode2" HeaderText="ObjectCode 2" SortExpression="FunctionObjItem2.ObjectCode"
                Aggregate="CountDistinct" FooterText="Objects: " UniqueName="ObjectCode2">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="FunctionObjItem2.FieldCode" FilterControlAltText="FieldCode2" HeaderText="Field Code 2" SortExpression="FunctionObjItem2.FieldCode"
                Aggregate="CountDistinct" FooterText="Field Codes: " UniqueName="FieldCode2">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="FunctionObjItem2.FromValue" FilterControlAltText="Value2" HeaderText="Value 2" SortExpression="FunctionObjItem2.FromValue"
                Aggregate="CountDistinct" FooterText="Field Values: " UniqueName="Value2">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="exception.ExceptionCode" FilterControlAltText="ExceptionCode" HeaderText="Exception Code" SortExpression="exception.ExceptionCode"
                UniqueName="ExceptionCode">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>
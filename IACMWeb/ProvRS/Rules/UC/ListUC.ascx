<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.ProvRS.Rules.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
 <cc1:IACMRadGridControl ID="NRuleSetGrid" runat="server" OnNeedDataSource="NRuleSetGrid_NeedDataSource" OnSelectedIndexChanged="NRuleSetGrid_SelectedIndexChanged">
                <MasterTableView DataKeyNames="Id,RuleId" TableLayout="Fixed"  Width="100%">
                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                    <Columns>
                        <telerik:GridBoundColumn DataField="Code" HeaderText="Code" SortExpression="Code" UniqueName="Code" FilterControlWidth="68%">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Description" HeaderText="Description" SortExpression="Description" UniqueName="Description" FilterControlWidth="68%">
                        </telerik:GridBoundColumn>
                         <telerik:GridBoundColumn DataField="Value" HeaderText="Value" SortExpression="Value" UniqueName="Value" FilterControlWidth="70%">
                        </telerik:GridBoundColumn>
                        <telerik:GridDateTimeColumn DataField="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" FilterControlWidth="90%">
                        </telerik:GridDateTimeColumn>
                        <telerik:GridBoundColumn DataField="CreatedBy" HeaderText="Created By" SortExpression="CreatedBy" UniqueName="CreatedBy" FilterControlWidth="70%">
                        </telerik:GridBoundColumn>
                        <telerik:GridDateTimeColumn DataField="LastModifiedDate" HeaderText="Last Modified Date" SortExpression="LastModifiedDate" UniqueName="LastModifiedDate" FilterControlWidth="90%">
                        </telerik:GridDateTimeColumn>
                        <telerik:GridBoundColumn DataField="LastModifiedBy" HeaderText="Last Modified By" SortExpression="LastModifiedBy" UniqueName="LastModifiedBy" FilterControlWidth="70%">
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </cc1:IACMRadGridControl>

           
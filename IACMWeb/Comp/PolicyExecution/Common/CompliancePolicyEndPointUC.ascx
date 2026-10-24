<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompliancePolicyEndPointUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.PolicyExecution.Common.CompliancePolicyEndPointUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                   
                    <div class="form-group">
                       <telerik:RadGrid ID="radExecutionOptions" runat="server" AllowSorting="True" CellSpacing="0" GridLines="None" OnPreRender="radExecutionOptions_PreRender">
                           <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" DataKeyNames="Id" EnableHeaderContextMenu="True">
                               <CommandItemSettings ShowAddNewRecordButton="False" ShowRefreshButton="False"></CommandItemSettings>
                                 <Columns>
                                     
                                   <telerik:GridBoundColumn AllowFiltering="False" DataField="EndpointSystemType.EndpointSystemTypeName" FilterControlAltText="EndpointSystemTypeName" ShowFilterIcon="False" SortExpression="EndpointSystemTypeName" UniqueName="EndpointSystemTypeName"></telerik:GridBoundColumn>
                                   <telerik:GridBoundColumn AllowFiltering="False" DataField="Level1" FilterControlAltText="Level1" HeaderText="" ShowFilterIcon="False" SortExpression="Level1" UniqueName="Level1"></telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn AllowFiltering="False" FilterControlAltText="Filter TemplateColumn column" HeaderText="" ShowFilterIcon="False" UniqueName="TemplateColumn">
                                     <ItemTemplate>
                                        <asp:CheckBox ID="chkLevel1" runat="server" />
                                     </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                   <telerik:GridBoundColumn AllowFiltering="False" DataField="Level2" FilterControlAltText="Level2" HeaderText="" ShowFilterIcon="False" SortExpression="Level2" UniqueName="Level2"></telerik:GridBoundColumn>
                                   <telerik:GridTemplateColumn AllowFiltering="False" FilterControlAltText="Filter TemplateColumn1 column" HeaderText="" ShowFilterIcon="False" UniqueName="TemplateColumn1">
                                    <ItemTemplate>
                                       <asp:CheckBox ID="chkLevel2" runat="server"/>
                                    </ItemTemplate>
                                   </telerik:GridTemplateColumn>
                                   <telerik:GridBoundColumn AllowFiltering="False" DataField="Level3" FilterControlAltText="Level3" HeaderText="" ShowFilterIcon="False" SortExpression="Level3" UniqueName="Level3"></telerik:GridBoundColumn>
                                   <telerik:GridTemplateColumn AllowFiltering="False" FilterControlAltText="Filter TemplateColumn2 column" HeaderText="" ShowFilterIcon="False" UniqueName="TemplateColumn2">
                                    <ItemTemplate>
                                   <asp:CheckBox ID="chkLevel3" runat="server" />
                                   </ItemTemplate>
                                   </telerik:GridTemplateColumn>
                                   <telerik:GridBoundColumn AllowFiltering="False" DataField="Level4" FilterControlAltText="Level4" HeaderText="" ShowFilterIcon="False" SortExpression="Level4" UniqueName="Level4"></telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn AllowFiltering="False" FilterControlAltText="Filter TemplateColumn3 column" HeaderText="" ShowFilterIcon="False" UniqueName="TemplateColumn3">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkLevel4" runat="server" />
                                    </ItemTemplate>
                                       </telerik:GridTemplateColumn>
                                   </Columns>
                                  </MasterTableView>
                              </telerik:RadGrid>
                    </div>
                    
                </div>
            </div>
        </div>
    </div>
    </div>

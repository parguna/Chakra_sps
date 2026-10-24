<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RejectedReviewsUC.ascx.cs" Inherits="DI.Chakra.Web.Report.Attestation.RejectedReviewsUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadGrid runat="server" ID="RejectedReviews" AutoGenerateColumns="False" OnDetailTableDataBind="RejectedReviews_DetailTableDataBind" GroupPanelPosition="Top"
    OnNeedDataSource="RejectedReviews_NeedDataSource" AllowPaging="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" AllowFilteringByColumn ="true">
       <PagerStyle Mode="NextPrevAndNumeric" PageSizes="10" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" 
        CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="Id" 
        NoMasterRecordsText="No Records Available"  >
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn SortExpression="AttestationName" HeaderText="Attestation Name" HeaderButtonType="TextButton" DataField="AttestationName" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="AttestationDecsription" HeaderText="Attestation Decsription" HeaderButtonType="TextButton" DataField="AttestationDecsription" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
              <telerik:GridBoundColumn SortExpression="WorkFlow" HeaderText="WorkFlow Name" HeaderButtonType="TextButton" DataField="WorkFlow" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="RequiresAttestationRemediation" HeaderText="Requires AttestationRemediation" HeaderButtonType="TextButton" DataField="RequiresAttestationRemediation" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
        </Columns>
        <DetailTables>
              <telerik:GridTableView DataKeyNames="TaskId" Name="ReviewerDetails" ShowHeadersWhenNoRecords="false" 
                NoDetailRecordsText="No Records found">
                <Columns>
                    <telerik:GridBoundColumn SortExpression="ReviewerName" HeaderText="Reviewer Name" HeaderButtonType="TextButton" DataField="ReviewerName" HeaderStyle-Width="34%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="DisplayName" HeaderText="Display Name" HeaderButtonType="TextButton" DataField="DisplayName" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="EmailId" HeaderText="Email Id" HeaderButtonType="TextButton" DataField="EmailId" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>

                </Columns>
                  <DetailTables>
                      <telerik:GridTableView DataKeyNames="ReviewId" Name="AttestationReviewDetails" ShowHeadersWhenNoRecords="false" 
                NoDetailRecordsText="No Records found">
                <Columns>
                    <telerik:GridBoundColumn SortExpression="UserName" HeaderText="User Name" HeaderButtonType="TextButton" DataField="UserName" HeaderStyle-Width="34%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="DisplayName" HeaderText="Display Name" HeaderButtonType="TextButton" DataField="DisplayName" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="EmailId" HeaderText="Email Id" HeaderButtonType="TextButton" DataField="EmailId" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>

                </Columns>
                          <DetailTables>
                               <telerik:GridTableView DataKeyNames="ReviewEndpointId" Name="UserEndPointSystemDetails" ShowHeadersWhenNoRecords="false" 
                NoDetailRecordsText="No Records found">
                <Columns>
                    <telerik:GridBoundColumn SortExpression="EndPointUserName" HeaderText="EndPoint UserName" HeaderButtonType="TextButton" DataField="EndPointUserName" HeaderStyle-Width="34%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="HostName" HeaderText="Host Name" HeaderButtonType="TextButton" DataField="HostName" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="TargetSystem" HeaderText="Target System" HeaderButtonType="TextButton" DataField="TargetSystem" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Approved" HeaderText="Approved" HeaderButtonType="TextButton" DataField="Approved" HeaderStyle-Width="34%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Rejected" HeaderText="Rejected" HeaderButtonType="TextButton" DataField="Rejected" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Comments" HeaderText="Comments" HeaderButtonType="TextButton" DataField="Comments" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                </Columns>
                                   <DetailTables >
                                        <telerik:GridTableView  Name="UserEndPointAccessDetails" ShowHeadersWhenNoRecords="false" 
                NoDetailRecordsText="No Records found">
                <Columns>
                    <telerik:GridBoundColumn SortExpression="AccessName" HeaderText="Access Name" HeaderButtonType="TextButton" DataField="AccessName" HeaderStyle-Width="34%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="AccessType" HeaderText="Access Type" HeaderButtonType="TextButton" DataField="AccessType" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                   
                    <telerik:GridBoundColumn SortExpression="Approved" HeaderText="Approved" HeaderButtonType="TextButton" DataField="Approved" HeaderStyle-Width="34%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Rejected" HeaderText="Rejected" HeaderButtonType="TextButton" DataField="Rejected" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                 
                </Columns>
                                            </telerik:GridTableView> 

                                   </DetailTables>
                                   </telerik:GridTableView>
                          </DetailTables>
                          </telerik:GridTableView> 
                  </DetailTables>
            </telerik:GridTableView>

          <%--  <telerik:GridTableView DataKeyNames="ReviewerId" Name="ReviewerDetails" ShowHeadersWhenNoRecords="false" 
                NoDetailRecordsText="No Records found">
                <Columns>
                    <telerik:GridBoundColumn SortExpression="ReviewerName" HeaderText="Reviewer Name" HeaderButtonType="TextButton" DataField="ReviewerName" HeaderStyle-Width="34%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="DisplayName" HeaderText="Display Name" HeaderButtonType="TextButton" DataField="DisplayName" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="EmailId" HeaderText="Email Id" HeaderButtonType="TextButton" DataField="EmailId" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>

                </Columns>
            </telerik:GridTableView>--%>
            
        </DetailTables>
    </MasterTableView>
</telerik:RadGrid>
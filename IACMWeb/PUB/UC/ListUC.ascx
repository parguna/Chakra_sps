<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.PUB.UC.ListUC" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:UpdatePanel runat="server" ID="upnlPUB">
    <ContentTemplate>
        
        <cc1:IACMRadGridControl ID="grdPUBFiles" runat="server" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CellSpacing="0" AllowCustomPaging="true" PageSize="50"
            ClientSettings-EnablePostBackOnRowClick="true" DisableRowSelect="true" Width="100%" Height="100%"
            OnNeedDataSource="Users_NeedDataSource" OnSelectedIndexChanged="Users_SelectedIndexChanged"   OnItemCommand="Users_ItemCommand"  >
            <PagerStyle Mode="NextPrevAndNumeric" PageSizes="50" Height="100%" AlwaysVisible="true" />
            <ClientSettings>
                <Scrolling AllowScroll="true" UseStaticHeaders="true" />
            </ClientSettings>
            <ClientSettings Selecting-AllowRowSelect="true" EnablePostBackOnRowClick="true">
            </ClientSettings>
            <MasterTableView AutoGenerateColumns="False" Width="130%" CommandItemDisplay="Top" ShowHeadersWhenNoRecords="true" DataKeyNames="Id" NoMasterRecordsText="No Records Available" TableLayout="Fixed" PageSize="50">
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>

                <Columns>

                    <telerik:GridBoundColumn HeaderText="Id" DataField="Id" FilterControlAltText="Id" SortExpression="Id" UniqueName="Id" HeaderStyle-Width="5%" FilterControlWidth="5%">
                    </telerik:GridBoundColumn>

                    <telerik:GridDateTimeColumn HeaderText="Received/Send Date" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" DataField="FileDateTime" FilterControlAltText="FileDateTime" SortExpression="FileDateTime" UniqueName="FileDateTime" HeaderStyle-Width="10%" FilterControlWidth="10%">
                    </telerik:GridDateTimeColumn>

                     <telerik:GridTemplateColumn DataField="FileName" FilterControlAltText="FileName" HeaderText="File Name" SortExpression="FileName" UniqueName="FileName" HeaderStyle-Width="15%" FilterControlWidth="15%">
                     <ItemTemplate>
                        <asp:LinkButton ID="lnkPUBFileStatus" runat="server" Text='<%#Eval("FileName")%>' CommandArgument='<%#Eval("Id")%>' CommandName="FileDonwload"    />
                     </ItemTemplate>
                     </telerik:GridTemplateColumn>

                    <telerik:GridBoundColumn DataField="PUBFileStatus" FilterControlAltText="PUBFileStatus" HeaderText="Status" SortExpression="PUBFileStatus" UniqueName="PUBFileStatus" HeaderStyle-Width="10%" FilterControlWidth="10%">
                     
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn DataField="PUBFileSource"   FilterControlAltText="PUBFileSource" HeaderText="Source" SortExpression="PUBFileSource" UniqueName="PUBFileSource" HeaderStyle-Width="7%" FilterControlWidth="7%">
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn DataField="PUBFileType" FilterControlAltText="PUBFileType" HeaderText="File Type" SortExpression="PUBFileType" UniqueName="PUBFileType" HeaderStyle-Width="7%" FilterControlWidth="7%">
                    </telerik:GridBoundColumn>

                     <telerik:GridDateTimeColumn DataField="ReportedDateTime" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" FilterControlAltText="ReportedDateTime" HeaderText="Reported DateTime" SortExpression="ReportedDateTime" UniqueName="ReportedDateTime" HeaderStyle-Width="10%" FilterControlWidth="10%">
                    </telerik:GridDateTimeColumn>

                    <telerik:GridTemplateColumn DataField="ReportFileName" FilterControlAltText="ReportFileName" HeaderText="Reported File Name" SortExpression="ReportFileName" UniqueName="ReportFileName" HeaderStyle-Width="10%" FilterControlWidth="10%">
                    <ItemTemplate>
                    <asp:LinkButton ID="lnkReportedStatus" runat="server" Text='<%#Eval("ReportFileName") %>' CommandArgument='<%#Eval("Id")%>'  CommandName="FileDonwload"   />
                    </ItemTemplate> 
                    </telerik:GridTemplateColumn> 

                    <telerik:GridDateTimeColumn DataField="ReportedStatus" FilterControlAltText="ReportedStatus" HeaderText="File Reported Status" SortExpression="ReportedStatus" UniqueName="ReportedStatus" HeaderStyle-Width="10%" FilterControlWidth="10%">
                     
                    </telerik:GridDateTimeColumn>

                   

                    <telerik:GridBoundColumn DataField="ValidationMsg" FilterControlAltText="ValidationMsg" HeaderText="Validation Message" SortExpression="ValidationMsg" UniqueName="ValidationMsg" HeaderStyle-Width="10%" FilterControlWidth="5%">
                    </telerik:GridBoundColumn>

                   <%-- <telerik:GridBoundColumn DataField="Remarks" FilterControlAltText="Remarks" HeaderText="Remarks" SortExpression="Remarks" UniqueName="Remarks" HeaderStyle-Width="10%" FilterControlWidth="10%">
                    </telerik:GridBoundColumn>--%>

                   


                    <telerik:GridDateTimeColumn DataField="CreatedDate" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" FilterControlAltText="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" HeaderStyle-Width="7%" FilterControlWidth="7%">
                    </telerik:GridDateTimeColumn>

                    <telerik:GridBoundColumn DataField="CreatedBy" FilterControlAltText="CreatedBy" HeaderText="Created By" SortExpression="CreatedBy" UniqueName="CreatedBy" HeaderStyle-Width="5%" FilterControlWidth="5%">
                    </telerik:GridBoundColumn>

                    <telerik:GridDateTimeColumn DataField="LastModifiedDate" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" FilterControlAltText="LastModifiedDate" HeaderText="Last ModifiedDate" SortExpression="LastModifiedDate" UniqueName="LastModifiedDate" HeaderStyle-Width="7%" FilterControlWidth="7%">
                    </telerik:GridDateTimeColumn>

                    <telerik:GridBoundColumn DataField="LastModifiedBy" FilterControlAltText="LastModifiedBy" HeaderText="Last ModifiedBy" SortExpression="LastModifiedBy" UniqueName="LastModifiedBy" HeaderStyle-Width="5%" FilterControlWidth="5%">
                    </telerik:GridBoundColumn>




                </Columns>
            </MasterTableView>
        </cc1:IACMRadGridControl>
        <script type="text/javascript">
            $(window).bind('beforeunload', function () {
                try {
                    $("#UpdateProgressBar").hide();
                } catch (e) {

                }
            });    
        </script>
    </ContentTemplate>

    <Triggers>

         <asp:PostBackTrigger ControlID="grdPUBFiles" />
    </Triggers>
   
</asp:UpdatePanel>


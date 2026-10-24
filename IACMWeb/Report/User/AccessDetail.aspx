<%@ Page Title="User AccessDetail" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="AccessDetail.aspx.cs" Inherits="DI.Chakra.Web.Report.User.AccessDetail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">    
                
                    <div runat="server" class="form-group" id="autocompany"  visible="true">
                        <asp:Label AssociatedControlID="autoCompleteCompany" ID="Label1" runat="server" CssClass="col-xs-2 control-label">Company</asp:Label>
                        <div class="col-xs-5">
                            <telerik:RadAutoCompleteBox ID="autoCompleteCompany" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
                                <TextSettings SelectionMode="Multiple" />
                                <WebServiceSettings Method="GetCompanies" Path="../../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                           
                        </div>
                    </div>                    
                            
                    <div class="form-group" runat="server" id="autoDept" visible="false">
                                        
                        <asp:Label ID="Label2" AssociatedControlID="autoCompleteDepartment" runat="server" CssClass="col-xs-2 control-label">Department</asp:Label>
                        <div class="col-xs-5">    
                            
                            <telerik:RadAutoCompleteBox ID="autoCompleteDepartment" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
                                <TextSettings SelectionMode="Multiple" />
                                <WebServiceSettings Method="GetDepartments" Path="../../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                          </div>                                  
                                    
                    </div>

                    <div runat="server" id="autoDivision" visible="false" class="form-group">
                                        
                        <asp:Label ID="Label3" AssociatedControlID="autoCompleteDivision" runat="server" CssClass="col-xs-2 control-label">Division</asp:Label>
                        <div class="col-xs-5">   
                            
                            <telerik:RadAutoCompleteBox ID="autoCompleteDivision" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
                                <TextSettings SelectionMode="Multiple" />
                                <WebServiceSettings Method="GetDivisionsByDeptID" Path="../../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                            </div>            
                    </div>

                    <div runat="server" id="autoSection" visible="false" class="form-group">
                                        
                        <asp:Label ID="lblSectionName" AssociatedControlID="autoCompleteSection" runat="server" CssClass="col-xs-2 control-label">Section</asp:Label>
                        <div class="col-xs-5"> 
                            <telerik:RadAutoCompleteBox ID="autoCompleteSection" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
                                <TextSettings SelectionMode="Multiple" />
                                <WebServiceSettings Method="GetDisticntSectionFromUsers" Path="../../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                          </div>             
                    </div>                              
                    
                    <div class="form-group">
                        <asp:Label ID="Label5" AssociatedControlID="UserAutoCompleteBox" runat="server" CssClass="col-xs-2 control-label">User Name</asp:Label>                   
                        <div class="col-xs-5">  
                            
                            <telerik:RadAutoCompleteBox ID="UserAutoCompleteBox" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
                                <TextSettings SelectionMode="Multiple" />
                                <WebServiceSettings Method="GetReportingUsersOfThisManagerIDandAdmin" Path="../../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                         <asp:Label ID="lblMsg" runat="server" ForeColor="Red" CssClass="errormsg"></asp:Label>   
                        </div>                         
                    </div>

                    <div class="form-group">
                        <div class="buttons text-right col-xs-7">
                            <asp:Button ID="btnSubmit" CssClass="btn btn-default btn-sm" runat="server" OnClick="btnSubmit_Click" Text="Submit" />
                        </div>
                    </div>                    
                </div>              
            </div>
        </div>
        <div runat="server" id="buttonsTR" class="buttons text-right nav-rectangle">     
              <asp:Button ID="btnShowALL" CssClass="btn btn-default btn-sm" runat="server" OnClick="btnShowALL_Click" Text="All Users"/>
            <asp:CustomValidator ID="CompanyValidator" Visible="false" runat="server" OnServerValidate="CompanyValidator_ServerValidate" ErrorMessage="Please Enter at least One Of The Above Fields">
            </asp:CustomValidator>  
          </div>
    </div>
    </div>

<telerik:RadGrid ID="UserRadGridTreeList" ShowStatusBar="True" GridLines="None" CellSpacing="0"
runat="server" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="false"
OnDetailTableDataBind="UserRadGridTreeList_DetailTableDataBind" 
OnNeedDataSource="UserRadGridTreeList_NeedDataSource" FilterMenu-CssClass="RadMenu_IACMFilterSkin" ClientSettings-EnablePostBackOnRowClick="true">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%"  AlwaysVisible="true"/>
<ClientSettings>
      <Scrolling AllowScroll="True" UseStaticHeaders="true"/>
    </ClientSettings>

    <GroupingSettings CaseSensitive="false" />

    
    <MasterTableView AutoGenerateColumns="False"
            CommandItemDisplay="None" EnableHeaderContextMenu="false" TableLayout="fixed"
            DataKeyNames="Id" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
            <CommandItemSettings ShowAddNewRecordButton="false"
                ShowRefreshButton="true"></CommandItemSettings>
        <DetailTables>
            <telerik:GridTableView DataKeyNames="Id,Name,EndpointSystemID" EnableHierarchyExpandAll="true"  Width="100%"
                runat="server" Name="Roles" NoDetailRecordsText="No records found.">
                <DetailTables>
            <telerik:GridTableView  EnableHierarchyExpandAll="true"  Width="100%"
                runat="server" Name="TCode" DataKeyNames="TCODE_Code,Role,EndpointSystemID" NoDetailRecordsText="No records found.">
                <DetailTables>
                    <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="sapLevel1DetailId" Width="100%"
                        runat="server" Name="Authorization" AllowPaging="false" NoDetailRecordsText="No records found.">
                        <Columns>
                            <telerik:GridBoundColumn SortExpression="sapLevel1Detail" HeaderText="Authorization"
                                DataField="sapLevel1Detail" UniqueName="DetailSapLevel1GridBoundColumn">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="sapLevel1DetailDesc" HeaderText="Description"
                                DataField="sapLevel1DetailDesc" UniqueName="DetailTypeSapLevel1GridBoundColumn">
                            </telerik:GridBoundColumn>                                                        
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>                                                                                     
                    <telerik:GridBoundColumn SortExpression="TCODE_Code" HeaderText="TCodes"
                        DataField="TCODE_Code" UniqueName="TCODE_Code">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="TCODE_Desc" HeaderText="Description"
                        DataField="TCODE_Desc" UniqueName="TCODE_Desc">
                    </telerik:GridBoundColumn>
                                                
                </Columns>
            </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Role" UniqueName="Name">
                    </telerik:GridBoundColumn>                                                
                        <telerik:GridBoundColumn DataField="Type" HeaderText="Type" UniqueName="Type">
                    </telerik:GridBoundColumn>                                                   

                </Columns>
            </telerik:GridTableView>
        </DetailTables>

        <Columns>
            <telerik:GridBoundColumn DataField="FullName" HeaderText="Full Name" UniqueName="FullName">
            </telerik:GridBoundColumn>                                        
            <telerik:GridBoundColumn DataField="UserName" HeaderText="User Name" UniqueName="UserName">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Name" HeaderText="Account Name" UniqueName="Name">
            </telerik:GridBoundColumn>                                                                       
            <telerik:GridBoundColumn DataField="EndpointSystemName" HeaderText="Application Type" UniqueName="EndpointSystemName">
            </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="EndpointHostName" HeaderText="Application Name" UniqueName="HostName">
            </telerik:GridBoundColumn>                                       
            <telerik:GridDateTimeColumn DataField="LastLogonDate" HeaderText="Last Logon" UniqueName="LastLogonDate">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn DataField="AccountType" HeaderText="Type" UniqueName="AccountType">
            </telerik:GridBoundColumn>
        </Columns>

    </MasterTableView>
</telerik:RadGrid> 	

</asp:Content>

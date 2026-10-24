<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Mail.Mail.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-5">
                    <div class="form-group">
                        <asp:Label ID="lblActivityType" AssociatedControlID="ddlActivityType" runat="server" CssClass="col-xs-3 control-label" Text="Activity Type"></asp:Label>
                        <div class="col-xs-8">
                            <asp:DropDownList ID="ddlActivityType" runat="server" AppendDataBoundItems="True" AutoPostBack="True" CssClass="form-control selectpicker" data-width="100%">
                                <asp:ListItem>All</asp:ListItem>
                                <asp:ListItem>None</asp:ListItem>
                                <asp:ListItem>Discovery</asp:ListItem>
                                <asp:ListItem>Attestation</asp:ListItem>
                                <asp:ListItem>Provisioning</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="col-xs-5">
                    <div class="form-group">
                        <asp:Label ID="lblStatus" AssociatedControlID="ddlStatus" runat="server" Text="Status" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-8">
                            <asp:DropDownList ID="ddlStatus" runat="server" AppendDataBoundItems="True" CssClass="form-control selectpicker" data-width="100%">
                                <asp:ListItem>All</asp:ListItem>
                                <asp:ListItem>Pending</asp:ListItem>
                                <asp:ListItem>Failed</asp:ListItem>
                                <asp:ListItem>Sent</asp:ListItem>
                                <asp:ListItem>Excluded</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                
            </div>
            
            <div class="row-fluid">
               <div class="col-xs-5">
                           <div class="form-group required">
                    <label for="From" class="col-xs-3 control-label">From</label>
                    <div class="col-xs-3">
                        <telerik:RadDatePicker runat="server" ID="DatePickerValidFrom" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                            DatePopupButton-Visible="true" Width="100%" CssClass="form-control" AutoPostBack="true">
                            <Calendar ID="Calendar1" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                <FooterTemplate>
                                    <div style="width: 100%; text-align: center; background-color: Gray;">
                                        <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('FButton')" AutoPostBack="false" />
                                    </div>
                                </FooterTemplate>
                            </Calendar>
                            <DateInput ID="DateInput1" ReadOnly="false" runat="server" ToolTip="Date input For From Date"
                                BorderStyle="NotSet" Label="" AutoPostBack="false">
                            </DateInput>
                        </telerik:RadDatePicker>
                    </div>
                           </div>     
                      </div>          
                    <div class="col-xs-5">
                         <div class="form-group required">
                    <label for="To" class="col-xs-3 control-label">To</label>
                    <div class="col-xs-3">
                        <telerik:RadDatePicker runat="server"  ID="DatePickerValidTo" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                            DatePopupButton-Visible="true" Width="100%" CssClass="form-control" AutoPostBack="false">
                            <Calendar ID="Calendar2" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                <FooterTemplate>
                                    <div style="width: 100%; text-align: center; background-color: Gray;">
                                        <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('TButton')" AutoPostBack="false" />
                                    </div>
                                </FooterTemplate>
                            </Calendar>
                            <DateInput ID="DateInput2" ReadOnly="false" runat="server" ToolTip="Date input For End Date"
                                BorderStyle="NotSet" Label="" AutoPostBack="false">
                            </DateInput>
                        </telerik:RadDatePicker>
                    </div>
                         </div>   
                   </div>             
            </div>    
           
            <div class="row-fluid">
                <div class="col-xs-5">
                    <div class="form-group">
                        <div class="col-xs-3">
                        &nbsp;
                        </div>
                        <div class="col-xs-3">
                             <div class="buttons">
                            <asp:Button ID="btnSearch" Text="Search" runat="server" OnClick="btnSearch_Click"
                                CssClass="btn btn-default btn-sm" />
                        </div>
                        </div>
                    </div>
                </div>
                </div>
             <div class="row-fluid">
                <div class="col-xs-5">
                    <div class="form-group">
                        <label for="From" class="col-xs-3 control-label">Export All</label>
                         <div class="col-xs-3">
                             <asp:CheckBox runat="server" ID="chkExportAll" Checked="false" />
                         </div>   
                    </div>
                </div>
            </div>
            </div>
        </div>
    </div>
<div id="divDateTime" runat="server">
</div>
 <cc1:IACMRadGridControl ID="gvEmailViewer" runat="server" OnNeedDataSource="gvEmailViewer_NeedDataSource" OnSelectedIndexChanged="gvEmailViewer_SelectedIndexChanged" AllowCustomPaging="true">
     <ClientSettings>
         <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true" />
     </ClientSettings>
        <MasterTableView NoMasterRecordsText="No Records Available" DataKeyNames="Id" TableLayout="Fixed" Width="100%">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
            <telerik:GridBoundColumn DataField="Subject" FilterControlAltText="Subject" SortExpression="Subject" HeaderText="Subject" UniqueName="Subject" HeaderStyle-Width="30%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="To" FilterControlAltText="To" SortExpression="To" HeaderText="To" UniqueName="To" HeaderStyle-Width="25%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn DataField="CC" FilterControlAltText="CC" SortExpression="CC" HeaderText="CC" UniqueName="CC" HeaderStyle-Width="25%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="Bcc" FilterControlAltText="Bcc" SortExpression="Bcc" HeaderText="Bcc" UniqueName="Bcc" HeaderStyle-Width="20%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="ActivityType" FilterControlAltText="ActivityType" SortExpression="ActivityType" HeaderText="Activity Type" UniqueName="ActivityType" HeaderStyle-Width="15%" FilterControlWidth="77%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status" HeaderStyle-Width="10%" FilterControlWidth="68%">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn DataField="CreatedDate" DataFormatString="{0:dd-MM-yyyy hh:mm:ss tt}" FilterControlAltText="CreatedDate" SortExpression="CreatedDate" HeaderText="Created Date & Time" UniqueName="CreatedDate" HeaderStyle-Width="20%" FilterControlWidth="87%"
                 EnableTimeIndependentFiltering="true">
            </telerik:GridDateTimeColumn>
            <telerik:GridDateTimeColumn DataField="SentDateTime" DataFormatString="{0:dd-MM-yyyy hh:mm:ss tt}" FilterControlAltText="SentDateTime" SortExpression="SentDateTime" HeaderText="Sent Date & Time" UniqueName="SentDateTime" HeaderStyle-Width="20%" FilterControlWidth="87%"
                 EnableTimeIndependentFiltering="true">
            </telerik:GridDateTimeColumn>

            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>

<%--<div class="buttons text-right nav-rectangle">
    <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btncancel_Click" />
</div>--%>
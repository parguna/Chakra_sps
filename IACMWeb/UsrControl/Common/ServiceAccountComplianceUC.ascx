<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServiceAccountComplianceUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.ServiceAccountComplianceUC" %>



<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxCtrl" %>

<telerik:RadGrid ID="RequestGrid" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                    GridLines="None" Width="100%" OnNeedDataSource="RequestGrid_NeedDataSource"  ShowFooter="false"
                    MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView AutoGenerateColumns="False"  >
                        <Columns>
                            <telerik:GridTemplateColumn HeaderStyle-Width="40%" HeaderText="Question" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="QuestionLabel" runat="server" Text='<%#Eval("Question") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="10%" HeaderText="Compliance" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="ComplianceLabel" runat="server" Text='<%#Eval("ComplianceResponse") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="50%" HeaderText="Comments" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="CommentsLabel" runat="server" Text='<%#Eval("Comments") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn> 
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
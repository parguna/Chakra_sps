<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Mail.Template.UC.DetailUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>

<script type="text/javascript">
    /*Telerik.Web.UI.Editor.Modules.RadEditorDomInspector.prototype._createRemoveLink = function () { }*/
    $(document).ready(function () {
        $('.floating-icon input').on('click', function () {
            __doPostBack(this.name, '');
            return false;
        });
    });


    function editorLoad(editor, args) {
        var contentAreaEl = editor.get_contentAreaElement();
        contentAreaEl.removeAttribute("title");

    }
    function OnClientModeChange(editor, args) {
        setTimeout(function () {
            editor.set_editable(false);
            editor.enableEditing(false);
            var tool = editor.getToolByName("ToggleScreenMode");
            tool.setState(0);

        }, 0);
    }
    function OnClientCommandExecuted(editor, args) {
        if ("AbsolutePosition" == args.get_commandName()) {
            var elem = editor.getSelectedElement();
            elem.style.position = "absolute";
            args.set_cancel(true);
        }
        if ("EnterNewLine" == args.get_commandName()) {
            var elem = editor.getSelectedElement();
            elem.style.position = "absolute";
            args.set_cancel(true);
        }

        if (args.get_commandName() === "ToggleScreenMode") {
            var editorElement = editor.get_element();
            if (editor.isFullScreen()) {
                editorElement.style.position = "absolute";
                editorElement.style.zIndex = "100000";
            } else {
                editorElement.style.position = "";
                editorElement.style.zIndex = "";
            }
        }
    }
</script>

<asp:Panel ID="ContentWrapper" runat="server" CssClass="content-wrapper">
    <div class="form">

        <asp:UpdatePanel runat="server">
            <ContentTemplate>

                <div class="form-horizontal left">
                    <div class="row-fluid">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <asp:Label ID="lblTempName" AssociatedControlID="txtTempName" runat="server" Text="Template Name" CssClass="col-xs-4 control-label"></asp:Label>
                                <div class="col-xs-7">
                                    <asp:TextBox ID="txtTempName" CssClass="form-control" Rows="3" runat="server" required="required"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblMailFrm" AssociatedControlID="txtMailFrm" runat="server" Text="From" CssClass="col-xs-4 control-label"></asp:Label>
                                <div class="col-xs-7">
                                    <asp:TextBox ID="txtMailFrm" CssClass="form-control" Rows="3" runat="server" required="required"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblMailSubj" AssociatedControlID="txtMailSubj" runat="server" Text=" Subject" CssClass="col-xs-4 control-label"></asp:Label>
                                <div class="col-xs-7">
                                    <asp:TextBox ID="txtMailSubj" CssClass="form-control" Rows="3" runat="server" required="required"></asp:TextBox>
                                </div>
                            </div>
                           
                            <div class="form-group">
                                <asp:Label ID="lblTo" AssociatedControlID="ddlTo" Text="To" runat="server" class="col-xs-4 control-label"></asp:Label>
                                <div class="col-xs-7">
                                    <asp:DropDownList ID="ddlTo" runat="server" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlTo_SelectedIndexChanged" data-width="100%" AppendDataBoundItems="true" AutoPostBack="true">
                                        <asp:ListItem>None</asp:ListItem>
                                        <asp:ListItem>User</asp:ListItem>
                                        <asp:ListItem>Free Text</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblToSelect" runat="server" CssClass="col-xs-4 control-label">&nbsp;</asp:Label>
                                <div class="col-xs-5">
                                    <uc1:UserSearchUC runat="server" ID="UserSearchUC" Visible="false" />
                                    <asp:TextBox ID="txtTo" CssClass="form-control" Rows="1" Visible="false" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-xs-1">
                                    <asp:Button ID="AddToEmail" runat="server" Text="Add" Visible="false" OnClick="AddToEmailButton_Click" CssClass="btn btn-default btn-sm" />
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="col-xs-4">
                                </div>
                                <div class="col-xs-7" style="overflow: hidden; display: inline-table;">
                                    <telerik:RadGrid ID="ToEmail_FormGrid" runat="server" AllowPaging="true" AllowSorting="false" Width="100%"
                                        OnItemCommand="ToEmail_ItemCommand" OnItemDataBound="ToEmail_ItemDataBound" OnPreRender="ToEmail_FormGrid_PreRender">
                                        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="false" />
                                        <ClientSettings>
                                            <Scrolling UseStaticHeaders="true" ScrollHeight="200px" />
                                            <Selecting AllowRowSelect="false" />
                                        </ClientSettings>
                                        <GroupingSettings CaseSensitive="false" />
                                        <MasterTableView DataKeyNames="Key" AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" CommandItemDisplay="None" EnableHeaderContextMenu="false" TableLayout="Auto"  Width="100%">
                                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="Key" FilterControlAltText="Key" SortExpression="Key" HeaderText="To Email Id" HeaderStyle-Width="70%" UniqueName="Key">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" SortExpression="Value" HeaderText="Status" HeaderStyle-Width="70%" UniqueName="Value">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="Delete" ConfirmText="Are you sure you want to delete ?" HeaderText="Delete" ConfirmDialogType="RadWindow"
                                                    UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="20%">
                                                    <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                                                </telerik:GridButtonColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <asp:Label ID="lblTempDesc" AssociatedControlID="txtTempDesc" runat="server" Text="Template Description" CssClass="col-xs-4 control-label"></asp:Label>
                                <div class="col-xs-7">
                                    <asp:TextBox ID="txtTempDesc" CssClass="form-control" Rows="3" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="form-group">
                                <asp:Label ID="lblCC" AssociatedControlID="ddlCC" Text="CC" runat="server" class="col-xs-4 control-label"></asp:Label>
                                <div class="col-xs-7">
                                    <asp:DropDownList ID="ddlCC" runat="server" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlCC_SelectedIndexChanged" data-width="100%" AppendDataBoundItems="true" AutoPostBack="true">
                                        <asp:ListItem>None</asp:ListItem>
                                        <asp:ListItem>User</asp:ListItem>
                                        <asp:ListItem>Free Text</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="Label2" runat="server" CssClass="col-xs-4 control-label">&nbsp;</asp:Label>
                                <div class="col-xs-5">
                                    <uc1:UserSearchUC runat="server" ID="CCUserSearchUC" Visible="false" />
                                    <asp:TextBox ID="txtCC" CssClass="form-control" Rows="1" Visible="false" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-xs-1">
                                    <asp:Button ID="AddCCEmailButton" runat="server" Text="Add" Visible="false" OnClick="AddCCEmailButton_Click" CssClass="btn btn-default btn-sm" />
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="col-xs-4">
                                </div>
                                <div class="col-xs-7" style="overflow: hidden; display: inline-table;">
                                    <telerik:RadGrid ID="CCEmail_FormGrid" runat="server" AllowPaging="true" AllowSorting="false" Width="100%"
                                        OnItemCommand="CCEmail_FormGrid_ItemCommand" OnItemDataBound="CCEmail_FormGrid_ItemDataBound" OnPreRender="CCEmail_FormGrid_PreRender">
                                        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="false" />
                                        <ClientSettings>
                                            <Scrolling UseStaticHeaders="true" ScrollHeight="200px" />
                                            <Selecting AllowRowSelect="false" />
                                        </ClientSettings>
                                        <GroupingSettings CaseSensitive="false" />
                                        <MasterTableView DataKeyNames="Key" AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" CommandItemDisplay="None" EnableHeaderContextMenu="false" TableLayout="Auto"  Width="100%">
                                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="Key" FilterControlAltText="Key" SortExpression="Key" HeaderText="CC Email Id" HeaderStyle-Width="70%" UniqueName="Key">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" SortExpression="Value" HeaderText="Status" HeaderStyle-Width="70%" UniqueName="Value">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="Delete" ConfirmText="Are you sure you want to delete ?" HeaderText="Delete" ConfirmDialogType="RadWindow"
                                                    UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="20%">
                                                    <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                                                </telerik:GridButtonColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12">
                            <div class="form-group">
                                <asp:Label ID="lblMailContent" AssociatedControlID="MailTemplateEditor" runat="server" Text="Mail Content" CssClass="col-xs-2 control-label"></asp:Label>
                                <div class="col-xs-10">
                                    <telerik:RadEditor ID="MailTemplateEditor" EditModes="All" EnableResize="true" Width="100%" Height="500px" runat="server" Skin="IACMSkin" DialogsCssFile="~/assets/styles/dialog.css">
                                        <Tools>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="Print" ShortCut="CTRL+P" />
                                                <telerik:EditorTool Name="AjaxSpellCheck" />
                                            </telerik:EditorToolGroup>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="Undo" ShortCut="CTRL+Z" />
                                                <telerik:EditorTool Name="Redo" ShortCut="CTRL+R" />
                                                <telerik:EditorSeparator />
                                                <telerik:EditorTool Name="FindAndReplace" ShortCut="CTRL+H" />
                                                <telerik:EditorTool Name="SelectAll" ShortCut="CTRL+A" />
                                                <telerik:EditorTool Name="Cut" ShortCut="CTRL+X" />
                                                <telerik:EditorTool Name="Copy" ShortCut="CTRL+C" />
                                                <telerik:EditorTool Name="Paste" ShortCut="CTRL+V" />
                                                <telerik:EditorTool Name="PasteStrip" />
                                            </telerik:EditorToolGroup>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="FontName" Text="Font" ShowText="true" />
                                                <telerik:EditorTool Name="RealFontSize" Text="Size" ShowText="true" />
                                                <telerik:EditorSeparator />
                                                <telerik:EditorTool Name="ForeColor" />
                                                <telerik:EditorSeparator />
                                                <telerik:EditorTool Name="Bold" ShortCut="CTRL+B" />
                                                <telerik:EditorTool Name="Italic" ShortCut="CTRL+I" />
                                                <telerik:EditorTool Name="Underline" ShortCut="CTRL+U" />
                                                <telerik:EditorTool Name="StrikeThrough" />
                                            </telerik:EditorToolGroup>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="ConvertToLower" />
                                                <telerik:EditorTool Name="ConvertToUpper" />
                                                <telerik:EditorSeparator />
                                                <telerik:EditorTool Name="JustifyLeft" />
                                                <telerik:EditorTool Name="JustifyCenter" />
                                                <telerik:EditorTool Name="JustifyRight" />
                                                <telerik:EditorTool Name="JustifyFull" />
                                                <telerik:EditorTool Name="JustifyNone" />
                                            </telerik:EditorToolGroup>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="InsertOrderedList" />
                                                <telerik:EditorTool Name="InsertUnorderedList" />
                                            </telerik:EditorToolGroup>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="Indent" />
                                                <telerik:EditorTool Name="Outdent" />
                                            </telerik:EditorToolGroup>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="InsertHorizontalRule" />
                                                <telerik:EditorTool Name="InsertGroupbox" />
                                            </telerik:EditorToolGroup>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="InsertSymbol" />
                                            </telerik:EditorToolGroup>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="InsertSnippet" Text="Email Token" ShowText="false" ShowIcon="true" />
                                            </telerik:EditorToolGroup>
                                        </Tools>
                                        <SpellCheckSettings DictionaryPath="~/App_Data/RadSpell" AjaxUrl="~/Telerik.Web.UI.SpellCheckHandler.axd" />
                                        <Languages>
                                            <telerik:SpellCheckerLanguage Code="en-US" Title="English" />
                                        </Languages>
                                    </telerik:RadEditor>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </ContentTemplate>

        </asp:UpdatePanel>

    </div>
</asp:Panel>
<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" CssClass="btn btn-default btn-sm cancel" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click" />
</div>

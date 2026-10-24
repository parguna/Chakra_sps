<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Mail.Mail.UC.DetailUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function editorLoad(editor) {
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

<asp:Panel runat="server" ID="ContentWrapper" CssClass="content-wrapper">
    <div class="form">
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <div class="form-horizontal left">
                    <div class="row-fluid">
                        <div class="col-xs-12">
                            <div class="form-group">
                                <label id="lblmailfrom" runat="server" class="col-xs-2 control-label">From</label>
                                <div class="col-xs-4">
                                    <asp:Label ID="lblFrom" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblTo" AssociatedControlID="txtTo" runat="server" Text="To" class="col-xs-2 control-label"></asp:Label>
                                <asp:Label ID="lbltomsg" runat="server" ForeColor="Red"></asp:Label>
                                <div class="col-xs-4">
                                    <asp:TextBox ID="txtTo" class="form-control" required="required" Rows="3" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblCC" AssociatedControlID="txtCC" runat="server" Text="Cc" class="col-xs-2 control-label"></asp:Label>
                                <asp:Label ID="lblccmsg" runat="server" ForeColor="Red"></asp:Label>
                                <div class="col-xs-4">
                                    <asp:TextBox ID="txtCC" class="form-control" Rows="3" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblBcc" AssociatedControlID="txtBcc" runat="server" Text="Bcc" class="col-xs-2 control-label"></asp:Label>
                                <asp:Label ID="lblbccmsg" runat="server" ForeColor="Red"></asp:Label>
                                <div class="col-xs-4">
                                    <asp:TextBox ID="txtBcc" class="form-control" Rows="3" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblsubject" AssociatedControlID="txtSubject" runat="server" Text="Subject" class="col-xs-2 control-label"></asp:Label>
                                <div class="col-xs-4">
                                    <asp:TextBox ID="txtSubject" class="form-control" Rows="3" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblMailContent" AssociatedControlID="RadEditor1" runat="server" Text="Mail Content" class="col-xs-2 control-label"></asp:Label>
                                <div class="col-xs-10">
                                    <telerik:RadEditor ID="RadEditor1" runat="server" EnableResize="true" Width="100%" Height="500px" OnClientLoad="editorLoad" Skin="IACMSkin" DialogsCssFile="~/assets/styles/dialog.css">
                                        <Tools>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="RichEditor" Text="Open Advanced Editor" />
                                                <telerik:EditorTool Name="Bold" />
                                                <telerik:EditorTool Name="Italic" />
                                                <telerik:EditorTool Name="Underline" />
                                                <telerik:EditorTool Name="Cut" />
                                                <telerik:EditorTool Name="Copy" />
                                                <telerik:EditorTool Name="Paste" />
                                                <telerik:EditorTool Name="FontName" />
                                                <telerik:EditorTool Name="RealFontSize" />
                                            </telerik:EditorToolGroup>
                                        </Tools>
                                        <Content></Content>
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
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="Button1" Text="Cancel" OnClick="Button1_Click" UseSubmitBehavior="false"></asp:Button>
</div>

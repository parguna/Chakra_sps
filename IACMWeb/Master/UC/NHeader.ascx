<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NHeader.ascx.cs" Inherits="DI.Chakra.Web.Master.UC.NHeader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="QuickNavigator.ascx" TagName="QuickNavigator" TagPrefix="uc1" %>
<%@ Register Src="~/Master/UC/Reports.ascx" TagPrefix="uc1" TagName="Reports" %>
<%--<header>
    <script type="text/javascript">
    function onClicking(sender, args) {
        var item = args.get_item();
        
        // if the clicked menu item has children AND it is NOT Home menu
        if (item._hasItems == true && item.get_index() != 0) {
            // clear the selection and focus look to remove the blue box
            item.set_selected(false);
            item.set_focused(false);
            // don't do postback
            args.set_cancel(true);
        }
    }
    </script>
<style>
    #divLogout a {
        color: white !important;
        line-height: 46px;
    }
</style>
     <div class="container-fluid">
    <!-- Fixed navbar -->
    <div class="row">
        <div class="">
           
        </div>

    </div>
        </div>
        <div class="container-fluid">
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-4">
                            <div class="page-app-brand">
                                <div class="c-logo">
                                    <asp:Image ID="imgLogo" runat="server" CssClass="img-responsive" ImageUrl="~/Images/Client/DI.png" />
                                </div>

                                <h1 class="c-app-name hide">
                                    <span class="c-name"><%= T("App.Branding.Company") %></span>
                                    <span class="c-subname"><%= T("App.Branding.SystemName") %></span>
                                </h1>
                            </div>

                            <p class="welcome-text">
                                Hello
                                <asp:Label ID="lblProfile" CssClass="user-firstname" runat="server"></asp:Label>
                            </p>
                        </div>
                        <uc1:QuickNavigator ID="QuickNavigator1" runat="server" />
                    </div>
                </div>
            </div>

        </div>

    <style>
        header {
            background-color: #2077BD;
            border-bottom: none;
            min-height: 45px;
        }
    </style>
</header>--%>
<header>
    <script type="text/javascript">
        function onClicking(sender, args) {
            var item = args.get_item();

            // if the clicked menu item has children AND it is NOT Home menu
            if (item._hasItems == true && item.get_index() != 0) {
                // clear the selection and focus look to remove the blue box
                item.set_selected(false);
                item.set_focused(false);
                // don't do postback
                args.set_cancel(true);
            }
        }
    </script>
    <style>
        .navbar-inverse {
            background-color: #2077BD;
            border-color: #2077BD;
        }

        a#header_mReport {
            font-weight: 400;
            text-decoration: none;
            color: #fff;
            font-size: 16px;
        }

        #divLogout a {
            color: white !important;
            line-height: 46px;
        }

        @media all and (min-width: 992px) {
            .dropdown-menu {
                width: 13rem;
            }

            .mega-submenu {
                left: 100%;
                top: 0;
                min-width: 25rem;
            }

            .ktm-mega-menu {
                position: static;
            }

            .mega-menu {
                left: 0;
                right: 0;
                width: 100%;
            }

            .dropdown-menu li {
                position: relative;
            }

            .dropdown-menu .submenu {
                display: none;
                left: 100%;
                top: 0;
            }

            .dropdown-menu > li:hover > .submenu,
            .dropdown:hover > .dropdown-menu {
                display: block;
            }
        }
    </style>
    <nav class="navbar navbar-inverse navbar-fixed-top di-navigation">
        <div class="container-fluid flex">
            <div id="navbar" class="" style="position: relative">
                <!-- Left Menu -->
                <div class="navbar-left">
                    <telerik:RadMenu ID="mnuMain" runat="server" OnItemClick="mnuMain_ItemClick" OnItemDataBound="mnuMain_ItemDataBound"
                        DataValueField="Id" DataFieldID="Id" DataTextField="MenuName" DataFieldParentID="ParentTabMenuId" ShowToggleHandle="true"
                        EnableRoundedCorners="false" EnableShadows="false" DataNavigateUrlField="PageUrl" OnClientItemClicking="onClicking">
                    </telerik:RadMenu>
                </div>
                <uc1:Reports runat="server" ID="Reports" />
                <!-- Right Menu -->
                <div id="divLogout" class="nav navbar-nav navbar-right">
                    <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click">Logout</asp:LinkButton>
                </div>
                <%--  <ul class="nav navbar-nav navbar-right">
                    <li class="dropdown">
                        <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">Profile <span class="caret"></span>
                            <span class="user-profile-picture">
                                <img id="Img1" src="~/images/Common/profile.png" runat="server"></span>
                        </a>
                        <ul class="dropdown-menu" role="menu">
                            <li id="liadmin" visible="false"  runat="server"><a href="<%=Page.ResolveUrl("~/Admin/AdminItems/List.aspx")%>">Admin Mode</a></li>
                            <li id="liPref" visible="false"  runat="server"><a href="<%=Page.ResolveUrl("~/Preferences/UserPreference.aspx")%>">Preferences</a></li>
                            <li>
                                <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click">Logout</asp:LinkButton></li>
                        </ul>
                    </li>
                </ul>--%>
            </div>
        </div>
    </nav>
    <!-- EOF Navigation -->

    <!--Welcome Box -->
    <!--Welcome Box -->
    <div class="di-welcome">
        <div class="container-fluid">
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-4">
                            <div class="page-app-brand">
                                <div class="c-logo">
                                    <asp:Image ID="imgLogo" runat="server" CssClass="img-responsive" ImageUrl="~/Images/Client/DI.png" />
                                </div>

                                <h1 class="c-app-name hide">
                                    <span class="c-name"><%= T("App.Branding.Company") %></span>
                                    <span class="c-subname"><%= T("App.Branding.SystemName") %></span>
                                </h1>
                            </div>

                            <p class="welcome-text">
                                Hello
                                <asp:Label ID="lblProfile" CssClass="user-firstname" runat="server"></asp:Label>
                            </p>
                        </div>
                        <uc1:QuickNavigator ID="QuickNavigator1" runat="server" />
                    </div>
                </div>
            </div>

        </div>
    </div>
</header>

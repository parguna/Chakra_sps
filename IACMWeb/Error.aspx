<%@ Page Title="Error" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="DI.Chakra.Web.Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <style>
        .page .nav > li {
            display: none;
        }

        .aligncenter {
            text-align: center;
        }

        #black {
            color: gray !important;
            font-size: 23px;
        }

        #errormsg {
            font-size: 14px;
            line-height: 1.2em;
            color: gray;
        }

        #ac-wrapper {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(238, 238, 238, .8);
            z-index: 1001;
        }

        #popup {
            width: 550px;
            height: 200px;
            background: #fff;
            border-radius: 10px;
            -moz-border-radius: 10px;
            -webkit-border-radius: 10px;
            border: 1px solid #DDD;
            box-shadow: #ccc 0px 0px 3px 3px;
            -moz-box-shadow: #ccc 0px 0px 3px 3px;
            -webkit-box-shadow: #ccc 0px 0px 3px 3px;
            padding: 20px;
            /*border: 5px solid #000;
            border-radius: 25px;
            -moz-border-radius: 25px;
            -webkit-border-radius: 25px;*/
            position: relative;
            top: 30%;
            /*left: 375px;*/
            margin: auto;
        }
    </style>

    <script type="text/javascript">
        function PopUp(hideOrshow) {
            if (hideOrshow == 'hide') document.getElementById('ac-wrapper').style.display = "none";
            else document.getElementById('ac-wrapper').removeAttribute('style');
        }
        window.onload = function () {
            setTimeout(function () {
                PopUp('show');
            }, 10);
        }
    </script>

    <div id="ac-wrapper" style='display: none'>
        <div id="popup" class="alert-onpage aligncenter">
            <img id="ErrorImage" runat="server" src="../../Images/error_icon.png" width="50" />
            <h2 class="alert-onpage-danger-title"><span class="glyphicon glyphicn-exclamation-sign" aria-hidden="true"></span>
                <span id="black">Error Occurred, please contact administrator.</span></h2>
        </div>
    </div>
</asp:Content>

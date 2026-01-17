<%@ Page Title="" Language="C#" MasterPageFile="~/BPRCPR.Master" AutoEventWireup="true" CodeBehind="Ops_DOGH.aspx.cs" Inherits="BPRCPR.Ops_DOGH" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--   <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.5.2/jquery.min.js"></script>
    <script src="http://cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.2/modernizr.js"></script>--%>
    <script src="js/jquery.min.js"></script>
    <script src="js/modernizr.js"></script>
    <script lang="javascript">
        function printDiv(divName) {


            var printContents = document.getElementById(divName).innerHTML;
            var originalContents = document.body.innerHTML;

            document.body.innerHTML = printContents;

            window.print();

            document.body.innerHTML = originalContents;
            //var divElements = document.getElementById(divName).innerHTML;
            ////Get the HTML of whole page
            //var oldPage = document.body.innerHTML;

            ////Reset the page's HTML with div's HTML only
            //document.body.innerHTML =
            //    "<html><head><title></title></head><body>" +
            //    divElements + "</body>";

            ////Print Page
            //window.print();

            ////Restore orignal HTML
            //document.body.innerHTML = oldPage;

        }


    </script>
    <script>
        $(window).load(function () {
            // Animate loader off screen
            $(".se-pre-con").hide();
        });

        $(document).ready(function () {
            $(".button-class").click(function () {

                if ($("#ContentPlaceHolder1_txtappNo").val() != "") {

                  //  $(".se-pre-con").show();
                }
                else {
                  //  $(".se-pre-con").hide();
                }

            });
        });
    </script>

    <style>
        .js #loader {
            display: block;
            position: absolute;
            left: 100px;
            top: 0;
        }

        .se-pre-con {
            position: fixed;
            left: 0px;
            top: 0px;
            width: 100%;
            height: 100%;
            z-index: 9999;
            background: url(images/Loading_1.gif) center no-repeat;
        }

        /*#pageFooter {
            display: table-footer-group;
        }

            #pageFooter:after {
                counter-increment: page;
                content: "Page " counter(page);
                left: 0;
                top: 100%;
                white-space: nowrap;
                z-index: 20px;
                -moz-border-radius: 5px;
                -moz-box-shadow: 0px 0px 4px #222;
                background-image: -moz-linear-gradient(top, #eeeeee, #cccccc);
                background-image: -moz-linear-gradient(top, #eeeeee, #cccccc);
            }*/
    </style>




    <!-- Jquery Core Js -->
    <script src="plugins/jquery/jquery.min.js"></script>
    <!-- Bootstrap Core Js -->
    <script src="plugins/bootstrap/js/bootstrap.js"></script>
    <script src="plugins/sweetalert/sweetalert.min.js"></script>

    <script>
        function swalalert(a, b, c) {
            //swal("Good job!", "Submitted!", "success");
            swal(a, b, c);


        }
    </script>


    <asp:ScriptManager runat="server" ID="script1"></asp:ScriptManager>
    <asp:HiddenField ID="hdfBranchID" runat="server" />

    <div class="se-pre-con">
    </div>



    <div class="well col-xs-12 col-sm-12 col-md-8 ">
        <div class="card">
            <div class="header">
                <h2>DOGH Form
                </h2>

            </div>
            <div class="body">

                <div class="row clearfix">
                    <div class="col-md-4">
                        <div class="form-group form-float">
                            <div class="form-line">
                                <asp:TextBox runat="server" ID="txtappNo" CssClass="form-control" required></asp:TextBox>

                                <label class="form-label">Application Number</label>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <div>
                                <asp:Button runat="server" ID="btnSearch" Text="Submit" CssClass="btn btn-success button-class" OnClick="btnSearch_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="container">

        <div class="row" runat="server" id="MainDiv" visible="false">


            <div class="well col-xs-12 col-sm-12 col-md-8 ">
                <div class="pull-right" id="DivBtn">

                    <asp:UpdatePanel runat="server" ID="upe1">
                        <ContentTemplate>


                            <asp:Button runat="server" ID="btnPrint" Text="Print" CssClass="btn btn-danger visi noprint" OnClientClick="printDiv('PrintDiv')" />
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                <div id="PrintDiv">

                    <table style="width: 100%; font-family: arial; font-size: 12px;">
                        <tr>
                            <td style="width: 3%">&nbsp;</td>
                            <td style="width: 94%">
                                <table style="width: 100%">
                                    <tbody id="content" >
                                        <tr style="width:50%;">
                                            <td style="width:50%;"> 
                                                <asp:Image runat="server" ID="Cimg" />
                                            </td>
                                        </tr>
                                    </tbody>
                                    <tr>
                                        <td>&nbsp;
                                        </td>
                                    </tr>
                                </table>

                            </td>
                            <td style="width: 3%">&nbsp;</td>
                        </tr>

                    </table>
               
                </div>

            </div>
        </div>
    </div>
    <!-- Jquery Core Js -->
    <script src="plugins/jquery/jquery.min.js"></script>

    <!-- Bootstrap Core Js -->
    <script src="plugins/bootstrap/js/bootstrap.js"></script>



    
    <script src="plugins/bootstrap-notify/bootstrap-notify.js"></script>
    <script>
        function showNotification(colorName, text, placementFrom, placementAlign, animateEnter, animateExit) {
            if (colorName === null || colorName === '') { colorName = 'bg-black'; }
            if (text === null || text === '') { text = 'Turning standard Bootstrap alerts'; }
            if (animateEnter === null || animateEnter === '') { animateEnter = 'animated fadeInDown'; }
            if (animateExit === null || animateExit === '') { animateExit = 'animated fadeOutUp'; }
            var allowDismiss = true;

            $.notify({
                message: text
            },
                {
                    type: colorName,
                    allow_dismiss: allowDismiss,
                    newest_on_top: true,
                    timer: 1000,
                    placement: {
                        from: placementFrom,
                        align: placementAlign
                    },
                    animate: {
                        enter: animateEnter,
                        exit: animateExit
                    },
                    template: '<div data-notify="container" class="bootstrap-notify-container alert alert-dismissible {0} ' + (allowDismiss ? "p-r-35" : "") + '" role="alert">' +
                        '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
                        '<span data-notify="icon"></span> ' +
                        '<span data-notify="title">{1}</span> ' +
                        '<span data-notify="message">{2}</span>' +
                        '<div class="progress" data-notify="progressbar">' +
                        '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
                        '</div>' +
                        '<a href="{3}" target="{4}" data-notify="url"></a>' +
                        '</div>'
                });
        }
    </script>
</asp:Content>

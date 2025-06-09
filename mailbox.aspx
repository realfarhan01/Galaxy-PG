<%@ Page Language="VB" AutoEventWireup="false" CodeFile="mailbox.aspx.vb" Inherits="mailbox" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>Company Name User Area</title>
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <link rel="shortcut icon" href="favicon.ico" />
</head>
<body class="login">
    <form runat="server" id="form1">
    <div class="logo">
    </div>
    <%--<div class="content">
        <asp:Panel ID="pnllog" runat="server">
            <div class="form-vertical login-form">
                <h3 class="form-title">
                    Send email and welcome letter</h3>
                <div runat="server" id="msg" style="display: none;" class="alert alert-error show">
                </div>
                <div class="control-group">
                    <label class="control-label visible-ie8 visible-ie9">
                        Email For Testing
                    </label>
                    <div class="controls">
                        <div class="input-icon left">
                            <i class="icon-user"></i>
                            <asp:TextBox runat="server" placeholder="Email-id" ID="txtemail" Width="400px" class="m-wrap placeholder-no-fix"
                                ValidationGroup="Login"></asp:TextBox><br /><br />
                        </div>
                    </div>
                </div>
                
                <div class="form-actions">
                    <label class="checkbox">
                       
                    </label>
                    <br />
                    <asp:Button runat="server" ID="commit" ValidationGroup="Login" Text="Send" class="btn green pull-right" />
                    <i class="m-icon-swapright m-icon-white">
                    <br />
                    </i>
                </div>
            </div>
            <br />
<asp:Literal ID="litmsg" runat="server"></asp:Literal>
        </asp:Panel>
    </div>
    --%>

    <div class="copyright">
    </div>
    <script src="User/assets/js/jquery-1.8.3.min.js"></script>
    <script src="User/assets/bootstrap/js/bootstrap.min.js"></script>
    <script src="User/assets/uniform/jquery.uniform.min.js"></script>
    <script src="User/assets/js/jquery.blockui.js"></script>
    <script type="text/javascript" src="User/assets/jquery-validation/dist/jquery.validate.min.js"></script>
    <script src="User/assets/js/app.js"></script>
    <script>
        jQuery(document).ready(function () {
            App.initLogin();
        });
    </script>
    </form>
</body>
</html>

Partial Class mailbox
    Inherits System.Web.UI.Page

    Shared EmailChk As Integer = 0
    Shared MobileChk As Integer = 0

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Not Request.Form("name") Is Nothing Then
                Dim name As String = Request.Form("name").Trim()
                Dim phone As String = Request.Form("phone").Trim()
                Dim message As String = Request.Form("message").Trim()
                Dim honeypot As String = Request.Form("website").Trim() ' hidden field

                ' ===== SPAM CHECK (honeypot) =====
                If honeypot <> "" Then
                    ' Bot filled hidden field
                    Return
                End If

                ' ===== SPAM CHECK (keywords) =====
                Dim spamWords() As String = {"viagra", "free money", "loan", "porn", "sex", "hack", "free", "xxx", "price", "http", "https"}
                For Each word In spamWords
                    'If message.ToLower().Contains(word) OrElse emails.ToLower().Contains(word) Then
                    '    ShowAlertAndGoBack("Spam content detected. Submission blocked.")
                    '    Return
                    'End If
                Next

                ' ===== VALIDATION =====
                If name = "" OrElse Not System.Text.RegularExpressions.Regex.IsMatch(name, "^[a-zA-Z\s]+$") Then
                    ShowAlertAndGoBack("Please enter a valid name.")
                    Return
                End If

                If phone = "" OrElse Not System.Text.RegularExpressions.Regex.IsMatch(phone, "^\d{10}$") Then
                    ShowAlertAndGoBack("Please enter a valid 10-digit mobile number.")
                    Return
                End If

                ' ===== SEND EMAIL =====
                Dim Result As String = Mail(name, phone, message)

                ClientScript.RegisterStartupScript(Page.[GetType](), "alert", "alert('Thank you for your request. We will contact you as soon as possible.');window.location.href='/';", True)
            End If
        End If
    End Sub

    Private Sub ShowAlertAndGoBack(ByVal message As String)
        ClientScript.RegisterStartupScript(Page.[GetType](), "alert", "alert('" & message.Replace("'", "\'") & "');window.history.back(-1);", True)
    End Sub

    Private Shared Function Mail(ByVal name As String, ByVal phone As String, ByVal message As String) As String
        Dim Result As String = ""
        Dim templateVars As New Hashtable()
        templateVars.Add("Name", name.ToUpper())
        templateVars.Add("Mobile", phone)
        templateVars.Add("Message", message)

        Result = Email.SendEmail("contact_email.htm", templateVars,
            System.Configuration.ConfigurationManager.AppSettings("email"),
            System.Configuration.ConfigurationManager.AppSettings("infoemail"),
            "Website Appointment",
            System.Configuration.ConfigurationManager.AppSettings("bccemail"))

        Return Result
    End Function
End Class


Partial Class mailbox
    Inherits System.Web.UI.Page
    Shared EmailChk As Integer = 0
    Shared MobileChk As Integer = 0
    'https://gesaleservices.in/mailbox.aspx?fnm=Abhi&mob=7791926666&adate=23-06-2021
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Not Request.Form("name") Is Nothing Then
                Dim Result As String = ""
                Result = Mail(Request.Form("name"), Request.Form("phone"), Request.Form("service"), Request.Form("emails"), Request.Form("message"))

                ClientScript.RegisterStartupScript(Page.[GetType](), "alert", "alert('Thank you for your request. We will contact you as soon as possible.');window.location.href='/';", True)



                'Response.Write(Result)
                'ClientScript.RegisterStartupScript(Page.[GetType](), "alert", "alert('Thank you for your request. Our team will contact you between 9 AM  to 5 PM.');window.location='http://goyalpilescentre.com/';", True)
                'Response.Redirect("flooring-services")
            End If
        End If
    End Sub
    'Protected Sub commit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles commit.Click
    '    If Not String.IsNullOrEmpty(txtemail.Text) Then
    '        SendEmail()
    '    End If
    'End Sub
    'Private Sub SendEmail()
    '    'Dim dt As DataTable = BLL.ExecDataTable("select MemberName,Password,Memberid from dbo.MemberMaster where Email=@Email", "@Email", txtemail.Text)
    '    'If (dt.Rows.Count > 0 AndAlso dt IsNot Nothing) Then
    '    Dim Result As String
    '    Try
    '        Dim templateVars As New Hashtable()
    '        templateVars.Add("IPAddress", Request.ServerVariables("remote_addr"))
    '        Result = Email.SendEmailTron("testing.htm", templateVars, "info@tronfactory.network", txtemail.Text, "Testing Mail", "")

    '        litmsg.Text = Result
    '    Catch ex As Exception
    '        litmsg.Text = ex.ToString()

    '    End Try

    'End Sub
    Private Shared Function Mail(ByVal name As String, ByVal phone As String, ByVal service As String, ByVal emails As String, ByVal message As String) As String

        Dim Result As String = ""
        Dim templateVars As New Hashtable()
        templateVars.Add("Name", name.ToUpper())
        templateVars.Add("Mobile", phone)
        templateVars.Add("Email", emails)
        templateVars.Add("Message", message)
        'templateVars.Add("Message", message)
        'Result = Email.SendEmail("contact_email.htm", templateVars, "website@aspiretechnosys.com", "testingvb.net@gmail.com", "Website Enquiry Relife Spine Care", "")
        Result = Email.SendEmail("contact_email.htm", templateVars, System.Configuration.ConfigurationManager.AppSettings("email"), System.Configuration.ConfigurationManager.AppSettings("infoemail"), "Website Appointment", System.Configuration.ConfigurationManager.AppSettings("bccemail"))
        Return Result
    End Function
End Class

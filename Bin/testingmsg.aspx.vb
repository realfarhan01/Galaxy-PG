
Partial Class testingmsg
    Inherits System.Web.UI.Page
    Shared EmailChk As Integer = 0
    Shared MobileChk As Integer = 0

    Protected Sub commit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles commit.Click
        If String.IsNullOrEmpty(txtemail.Text) AndAlso String.IsNullOrEmpty(txtmemberid.Text) Then
            ClientScript.RegisterStartupScript(Page.[GetType](), "alert", "alert('Please enter email-id or mobile number');", True)
        End If
        If Not String.IsNullOrEmpty(txtemail.Text) Then
            SendEmail()
        End If
    End Sub
    Private Sub SendEmail()
        'Dim dt As DataTable = BLL.ExecDataTable("select MemberName,Password,Memberid from dbo.MemberMaster where Email=@Email", "@Email", txtemail.Text)
        'If (dt.Rows.Count > 0 AndAlso dt IsNot Nothing) Then
        Dim Result As String
        Try
            Dim templateVars As New Hashtable()
            templateVars.Add("IPAddress", Request.ServerVariables("remote_addr"))
            Result = Email.SendEmail("testing.htm", templateVars, System.Configuration.ConfigurationManager.AppSettings("email"), txtemail.Text, "Testing Mail", System.Configuration.ConfigurationManager.AppSettings("bccemail"))

            litmsg.Text = Result
        Catch ex As Exception
            litmsg.Text = ex.ToString()

        End Try

    End Sub
    Private Sub SendMessage()
        'Dim dt As DataTable = BLL.ExecDataTable("select MemberName,Password,Memberid from dbo.MemberMaster where Mobile=@Mobile", "@Mobile", txtmobile.Text)
        'If (dt.Rows.Count > 0 AndAlso dt IsNot Nothing) Then
        Dim Result As String
        Try
            Result = SendSmsToMobile.SendSms("7791926666", "Dear Member Thanks for joining Us. Your user name is Testing Contact our support for any assistance.")

            litmsg.Text = Result
        Catch ex As Exception
            litmsg.Text = ex.ToString()
        End Try
        'ClientScript.RegisterStartupScript(Page.[GetType](), "alert", "alert('Message successfully sent');window.location='EmailMessage.aspx';", True)

        'Else
        'ClientScript.RegisterStartupScript(Page.[GetType](), "alert", "alert('Sorry !! record not exists');", True)

        'End If

    End Sub
End Class

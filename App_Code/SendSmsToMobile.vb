Imports System.Web
Imports System.Net
Imports System.IO
Public Class SendSmsToMobile
    '--------------Function to be used-------------------
    'Protected Shared Function SendSMSToNum(ByVal strUser As String, ByVal strPassword As String, ByVal senderid As String, ByVal strRecip As String, ByVal strMsgText As String, Optional ByVal strSMSScheduleDate As String = "") As String
    '    Dim strUrl As String
    '    strUrl = System.Configuration.ConfigurationManager.AppSettings("sms_url") _
    '                     & "username=" & HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings("sms_userId")) _
    '                     & "&password=" & HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings("sms_pwd")) _
    '                     & "&sendername=" & HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings("sms_senderid")) _
    '                     & "&mobileno=" & HttpUtility.UrlEncode(strRecip) _
    '                     & "&message=" & HttpUtility.UrlEncode(strMsgText)
    '    Dim objURI As Uri = New Uri(strUrl)
    '    Dim objWebRequest As WebRequest = WebRequest.Create(objURI)
    '    Dim objWebResponse As WebResponse = objWebRequest.GetResponse()
    '    Dim objStream As Stream = objWebResponse.GetResponseStream()
    '    Dim objStreamReader As StreamReader = New StreamReader(objStream)
    '    Dim strHTML As String = objStreamReader.ReadToEnd
    '    SendSMSToNum = strHTML

    '    '& HttpUtility.UrlEncode("BSAIL") _
    'End Function
    Protected Shared Function SendSMSToNum(ByVal strUser As String, ByVal strPassword As String, ByVal senderid As String, ByVal strRecip As String, ByVal strMsgText As String, Optional ByVal strSMSScheduleDate As String = "") As String
        Dim strUrl As String
        strUrl = "http://103.16.101.52:8000/bulksms/bulksms?" _
                         & "username=" & HttpUtility.UrlEncode(strUser) _
                         & "&password=" & HttpUtility.UrlEncode(strPassword) _
                         & "&source=MWINGS&format=text&type=0&dlr=1" _
                         & "&destination=" & HttpUtility.UrlEncode(strRecip) _
                         & "&message=" & HttpUtility.UrlEncode(strMsgText)
        Dim objURI As Uri = New Uri(strUrl)
        Dim objWebRequest As WebRequest = WebRequest.Create(objURI)
        Dim objWebResponse As WebResponse = objWebRequest.GetResponse()
        Dim objStream As Stream = objWebResponse.GetResponseStream()
        Dim objStreamReader As StreamReader = New StreamReader(objStream)
        Dim strHTML As String = objStreamReader.ReadToEnd
        SendSMSToNum = strHTML

        '& HttpUtility.UrlEncode("BSAIL") _
    End Function
    '--------------Function to be used-------------------

    Public Shared Function SendSms(ByVal strMobileNo As String, ByVal strTextMsg As String) As String
        Dim strGatewayResponse As String = "Message not send"
        Try
            If strMobileNo.Length = 10 And IsNumeric(strMobileNo) And strTextMsg.ToString.Length > 0 Then
                strMobileNo = "91" & strMobileNo
                strGatewayResponse = SendSMSToNum(System.Configuration.ConfigurationManager.AppSettings("sms_userId"), System.Configuration.ConfigurationManager.AppSettings("sms_pwd"), "", strMobileNo, strTextMsg, "")
            End If
        Catch ex As Exception
            strGatewayResponse = ex.Message & strGatewayResponse & "Message not send"
        End Try

        Return strGatewayResponse
    End Function
    'Public Shared Function SendmultipleSms(ByVal strMobileNo As String, ByVal strTextMsg As String) As String
    '    Dim strGatewayResponse As String = "Message not send"
    '    Try
    '        strGatewayResponse = SendSMSToNum(System.Configuration.ConfigurationManager.AppSettings("sms_userId"), System.Configuration.ConfigurationManager.AppSettings("sms_pwd"), "", strMobileNo, strTextMsg, "")
    '    Catch ex As Exception
    '        strGatewayResponse = ex.Message & strGatewayResponse & "Message not send"
    '    End Try
    '    Return strGatewayResponse
    'End Function
    'Public Shared Function GetResponse(ByVal Response As String) As String
    '    Dim strUrl As String

    '    strUrl = "http://sms.shoppingmartlive.co.cc/api/dlr.php?" _
    '             & "uid=" & HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings("sms_userId")) _
    '             & "&pin=" & HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings("sms_pwd")) _
    '             & "&msgid=" & HttpUtility.UrlEncode(Response)

    '    Dim objURI As Uri = New Uri(strUrl)
    '    Dim objWebRequest As WebRequest = WebRequest.Create(objURI)
    '    Dim objWebResponse As WebResponse = objWebRequest.GetResponse()
    '    Dim objStream As Stream = objWebResponse.GetResponseStream()
    '    Dim objStreamReader As StreamReader = New StreamReader(objStream)
    '    Dim strHTML As String = objStreamReader.ReadToEnd
    '    GetResponse = strHTML

    'End Function
    Public Shared Function SendSMSNew(ByVal User As String, ByVal password As String, ByVal Mobile_Number As String, ByVal Message As String, Optional ByVal MType As String = "N") As String
        Dim stringpost As String = "User=" & User & "&passwd=" & password & "&mobilenumber=" & Mobile_Number & "&message=" & Message & "&MTYPE=" & MType & "&sid=" & "WINGS"
        'Response.Write(stringpost)
        Dim functionReturnValue As String = Nothing
        functionReturnValue = ""

        Dim objWebRequest As HttpWebRequest = Nothing
        Dim objWebResponse As HttpWebResponse = Nothing
        Dim objStreamWriter As StreamWriter = Nothing
        Dim objStreamReader As StreamReader = Nothing

        Try
            Dim stringResult As String = Nothing

            objWebRequest = DirectCast(WebRequest.Create("http://info.bulksms-service.com/WebserviceSMS.aspx"), HttpWebRequest)
            objWebRequest.Method = "POST"

            ' Response.Write(objWebRequest)

            ' Use below code if you want to SETUP PROXY. 
            'Parameters to pass: 1. ProxyAddress 2. Port 
            'You can find both the parameters in Connection settings of your internet explorer.

            ' If you are in the proxy then Uncomment below lines and enter IP and Port.
            ' Dim myProxy As New Net.WebProxy("192.168.1.108", 6666)
            'myProxy.BypassProxyOnLocal = True
            'objWebRequest.Proxy = myProxy

            objWebRequest.ContentType = "application/x-www-form-urlencoded"

            objStreamWriter = New StreamWriter(objWebRequest.GetRequestStream())
            objStreamWriter.Write(stringpost)
            objStreamWriter.Flush()
            objStreamWriter.Close()

            objWebResponse = DirectCast(objWebRequest.GetResponse(), HttpWebResponse)


            objWebResponse = DirectCast(objWebRequest.GetResponse(), HttpWebResponse)

            objStreamReader = New StreamReader(objWebResponse.GetResponseStream())
            stringResult = objStreamReader.ReadToEnd()
            objStreamReader.Close()
            Return (stringResult)
        Catch ex As Exception
            Return (ex.ToString)
        Finally

            If (objStreamWriter IsNot Nothing) Then
                objStreamWriter.Close()
            End If
            If (objStreamReader IsNot Nothing) Then
                objStreamReader.Close()
            End If
            objWebRequest = Nothing
            objWebResponse = Nothing

        End Try
    End Function
End Class


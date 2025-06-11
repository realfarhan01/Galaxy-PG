Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Drawing.Imaging
Imports System.Drawing
Imports System.Web.Mail
Imports System.Xml
Imports System.Net.WebRequest
Imports System.Data
Imports System.Net

Public Class BusinessLogicLayer
    Inherits DataAccessLayer
    Dim ctx As HttpContext = HttpContext.Current
   




    Public Function GenerateRandomString(ByRef iLength As Integer) As String
        Dim rdm As New Random()
        Dim allowChrs() As Char = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789".ToCharArray()
        Dim sResult As String = ""

        For i As Integer = 0 To iLength - 1
            sResult += allowChrs(rdm.Next(0, allowChrs.Length))
        Next

        Return sResult
    End Function
    Public Function isImage(ByVal strm As System.IO.Stream) As Boolean
        Dim bool As Boolean
        Try

            Dim image As System.Drawing.Image = System.Drawing.Image.FromStream(strm)
            Dim FormetType As String = String.Empty
            If image.RawFormat.Guid = System.Drawing.Imaging.ImageFormat.Tiff.Guid Then
                FormetType = "TIFF"
                bool = True
            ElseIf image.RawFormat.Guid = System.Drawing.Imaging.ImageFormat.Gif.Guid Then
                FormetType = "GIF"
                bool = True
            ElseIf image.RawFormat.Guid = System.Drawing.Imaging.ImageFormat.Jpeg.Guid Then
                FormetType = "JPG"
                bool = True
            ElseIf image.RawFormat.Guid = System.Drawing.Imaging.ImageFormat.Bmp.Guid Then
                FormetType = "BMP"
                bool = True
            ElseIf image.RawFormat.Guid = System.Drawing.Imaging.ImageFormat.Png.Guid Then
                FormetType = "PNG"
                bool = True
            ElseIf image.RawFormat.Guid = System.Drawing.Imaging.ImageFormat.Icon.Guid Then
                FormetType = "ICO"
                bool = True
            Else
                bool = False
            End If
        Catch exp As System.ArgumentException
            bool = False
        Catch ex As Exception
            bool = False
        End Try
        Return bool
    End Function
    Public Sub ExportToExcel(ByVal gv As GridView, ByVal fileName As String)
        ctx.Response.Clear()
        ctx.Response.Buffer = True
        Dim sw As New System.IO.StringWriter()
        Dim hw As New System.Web.UI.HtmlTextWriter(sw)
        ctx.Response.AddHeader("content-disposition", "attachment;filename=" & fileName)
        ctx.Response.Charset = ""
        ctx.Response.ContentType = "application/vnd.ms-excel"
        gv.RenderControl(hw)
        ctx.Response.Write(sw.ToString())
        ctx.Response.End()
    End Sub
    Public Sub ExportToWord(ByVal gv As GridView, ByVal fileName As String)
        ctx.Response.Clear()
        ctx.Response.Buffer = True
        ctx.Response.AddHeader("content-disposition", "attachment;filename=" & fileName)
        ctx.Response.Charset = ""
        ctx.Response.ContentType = "application/vnd.ms-word "
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        gv.RenderControl(hw)
        ctx.Response.Output.Write(sw.ToString())
        ctx.Response.Flush()
        ctx.Response.[End]()
    End Sub

    Public Sub ExportToCsv(ByVal dt As DataTable, ByVal fileName As String)
        ctx.Response.Clear()
        ctx.Response.Buffer = True
        ctx.Response.AddHeader("content-disposition", "attachment;filename=" & fileName)
        ctx.Response.Charset = ""
        ctx.Response.ContentType = "application/text "


        Dim sb As New StringBuilder()
        For k As Integer = 0 To dt.Columns.Count - 1
            'add separator 
            sb.Append(dt.Columns(k).ColumnName + ","c)
        Next

        'append new line 
        sb.Append(vbCr & vbLf)

        For i As Integer = 0 To dt.Rows.Count - 1
            For k As Integer = 0 To dt.Columns.Count - 1
                'add separator 
                sb.Append(dt.Rows(i).Item(k).ToString() + ","c)

            Next
            'append new line 
            sb.Append(vbCr & vbLf)

        Next
        ctx.Response.Output.Write(sb.ToString())
        ctx.Response.Flush()
        ctx.Response.End()
    End Sub

    Public Function GenerateCode(ByVal len As Integer) As String
        Dim KeyGen As New KeyGenerate
        Dim RandomKey As String
        KeyGen = New KeyGenerate
        'KeyGen.KeyLetters = "abcdefghjklmnpqrstuvwxyz"
        KeyGen.KeyLetters = "123456789"
        KeyGen.KeyNumbers = "23456789"
        KeyGen.KeyChars = len
        RandomKey = KeyGen.Generate().ToUpper()
        Return RandomKey
    End Function

    Public Function GetDistinctValues(ByVal array As String()) As String()
        Dim list As New System.Collections.Generic.List(Of String)()
        For i As Integer = 0 To array.Length - 1
            If list.Contains(array(i)) Then
                Continue For
            End If
            list.Add(array(i))
        Next
        Return list.ToArray()
    End Function
     
    Public Function CreateMenuNew() As String
        Dim menu As String = String.Empty
        Dim dt As DataTable
        Dim a As String = ctx.Session("menustr")
        Dim mstr As String = Left(ctx.Session("menustr"), ctx.Session("menustr").ToString.Length - 1)
        menu = "<ul class='navigation'>"
        Dim str As String = "select Row_number() over(order by sno) rno,Sno,MenuName,isnull(Url,'') Url,ParentMenuid from DynamicMenu where Active=1 and sno in (" & mstr & ")"
        dt = ExecDataTable(str)
        Dim Childstr As String = ""
        Dim drow As DataRow() = dt.Select("ParentMenuid=0")

        Dim K As Integer = 1
        For i As Integer = 0 To drow.Length - 1
            Dim childrow As DataRow() = dt.Select("Parentmenuid=" & drow(i).Item("Sno"))
            If childrow.Length > 0 Then
                menu = menu & String.Format("<li><a href='{0}' class='expand'><i class='fa fa-align-justify'></i>{2}</a>", drow(i).Item("Url"), K, drow(i).Item("MenuName"))
                For j As Integer = 0 To childrow.Length - 1
                    If j = 0 Then
                        menu = menu & "<ul ><li><a href='" & childrow(j).Item("Url") & "'>" & childrow(j).Item("MenuName") & "</a>" & GetStr(dt.Select("Parentmenuid=" & childrow(j).Item("Sno"))) & "</li>"
                        K = K + 1
                    Else
                        menu = menu & "<li><a href='" & childrow(j).Item("Url") & "'>" + childrow(j).Item("MenuName") & "</a>" & GetStr(dt.Select("Parentmenuid=" & childrow(j).Item("Sno"))) & "</li>"
                    End If
                    If j = childrow.Length - 1 Then
                        menu = menu & "</ul></li>"
                    End If
                Next
            Else
                menu = menu & String.Format("<li><a href='{0}' ><i class='fa fa-align-justify'></i>{1}</a></li>", drow(i).Item("Url"), drow(i).Item("MenuName"))
            End If

        Next
        menu = menu & "</ul>"
        'Childstr = Childstr & "</ul>"

        Return menu

    End Function

            Public Function CreateMenu() As String
                Dim menu As String = String.Empty
                Dim dt As DataTable
                Dim a As String = ctx.Session("menustr")
                Dim mstr As String = Left(ctx.Session("menustr"), ctx.Session("menustr").ToString.Length - 1)
                menu = "<table cellspacing='0' cellpadding='0' border='0'><tr> <td><div class='topmainmenu' id='ddtopmenubar'><ul id='main_menu'>"
                Dim str As String = "select Row_number() over(order by sno) rno,Sno,MenuName,isnull(Url,'') Url,ParentMenuid from DynamicMenu where Active=1 and sno in (" & mstr & ")"
                dt = ExecDataTable(str)
                Dim Childstr As String = ""
                Dim drow As DataRow() = dt.Select("ParentMenuid=0")
                Dim K As Integer = 1
                For i As Integer = 0 To drow.Length - 1
                    Dim childrow As DataRow() = dt.Select("Parentmenuid=" & drow(i).Item("Sno"))
                    If childrow.Length > 0 Then
                        menu = menu & String.Format("<li><a href='{0}' rel='ddsubmenu{1}'>{2}</a></li>", drow(i).Item("Url"), K, drow(i).Item("MenuName"))
                    Else
                        menu = menu & String.Format("<li><a href='{0}' >{1}</a></li>", drow(i).Item("Url"), drow(i).Item("MenuName"))
                    End If
                    For j As Integer = 0 To childrow.Length - 1
                        If j = 0 Then
                            Childstr = Childstr & "<ul visible='false' class='ddsubmenustyle' id='ddsubmenu" & (K).ToString() & "'><li><a href='" & childrow(j).Item("Url") & "'>" & childrow(j).Item("MenuName") & "</a>" & GetStr(dt.Select("Parentmenuid=" & childrow(j).Item("Sno"))) & "</li>"
                            K = K + 1
                        Else
                            Childstr = Childstr & "<li><a href='" & childrow(j).Item("Url") & "'>" + childrow(j).Item("MenuName") & "</a>" & GetStr(dt.Select("Parentmenuid=" & childrow(j).Item("Sno"))) & "</li>"
                        End If
                        If j = childrow.Length - 1 Then
                            Childstr = Childstr & "</ul>"
                        End If
                    Next
                Next
                menu = menu & "</ul></div><script type='text/javascript'>ddlevelsmenu.setup('ddtopmenubar', 'topbar') //ddlevelsmenu.setup('mainmenuid', 'topbar|sidebar')</script>"
                Childstr = Childstr & "</td></tr></table>"

                Return menu + Childstr

            End Function
            Public Sub CreateOperatorMenuFile(ByVal Loginid As String, ByVal Menustr As String)
                Dim mstr As String = Left(Menustr, Menustr.Length - 1)
                Dim xd As New System.Xml.XmlDocument
                Dim MenusNode As XmlNode = xd.CreateElement("Menus")
                xd.AppendChild(MenusNode)

                Dim TopNode, ChildNode As XmlNode
                Dim attr As XmlAttribute
                Dim dr As SqlDataReader = ExecDataReader("select Row_number() over(order by sno) rno,Sno,MenuName,isnull(Url,'') Url,ParentMenuid from DynamicMenu where Active=1 and ParentMenuid=0 and sno in (" & mstr & ")")
                While dr.Read
                    TopNode = xd.CreateElement("Menu")
                    attr = xd.CreateAttribute("MenuName")
                    attr.Value = dr("MenuName")
                    TopNode.Attributes.Append(attr)
                    attr = xd.CreateAttribute("Url")
                    attr.Value = dr("Url")
                    TopNode.Attributes.Append(attr)
                    attr = xd.CreateAttribute("Id")
                    attr.Value = dr("sno")
                    TopNode.Attributes.Append(attr)
                    Dim dr2 As SqlDataReader = ExecDataReader("select Row_number() over(order by sno) rno,Sno,MenuName,isnull(Url,'') Url,ParentMenuid from DynamicMenu where Active=1 and sno in (" & mstr & ") and ParentMenuid=@id", "@id", dr("sno"))
                    While dr2.Read
                        ChildNode = xd.CreateElement("ChildMenu")
                        attr = xd.CreateAttribute("MenuName")
                        attr.Value = dr2("MenuName")
                        ChildNode.Attributes.Append(attr)
                        attr = xd.CreateAttribute("Url")
                        attr.Value = dr2("Url")
                        ChildNode.Attributes.Append(attr)
                        TopNode.AppendChild(ChildNode)
                    End While
                    MenusNode.AppendChild(TopNode)
                End While
                xd.Save(ctx.Server.MapPath("~/Xml/") & Loginid & ".xml")
            End Sub
            Public Sub CreateAdminMenuFile(ByVal Loginid As String, ByVal Menustr As String)
                Dim mstr As String = Left(Menustr, Menustr.Length - 1)
                Dim xd As New System.Xml.XmlDocument
                Dim MenusNode As XmlNode = xd.CreateElement("Menus")
                xd.AppendChild(MenusNode)

                Dim TopNode, ChildNode As XmlNode
                Dim attr As XmlAttribute
                Dim dr As SqlDataReader = ExecDataReader("select Row_number() over(order by sno) rno,Sno,MenuName,isnull(Url,'') Url,ParentMenuid from DynamicMenu where Active=1 and ParentMenuid=0 and sno in (" & mstr & ")")
                While dr.Read
                    TopNode = xd.CreateElement("Menu")
                    attr = xd.CreateAttribute("MenuName")
                    attr.Value = dr("MenuName")
                    TopNode.Attributes.Append(attr)
                    attr = xd.CreateAttribute("Url")
                    attr.Value = dr("Url")
                    TopNode.Attributes.Append(attr)
                    attr = xd.CreateAttribute("Id")
                    attr.Value = dr("sno")
                    TopNode.Attributes.Append(attr)
                    Dim dr2 As SqlDataReader = ExecDataReader("select Row_number() over(order by sno) rno,Sno,MenuName,isnull(Url,'') Url,ParentMenuid from DynamicMenu where Active=1 and sno in (" & mstr & ") and ParentMenuid=@id", "@id", dr("sno"))
                    While dr2.Read
                        ChildNode = xd.CreateElement("ChildMenu")
                        attr = xd.CreateAttribute("MenuName")
                        attr.Value = dr2("MenuName")
                        ChildNode.Attributes.Append(attr)
                        attr = xd.CreateAttribute("Url")
                        attr.Value = dr2("Url")
                        ChildNode.Attributes.Append(attr)
                        TopNode.AppendChild(ChildNode)
                    End While
                    MenusNode.AppendChild(TopNode)
                End While
                xd.Save(ctx.Server.MapPath("~/Xml/") & Loginid & ".xml")
            End Sub
            Public Function GetStr(ByVal mrow As DataRow()) As String
                Dim str As String = "<ul>"
                For i As Integer = 0 To mrow.Length - 1
                    str = str & "<li><a href='" & mrow(i).Item("url") + "'>" & mrow(i).Item("menuName") + "</a></li>"
                Next
                If str = "<ul>" Then
                    str = ""
                Else
                    str = str & "</ul>"
                End If
                Return str
            End Function
           
            Public Function IsValidForPage() As Boolean
                Dim pagename As String = GetCurrentPageName()
                Dim pageid As String = ExecScalar("Select sno from  DynamicMenu where pagename=@PageName", "@PageName", pagename)
                pageid = "," & pageid & ","
                Dim mstr As String = "," & ctx.Session("menustr").ToString()
                If mstr.Contains(pageid) Then
                    Return True
                Else
                    Return False
                End If
            End Function

    'Public Function GetCurrentPageName() As String
    '    Dim sPath As String = System.Web.HttpContext.Current.Request.Url.AbsolutePath
    '    Dim oInfo As System.IO.FileInfo = New System.IO.FileInfo(sPath)
    '    Dim sRet As String = oInfo.Name
    '    Return sRet
    'End Function
    Public Function GetCurrentPageName() As String
        Dim sPath As String = System.Web.HttpContext.Current.Request.Url.AbsolutePath
        'Dim oInfo As System.IO.FileInfo = New System.IO.FileInfo(sPath)
        Dim sRet As String
        If sPath.Contains("MataSatyawati") Then
            sRet = sPath.Replace("/MataSatyawati/Admin/", "")
        Else
            sRet = sPath.Replace("/Admin/", "")
        End If

        Return sRet
    End Function
            Public Function getPageParentid(ByVal pagename As String) As Integer
                Dim pageid As Integer = 0
                pageid = ExecScalar("Select MenuParentId from  MemberMenu where MenuPageName=@MenuPageName", "@MenuPageName", pagename)
                If IsDBNull(pageid) Then
                    pageid = 0
                End If
                Return pageid
            End Function
            Public Function getAdminPageParentid(ByVal pagename As String) As Integer
                Dim pageid As Integer = 0
                pageid = ExecScalar("Select ParentMenuId from DynamicMenu where PageName=@PageName", "@PageName", pagename)
                If IsDBNull(pageid) Then
                    pageid = 0
                End If
                Return pageid
    End Function

    

            Sub CreatePaging(ByVal intTotalRecords As Integer, ByVal intTotalPages As Integer, ByVal RecordsPerPage As Integer, ByVal CurrentPage As Integer, ByVal TotalMessages As Label, ByVal PagingLabel As Label, ByVal RecordsCount As Label)
                If intTotalRecords Mod RecordsPerPage = 0 Then
                    intTotalPages = CInt(Int(intTotalRecords / RecordsPerPage))
                Else
                    intTotalPages = CInt(Int(intTotalRecords / RecordsPerPage) + 1)
                End If
                TotalMessages.Text = "Page <b>" & CurrentPage & "</b> of <b>" & intTotalPages & "</b>"
                RecordsCount.Text = "<b>" & intTotalRecords & "</b> Records"
                Dim i As Integer
                Dim NavigationText As String = ""
                If CurrentPage > 1 Then
                    NavigationText += "<a href=" & HttpContext.Current.Request.ServerVariables("SCRIPT_NAME") & "?Page=" & CurrentPage - 1 & "><<</a> "
                End If
                For i = 1 To intTotalPages
                    If CurrentPage = i Then
                        NavigationText += "<b>" & i & "</b>    "
                    Else
                        NavigationText += "<a href=" & HttpContext.Current.Request.ServerVariables("SCRIPT_NAME") & "?Page=" & i & ">" & i & "</a> "
                    End If
                Next i
                If CurrentPage < intTotalPages Then
                    NavigationText += "<a href=" & HttpContext.Current.Request.ServerVariables("SCRIPT_NAME") & "?Page=" & CurrentPage + 1 & ">>></a> "
                End If
                PagingLabel.Text = NavigationText
            End Sub

            Public Sub ShowNoResultFound(ByVal source As DataTable, ByVal gv As GridView)
                Dim dt As DataTable = source.Clone
                For Each c As DataColumn In dt.Columns
                    c.AllowDBNull = True
                Next
                dt.Rows.Add(dt.NewRow()) ' // create a new blank row to the DataTable
                '// Bind the DataTable which contain a blank row to the GridView
                gv.DataSource = dt
                gv.DataBind()
                '// Get the total number of columns in the GridView to know what the Column Span should be
                Dim columnsCount As Integer
                If gv.Columns.Count = 0 Then
                    columnsCount = source.Columns.Count
                Else
                    columnsCount = gv.Columns.Count
                End If

                gv.Rows(0).Cells.Clear() '// clear all the cells in the row
                gv.Rows(0).Cells.Add(New TableCell()) ' //add a new blank cell
                gv.Rows(0).Cells(0).ColumnSpan = columnsCount ' //set the column span to the new added cell

                ' //You can set the styles here
                gv.Rows(0).Cells(0).HorizontalAlign = HorizontalAlign.Center ';
                gv.Rows(0).Cells(0).ForeColor = System.Drawing.Color.Red '
                gv.Rows(0).Cells(0).Font.Bold = True '
                ' //set No Results found to the new added cell
                gv.Rows(0).Cells(0).Text = "NO RESULT FOUND!" '
            End Sub


            Public Function GeneratePassword(ByVal length As Integer, ByVal numberOfNonAlphanumericCharacters As Integer) As String
                'Make sure length and numberOfNonAlphanumericCharacters are valid....
                If ((length < 1) OrElse (length > 128)) Then
                    Throw New ArgumentException("Membership_password_length_incorrect")
                End If

                If ((numberOfNonAlphanumericCharacters > length) OrElse (numberOfNonAlphanumericCharacters < 0)) Then
                    Throw New ArgumentException("Membership_min_required_non_alphanumeric_characters_incorrect")
                End If

                Do While True
                    Dim i As Integer
                    Dim nonANcount As Integer = 0
                    Dim buffer1 As Byte() = New Byte(length - 1) {}

                    'chPassword contains the password's characters as it's built up
                    Dim chPassword As Char() = New Char(length - 1) {}

                    'chPunctionations contains the list of legal non-alphanumeric characters
                    Dim chPunctuations As Char() = "!@@$%^^*()_-+=[{]};:>|./?".ToCharArray()

                    'Get a cryptographically strong series of bytes
                    Dim rng As New System.Security.Cryptography.RNGCryptoServiceProvider
                    rng.GetBytes(buffer1)

                    For i = 0 To length - 1
                        'Convert each byte into its representative character
                        Dim rndChr As Integer = (buffer1(i) Mod 87)
                        If (rndChr < 10) Then
                            chPassword(i) = Convert.ToChar(Convert.ToUInt16(48 + rndChr))
                        Else
                            If (rndChr < 36) Then
                                chPassword(i) = Convert.ToChar(Convert.ToUInt16((65 + rndChr) - 10))
                            Else
                                If (rndChr < 62) Then
                                    chPassword(i) = Convert.ToChar(Convert.ToUInt16((97 + rndChr) - 36))
                                Else
                                    chPassword(i) = chPunctuations(rndChr - 62)
                                    nonANcount += 1
                                End If
                            End If
                        End If
                    Next

                    If nonANcount < numberOfNonAlphanumericCharacters Then
                        Dim rndNumber As New Random
                        For i = 0 To (numberOfNonAlphanumericCharacters - nonANcount) - 1
                            Dim passwordPos As Integer
                            Do
                                passwordPos = rndNumber.Next(0, length)
                            Loop While Not Char.IsLetterOrDigit(chPassword(passwordPos))
                            chPassword(passwordPos) = chPunctuations(rndNumber.Next(0, chPunctuations.Length))
                        Next
                    End If

                    Return New String(chPassword)
                Loop
            End Function

            Private Function PrepareCommand(ByVal ParamName As String, ByVal ParamValue As Object, ByVal ParamType As SqlDbType, ByVal ParamSize As Int16, ByVal ParamDir As ParameterDirection) As SqlParameter
                Dim Param As New SqlParameter
                Param.ParameterName = ParamName
                If ParamValue Is Nothing Then
                    Param.Value = DBNull.Value
                Else
                    Param.Value = ParamValue
                End If
                Param.SqlDbType = ParamType
                Param.Size = ParamSize
                Param.Direction = ParamDir
                Return Param
            End Function
           

            Public Function CheckAdminLogin(ByVal UserId As String, ByVal Password As String, ByVal IPAddress As String) As SqlDataReader
                Dim arrList As New ArrayList
                arrList.Add(PrepareCommand("@UserId", UserId, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Password", Password, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@IPAddress", IPAddress, SqlDbType.VarChar, 20, ParameterDirection.Input))
                Dim dr As SqlDataReader = ExecDataReaderProc("CheckAdminLogin", arrList.ToArray())
                Return dr
            End Function


            Function Get_Operator() As SqlDataReader
                Dim dr As SqlDataReader
                dr = ExecDataReader("Select UserName From UserMaster")
                Return dr
            End Function
           
            Function CreateOperator(ByVal UserName As String, ByVal UserId As String, ByVal EmailId As String, ByVal Address As String, ByVal Password As String, ByVal Mobile As String) As String
                Dim arrList As New ArrayList
                arrList.Add(PrepareCommand("@UserName", UserName, SqlDbType.VarChar, 100, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@LoginId", UserId, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Email", EmailId, SqlDbType.VarChar, 100, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Address", Address, SqlDbType.VarChar, 200, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Password", Password, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Mobile", Mobile, SqlDbType.VarChar, 10, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Sno", "", SqlDbType.Int, 0, ParameterDirection.Output))
                Dim str As String = ExecNonQueryProc("Prc_CreateOperator", arrList.ToArray())
                Return str
            End Function

            Public Function DisabledButtonCode(Optional ByVal validationGroup As String = "") As String
                Dim sbValid As New System.Text.StringBuilder()
                sbValid.Append("if (typeof(Page_ClientValidate) == 'function') { ")
                sbValid.Append("if (Page_ClientValidate('" & validationGroup & "') == false) { return false; }} ")
                sbValid.Append("this.value = 'Please wait...';")
                sbValid.Append("this.disabled = true;")
                Return sbValid.ToString
            End Function
            Public Function isOperator(ByVal LoginId As String) As Boolean
                Dim isExists As Boolean
                Try
                    isExists = (New BusinessLogicLayer).ExecScalar("if Exists(select 1 from UserMaster where LoginId=@LoginId) select 1 else select 0", "@LoginId", LoginId)
                Catch ex As Exception
                    isExists = True
                Finally

                End Try
                Return isExists
            End Function

            
            Public Function InsertMail(ByVal _From As String, ByVal _To As String, ByVal Subject As String, ByVal Message As String, ByVal MsgType As String, ByVal InsertBy As String) As String
                Dim result As String
                Dim arrList As New ArrayList
                arrList.Add(PrepareCommand("@From", _From, SqlDbType.VarChar, 50, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@To", _To, SqlDbType.VarChar, 50, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Subject", Subject, SqlDbType.VarChar, 500, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Message", Message, SqlDbType.VarChar, 500, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@MsgType", MsgType, SqlDbType.VarChar, 50, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@InsertBy", InsertBy, SqlDbType.VarChar, 50, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Result", "", SqlDbType.VarChar, 200, ParameterDirection.Output))
                result = ExecNonQueryProc("prc_InsertMail", arrList.ToArray())
                Return result
            End Function
            
            Public Sub CreateMengerMenuFile(ByVal MenuStr As String)

                Dim xd As New System.Xml.XmlDocument
                Dim MenusNode As XmlNode = xd.CreateElement("Menus")
                xd.AppendChild(MenusNode)

                Dim TopNode, ChildNode As XmlNode
                Dim attr As XmlAttribute
                Dim sql As String = "select row_number() over(order by menuid) as Sno,* from MemberMenu where IsMenuActive=1 And MenuLevel=1 And MenuID in (" & MenuStr & ")"
                Dim dr As SqlDataReader = ExecDataReader(sql)
                While dr.Read
                    TopNode = xd.CreateElement("Menu")
                    attr = xd.CreateAttribute("MenuName")
                    attr.Value = dr("MenuName").ToString()
                    TopNode.Attributes.Append(attr)
                    attr = xd.CreateAttribute("Url")
                    attr.Value = dr("VirtualName").ToString()
                    TopNode.Attributes.Append(attr)
                    attr = xd.CreateAttribute("Id")
                    attr.Value = dr("MenuID")
                    TopNode.Attributes.Append(attr)
                    attr = xd.CreateAttribute("MenuIcon")
                    attr.Value = dr("MenuIcon")
                    TopNode.Attributes.Append(attr)
                    Dim dr2 As SqlDataReader = ExecDataReader("select row_number() over(order by menuid) as Sno,* from MemberMenu where IsMenuActive=1 And MenuLevel=2 And MenuParentID='" & dr("MenuID") & "' And MenuID in (" & MenuStr & ")")
                    While dr2.Read
                        ChildNode = xd.CreateElement("ChildMenu")
                        attr = xd.CreateAttribute("MenuName")
                        attr.Value = dr2("MenuName").ToString()
                        ChildNode.Attributes.Append(attr)
                        attr = xd.CreateAttribute("Url")
                        attr.Value = dr2("VirtualName").ToString()
                        ChildNode.Attributes.Append(attr)
                        TopNode.AppendChild(ChildNode)
                    End While
                    MenusNode.AppendChild(TopNode)
                End While
                xd.Save(ctx.Server.MapPath("~/Xml/") & "membermenu.xml")
            End Sub

            Public Sub CreateMengerMenuFileVirtual(ByVal MenuStr As String)

                Dim xd As New System.Xml.XmlDocument
                Dim MenusNode As XmlNode = xd.CreateElement("Menus")
                xd.AppendChild(MenusNode)


                Dim TopNode, ChildNode As XmlNode
                Dim attr As XmlAttribute
                Dim sql As String = "select row_number() over(order by menuid) as Sno,* from MemberMenu where IsMenuActive=1 And MenuLevel=1 And MenuID in (" & MenuStr & ")"
                Dim dr As SqlDataReader = ExecDataReader(sql)
                While dr.Read
                    TopNode = xd.CreateElement("Menu")
                    attr = xd.CreateAttribute("MenuName")
                    attr.Value = dr("MenuName").ToString()
                    TopNode.Attributes.Append(attr)
                    attr = xd.CreateAttribute("Id")
                    attr.Value = dr("MenuID")
                    TopNode.Attributes.Append(attr)
                    attr = xd.CreateAttribute("Url")
                    attr.Value = dr("VirtualName").ToString()
                    TopNode.Attributes.Append(attr)
                    attr = xd.CreateAttribute("Title")
                    attr.Value = dr("PageTitle").ToString()
                    TopNode.Attributes.Append(attr)
                    attr = xd.CreateAttribute("PageHeader")
                    attr.Value = dr("PageHeader").ToString()
                    TopNode.Attributes.Append(attr)


                    Dim dr2 As SqlDataReader = ExecDataReader("select row_number() over(order by menuid) as Sno,* from MemberMenu where IsMenuActive=1 And MenuLevel=2 And MenuParentID='" & dr("MenuID") & "' And MenuID in (" & MenuStr & ")")
                    While dr2.Read
                        ChildNode = xd.CreateElement("ChildMenu")
                        attr = xd.CreateAttribute("MenuName")
                        attr.Value = dr2("MenuName").ToString()
                        ChildNode.Attributes.Append(attr)
                        attr = xd.CreateAttribute("Id")
                        attr.Value = dr2("MenuId")
                        ChildNode.Attributes.Append(attr)
                        attr = xd.CreateAttribute("Url")
                        attr.Value = dr2("VirtualName").ToString()
                        ChildNode.Attributes.Append(attr)
                        attr = xd.CreateAttribute("Title")
                        attr.Value = dr2("PageTitle").ToString()
                        ChildNode.Attributes.Append(attr)
                        attr = xd.CreateAttribute("PageHeader")
                        attr.Value = dr2("PageHeader").ToString()
                        ChildNode.Attributes.Append(attr)

                        TopNode.AppendChild(ChildNode)
                    End While
                    MenusNode.AppendChild(TopNode)
                End While
                xd.Save(ctx.Server.MapPath("~/Xml/") & "virtualmenu.xml")
            End Sub
            Public Sub ReadXML()
                Dim reader As New XmlTextReader("books.xml")
                While reader.Read()
                    Dim str As String = reader.ReadInnerXml("Url").ToString()

                End While
            End Sub

            Public Sub CreateMengerMenuFileVirtual1(ByVal MenuStr As String)

                Dim xd As New System.Xml.XmlDocument
                Dim MenusNode As XmlNode = xd.CreateElement("Menus")
                xd.AppendChild(MenusNode)



                Dim sql As String = "select row_number() over(order by menuid) as Sno,* from MemberMenu where MenuID in (" & MenuStr & ")"
                Dim dr As SqlDataReader = ExecDataReader(sql)
                While dr.Read


                    Dim Menu As XmlElement = xd.CreateElement("Menu")


                    Dim Id As XmlElement = xd.CreateElement("Id")
                    Id.InnerText = dr("MenuID")
                    Menu.AppendChild(Id)
                    Dim MenuName As XmlElement = xd.CreateElement("MenuName")
                    MenuName.InnerText = dr("MenuName").ToString()
                    Menu.AppendChild(MenuName)
                    Dim Url As XmlElement = xd.CreateElement("Url")
                    Url.InnerText = dr("MenuUrl").ToString()
                    Menu.AppendChild(Url)
                    Dim VirtualName As XmlElement = xd.CreateElement("VirtualName")
                    VirtualName.InnerText = dr("VirtualName").ToString()
                    Menu.AppendChild(VirtualName)
                    Dim Title As XmlElement = xd.CreateElement("Title")
                    Title.InnerText = dr("PageTitle").ToString()
                    Menu.AppendChild(Title)
                    Dim PageHeader As XmlElement = xd.CreateElement("PageHeader")
                    PageHeader.InnerText = dr("PageHeader").ToString()
                    Menu.AppendChild(PageHeader)
                    Dim HelpText As XmlElement = xd.CreateElement("HelpText")
                    HelpText.InnerText = dr("HelpText").ToString()
                    Menu.AppendChild(HelpText)

                    MenusNode.AppendChild(Menu)






                End While
                xd.Save(ctx.Server.MapPath("~/Xml/") & "virtualmenu1.xml")
            End Sub




            Public Function SetSMSTemplate(ByVal TemplateName As String, ByVal ParamArray obj() As Object) As String
                'Dim xmldoc As New XmlDocument()
                'xmldoc.Load("SmsTemplate.xml")

                Dim Template As String = ""
                'Dim elemList As XmlNodeList = xmldoc.GetElementsByTagName("Tfor")
                Dim reader As New XmlTextReader(HttpContext.Current.Server.MapPath("~/Xml/SmsTemplate.xml"))
                Dim dt As New DataSet
                dt.ReadXml(reader)
                Dim table As DataTable = dt.Tables(0)
                Dim foundRows() As DataRow
                Dim Isactive As String = ""
                foundRows = table.Select("TFor = '" & TemplateName & "'")
                For Each row In foundRows
                    If row("Isactive") = "Active" Then
                        Template = row("Message")
                    End If
                Next

                'Dim doc As New XmlDocument()
                'doc.Load(reader)
                'reader.Close()
                'Dim oldCd As XmlNode
                'Dim root As XmlElement = doc.DocumentElement
                'oldCd = root.SelectSingleNode("//templateAd[@Tfor='" & TemplateName & "' ]")

                Dim VName, VValue As String
                For i As Integer = 0 To obj.Length - 1
                    VName = obj(i)
                    i = i + 1
                    VValue = obj(i)
                    Template = Template.Replace(VName, VValue)
                Next
                Return Template
            End Function

            Public Function SetPageUrl(ByVal VirtualName As String, ByVal ParamArray obj() As Object) As String


                Dim MenuName As String

                Dim reader As New XmlTextReader(HttpContext.Current.Server.MapPath("~/Xml/virtualmenu1.xml"))
                Dim dt As New DataSet
                dt.ReadXml(reader)
                Dim table As DataTable = dt.Tables(0)
                Dim foundRows() As DataRow
                foundRows = table.Select("VirtualName = '" & VirtualName & "'")
                For Each row In foundRows
                    MenuName = row("Url")
                Next




                Dim VName, VValue As String
                For i As Integer = 0 To obj.Length - 1
                    VName = obj(i)
                    i = i + 1
                    VValue = obj(i)
                    MenuName = MenuName.Replace(VName, VValue)
                Next
                Return MenuName
            End Function

            Public Function Notification(ByVal faction As String, ByVal fid As Integer, ByVal fsubject As String, ByVal fmessage As String) As Integer
                Dim val As Integer
                Dim arrList As New ArrayList
                arrList.Add(PrepareCommand("@faction", faction, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@fid", fid, SqlDbType.Int, 0, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@fsubject", fsubject, SqlDbType.VarChar, 200, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@fmessage", fmessage, SqlDbType.VarChar, 500, ParameterDirection.Input))
                val = ExecNonQueryProc("prc_managenotification", arrList)
                Return val
            End Function
            Public Function GetNotification(ByVal fid As Integer, ByVal listcount As Integer) As DataTable
                Dim arrList As New ArrayList
                arrList.Add(PrepareCommand("@fid", fid, SqlDbType.Int, 0, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@listcount", listcount, SqlDbType.Int, 0, ParameterDirection.Input))
                Dim dt As DataTable = ExecDataTableProc("prc_getnotification", arrList.ToArray)
                Return dt
            End Function
            Public Function GetItemWiseMember(ByVal introid As String) As DataTable

                Dim arrList As New ArrayList
                arrList.Add(PrepareCommand("@introid", introid, SqlDbType.VarChar, 100, ParameterDirection.Input))
                Dim dt As DataTable = ExecDataTableProc("prc_SelectItemWiseMember", arrList.ToArray)
                Return dt
            End Function
            Public Function SelectLineChart(ByVal memberid As String) As DataTable
                Dim arrList As New ArrayList
                arrList.Add(PrepareCommand("@memberid", memberid, SqlDbType.VarChar, 20, ParameterDirection.Input))
                Dim dt As DataTable = ExecDataTableProc("prc_selectLineChart", arrList.ToArray)
                Return dt
            End Function
            Public Function Add_AdminBankDetails(ByVal id As Integer, ByVal Bankname As String, ByVal branch As String, ByVal Ifscode As String, ByVal acno As String, ByVal acholder As String, ByVal actype As String, ByVal isactive As String, ByVal BankImage As String, ByVal UsedFor As String) As String

                Dim result As String
                Dim arrList As New ArrayList

                arrList.Add(PrepareCommand("@Id", id, SqlDbType.Int, 0, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@BankName", Bankname, SqlDbType.VarChar, 50, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@BranchName", branch, SqlDbType.VarChar, 50, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Ifscode", Ifscode, SqlDbType.VarChar, 50, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@AcNo", acno, SqlDbType.VarChar, 50, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@AcHolderName ", acholder, SqlDbType.VarChar, 30, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@acType", actype, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@isActive", isactive, SqlDbType.Int, 0, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@BankImage", BankImage, SqlDbType.VarChar, 200, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@UsedFor", UsedFor, SqlDbType.VarChar, 200, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Result", "", SqlDbType.VarChar, 20, ParameterDirection.Output))

                result = ExecNonQueryProc("Add_AdminBankDetails", arrList.ToArray())
                Return result
            End Function
    Public Sub CreateMainmenu(ByVal Sno As Integer, ByVal MenuName As String, ByVal Url As String, ByVal ParentMenuid As Integer, ByVal Pagename As String, ByVal Active As Integer, ByVal Snostr As String, ByVal MenuClass As String)
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@Sno", Sno, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@MenuName", MenuName, SqlDbType.VarChar, 100, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Url", Url, SqlDbType.VarChar, 500, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@ParentMenuid", ParentMenuid, SqlDbType.Int, 50, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Pagename", Pagename, SqlDbType.VarChar, 20, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Active", Active, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Snostr", Snostr, SqlDbType.VarChar, 20, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@MenuClass", MenuClass, SqlDbType.VarChar, 200, ParameterDirection.Input))
        ExecNonQueryProc("Prc_CreateMainMenu", arrList.ToArray)
    End Sub
    Public Sub CreateSubmenu(ByVal Sno As Integer, ByVal MenuName As String, ByVal Url As String, ByVal ParentMenuid As Integer, ByVal Pagename As String, ByVal Active As Integer, ByVal Snostr As String, ByVal MenuClass As String)
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@Sno", Sno, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@MenuName", MenuName, SqlDbType.VarChar, 100, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Url", Url, SqlDbType.VarChar, 500, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@ParentMenuid", ParentMenuid, SqlDbType.Int, 50, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Pagename", Pagename, SqlDbType.VarChar, 20, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Active", Active, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Snostr", Snostr, SqlDbType.VarChar, 20, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@MenuClass", MenuClass, SqlDbType.VarChar, 20, ParameterDirection.Input))
        ExecNonQueryProc("Prc_CreateSubMenu", arrList.ToArray)
    End Sub



            '---------------------------------Ticket Support System-----------------------------------
            Public Function SupportOpenTicket(ByVal MsrNo As Integer, ByVal TicketId As String, ByVal Deptid As Integer, ByVal subject As String, ByVal description As String, ByVal filepath As String, ByVal generatedby As String) As String

                Dim result As String
                Dim arrList As New ArrayList
                arrList.Add(PrepareCommand("@fuserid", MsrNo, SqlDbType.Int, 0, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@fticketid", TicketId, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@fdeptid", Deptid, SqlDbType.Int, 0, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@fsubject", subject, SqlDbType.VarChar, 500, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@fdescription", description, SqlDbType.VarChar, 5000, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@ffilepath", filepath, SqlDbType.VarChar, 200, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@fgenerateby", generatedby, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@return", "", SqlDbType.VarChar, 500, ParameterDirection.Output))
                result = ExecNonQueryProc("Prc_SupportOpenTicket", arrList.ToArray())
                Return result
            End Function
            Public Function SupportTicketReply(ByVal TicketId As String, ByVal description As String, ByVal filepath As String, ByVal generatedby As String) As String

                Dim result As String
                Dim arrList As New ArrayList

                arrList.Add(PrepareCommand("@fticketid", TicketId, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@fdescription", description, SqlDbType.VarChar, 5000, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@ffilepath", filepath, SqlDbType.VarChar, 200, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@fgenerateby", generatedby, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@return", "", SqlDbType.VarChar, 500, ParameterDirection.Output))
                result = ExecNonQueryProc("Prc_SupportInsertReply", arrList.ToArray())
                Return result
            End Function


            Public Function SelectSupportTicket(ByVal FromDate As String, ByVal ToDate As String, ByVal Status As String, ByVal MemberId As String, ByVal Export As Integer) As DataTable
                Dim arrList As New ArrayList
                If Not IsDate(FromDate) Then
                    FromDate = Nothing
                End If
                If Not IsDate(ToDate) Then
                    ToDate = Nothing
                End If
                arrList.Add(PrepareCommand("@FromDate", FromDate, SqlDbType.VarChar, 10, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Todate", ToDate, SqlDbType.VarChar, 10, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Status", Status, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@MemberId", MemberId, SqlDbType.VarChar, 20, ParameterDirection.Input))
                arrList.Add(PrepareCommand("@Export", Export, SqlDbType.Bit, 0, ParameterDirection.Input))
                Dim dr As DataTable = ExecDataTableProc("Prc_SupportSelectTicket", arrList.ToArray)
                Return dr
            End Function

            Public Function GetPagePopupScript(ByVal Page As String) As String
                Dim PopupScript As String = ""
                Dim PopupImage As String = ""
                Dim NotificationHeader As String = ""
                Dim NotificationMsg As String = ""
                Dim NotificationType As String = ""
                Dim reader As New XmlTextReader(HttpContext.Current.Server.MapPath("~/Xml/popup.xml"))
                Dim dt As New DataSet
                dt.ReadXml(reader)
                Dim table As DataTable = dt.Tables(0)
                Dim foundRows() As DataRow
                foundRows = table.Select("MenuUrl = '" & Page & "'")
                For Each row In foundRows
                    NotificationType = row("NotificationType")
                    NotificationHeader = row("NotificationHeader")
                    NotificationMsg = row("NotificationMsg")
                    PopupImage = row("PopupImage")
                Next
                If NotificationType <> "" Then
                    If NotificationType = "Image" Then
                        PopupScript = "<link href='popup/popup.css' rel='stylesheet' type='text/css' /><script type='text/javascript'> window.onload = function () {$('#myModal').modal('show');};</script><div class='modal fade popupbg' id='myModal' tabindex='-1' role='dialog' aria-labelledby='myModalLabel' aria-hidden='true'><span class='btnpopupclose b-close' data-dismiss='modal'><span>X</span></span><img src='popup/" & PopupImage & "'></div>"
                    ElseIf NotificationType = "Text" Then
                        PopupScript = "<link href='popup/popup.css' rel='stylesheet' type='text/css' /><script type='text/javascript'> window.onload = function () {$('#myModal').modal('show');};</script><div class='modal fade popupbg' id='myModal' tabindex='-1' role='dialog' aria-abelledby='myModalLabel' aria-hidden='true'><div class='modal-header'><button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button><h3>" & NotificationHeader & "</h3> </div><div class='modal-body'><p>" & NotificationMsg & "</p></div><div class='modal-footer'><button type='button' class='btn btn-default' data-dismiss='modal'>Close</button></div></div>"
                    End If
                End If

                Dim sPath As String = System.Web.HttpContext.Current.Request.Url.AbsolutePath
                If Not sPath.Contains("user/") Then
                    PopupScript = Replace(PopupScript, "popup/", "user/popup/")
                End If


                Return PopupScript
            End Function
            Public Function GetPageVirtualName(ByVal PageUrl As String, ByVal ParamArray obj() As Object) As String
                Dim VirtualName As String = ""
                Dim reader As New XmlTextReader(HttpContext.Current.Server.MapPath("~/Xml/virtualmenu1.xml"))
                Dim dt As New DataSet
                dt.ReadXml(reader)
                Dim table As DataTable = dt.Tables(0)
                Dim foundRows() As DataRow
                foundRows = table.Select("Url = '" & PageUrl & "'")
                For Each row In foundRows
                    VirtualName = row("VirtualName")
                Next
                Return VirtualName
            End Function

            
           

    Public Function BindLocality() As DataTable
        Dim mDataTable As DataTable
        Try
            mDataTable = ExecDataTable("select * from dbo.LocalityMaster")
        Catch ex As Exception
            mDataTable = Nothing
        Finally

        End Try
        Return mDataTable
    End Function

    Public Function BindState() As DataTable
        Dim mDataTable As DataTable
        Try
            mDataTable = ExecDataTable("select * from dbo.tblState")
        Catch ex As Exception
            mDataTable = Nothing
        Finally

        End Try
        Return mDataTable
    End Function
    Public Function BindCity(ByVal State As String) As DataTable
        Dim mDataTable As DataTable
        Try
            mDataTable = ExecDataTable("select * from dbo.City where State=@State", "@State", State)
        Catch ex As Exception
            mDataTable = Nothing
        Finally

        End Try
        Return mDataTable
    End Function
    

    Sub AddNews(ByVal cnt As Integer, ByVal Title As String, ByVal News As String, ByVal Deactivated As Integer, ByVal UserName As String)
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@cnt", cnt, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Title", Title, SqlDbType.VarChar, 1000, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@News", News, SqlDbType.NVarChar, 8000, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Deactivated", Deactivated, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@UserName", UserName, SqlDbType.VarChar, 200, ParameterDirection.Input))


        ExecNonQueryProc("Prc_AddUpdateNews", arrList.ToArray())

    End Sub


    Sub AddEvent(ByVal cnt As Integer, ByVal Title As String, ByVal EventDetail As String, ByVal Location As String, ByVal OnDate As String, ByVal EventTime As String, ByVal Deactivated As Integer, ByVal UserName As String)
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@cnt", cnt, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Title", Title, SqlDbType.VarChar, 1000, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@EventDetail", EventDetail, SqlDbType.NVarChar, 2000, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Deactivated", Deactivated, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@UserName", UserName, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Location", Location, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@EventTime", EventTime, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@OnDate", OnDate, SqlDbType.VarChar, 200, ParameterDirection.Input))


        ExecNonQueryProc("Prc_AddUpdateEvents", arrList.ToArray())

    End Sub


    Sub AddGallaryImage(ByVal ImageName As String, ByVal GallaryId As Integer, ByVal FolderPath As String, ByVal UserName As String)
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@ImageName", ImageName, SqlDbType.VarChar, 50, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@FolderPath", FolderPath, SqlDbType.NVarChar, 500, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@GallaryId", GallaryId, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@UserName", UserName, SqlDbType.VarChar, 200, ParameterDirection.Input))

        ExecNonQueryProc("Prc_SaveGallaryImage", arrList.ToArray())

    End Sub

    Function AddNotice(ByVal Notice As String, ByVal NoticeFor As String, ByVal NoticeBy As String, ByVal ClassId As Integer) As String
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@Notice", Notice, SqlDbType.NVarChar, 8000, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@ClassId", ClassId, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@NoticeFor", NoticeFor, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@NoticeBy", NoticeBy, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Result", "", SqlDbType.VarChar, 200, ParameterDirection.Output))
        Dim Result As String = ExecNonQueryProc("Prc_AddNotice", arrList.ToArray())
        Return Result
    End Function



    Function AddHolydays(ByVal HolidaysStuctureTableType As DataTable) As String
        Dim arrList As New ArrayList

        arrList.Add(PrepareCommand("@Holidaystbl", HolidaysStuctureTableType, SqlDbType.Structured, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Result", "", SqlDbType.VarChar, 200, ParameterDirection.Output))
        Dim Result As String = ExecNonQueryProc("Prc_Holidays", arrList.ToArray())
        Return Result
    End Function
   
   

    Function AddDocument(ByVal DocumentName As String, ByVal DocumentDetail As String, ByVal DownloadURL As String, ByVal UploadBy As String) As String
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@DocumentName", DocumentName, SqlDbType.NVarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@DocumentDetail", DocumentDetail, SqlDbType.NVarChar, 4000, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@DownloadURL", DownloadURL, SqlDbType.VarChar, 8000, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@UploadBy", UploadBy, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Result", "", SqlDbType.VarChar, 200, ParameterDirection.Output))

        Dim Result As String = ExecNonQueryProc("Prc_UploadDocuments", arrList.ToArray())
        Return Result
    End Function

    Sub SaveWebsiteHit(ByVal Website As String, ByVal PageURL As String, ByVal UserName As String, ByVal IPAddress As String, ByVal Location As String)
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@Website", Website, SqlDbType.NVarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@PageURL", PageURL, SqlDbType.NVarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@UserName", UserName, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@IPAddress", IPAddress, SqlDbType.VarChar, 100, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Location", Location, SqlDbType.VarChar, 200, ParameterDirection.Input))

        ExecNonQueryProc("Prc_SaveWebsiteHit", arrList.ToArray())
    End Sub
    Sub SaveWebsiteQuery(ByVal Website As String, ByVal UserName As String, ByVal IPAddress As String, ByVal ContactName As String, ByVal Country As String, ByVal State As String, ByVal City As String, ByVal Email As String, ByVal Mobile As String, ByVal Query As String)
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@Website", Website, SqlDbType.NVarChar, 100, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@UserName", UserName, SqlDbType.VarChar, 50, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@IPAddress", IPAddress, SqlDbType.VarChar, 50, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@ContactName", ContactName, SqlDbType.NVarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Country", Country, SqlDbType.VarChar, 50, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@State", State, SqlDbType.VarChar, 50, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@City", City, SqlDbType.VarChar, 50, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Email", Email, SqlDbType.VarChar, 50, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Mobile", Mobile, SqlDbType.VarChar, 50, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Query", Query, SqlDbType.VarChar, 2000, ParameterDirection.Input))

        ExecNonQueryProc("Prc_SaveWebsiteQueries", arrList.ToArray())
    End Sub


    
    Function Get_Notice(ByVal EmployeeId As String, ByVal StudenId As String, ByVal ParentId As String) As String
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@EmployeeId", EmployeeId, SqlDbType.VarChar, 20, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@StudenId", StudenId, SqlDbType.VarChar, 20, ParameterDirection.Input))

        arrList.Add(PrepareCommand("@ParentId", ParentId, SqlDbType.VarChar, 20, ParameterDirection.Input))
        Dim dt As DataTable = ExecDataTableProc("Get_Notice", arrList.ToArray())
        Dim Notice As String = ""
        If dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                Notice = row.Item("Notice") + "</br>"
            Next
        End If


        Return Notice
    End Function
    Public Function ClintIpAddress() As String
        Dim strIpAddress As String
        strIpAddress = System.Web.HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
        If strIpAddress = "" Then
            strIpAddress = System.Web.HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
        End If
        ClintIpAddress = strIpAddress
        Return ClintIpAddress
    End Function
    Public Function IPLocation() As String
        Try
            Dim IP, strUrl As String
            IP = ClintIpAddress()
            strUrl = "http://api.ipinfodb.com/v3/ip-city/?key=9effe535bd412cf02f722ec43ad0b927f673753e9f3433637917c4c5e5e9b36d"
            strUrl += "&ip=" & IP & ""
            Dim objURI As Uri = New Uri(strUrl)
            Dim objWebRequest As WebRequest = WebRequest.Create(objURI)
            Dim objWebResponse As WebResponse = objWebRequest.GetResponse()
            Dim objStream As Stream = objWebResponse.GetResponseStream()
            Dim objStreamReader As StreamReader = New StreamReader(objStream)
            Dim strHTML As String = objStreamReader.ReadToEnd
            IPLocation = strHTML
        Catch ex As Exception
            IPLocation = ""
        End Try
        Return IPLocation
    End Function
    Public Function geospilt(ByVal Number As Integer) As String
        Dim location, ReturnData As String
        location = IPLocation()
        Dim arr As Array
        arr = location.Split(";")
        ReturnData = arr(Number)
        Return ReturnData
    End Function
    Public Function AddUpdate_Product(ByVal Cnt As Integer, ByVal UserName As String, ByVal ImageName As String, ByVal GallaryId As Integer, ByVal FolderPath As String, ByVal ProductName As String, ByVal ShortDesc As String, ByVal ProductDesc As String, ByVal DeActivated As Integer) As String

        Dim result As String
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@Cnt", Cnt, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@UserName", UserName, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@ImageName", ImageName, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@GallaryId", GallaryId, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@FolderPath", FolderPath, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@ProductName", ProductName, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@ShortDesc ", ShortDesc, SqlDbType.VarChar, 2000, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@ProductDesc", ProductDesc, SqlDbType.NVarChar, 8000, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@DeActivated", DeActivated, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Result", "", SqlDbType.VarChar, 200, ParameterDirection.Output))
        result = ExecNonQueryProc("AddUpdate_Product", arrList.ToArray())
        Return result
    End Function
    Public Function AddUpdate_Blog(ByVal BlogId As Integer, ByVal BlogTitle As String, ByVal BlogContent As String,
        ByVal UserName As String, ByVal ImageName As String, ByVal MetaTag As String, ByVal PageURL As String, ByVal PageTitle As String,
        ByVal MetaDescription As String, ByVal MetaKeywords As String, ByVal DeActivated As Integer) As String

        Dim result As String
        Dim arrList As New ArrayList
        arrList.Add(PrepareCommand("@BlogId", BlogId, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@BlogTitle", BlogTitle, SqlDbType.NVarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@BlogContent", BlogContent, SqlDbType.NVarChar, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@ImageName", ImageName, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@MetaTag", MetaTag, SqlDbType.NVarChar, 8000, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@PageURL", PageURL, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@PageTitle", PageTitle, SqlDbType.NVarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@MetaDescription", MetaDescription, SqlDbType.NVarChar, 500, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@MetaKeywords", MetaKeywords, SqlDbType.NVarChar, 1000, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@DeActivated", DeActivated, SqlDbType.Int, 0, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@UserName", UserName, SqlDbType.VarChar, 200, ParameterDirection.Input))
        arrList.Add(PrepareCommand("@Result", "", SqlDbType.VarChar, 200, ParameterDirection.Output))
        result = ExecNonQueryProc("Prc_AddUpdateBlog", arrList.ToArray())
        Return result
    End Function
End Class



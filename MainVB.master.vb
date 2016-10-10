Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Partial Class MainVB
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(sender As Object, e As EventArgs)
        If Not Me.IsPostBack Then
            Dim dt As DataTable = Me.GetData(0)
            PopulateMenu(dt, 0, Nothing)
        End If
    End Sub

    Private Function GetData(parentMenuId As Integer) As DataTable
        Dim query As String = "SELECT [MenuId], [Title], [Description], [Url] FROM [Menus] WHERE ParentMenuId = @ParentMenuId"
        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Using con As New SqlConnection(constr)
            Dim dt As New DataTable()
            Using cmd As New SqlCommand(query)
                Using sda As New SqlDataAdapter()
                    cmd.Parameters.AddWithValue("@ParentMenuId", parentMenuId)
                    cmd.CommandType = CommandType.Text
                    cmd.Connection = con
                    sda.SelectCommand = cmd
                    sda.Fill(dt)
                End Using
            End Using
            Return dt
        End Using
    End Function

    Private Sub PopulateMenu(dt As DataTable, parentMenuId As Integer, parentMenuItem As MenuItem)
        Dim currentPage As String = Path.GetFileName(Request.Url.AbsolutePath)
        For Each row As DataRow In dt.Rows
            Dim menuItem As New MenuItem() With { _
             .Value = row("MenuId").ToString(), _
             .Text = row("Title").ToString(), _
              .NavigateUrl = row("Url").ToString(), _
             .Selected = row("Url").ToString().EndsWith(currentPage, StringComparison.CurrentCultureIgnoreCase) _
            }
            If parentMenuId = 0 Then
                Menu1.Items.Add(menuItem)
                Dim dtChild As DataTable = Me.GetData(Integer.Parse(menuItem.Value))
                PopulateMenu(dtChild, Integer.Parse(menuItem.Value), menuItem)
            Else
                parentMenuItem.ChildItems.Add(menuItem)
            End If
        Next
    End Sub
End Class

Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Net.Http
Imports System.Runtime.CompilerServices

Module Utils

    Public SaveFile As String = "usersave.json"
    Public MySave As UserSave = New UserSave()
    Public SmoothingMode As SmoothingMode = SmoothingMode.Default
    Public CompositingQuality As CompositingQuality = CompositingQuality.Default
    Public InterpolationMode As InterpolationMode = InterpolationMode.Default
    Public PixelOffsetMode As PixelOffsetMode = PixelOffsetMode.Default
    Public TimerIntervals As Integer = 30
    Public CpuUsagePauseValue As Integer = 60
    Public BackgroundImage As String = Nothing
    Public SizeMode As PictureBoxSizeMode = PictureBoxSizeMode.Zoom
    Public BackgroundColor As String = ColorTranslator.ToHtml(Color.Black)
    Public SignalRGBPort As Integer = 8123

    Public Sub UpdateSRGBConfigValues(s As SignalRGBSettingsChangedEventArgs)
        Try
            SmoothingMode = s.SmoothingMode
            CompositingQuality = s.CompositingQuality
            InterpolationMode = s.InterpolationMode
            PixelOffsetMode = s.PixelOffsetMode
            TimerIntervals = s.LEDUpdateInterval
            SizeMode = s.CoverImageSizeMode
            BackgroundColor = ColorTranslator.ToHtml(s.BackgroundColor)
            CpuUsagePauseValue = s.CPUUsagePauseValue
            BackgroundImage = s.CoverImage
        Catch ex As Exception
            Logger.Log($"{ex.Message} {ex.StackTrace}")
        End Try
    End Sub

    Public Sub ReadSaveValues(s As UserSave)
        Try
            SmoothingMode = s.SmoothingMode
            CompositingQuality = s.CompositingQuality
            InterpolationMode = s.InterpolationMode
            PixelOffsetMode = s.PixelOffsetMode
            TimerIntervals = s.LedUpdateInterval
            SizeMode = s.CoverImageSizeMode
            BackgroundColor = ColorTranslator.ToHtml(s.BackgroundColor)
            CpuUsagePauseValue = s.CpuUsagePauseValue
            BackgroundImage = s.CoverImage
        Catch ex As Exception
            Logger.Log($"{ex.Message} {ex.StackTrace}")
        End Try
    End Sub

    <Extension>
    Public Function TryParseCoverImage(src As String) As Image
        If src.StartsWith("http") Then
            Try
                Using client As New HttpClient()
                    Dim bytes As Byte() = client.GetByteArrayAsync(src).Result
                    Dim ms As New MemoryStream(bytes)
                    Return Image.FromStream(ms)
                End Using
            Catch ex As Exception
                Return New Bitmap(0, 0)
            End Try
        Else
            If File.Exists(src) Then
                Return Image.FromFile(src)
            Else
                Return New Bitmap(0, 0)
            End If
        End If
    End Function

    <Extension>
    Public Sub DrawRoundedRectangle(graphics As Graphics, pen As Pen, bounds As Rectangle, radius As Integer)
        If graphics Is Nothing Then
            Throw New ArgumentNullException("graphics")
        End If
        If pen Is Nothing Then
            Throw New ArgumentNullException("prush")
        End If

        Using path As GraphicsPath = RoundedRect(bounds, radius)
            graphics.DrawPath(pen, path)
        End Using
    End Sub

    <Extension>
    Public Sub FillRoundedRectangle(graphics As Graphics, brush As Brush, bounds As Rectangle, radius As Integer)
        If graphics Is Nothing Then
            Throw New ArgumentNullException("graphics")
        End If
        If brush Is Nothing Then
            Throw New ArgumentNullException("brush")
        End If

        Using path As GraphicsPath = RoundedRect(bounds, radius)
            graphics.FillPath(brush, path)
        End Using
    End Sub

    Private Function RoundedRect(bounds As Rectangle, radius As Integer) As GraphicsPath
        Dim diameter As Integer = radius * 2
        Dim size As Size = New Size(diameter, diameter)
        Dim arc As Rectangle = New Rectangle(bounds.Location, size)
        Dim path As GraphicsPath = New GraphicsPath

        If (radius = 0) Then
            path.AddRectangle(bounds)
            Return path
        End If

        'top left arc
        path.AddArc(arc, 180, 90)

        'top right arc
        arc.X = bounds.Right - diameter
        path.AddArc(arc, 270, 90)

        'bottom right arc
        arc.Y = bounds.Bottom - diameter
        path.AddArc(arc, 0, 90)

        'bottom left arc
        arc.X = bounds.Left
        path.AddArc(arc, 90, 90)

        path.CloseFigure()
        Return path
    End Function

    <Extension>
    Public Function Base64ToImage(Image As String) As Image
        Try
            If Image = Nothing Then
                Return Nothing
            Else
                Dim b64 As String = Image.Replace(" ", "+")
                Dim bite() As Byte = Convert.FromBase64String(b64)
                Dim stream As New MemoryStream(bite)
                Return Drawing.Image.FromStream(stream)
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    <Extension>
    Public Function ImageToBase64(img As Image, Optional forceFormat As ImageFormat = Nothing, Optional formatting As Base64FormattingOptions = Base64FormattingOptions.InsertLineBreaks) As String
        Try
            If img IsNot Nothing Then
                If forceFormat Is Nothing Then forceFormat = img.RawFormat
                Dim stream As New MemoryStream
                img.Save(stream, forceFormat)
                Return Convert.ToBase64String(stream.ToArray, formatting)
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    <Extension>
    Public Function ResizeImage(ByVal image As Image, ByVal size As Size, Optional ByVal preserveAspectRatio As Boolean = True) As Image
        Dim newWidth As Integer
        Dim newHeight As Integer
        If preserveAspectRatio Then
            Dim originalWidth As Integer = image.Width
            Dim originalHeight As Integer = image.Height
            Dim percentWidth As Single = CSng(size.Width) / CSng(originalWidth)
            Dim percentHeight As Single = CSng(size.Height) / CSng(originalHeight)
            Dim percent As Single = If(percentHeight < percentWidth,
                    percentHeight, percentWidth)
            newWidth = CInt(originalWidth * percent)
            newHeight = CInt(originalHeight * percent)
        Else
            newWidth = size.Width
            newHeight = size.Height
        End If
        Dim newImage As Image = New Bitmap(newWidth, newHeight)
        Using graphicsHandle As Graphics = Graphics.FromImage(newImage)
            graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic
            graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight)
        End Using
        Return newImage
    End Function

    <Extension>
    Public Function ToColor(customcolor As String) As Color
        Try
            Dim red = Math.Ceiling(CSng(customcolor.Split(" ")(0)) * 255)
            Dim green = Math.Ceiling(CSng(customcolor.Split(" ")(1)) * 255)
            Dim blue = Math.Ceiling(CSng(customcolor.Split(" ")(2)) * 255)
            Return Color.FromArgb(red, green, blue)
        Catch ex As Exception
            Return Color.Black
        End Try
    End Function

    Public Function IsSignalRGBRunning() As Boolean
        Dim SignalRGB As Process = Process.GetProcessesByName("SignalRgb").FirstOrDefault
        Return Not SignalRGB Is Nothing
    End Function

End Module

Public Enum LEDShape
    Rectangle
    RoundedRectangle
    Sphere
End Enum

Public Enum MatrixSizeType
    Landscape4_3
    Portrait4_3
    Landscape5_4
    Portrait5_4
    Landscape16_9
    Portrait16_9
    Landscape16_10
    Portrait16_10
    Landscape21_9
    Portrait21_9
    Landscape32_9
    Portrait32_9
End Enum
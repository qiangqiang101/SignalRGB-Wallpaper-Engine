Imports System.ComponentModel
Imports System.Drawing.Printing
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading
Imports Windows.Win32

Public Class frmWallpaper

    Dim configFile As String = WallpaperEngineConfig()
    Dim monitordetection As String = "devicepath"
    Dim display As String = ScreenDevicePath()
    Dim configLastDate As Date = Now

    Public cpuUsage As New PerformanceCounter("Processor", "% Processor Time", "_Total")
    Public WithEvents srgbClient As SignalRGBClient = Nothing
    Public srgbThread As Thread = Nothing

    Dim connectString As String = Nothing
    Dim drawErrorStringOnScreen As Boolean = True

    Private RGBRect As New Rectangle(Location, Size)
    Private WithEvents pbDiffuser As New PictureBox() With {.Dock = DockStyle.None, .BackColor = Color.Transparent, .SizeMode = PictureBoxSizeMode.StretchImage,
        .Anchor = AnchorStyles.Bottom And AnchorStyles.Left And AnchorStyles.Top And AnchorStyles.Right}

    Private Sub frmWallpaper_Load(sender As Object, e As EventArgs) Handles Me.Load
        If File.Exists(SaveFile) Then MySave = New UserSave().Load(SaveFile)

        If configFile <> "error" Then
            monitordetection = TryGetUserSettings("monitordetection", "devicepath", configFile)
            Select Case monitordetection
                Case "devicepath"
                    display = ScreenDevicePath()
                Case "managed"
                    display = ScreenManaged()
                Case "layout"
                    display = ScreenLayout()
            End Select

            UpdateWEConfigValues(configFile, display)

            configLastDate = File.GetLastWriteTime(configFile)
            tmUpdate.Interval = TimerIntervals
            BackColor = ColorTranslator.FromHtml(BackgroundColor)
            pbDiffuser.Image = If(Utils.BackgroundImage = Nothing, Nothing, Image.FromFile(Utils.BackgroundImage))
            pbDiffuser.SizeMode = Utils.SizeMode

            RGBRect.Location = MySave.Location
            RGBRect.Size = MySave.Size
            pbDiffuser.Location = MySave.Location
            pbDiffuser.Size = MySave.Size

            'RGBRect.Location = New Point(PercentageToPoint(MySave.LocationPercentage.X, PositionType.X), PercentageToPoint(MySave.LocationPercentage.Y, PositionType.Y))
            'RGBRect.Size = New Size(PercentageToSize(MySave.SizePercentage.Width, Width), PercentageToSize(MySave.SizePercentage.Height, Height))
            'pbDiffuser.Location = RGBRect.Location
            'pbDiffuser.Size = RGBRect.Size

            'UpdatePanelPositionX(PercentageToPoint(MySave.LocationPercentage.X, PositionType.X))
            'UpdatePanelPositionY(PercentageToPoint(MySave.LocationPercentage.Y, PositionType.Y))
            'ResizePanel(PercentageToSize(MySave.SizePercentage.Width, Width), PercentageToSize(MySave.SizePercentage.Height, Height))

            Controls.Add(pbDiffuser)
            Connect()
        End If
    End Sub

    Public Sub Connect()
        If IsSignalRGBRunning() Then
            Try
                If srgbClient Is Nothing Then
                    srgbClient = New SignalRGBClient(MySave, SignalRGBPort)
                    tmCheckSignalRGB.Stop()
                    srgbThread = New Thread(AddressOf srgbClient.StartListening)
                    With srgbThread
                        .IsBackground = True
                        .Start()
                    End With
                    connectString = Nothing
                Else
                    tmCheckSignalRGB.Stop()
                    srgbThread = New Thread(AddressOf srgbClient.StartListening)
                    With srgbThread
                        .IsBackground = True
                        .Start()
                    End With
                    connectString = Nothing
                End If
            Catch ex As Exception
                Logger.Log($"{ex.Message} {ex.StackTrace}")
                connectString &= $"{vbCrLf}[{Now.ToString("hh:mm:ss tt")}] Connection attempt failed, Local SignalRGB server unavailable."
                tmCheckSignalRGB.Start()
            End Try
        Else
            connectString &= $"{vbCrLf}[{Now.ToString("hh:mm:ss tt")}] Connection attempt failed, SignalRGB isn't running."
            tmCheckSignalRGB.Start()
        End If
    End Sub

    Private Sub PrepareGraphics(graphic As Graphics)
        graphic.SmoothingMode = SmoothingMode
        graphic.CompositingQuality = CompositingQuality
        graphic.InterpolationMode = InterpolationMode
        graphic.PixelOffsetMode = PixelOffsetMode
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim graphic As Graphics = e.Graphics
        PrepareGraphics(graphic)
        graphic.Clear(BackColor)

        Try
            If srgbClient IsNot Nothing Then
                If srgbClient.IsListening Then
                    Dim msWidth As Integer = srgbClient.MatrixSize.Width
                    Dim msHeight As Integer = srgbClient.MatrixSize.Height
                    Dim LedCount As Integer = msWidth * msHeight
                    Dim lastWorkingColor As Color = Color.Black
                    Dim rectangleSize As New SizeF(RGBRect.Width / msWidth, RGBRect.Height / msHeight)

                    Dim matrix(msWidth - 1, msHeight - 1) As String
                    Dim count As Integer = 0

                    For row As Integer = 0 To matrix.GetUpperBound(1) 'j
                        For col As Integer = 0 To matrix.GetUpperBound(0) 'i
                            Try
                                Dim rgbColor = srgbClient.Colors(count)
                                ApplyColor(graphic, srgbClient, col, row, rgbColor, rectangleSize)
                                lastWorkingColor = rgbColor
                            Catch ex As Exception
                                ApplyColor(graphic, srgbClient, col, row, lastWorkingColor, rectangleSize)
                            End Try

                            count += 1
                            If count >= LedCount Then count = 0
                        Next
                    Next
                Else
                    tmCheckSignalRGB.Start()
                End If
            End If
        Catch ex As Exception
            Logger.Log($"{ex.Message} {ex.StackTrace}")
        End Try

        MyBase.OnPaint(e)
    End Sub

    Private Sub ApplyColor(graphic As Graphics, srgbClient As SignalRGBClient, col As Integer, row As Integer, color As Color, rectangleSize As SizeF)
        Using sb As New SolidBrush(color)
            Dim X As Single = (rectangleSize.Width * col) + RGBRect.Location.X
            Dim Y As Single = (rectangleSize.Height * row) + RGBRect.Location.Y
            Dim W As Single = rectangleSize.Width
            Dim H As Single = rectangleSize.Height
            Dim P As Single = srgbClient.LEDPadding

            Select Case srgbClient.LEDShape
                Case LEDShape.Rectangle
                    graphic.FillRectangle(sb, New RectangleF(X + P, Y + P, W - P, H - P))
                Case LEDShape.RoundedRectangle
                    graphic.FillRoundedRectangle(sb, New Rectangle(X + P, Y + P, W - P, H - P), srgbClient.RoundedRectangleCornerRadius)
                Case LEDShape.Sphere
                    graphic.FillEllipse(sb, New RectangleF(X + P, Y + P, W - P, H - P))
            End Select
        End Using
    End Sub

    Private Function HighCpuUsage() As Boolean
        Return CInt(Math.Ceiling(cpuUsage.NextValue)) >= CpuUsagePauseValue
    End Function

    Private Sub tmUpdate_Tick(sender As Object, e As EventArgs) Handles tmUpdate.Tick
        If Not HighCpuUsage() Then
            Invalidate()
            If connectString <> Nothing Then pbDiffuser.Invalidate()
        End If
    End Sub

    Private Sub tmCheckSignalRGB_Tick(sender As Object, e As EventArgs) Handles tmCheckSignalRGB.Tick
        connectString &= $"{vbCrLf}[{Now.ToString("hh:mm:ss tt")}] Attempting to connect to local SignalRGB server."
        Connect()
    End Sub

    Private Sub tmConfig_Tick(sender As Object, e As EventArgs) Handles tmConfig.Tick
        Try
            Dim configDate As Date = File.GetLastWriteTime(configFile)
            If configLastDate <> configDate Then
                configLastDate = configDate
                UpdateWEConfigValues(configFile, display)

                configLastDate = File.GetLastWriteTime(configFile)
                tmUpdate.Interval = TimerIntervals
                BackColor = ColorTranslator.FromHtml(BackgroundColor)
                pbDiffuser.Image = If(Utils.BackgroundImage = Nothing, Nothing, Image.FromFile(Utils.BackgroundImage))
                pbDiffuser.SizeMode = Utils.SizeMode

                RGBRect.Location = MySave.Location
                RGBRect.Size = MySave.Size
                pbDiffuser.Location = MySave.Location
                pbDiffuser.Size = MySave.Size

                'RGBRect.Location = New Point(PercentageToPoint(PanelX, PositionType.X), PercentageToPoint(PanelY, PositionType.Y))
                'RGBRect.Size = New Size(PercentageToSize(PanelWidth, Width), PercentageToSize(PanelHeight, Height))
                'pbDiffuser.Location = RGBRect.Location
                'pbDiffuser.Size = RGBRect.Size

                'UpdatePanelPositionX(PercentageToPoint(MySave.LocationPercentage.X, PositionType.X))
                'UpdatePanelPositionY(PercentageToPoint(MySave.LocationPercentage.Y, PositionType.Y))
                'ResizePanel(PercentageToSize(MySave.SizePercentage.Width, Width), PercentageToSize(MySave.SizePercentage.Height, Height))
            End If
        Catch ex As Exception
            Logger.Log($"{ex.Message} {ex.StackTrace}")
        End Try
    End Sub

    Private Sub pbDiffuser_Paint(sender As Object, e As PaintEventArgs) Handles pbDiffuser.Paint
        Try
            If drawErrorStringOnScreen AndAlso connectString <> Nothing Then
                e.Graphics.DrawString(connectString, Font, Brushes.White, New PointF(20, 1))
            End If
        Catch ex As Exception
            Logger.Log($"{ex.Message} {ex.StackTrace}")
        End Try
    End Sub

    Protected Overrides Sub OnResize(eventargs As EventArgs)
        UpdatePanelPositionX(PanelX)
        UpdatePanelPositionY(PanelY)
        ResizePanel(PanelWidth, PanelHeight)

        MyBase.OnResize(eventargs)
    End Sub

    Private Sub srgbClient_SettingsChanged(sender As Object, e As SignalRGBSettingsChangedEventArgs) Handles srgbClient.SettingsChanged
        UpdateSRGBConfigValues(e)

        UpdatePanelPositionX(e.LocationPercentage.X)
        UpdatePanelPositionY(e.LocationPercentage.Y)
        ResizePanel(e.SizePercentage)

        With MySave
            .MatrixSizeType = e.MatrixSizeType
            .SmoothingMode = e.SmoothingMode
            .CompositingQuality = e.CompositingQuality
            .InterpolationMode = e.InterpolationMode
            .PixelOffsetMode = e.PixelOffsetMode
            .LedShape = e.LEDShape
            .RoundedRectangleCornerRadius = e.RoundedRectangleCornerRadius
            .LedPadding = e.LEDPadding
            .LedUpdateInterval = e.LEDUpdateInterval
            .CoverImageSizeMode = e.CoverImageSizeMode
            .BackgroundColor = e.BackgroundColor
            .CpuUsagePauseValue = e.CPUUsagePauseValue
            .LocationPercentage = e.LocationPercentage
            .SizePercentage = e.SizePercentage
        End With
        MySave.Save(SaveFile)
    End Sub

    Private Sub UpdatePanelPositionX(percentage As Integer)
        Dim availableWidth As Integer = Width - RGBRect.Width
        Dim newX As Integer = CInt(availableWidth * (percentage / 100))
        RGBRect.Location = New Point(newX, RGBRect.Location.Y)

        If pbDiffuser.InvokeRequired Then
            pbDiffuser.Invoke(Sub() pbDiffuser.Location = RGBRect.Location)
        Else
            pbDiffuser.Location = RGBRect.Location
        End If
    End Sub

    Private Sub UpdatePanelPositionY(percentage As Integer)
        Dim availableHeight As Integer = Height - RGBRect.Height
        Dim newY As Integer = CInt(availableHeight * (percentage / 100))
        RGBRect.Location = New Point(RGBRect.Location.X, newY)

        If pbDiffuser.InvokeRequired Then
            pbDiffuser.Invoke(Sub() pbDiffuser.Location = RGBRect.Location)
        Else
            pbDiffuser.Location = RGBRect.Location
        End If
    End Sub

    Private Sub ResizePanel(sizePercent As Size)
        RGBRect.Size = New Size(PercentageToSize(sizePercent.Width, Width), PercentageToSize(sizePercent.Height, Height))

        If pbDiffuser.InvokeRequired Then
            pbDiffuser.Invoke(Sub() pbDiffuser.Size = RGBRect.Size)
        Else
            pbDiffuser.Size = RGBRect.Size
        End If
    End Sub

    Private Sub ResizePanel(widthPercent As Integer, heightPercent As Integer)
        RGBRect.Size = New Size(PercentageToSize(widthPercent, Width), PercentageToSize(heightPercent, Height))

        If pbDiffuser.InvokeRequired Then
            pbDiffuser.Invoke(Sub() pbDiffuser.Size = RGBRect.Size)
        Else
            pbDiffuser.Size = RGBRect.Size
        End If
    End Sub

    Private Function PercentageToPoint(percentage As Integer, pos As PositionType) As Integer
        Dim availableSize As Integer
        Select Case pos
            Case PositionType.X
                availableSize = Width - RGBRect.Width
            Case PositionType.Y
                availableSize = Height - RGBRect.Height
        End Select
        Dim newVal As Integer = CInt(availableSize * (percentage / 100))
        Return newVal
    End Function

    Private Function PercentageToSize(percentage As Integer, maxSize As Integer) As Integer
        Dim minSize As Integer = 0
        Dim newSize As Integer = CInt(maxSize * (percentage / 100))
        If newSize < minSize Then newSize = minSize
        Return newSize
    End Function

    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        If srgbClient IsNot Nothing Then
            If srgbClient.IsListening Then
                srgbClient.StopListening()
                srgbThread = Nothing
            End If
        End If

        MyBase.OnClosing(e)
    End Sub

    Private Sub pbDiffuser_LocationOrSizeChanged(sender As Object, e As EventArgs) Handles pbDiffuser.LocationChanged, pbDiffuser.SizeChanged
        If srgbClient IsNot Nothing Then
            If srgbClient.IsListening Then
                MySave.Location = pbDiffuser.Location
                MySave.Size = pbDiffuser.Size
                MySave.Save(SaveFile)
            End If
        End If
    End Sub

End Class

Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading

Public Class frmWallpaper

    'Dim OffColor As Color = Color.FromArgb(255, 0, 0, 0)
    Dim configFile As String = WallpaperEngineConfig()
    Dim monitordetection As String = "devicepath"
    Dim display As String = ScreenDevicePath
    Dim configLastDate As Date = Now

    Public cpuUsage As New PerformanceCounter("Processor", "% Processor Time", "_Total")
    Public srgbClient As SignalRGBClient = Nothing
    Public srgbThread As Thread = Nothing

    Dim connectString As String = Nothing
    Dim drawErrorStringOnScreen As Boolean = True

    Private Sub frmWallpaper_Load(sender As Object, e As EventArgs) Handles Me.Load
        If configFile <> "error" Then
            monitordetection = TryGetUserSettings("monitordetection", "devicepath", configFile)
            Select Case monitordetection
                Case "devicepath"
                    display = ScreenDevicePath
                Case "managed"
                    display = ScreenManaged
                Case "layout"
                    display = ScreenLayout
            End Select

            UpdateWEConfigValues(configFile, display)

            configLastDate = File.GetLastWriteTime(configFile)
            tmUpdate.Interval = TimerIntervals
            BackColor = ColorTranslator.FromHtml(BackgroundColor)
            pbDiffuser.Image = If(Utils.BackgroundImage = Nothing, Nothing, Image.FromFile(Utils.BackgroundImage))
            pbDiffuser.SizeMode = Utils.SizeMode

            Connect()
        End If
    End Sub

    Public Sub Connect()
        If IsSignalRGBRunning() Then
            Try
                If srgbClient Is Nothing Then
                    srgbClient = New SignalRGBClient(Port)
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

    Private Function HighCpuUsage() As Boolean
        Return CInt(Math.Ceiling(cpuUsage.NextValue)) >= CpuUsagePauseValue
    End Function

    Private Sub tmUpdate_Tick(sender As Object, e As EventArgs) Handles tmUpdate.Tick
        If Not HighCpuUsage() Then
            Invalidate()
            If connectString <> Nothing Then pbDiffuser.Invalidate()
        End If
    End Sub

    Private Sub frmWallpaper_Closing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing
        If srgbClient IsNot Nothing Then
            If srgbClient.IsListening Then
                srgbClient.StopListening()
                srgbThread = Nothing
            End If

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
                    Dim Width As Integer = srgbClient.MatrixSize.Width
                    Dim Height As Integer = srgbClient.MatrixSize.Height
                    Dim LedCount As Integer = Width * Height

                    Dim rectangleSize As SizeF
                    Try
                        rectangleSize = New SizeF(ClientRectangle.Width / (LedCount / Height), ClientRectangle.Height / Height)
                    Catch ex As Exception
                        'shut up!!
                    End Try


                    Dim matrix(Width - 1, Height - 1) As String
                    Dim count As Integer = 0
                    For j As Integer = 0 To matrix.GetUpperBound(0)
                        For i As Integer = 0 To matrix.GetUpperBound(0)
                            Try
                                Dim rgbColor = srgbClient.Colors(count)
                                Using sb As New SolidBrush(rgbColor)
                                    Dim X As Single = rectangleSize.Width * i
                                    Dim Y As Single = rectangleSize.Height * j
                                    Dim W As Single = rectangleSize.Width
                                    Dim H As Single = rectangleSize.Height
                                    Dim P As Single = LEDPadding

                                    Select Case Utils.LEDShape
                                        Case LEDShape.Rectangle
                                            graphic.FillRectangle(sb, New RectangleF(X + P, Y + P, W - P, H - P))
                                        Case LEDShape.RoundedRectangle
                                            graphic.FillRoundedRectangle(sb, New Rectangle(X + P, Y + P, W - P, H - P), RoundedRectangleCornerRadius)
                                        Case LEDShape.Sphere
                                            graphic.FillEllipse(sb, New RectangleF(X + P, Y + P, W - P, H - P))
                                    End Select
                                End Using
                            Catch ex As Exception
                                'shutup
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
End Class

Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports System.Windows.Threading

Class MainWindow
    Inherits Window

    Public WithEvents srgbClient As SignalRGBClient = Nothing
    Public srgbThread As Thread = Nothing

    Dim connectString As String = Nothing
    Dim drawErrorStringOnScreen As Boolean = True
    Private tmConfig, tmCheckSignalRGB, tmUpdate As DispatcherTimer

    Public Sub New()
        InitializeComponent()

        tmConfig = New DispatcherTimer() With {.Interval = TimeSpan.FromSeconds(5), .IsEnabled = True}
        AddHandler tmConfig.Tick, AddressOf tmConfig_Tick
        tmCheckSignalRGB = New DispatcherTimer() With {.Interval = TimeSpan.FromSeconds(10), .IsEnabled = False}
        AddHandler tmCheckSignalRGB.Tick, AddressOf tmCheckSignalRGB_Tick
        tmUpdate = New DispatcherTimer() With {.Interval = TimeSpan.FromSeconds(1), .IsEnabled = True}
        AddHandler tmUpdate.Tick, AddressOf tmUpdate_Tick
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Try
            If File.Exists(SaveFile) Then
                MySave = New UserSave().Load(SaveFile)
                ReadSaveValues(MySave)
            End If

            Background = BackgroundColor.SetBrushColor()
            ledCanvas.SetBlurIntensity(Utils.BlurIntensity)
            DiffuserImage.Source = If(Utils.BackgroundImage = Nothing, Nothing, Utils.BackgroundImage.TryParseCoverImage())
            DiffuserImage.Stretch = Utils.Stretch
            Connect()
        Catch ex As Exception
            Logger.Log($"{ex.Message} {ex.StackTrace}")
        End Try
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
                    UpdateStatusText()
                    ledCanvas.Client = srgbClient
                Else
                    tmCheckSignalRGB.Stop()
                    srgbThread = New Thread(AddressOf srgbClient.StartListening)
                    With srgbThread
                        .IsBackground = True
                        .Start()
                    End With
                    connectString = Nothing
                    UpdateStatusText()
                    ledCanvas.Client = srgbClient
                End If
            Catch ex As Exception
                Logger.Log($"{ex.Message} {ex.StackTrace}")
                connectString &= $"{vbCrLf}[{Now.ToString("hh:mm:ss tt")}] Connection attempt failed, Local SignalRGB server unavailable."
                UpdateStatusText()
                tmCheckSignalRGB.Start()
            End Try
        Else
            connectString &= $"{vbCrLf}[{Now.ToString("hh:mm:ss tt")}] Connection attempt failed, SignalRGB isn't running."
            UpdateStatusText()
            tmCheckSignalRGB.Start()
        End If
    End Sub

    Private Sub tmUpdate_Tick(sender As Object, e As EventArgs)
        txtFPS.Visibility = If(ShowFps, Visibility.Visible, Visibility.Collapsed)
        If ShowFps Then txtFPS.Text = ledCanvas.CurrentFps

        If connectString <> Nothing Then
            UpdateStatusText()
        End If
    End Sub

    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If srgbClient IsNot Nothing AndAlso srgbClient.IsListening Then
            srgbClient.StopListening()
            srgbThread = Nothing
        End If
    End Sub

    Private Sub tmCheckSignalRGB_Tick(sender As Object, e As EventArgs)
        connectString &= $"{vbCrLf}[{Now.ToString("hh:mm:ss tt")}] Attempting to connect to local SignalRGB server."
        UpdateStatusText()
        Connect()
    End Sub

    Private Sub tmConfig_Tick(sender As Object, e As EventArgs)
        Try
            Background = BackgroundColor.SetBrushColor()
            ledCanvas.SetBlurIntensity(Utils.BlurIntensity)
            DiffuserImage.Source = If(Utils.BackgroundImage = Nothing, Nothing, Utils.BackgroundImage.TryParseCoverImage())
            DiffuserImage.Stretch = Utils.Stretch
        Catch ex As Exception
            Logger.Log($"{ex.Message} {ex.StackTrace}")
        End Try
    End Sub

    Private Sub srgbClient_SettingsChanged(sender As Object, e As SignalRGBSettingsChangedEventArgs) Handles srgbClient.SettingsChanged
        UpdateSRGBConfigValues(e)
        With MySave
            .MatrixSizeType = e.MatrixSizeType
            .MatrixSizeTier = e.MatrixSizeTier
            .CompositingQuality = e.CompositingQuality
            .ShowFps = e.ShowFps
            .BlurIntensity = e.BlurIntensity
            .LedShape = e.LEDShape
            .RoundedRectangleCornerRadius = e.RoundedRectangleCornerRadius
            .LedPadding = e.LEDPadding
            .FPS = e.FPS
            .CoverImageStretch = e.CoverImageStretch
            .BackgroundColor = e.BackgroundColor
            .CpuUsagePauseValue = e.CPUUsagePauseValue
            .CoverImage = e.CoverImage
        End With
        MySave.Save(SaveFile)

        ledCanvas.Client = srgbClient
    End Sub

    Private Sub UpdateStatusText()
        If drawErrorStringOnScreen Then
            txtConnectStatus.Text = connectString
        End If
    End Sub

End Class

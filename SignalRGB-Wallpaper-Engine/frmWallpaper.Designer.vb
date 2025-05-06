<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmWallpaper
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmWallpaper))
        tmConfig = New Timer(components)
        tmCheckSignalRGB = New Timer(components)
        tmUpdate = New Timer(components)
        pbDiffuser = New PictureBox()
        CType(pbDiffuser, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' tmConfig
        ' 
        tmConfig.Enabled = True
        tmConfig.Interval = 5000
        ' 
        ' tmCheckSignalRGB
        ' 
        tmCheckSignalRGB.Interval = 10000
        ' 
        ' tmUpdate
        ' 
        tmUpdate.Enabled = True
        tmUpdate.Interval = 10
        ' 
        ' pbDiffuser
        ' 
        pbDiffuser.BackColor = Color.Transparent
        pbDiffuser.Dock = DockStyle.Fill
        pbDiffuser.Location = New Point(0, 0)
        pbDiffuser.Name = "pbDiffuser"
        pbDiffuser.Size = New Size(512, 288)
        pbDiffuser.SizeMode = PictureBoxSizeMode.AutoSize
        pbDiffuser.TabIndex = 0
        pbDiffuser.TabStop = False
        ' 
        ' frmWallpaper
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Black
        ClientSize = New Size(512, 288)
        ControlBox = False
        Controls.Add(pbDiffuser)
        DoubleBuffered = True
        ForeColor = Color.White
        FormBorderStyle = FormBorderStyle.None
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "frmWallpaper"
        ShowIcon = False
        ShowInTaskbar = False
        StartPosition = FormStartPosition.Manual
        Text = "SignalRGB Wallpaper"
        CType(pbDiffuser, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents tmConfig As Timer
    Friend WithEvents tmCheckSignalRGB As Timer
    Friend WithEvents tmUpdate As Timer
    Friend WithEvents pbDiffuser As PictureBox

End Class

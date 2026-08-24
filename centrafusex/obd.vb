Imports System
Imports System.Diagnostics
Imports System.Xml
Imports System.Web
Imports System.IO
Imports System.Windows.Forms
Imports centrafuse.Plugins
Imports Microsoft.Win32

Namespace OBD

    Public Class OBD
        Inherits CFPlugin

#Region " Constants "

        Private Const PluginName As String = "OBD"
        Private Const PluginPath As String = "plugins\" + PluginName + "\"
        Private Const PluginPathSkins As String = PluginPath + "Skins\"
        Private Const PluginPathLanguages As String = PluginPath + "Languages\"
        Private Const PluginPathIcons As String = PluginPath + "Icons\"
        Private Const ConfigurationFile As String = "config.xml"
        Private Const LogFile As String = "obd.log"

#End Region

#Region " Variables "

        ' we write the log to the appropriate users local appdata directory in the Plugins subfolder...
        ' NOTE:  this is the %programdata% directory, not the %appdata% directory
        ' typically C:\ProgramData\Centrafuse\Centrafuse Auto\dave\Plugins\OBD
        Public LogFilePath As String = CFTools.AppDataPath + "\\Plugins\\" + PluginName + "\\" + LogFile

#End Region

        Public Sub New()
            Debug.Print(My.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData)
            Debug.Print(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData)

            ' Set Settings flags, show in basic settings and advanced settings
            Me.CF_params.hasSettings = True
            Me.CF_params.hasBasicSettings = True

        End Sub

        Private Sub WriteLog(ByVal msg As String)
            Try
                If (Boolean.Parse(Me.pluginConfig.ReadField("/APPCONFIG/LOGEVENTS"))) Then
                    CFTools.writeModuleLog(msg, LogFilePath)
                End If
            Catch
            End Try
        End Sub

#Region "CFPlugin methods"

        ''' <summary>
        ''' Initializes the plugin.  This is called from the main application
        ''' when the plugin is first loaded.
        ''' </summary>
        Public Overrides Sub CF_pluginInit()

            Try
                ' CF3_initPlugin() Will configure pluginConfig and pluginLang automatically
                ' All plugins must call this method once
                Me.CF3_initPlugin("OBD", True)

                ' All controls should be created or Setup in CF_localskinsetup.
                ' This method is also called when the resolution or skin has changed.
                Me.CF_localskinsetup()

                ' loads Settings
                LoadSettings()

                CF_loadConfig("C:\Users\dave.ROBINSON\AppData\Local\Centrafuse\Plugins\OBD\config.xml")
                ' Note CF_loadConfig() must be called before WriteLog() can be used
                WriteLog("CF_pluginInit")

                ' add event handlers for keyboard and power mode change
                AddHandler Me.CF_events.powerModeChanged, AddressOf OBD_CF_Event_powerModeChanged
                AddHandler Me.KeyDown, AddressOf OBD_KeyDown

            Catch errmsg As Exception
                WriteLog(errmsg.ToString())
            End Try
        End Sub


        ''' <summary>
        ''' This is called to setup the skin.  This will usually be called
        ''' in CF_pluginInit.  It will also called by the system when
        ''' the resolution has been changed.
        ''' </summary>
        Public Overrides Sub CF_localskinsetup()

            WriteLog("CF_localskinsetup")
            Try
                ' Read the skin file, controls will be automatically created
                ' CF_localskinsetup() should always call CF3_initSection() first, with the exception of setting any
                ' CF_displayHooks flags, which affect the behaviour of the CF3_initSection() call.
                Me.CF3_initSection("OBD")

                Me.CF_createButtonClick("BTN5", AddressOf BTN5_Click)
                Me.CF_createButtonClick("BTN6", AddressOf BTN6_Click)

            Catch errmsg As Exception
                WriteLog(errmsg.ToString())
            End Try
        End Sub


        ''' <summary>
        ''' This is called by the system when it exits
        ''' or the plugin has been deleted.
        ''' </summary>
        Public Overrides Sub CF_pluginClose()

            WriteLog("CF_pluginClose")
            ' disposes the plugin.
            Me.Dispose()

        End Sub


        ''' <summary>
        ''' This is called by the system when a button
        ''' with this plugin action has been clicked.
        ''' </summary>
        Public Overrides Sub CF_pluginShow()

            WriteLog("CF_pluginShow")

            ' Shows the plugin and selects the listbox.
            Me.Visible = True

            ' If your plugin has a listbox, it's a good idea to give the focus to it when
            ' the plugin shows up. To do so, remove the comment and replace the $LISTBOX-NAME$
            ' with the name of your listbox.
            '$LISTBOX-NAME$.Focus()
        End Sub


        ''' <summary>
        ''' This is called by the system when the
        ''' plugin setup is clicked.
        ''' </summary>
        ''' <returns>Returns the dialog result.</returns>
        Public Overrides Function CF_pluginShowSetup() As DialogResult

            WriteLog("CF_pluginShowSetup")

            ' Return DialogResult.OK for the main application
            ' to update from plugin changes.
            Dim returnvalue As DialogResult = DialogResult.Cancel

            Try

                ' Creates a new plugin setup instance. If you create a CFDialog or CFSetup you must
                ' set its MainForm property to the main plugins MainForm property.
                Dim setup As Setup = New Setup(Me.MainForm, Me.pluginConfig, Me.pluginLang)
                returnvalue = setup.ShowDialog()
                If returnvalue = DialogResult.OK Then
                    LoadSettings() ' reload settings
                    CFTools.writeLog("OBD", "New display name = " + Me.CF_params.displayName)
                End If
                setup.Close()
                setup = Nothing
            Catch errmsg As Exception
                WriteLog(errmsg.ToString())
            End Try

            Return returnvalue
        End Function


        ''' <summary>
        ''' This method is called by the system when it
        ''' pauses all audio.
        ''' </summary>
        Public Overrides Sub CF_pluginPause()

            WriteLog("CF_pluginPause")
        End Sub


        ''' <summary>
        ''' This is called by the system when it
        ''' resumes all audio.
        ''' </summary>
        Public Overrides Sub CF_pluginResume()

            WriteLog("CF_pluginResume")
        End Sub


        ''' <summary>
        ''' Used for plugin to plugin communication.
        ''' Parameters can be passed into CF_Main_systemCommands
        ''' with CF_Actions.PLUGIN, plugin name, plugin command,
        ''' and a command parameter.
        ''' </summary>
        ''' <param name="command">The command to execute.</param>
        ''' <param name="param1">The first parameter.</param>
        ''' <param name="param2">The second parameter.</param>
        Public Overrides Sub CF_pluginCommand(ByVal command As String, ByVal param1 As String, ByVal param2 As String)

            WriteLog("CF_pluginCommand: " + command + " " + param1 + ", " + param2)
        End Sub


        ''' <summary>
        ''' Used for retrieving information from plugins.
        ''' You can run CF_getPluginData with a plugin name,
        '''	command, and parameter to retrieve information
        '''	from other plugins running on the system.
        ''' </summary>
        ''' <param name="command">The command to execute.</param>
        ''' <param name="param">The parameter.</param>
        ''' <returns>Returns whatever is appropriate.</returns>
        Public Overrides Function CF_pluginData(ByVal command As String, ByVal param As String) As String

            WriteLog("CF_pluginData: " + command + " " + param)
            Dim returnValue As String = ""
            Return returnValue
        End Function


        Public Overrides Function CF_pluginCMLCommand(ByVal id As String, ByVal state As CF_ButtonState, ByVal rear As Boolean) As Boolean

            If (state <> CF_ButtonState.Click) Then
                Return False
            End If

            Select Case (id.ToUpper())

                Case "BTN1"
                    BTN1_Click()
                    Return True
                Case "BTN2"
                    BTN2_Click()
                    Return True
                Case "BTN3"
                    BTN3_Click()
                    Return True
                Case "BTN4"
                    BTN4_Click()
                    Return True

            End Select

            Return False

        End Function

#End Region

#Region "System Functions"

        ''' <summary>
        ''' Reloads the plugin settings.
        ''' </summary>
        Private Sub LoadSettings()

            ' The display name is shown in the application to represent
            ' the plugin.  This sets the display name from the configuration file.
            Me.CF_params.displayName = Me.pluginLang.ReadField("/APPLANG/OBD/DISPLAYNAME")

        End Sub

#End Region

#Region "Click Events"

        Private Sub BTN1_Click()
            ' The Button #1 has been clicked...
            WriteLog("BTN1 clicked.")
            Try
                ' Update the text of the bottom on-screen label
                Me.CF_updateText("LBLTEXT", Me.pluginLang.ReadField("/APPLANG/OBD/LBLTEXT1"))
            Catch errmsg As Exception
                WriteLog(errmsg.ToString())
            End Try
        End Sub

        Private Sub BTN2_Click()
            ' The Button #2 has been clicked...
            WriteLog("BTN2 clicked.")
            Try
                ' Update the text of the bottom on-screen label
                Me.CF_updateText("LBLTEXT", Me.pluginLang.ReadField("/APPLANG/OBD/LBLTEXT2"))
            Catch errmsg As Exception
                WriteLog(errmsg.ToString())
            End Try
        End Sub

        Private Sub BTN3_Click()
            ' The Button #3 has been clicked...
            WriteLog("BTN3 clicked.")
            Try
                Dim song As String = ""
                ' Emit CF action to retrieve the name of the current song playing
                song = CF_systemGetText(CF_TextItems.CurrentTitle)
                ' You could also retrieve artist, album, etc...
                ' CF_getText(CF_TextItems.CurrentArtist) 
                ' CF_getText(CF_TextItems.CurrentAlbum)
                ' Update the text of the bottom on-screen label
                ' Update the text of the bottom on-screen label
                Me.CF_updateText("LBLTEXT", song)
            Catch errmsg As Exception
                WriteLog(errmsg.ToString())
            End Try
        End Sub

        Private Sub BTN4_Click()
            ' The Button #4 has been clicked...
            WriteLog("BTN4 clicked.")
            Try
                ' Display an ok box that will make the user confirm a message
                Me.CF_systemDisplayDialog(CF_Dialogs.OkBox, "This is an OK box")
            Catch errmsg As Exception
                WriteLog(errmsg.ToString())
            End Try
        End Sub

        Private Sub BTN5_Click(ByVal sender As Object, ByVal e As MouseEventArgs)
            ' The Button #5 has been clicked...
            WriteLog("BTN5 clicked.")
            Try
                ' Update the text of the bottom on-screen label
                Me.CF_updateText("LBLTEXT", Me.pluginLang.ReadField("/APPLANG/OBD/LBLTEXT1"))
            Catch errmsg As Exception
                WriteLog(errmsg.ToString())
            End Try
        End Sub

        Private Sub BTN6_Click(ByVal sender As Object, ByVal e As MouseEventArgs)
            ' The Button #6 has been clicked...
            WriteLog("BTN6 clicked.")
            Try
                ' Update the text to what user enters in osk
                Dim tempobject As Object = New Object
                Dim resultvalue As String = "", resulttext As String = ""
                If (Me.CF_systemDisplayDialog(CF_Dialogs.OSK, Me.pluginLang.ReadField("/APPLANG/OBD/ENTERTEXT"), "", Nothing, resultvalue, resulttext, tempobject, Nothing, True, True, True, True, False, False, 1) = DialogResult.OK) Then
                    Me.CF_updateText("LBLTEXT", resultvalue)
                End If
            Catch errmsg As Exception
                WriteLog(errmsg.ToString())
            End Try
        End Sub

#End Region

#Region "Other events"

        Private Sub OBD_CF_Event_powerModeChanged(ByVal sender As Object, ByVal e As Microsoft.Win32.PowerModeChangedEventArgs)

        End Sub

        ' If the plugin uses back/forward buttons, we need to catch the left/right keyboard commands too...
        Private Sub OBD_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
            e.Handled = True

            If e.KeyCode = Keys.Left Then
                '---------------------------------------------------------------------------
                ' TODO: replace this if needed
                '--------------------------------------------------------------------------- 
                ' Me.back_Click(Me, New MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0))
            ElseIf e.KeyCode = Keys.Right Then
                '---------------------------------------------------------------------------
                ' TODO: replace this if needed
                '--------------------------------------------------------------------------- 
                ' Me.forward_Click(Me, New MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0))
            End If
        End Sub

#End Region

    End Class

End Namespace

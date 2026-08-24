Imports System
Imports System.Diagnostics
Imports System.Xml
Imports System.Web
Imports System.IO
Imports System.Windows.Forms
Imports centrafuse.Plugins
Imports Microsoft.Win32

Namespace OBD

    ''' <summary>
    ''' Setup class inherits from CFSetup so that it will not show up as a separate
    ''' plugin, but a dialog within a plugin.
    ''' It uses the standard Setup screens from the main application.
    ''' </summary>
    Public Class Setup
        Inherits CFSetup

        ' String constants
        Private Const PluginPath As String = "plugins\OBD\"
        Private Const PluginPathLanguages As String = PluginPath + "Languages\"
        Private Const ConfigurationFile As String = "config.xml"
        Private Const ConfigSection As String = "/APPCONFIG/"
        Private Const LanguageSection As String = "/APPLANG/SETUP/"
        Private Const LanguageControlSection As String = "/APPLANG/OBD/"

        Public Sub New(ByVal mForm As ICFMain, ByVal config As ConfigReader, ByVal lang As LanguageReader)
            Dim PluginName As String = "OBD"
            Dim LogFile As String = "obd.log"

            Me.MainForm = mForm

            Me.pluginConfig = config
            Me.pluginLang = lang

            ' When CF_initSetup() is called, the CFPlugin layer will call back into CF_setupReadSettingsPage() to read the page
            ' Note that Me.pluginConfig and Me.pluginLang must be set before making this call
            Me.CF_initSetup(1, 1)

            'Me.CF_updateText("TITLE", Me.pluginLang.ReadField("/APPLANG/SETUP/TITLE"))
            Me.CF_updateText("TITLE", CFTools.AppDataPath + "\\Plugins\\" + PluginName + "\\" + LogFile)

        End Sub

        ' Reads the configuration file and populates the button text.
        Public Overrides Sub CF_setupReadSettings(ByVal currentpage As Integer, ByVal advanced As Boolean)

            Try
                Dim i As Integer = CFSetupButton.One

                If advanced Then

                    '*******************************************************************************************/
                    '*****  ADVANCED SETTINGS - PAGE 1  ********************************************************/
                    '*******************************************************************************************/
                    If currentpage = 1 Then

                        ' TEXT BUTTONS (1-4)

                        ButtonHandler(i) = AddressOf SetDisplayName
                        ButtonText(i) = Me.pluginLang.ReadField("APPLANG/SETUP/DISPLAYNAME")
                        ButtonValue(i) = Me.pluginLang.ReadField("APPLANG/OBD/DISPLAYNAME")
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                        i = i + 1

                        ' BOOL BUTTONS (5-8)

                        ButtonHandler(i) = AddressOf SetLogEvents
                        ButtonText(i) = Me.pluginLang.ReadField("/APPLANG/SETUP/LOGEVENTS")
                        ButtonValue(i) = Me.pluginConfig.ReadField("/APPCONFIG/LOGEVENTS")
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                    End If
                Else
                    '*******************************************************************************************/
                    '*****  BASIC SETTINGS - PAGE 1  ***********************************************************/
                    '*******************************************************************************************/
                    If currentpage = 1 Then

                        ' TEXT BUTTONS (1-4)

                        ButtonHandler(i) = AddressOf SetDisplayName
                        ButtonText(i) = Me.pluginLang.ReadField("APPLANG/SETUP/DISPLAYNAME")
                        ButtonValue(i) = Me.pluginLang.ReadField("APPLANG/OBD/DISPLAYNAME")
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                        i = i + 1

                        ' BOOL BUTTONS (5-8)

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                        i = i + 1

                        ButtonHandler(i) = Nothing
                        ButtonText(i) = ""
                        ButtonValue(i) = ""
                    End If
                End If
            Catch errmsg As Exception
                CFTools.writeError(errmsg.Message, errmsg.StackTrace)
            End Try
        End Sub

        Private Sub SetDisplayName(ByRef value As Object)

            Try
                Dim tempobject As Object = New Object
                Dim resultvalue As String = "", resulttext As String = ""
                If Me.CF_systemDisplayDialog(CF_Dialogs.OSK, Me.pluginLang.ReadField("/APPLANG/SETUP/DISPLAYNAME"), ButtonValue(value), Nothing, resultvalue, resulttext, tempobject, Nothing, True, True, True, True, False, False, 1) = DialogResult.OK Then
                    Me.pluginLang.WriteField("/APPLANG/OBD/DISPLAYNAME", resultvalue)
                    ButtonValue(value) = resultvalue
                End If

            Catch errmsg As Exception
                CFTools.writeError(errmsg.Message, errmsg.StackTrace)
            End Try
        End Sub

        Private Sub SetLogEvents(ByRef value As Object)
            Me.pluginConfig.WriteField( "/APPCONFIG/LOGEVENTS", value.ToString())
        End Sub

    End Class

End Namespace
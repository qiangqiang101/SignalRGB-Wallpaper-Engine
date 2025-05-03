Item {
    anchors.fill: parent
    
    Column {
        width: 450
        height: 65
        Rectangle {
            width: parent.width
            height: parent.height - 10
            color: Qt.lighter(theme.background2, 1.3)
            radius: 5
            Column {
                x: 10
                y: 10
                width: parent.width - 20
                spacing: 0
                Text {
                    color: theme.primarytextcolor
                    textFormat: Text.RichText
                    text: "<table><tr><td width=\"24\" style=\"text-align:center;vertical-align:middle\"><img src=\"https://raw.githubusercontent.com/SRGBmods/public/main/images/wled/info-circle-fill.png\"></style></td><td><u><strong>Important:<strong></u><br>this service requires <strong><style>a:link { color: \"#FFFFFF\"; }</style><a href=\"https://steamcommunity.com/sharedfiles/filedetails/?id=3475033880\">SignalRGB Wallpaper Engine</a></strong> to work!</td></tr></table>"
                    onLinkActivated: Qt.openUrlExternally(link)
                    font.pixelSize: 13
                    font.family: "Poppins"
                    font.bold: false
                }
            }
        }
    }

    Column {
        y: 70
        width: parent.width
        height: parent.height - 200
        
         Repeater{
            model: service.controllers          

            delegate: Item {
                id: root
                width: 300
                height: content.height
                property var device: model.modelData.obj

                Rectangle {
                    width: parent.width
                    height: parent.height
                    color: Qt.lighter(theme.background2, 1.3)
                    radius: 5
                }

                Column{
                    id: content
                    width: parent.width
                    padding: 10
                    spacing: 5

                    Row{
                        width: parent.width
                        height: childrenRect.height

                        Column{
                            id: leftCol
                            width: 250
                            height: childrenRect.height
                            spacing: 2

                            Text{
                                color: theme.primarytextcolor
                                text: device.name
                                font.pixelSize: 16
                                font.family: "Poppins"
                                font.weight: Font.Bold
                            }

                            Text{
                                color: theme.secondarytextcolor
                                text: "ID: " + device.id
                            }

                            Text{
                                color: theme.secondarytextcolor
                                text: "IP Address: " + (device.ip != "" ? device.ip : "Unknown")
                            }
                        }
                    }
                }
            }  
        }
    }
}

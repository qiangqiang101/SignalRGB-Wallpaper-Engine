Flickable {
    id: rootContainer
    anchors.fill: parent
    contentHeight: mainLayout.height + 40
    clip: true

    ScrollBar.vertical: ScrollBar {
        id: scrollbar
        width: 6
        policy: ScrollBar.AsNeeded
        anchors.right: parent.right
        
        contentItem: Rectangle {
            radius: 3
            color: scrollbar.active ? "#009bde" : "#444"
            opacity: scrollbar.active ? 0.8 : 0.4
            
            Behavior on color { ColorAnimation { duration: 150 } }
            Behavior on opacity { NumberAnimation { duration: 150 } }
        }
    }

    Column {
        id: mainLayout
        x: 15
        y: 15
        width: parent.width - 30
        spacing: 20

        // Info Banner
        Rectangle {
            width: Math.min(parent.width, 500)
            height: 75
            color: Qt.rgba(0.08, 0.48, 1, 0.15) // Icey blue translucent background
            radius: 8
            border.color: Qt.rgba(0.08, 0.48, 1, 0.3)
            border.width: 1

            Row {
                anchors.fill: parent
                anchors.margins: 12
                spacing: 12

                Rectangle {
                    width: 32
                    height: 32
                    radius: 16
                    color: Qt.rgba(0.08, 0.48, 1, 0.25)
                    anchors.verticalCenter: parent.verticalCenter

                    Text {
                        anchors.centerIn: parent
                        text: "i"
                        color: "#FFFFFF"
                        font.family: "Poppins"
                        font.pixelSize: 16
                        font.bold: true
                    }
                }

                Text {
                    width: parent.width - 54
                    color: theme.primarytextcolor
                    textFormat: Text.RichText
                    text: "<strong>Important:</strong> this service requires <strong><style>a:link { color: \"#009bde\"; }</style><a href=\"https://steamcommunity.com/sharedfiles/filedetails/?id=3475033880\">SignalRGB Wallpaper Engine</a></strong> to work correctly."
                    onLinkActivated: Qt.openUrlExternally(link)
                    font.pixelSize: 12
                    font.family: "Poppins"
                    wrapMode: Text.WordWrap
                    anchors.verticalCenter: parent.verticalCenter
                }
            }
        }

        // Add Device Form Card
        Column {
            width: Math.min(parent.width, 500)
            spacing: 8

            Text {
                text: "Add Wallpaper Device"
                color: theme.primarytextcolor
                font.pixelSize: 14
                font.family: "Poppins"
                font.bold: true
            }

            Rectangle {
                width: parent.width
                height: 160
                color: Qt.lighter(theme.background2, 1.1)
                radius: 8
                border.color: Qt.lighter(theme.background2, 1.3)
                border.width: 1

                Column {
                    anchors.fill: parent
                    anchors.margins: 15
                    spacing: 15

                    // Device Name (Full Width)
                    Column {
                        width: parent.width
                        spacing: 4
                        Text {
                            text: "Device Name"
                            color: theme.secondarytextcolor
                            font.pixelSize: 11
                            font.family: "Poppins"
                        }
                        Rectangle {
                            width: parent.width
                            height: 36
                            color: Qt.darker(theme.background2, 1.1)
                            radius: 6
                            border.color: nameInput.activeFocus ? "#009bde" : Qt.lighter(theme.background2, 1.3)
                            border.width: nameInput.activeFocus ? 1.5 : 1
                            
                            Behavior on border.color { ColorAnimation { duration: 150 } }

                            TextField {
                                id: nameInput
                                anchors.fill: parent
                                leftPadding: 10
                                rightPadding: 10
                                color: theme.primarytextcolor
                                font.pixelSize: 12
                                font.family: "Poppins"
                                placeholderText: "Wallpaper Device"
                                selectByMouse: true
                                background: Item {}
                            }
                        }
                    }

                    Row {
                        width: parent.width
                        spacing: 10

                        // Port
                        Column {
                            width: (parent.width - 10) * 0.5
                            spacing: 4
                            Text {
                                text: "Port"
                                color: theme.secondarytextcolor
                                font.pixelSize: 11
                                font.family: "Poppins"
                            }
                            Rectangle {
                                width: parent.width
                                height: 36
                                color: Qt.darker(theme.background2, 1.1)
                                radius: 6
                                border.color: portInput.activeFocus ? "#009bde" : Qt.lighter(theme.background2, 1.3)
                                border.width: portInput.activeFocus ? 1.5 : 1

                                Behavior on border.color { ColorAnimation { duration: 150 } }

                                TextField {
                                    id: portInput
                                    anchors.fill: parent
                                    leftPadding: 10
                                    rightPadding: 10
                                    color: theme.primarytextcolor
                                    font.pixelSize: 12
                                    font.family: "Poppins"
                                    placeholderText: "8133-8136"
                                    text: ""
                                    selectByMouse: true
                                    background: Item {}
                                }
                            }
                        }

                        // Add Button
                        Column {
                            width: (parent.width - 10) * 0.5
                            height: 56
                            
                            Rectangle {
                                anchors.bottom: parent.bottom
                                width: parent.width
                                height: 36
                                color: addMouseArea.pressed ? "#0078ad" : (addMouseArea.containsMouse ? "#00aeff" : "#009bde")
                                radius: 6
                                
                                Behavior on color { ColorAnimation { duration: 150 } }

                                Text {
                                    anchors.centerIn: parent
                                    text: "Add Device"
                                    color: "#FFFFFF"
                                    font.bold: true
                                    font.pixelSize: 12
                                    font.family: "Poppins"
                                }

                                MouseArea {
                                    id: addMouseArea
                                    anchors.fill: parent
                                    hoverEnabled: true
                                    cursorShape: Qt.PointingHandCursor
                                    onClicked: {
                                        discovery.addManualDevice(
                                            nameInput.text.trim() || "Wallpaper Device",
                                            "127.0.0.1",
                                            portInput.text.trim() || "8133"
                                        );
                                        nameInput.text = "";
                                        portInput.text = "8133";
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Device List Section
        Column {
            width: Math.min(parent.width, 500)
            spacing: 8

            Text {
                text: "Configured Devices"
                color: theme.primarytextcolor
                font.pixelSize: 14
                font.family: "Poppins"
                font.bold: true
            }

            Repeater {
                model: service.controllers          

                delegate: Item {
                    id: root
                    width: 500
                    height: 86
                    property var device: model.modelData.obj

                    Rectangle {
                        width: parent.width
                        height: parent.height - 8
                        color: Qt.lighter(theme.background2, 1.2)
                        radius: 8
                        border.color: Qt.lighter(theme.background2, 1.4)
                        border.width: 1

                        Row {
                            anchors.fill: parent
                            anchors.margins: 12
                            spacing: 12

                            // Device Icon/Indicator
                            Rectangle {
                                width: 44
                                height: 44
                                radius: 22
                                color: "#009bde"
                                anchors.verticalCenter: parent.verticalCenter
                                opacity: removeMouseArea.containsMouse ? 0.8 : 1
                                Behavior on opacity { NumberAnimation { duration: 150 } }
                                
                                Image {
                                    anchors.centerIn: parent
                                    width: 26
                                    height: 26
                                    source: "https://raw.githubusercontent.com/qiangqiang101/SignalRGB-Wallpaper-Engine/refs/heads/main/srgbwallpaper.png"
                                    fillMode: Image.PreserveAspectFit
                                }
                            }

                            // Info text
                            Column {
                                width: parent.width - 110
                                anchors.verticalCenter: parent.verticalCenter
                                spacing: 2

                                Text {
                                    color: theme.primarytextcolor
                                    text: device.name
                                    font.pixelSize: 13
                                    font.family: "Poppins"
                                    font.bold: true
                                    elide: Text.ElideRight
                                }

                                Text {
                                    color: theme.secondarytextcolor
                                    text: device.ip + ":" + device.port
                                    font.pixelSize: 11
                                    font.family: "Poppins"
                                }

                                Text {
                                    color: theme.secondarytextcolor
                                    text: "ID: " + device.id
                                    font.pixelSize: 9
                                    font.family: "Poppins"
                                    elide: Text.ElideRight
                                }
                            }

                            // Remove button
                            Rectangle {
                                width: 28
                                height: 28
                                radius: 14
                                color: removeMouseArea.pressed ? "#c22b2b" : (removeMouseArea.containsMouse ? "#ff4d4d" : "#e63946")
                                anchors.verticalCenter: parent.verticalCenter
                                
                                Behavior on color { ColorAnimation { duration: 150 } }

                                Text {
                                    anchors.centerIn: parent
                                    text: "✕"
                                    color: "#FFFFFF"
                                    font.bold: true
                                    font.pixelSize: 11
                                }

                                MouseArea {
                                    id: removeMouseArea
                                    anchors.fill: parent
                                    hoverEnabled: true
                                    cursorShape: Qt.PointingHandCursor
                                    onClicked: {
                                        discovery.removeManualDevice(device.id);
                                    }
                                }
                            }
                        }
                    }
                }  
            }
        }
    }
}

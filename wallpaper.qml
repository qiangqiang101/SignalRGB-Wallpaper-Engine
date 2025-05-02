Item {
    anchors.fill: parent
    
    Column {
        width: parent.width
        height: parent.height
        
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
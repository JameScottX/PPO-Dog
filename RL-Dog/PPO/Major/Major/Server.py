from PyQt5.QtCore import *
from PyQt5.QtWidgets import *
from PyQt5.QtNetwork import *
import string
import copy



class Server(object):

    Receive_Msg  = pyqtSignal(str);

    def __init__(self,port = 50213):
  
        self.tcpServer = None
        self.networkSession = None      
        self.clientNum = 0    
        self.shutFlag =False

        self.port = port

    def buildServer(self):

        manager = QNetworkConfigurationManager()

        if manager.capabilities() & QNetworkConfigurationManager.NetworkSessionRequired:
            settings = QSettings(QSettings.UserScope, 'QtProject')
            settings.beginGroup('QtNetwork')
            id = settings.value('DefaultNetworkConfiguration', '')
            settings.endGroup()
  
            config = manager.configurationFromIdentifier(id)
            if config.state() & QNetworkConfiguration.Discovered == 0:
                config = manager.defaultConfiguration()
  
            self.networkSession = QNetworkSession(config, self)
            self.networkSession.opened.connect(self.sessionOpened)
  
            self.statusLabel.setText("Opening network session.")
            self.networkSession.open()
        else:
            self.sessionOpened()
  
        self.tcpServer.newConnection.connect(self.ClientInit)

    def sessionOpened(self):

        if self.networkSession is not None:
            config = self.networkSession.configuration()
  
            if config.type() == QNetworkConfiguration.UserChoice:
                id = self.networkSession.sessionProperty('UserChoiceConfiguration')
            else:
                id = config.identifier()
  
            settings = QSettings(QSettings.UserScope, 'QtProject')
            settings.beginGroup('QtNetwork')
            settings.setValue('DefaultNetworkConfiguration', id)
            settings.endGroup();
  
        self.tcpServer = QTcpServer()
        self.tcpServer.listen(QHostAddress.Any,self.port)

        if self.tcpServer.isListening() == False:
            QMessageBox.critical(self, "Fortune Server",
                    "Unable to start the server: %s." % self.tcpServer.errorString())
            self.close()
            return
  
        for ipAddress in QNetworkInterface.allAddresses():
            if ipAddress != QHostAddress.LocalHost and ipAddress.toIPv4Address() != 0:
                break
        else:
            ipAddress = QHostAddress(QHostAddress.LocalHost)

        print(ipAddress.toString())
        print(self.tcpServer.serverPort())

        self.ipAddress =ipAddress.toString()
        self.ipPort = self.tcpServer.serverPort()
        
    def ClientInit(self):

        if self.clientNum ==1:

            self.serverShutCurrentClient()
            print("Sudden client builds the connection")
        

        if self.clientNum==0:

            print("New client builds the connection")
            self.clientConnection = self.tcpServer.nextPendingConnection()
            self.clientConnection.disconnected.connect(self.serverShutCurrentClient)
            self.clientConnection.readyRead.connect(self.readData)
            self.clientNum = 1
            self.shutFlag =True

    def sendData(self,str_msg):

        if self.clientNum == 1:

            block = QByteArray()        
            block.append(str_msg)         
            self.clientConnection.write(block)
            

    def readData(self):

        tempdata = self.clientConnection.read(1024)
        tempstr = str(tempdata)        
        tempstr = tempstr.replace("b'",'')
        tempstr = tempstr.replace("'",'')

        self.Receive_Msg.emit(tempstr)


    def serverShutCurrentClient(self):

        self.clientConnection.disconnectFromHost()
        self.clientNum =0
        print("Disconnect with the last client")
        
    def serverShutDown(self):

        if self.shutFlag :
            self.clientConnection.close()
            self.shutFlag =False
        print("The Server is shut down!")







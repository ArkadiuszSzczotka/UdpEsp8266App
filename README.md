# **Communicator UDP TCP MQTT Switch Control App**

The **Communicator UDP TCP MQTT** is a modern .NET MAUI application designed for controlling devices using either **TCP** or **MQTT** communication protocols. The app provides a dynamic interface with toggling switches and allows users to configure communication settings, ensuring seamless device interaction.

## **Features**

- 🌐 **Flexible Communication**: Choose between **UDP**(MainPage), **TCP** or **MQTT** as the communication type.
- 🛠️ **Configurable Settings**:
  - **UDP**: Leverages an active connection managed by `MainPage`.
  - **TCP**: Leverages an active connection managed by `TcpPage`.
  - **MQTT**: Supports broker configuration, topic selection, and QoS levels.
- 🔄 **Dynamic Switch Controls**: 
  - Toggle device states with labeled switches.
  - Scalable design that can handle more switches in the future.
- 📜 **Error Handling**:
  - Displays success and error messages via consistent `DisplayAlert` dialogs.
- 💡 **MVVM Architecture**:
  - Clean separation of concerns between the View, ViewModel, and business logic.

---

## **Screenshots**
![screenshot2](https://github.com/ArkadiuszSzczotka/UdpEsp8266App/blob/main/Screenshots/Screenshot2.jpg?raw=true)
![screenshot6](https://github.com/ArkadiuszSzczotka/UdpEsp8266App/blob/main/Screenshots/Screenshot6.jpg?raw=true)



---

## **Getting Started**

### **Prerequisites**
- .NET 6 or higher installed.
- Visual Studio 2022 with **.NET MAUI** workload.
- NuGet packages:
  - [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm)
  - [MQTTnet](https://www.nuget.org/packages/MQTTnet)

### **Installation**
1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/switch-control-app.git
   cd switch-control-app
   ```
2. Open the project in Visual Studio 2022.
3. Restore NuGet packages:
    ```bash
     dotnet restore
    ```
4. Run the app on your target platform (Android, iOS, Windows, or Mac).


### **How to use**


## **Communication** Type Selection
1. Open the app and select a communication type:
 - UDP: Messages are sent or received using the UDP protocol.
 - TCP: Messages are sent or received through a persistent TCP connection.
 - MQTT: Messages are sent to an MQTT broker.

## **Configuring UDP**
1. Navigate to the MainPage for UDP communication.
2. Configure the following settings:
 - Local Port: Enter the port on which the app listens for incoming messages.
 - Remote IP: Enter the IP address of the remote device.
 - Remote Port: Enter the port to send messages to.
3. Click Start Connection to establish the UDP connection.

## **Configuring TCP**
1. Navigate to the TCP Configuration page.
2. Start the server with the desired IP address and port (according to what you have in the client-side code).
The Switch Control page uses this active TCP connection to send messages.

## **Configuring MQTT**
1. Enter the MQTT Broker URL, Topic, and QoS.
2. Click Connect to MQTT to establish a connection.

## **Toggling Switches**
1. Use the switches on the Switch Control page to toggle device states.
The app sends the state (ON or OFF) via the selected communication type.



## **Project Architecture**
### **Pages**

1. TcpPage:
   - Manages the TCP connection (server-side logic).
2. SwitchControlPage:
   - Dynamic interface for toggling switches.
   - Configures and uses the selected communication type (TCP or MQTT).

## **ViewModel**
1. SwitchControlViewModel:
   - Manages switch state changes.
   - Handles TCP and MQTT communication logic.
   - Implements configurable communication settings (MQTT Broker, Topic, QoS).

## **Models**
1. SwitchModel:
   - Represents individual switches with their labels and states.

## **Dependency Injection**
1. TcpPage is registered as a singleton to share the TCP connection across pages.
2. SwitchControlPage is registered as transient for dynamic state management.


### **Technologies Used**
  - .NET MAUI: For cross-platform development.
  - CommunityToolkit.Mvvm: For MVVM architecture and command handling.
  - MQTTnet: For robust MQTT communication.
  - Dependency Injection: Built-in DI container in .NET MAUI for managing services.
    
---


### **Future Enhancements**
    🛠️ Add support for WebSocket communication.
    📈 Expand switch count up.
    🔒 Add secure communication:
      - TLS for MQTT.
      - Authentication for TCP.
    📊 Add real-time monitoring of message status.
    🌍 Support for multiple languages.

### **Contributing**

## **Contributions are welcome! To contribute:**

1. Fork the repository.
2. Create new branch:
   ```bash
    git checkout -b feature-name
   ```
3. Make your changes and commit:
    ```bash
    git commit -m "Add feature name"
    ```
4. Push the changes:
    ```bash
    git push origin feature-name
    ```
5. Open a pull request on GitHub.

License
This project is licensed under the MIT License. See the LICENSE file for details.

## **Contact:**
  📧 Email: szczotka.ark@gmail.com <br>
  🌐 GitHub: https://github.com/ArkadiuszSzczotka <br>
  🐦 X: https://x.com/Arek_Szczotka <br>
    [![text](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/arkadiusz-szczotka-b48601233/)

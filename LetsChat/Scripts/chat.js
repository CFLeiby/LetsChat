"use strict";

//Disable send button until connection is established
var button = document.getElementById("sendButton");
button.disabled = true;

//Currently selected room
var currentRoom = null;

//Message list for the current room
var messageList = document.getElementById("messagesList");

//User input; start focus here
var userInput = document.getElementById("userInput");
userInput.focus();

//Box to enter a message. Pressing enter should fire send
var msgInput = document.getElementById("messageInput");
msgInput.addEventListener('keypress', function (event) {
    if (event.keyCode == 13)
        button.click();
});

//Connection to our hub
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

//When they click the Send button, we'll send the message through the connection
button.addEventListener("click", function (event) {    
    event.preventDefault();

    var user = userInput.value;
    if (!user) {
        alert("Tell us who you are.");
        userInput.focus();
        return;
    }

    if (!currentRoom) {
        alert("Pick a room first.");
        return;
    }

    var msgText = msgInput.value;
    if (!msgText) {
        alert("What are you trying to say?");
        msgInput.focus();
        return;
    }

    var message = {
        ChatRoom: currentRoom,
        MessageContent: msgText,
        UserName: user
    }

    connection.invoke("PostMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    msgInput.value = "";
    msgInput.focus();
});

//When they switch rooms, we'll want to leave the old one and join the new one
document.getElementById("roomSelect").addEventListener("change", function (event) {
    connection.invoke("LeaveRoom", currentRoom);
    currentRoom = event.target.value;
    messageList.innerHTML = "";
    connection.invoke("JoinRoom", currentRoom);
});

//Handle the ReceiveMessage event by adding the content to our list of messages
connection.on("ReceiveMessage", function (message) {
    var li = document.createElement("li");
    li.textContent = message.userName + ": " + message.messageContent;
    messageList.appendChild(li);
});

//Start up the connection
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


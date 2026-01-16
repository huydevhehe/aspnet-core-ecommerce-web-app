document.addEventListener("DOMContentLoaded", function () {
    const chatToggle = document.getElementById("chat-toggle");
    const chatBox = document.getElementById("chat-box");
    const closeChat = document.getElementById("close-chat");
    const sendChat = document.getElementById("send-chat");
    const chatMessages = document.getElementById("chat-messages");
    const chatText = document.getElementById("chat-text");

    // Mở chatbox với hiệu ứng
    chatToggle.addEventListener("click", function () {
        chatBox.classList.toggle("show-chat");
    });

    // Đóng chatbox
    closeChat.addEventListener("click", function () {
        chatBox.classList.remove("show-chat");
    });

    // Gửi tin nhắn
    sendChat.addEventListener("click", sendMessage);
    chatText.addEventListener("keypress", function (event) {
        if (event.key === "Enter") sendMessage();
    });

    function sendMessage() {
        const message = chatText.value.trim();
        if (message === "") return;

        chatMessages.innerHTML += `<p class="user-message"><strong>Bạn:</strong> ${message}</p>`;

        fetch("/Chat/GetAnswer", {
            method: "POST",
            body: JSON.stringify(message),
            headers: { "Content-Type": "application/json" }
        })
            .then(response => response.json())
            .then(data => {
                chatMessages.innerHTML += `<p class="bot-message"><strong>Bot:</strong> ${data.reply}</p>`;
                chatMessages.scrollTop = chatMessages.scrollHeight;
            });

        chatText.value = "";
    }
});

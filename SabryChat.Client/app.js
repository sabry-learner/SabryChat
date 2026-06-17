const userNameInput = document.getElementById("userName");
const chatRoomInput = document.getElementById("chatRoom");

const joinBtn = document.getElementById("joinBtn");
const joinModal = document.getElementById("joinModal");

const messagesDiv = document.getElementById("messages");
const messageInput = document.getElementById("messageInput");
const sendBtn = document.getElementById("sendBtn");

const leaveBtn = document.getElementById("leaveBtn");

const notificationBtn = document.getElementById("notificationBtn");
const notificationDrawer = document.getElementById("notificationDrawer");
const notificationsDiv = document.getElementById("notifications");
const notificationBadge = document.getElementById("notificationBadge");
const closeNotificationBtn = document.getElementById("closeNotificationBtn");

const notifySound = document.getElementById("notifySound");

const navOnlineUsers = document.getElementById("navOnlineUsers");
const navOnlineCount = document.getElementById("navOnlineCount");

let userName = "";
let chatRoom = "";
let unread = 0;

let connection = null;
let isConnected = false;

// ================= CONNECT =================
function createConnection() {

    if (connection) return;

    connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5014/chat")
        .withAutomaticReconnect()
        .build();

    // ================= MESSAGES =================
    connection.on("ReceiveMessage", (user, message) => {

        const div = document.createElement("div");
        div.className = `message-row ${user === userName ? "me" : "other"}`;

        div.innerHTML = `
            <div class="avatar">${user[0].toUpperCase()}</div>
            <div class="message-content">
                <div class="sender-name">${user}</div>
                <div class="message-bubble">${message}</div>
            </div>
        `;

        messagesDiv.appendChild(div);
        messagesDiv.scrollTop = messagesDiv.scrollHeight;
    });

    // ================= SYSTEM =================
    connection.on("SystemMessage", (msg) => {

        const div = document.createElement("div");
        div.className = "system-message";
        div.textContent = msg;

        messagesDiv.appendChild(div);
    });

    // ================= ONLINE USERS =================
    connection.on("UpdateUsers", (users) => {

        if (!users) return;

        const filtered = users.filter(u => u !== userName);

        navOnlineCount.textContent = `${filtered.length} online`;

        navOnlineUsers.innerHTML = "";

        filtered.forEach(u => {

            const avatar = document.createElement("div");

            avatar.className =
                "w-8 h-8 rounded-full bg-purple-600 flex items-center justify-center text-xs font-bold border border-white/20";

            avatar.textContent = u[0].toUpperCase();
            avatar.title = u;

            navOnlineUsers.appendChild(avatar);
        });
    });

    // ================= NOTIFICATION =================
    connection.on("ReceiveNotification", (n) => {

        if (!n) return;

        addNotification(n.senderName, n.message);

        if (notifySound) {
            notifySound.currentTime = 0;
            notifySound.play().catch(() => {});
        }
    });
}

// ================= JOIN =================
joinBtn.onclick = async () => {

    userName = userNameInput.value.trim();
    chatRoom = chatRoomInput.value.trim();

    if (!userName || !chatRoom) return;

    createConnection();

    // 🚨 avoid reconnect spam
    if (!isConnected) {
        await connection.start();
        isConnected = true;
    }

    await connection.invoke("JoinChat", userName, chatRoom);

    joinModal.style.display = "none";
};

// ================= SEND =================
sendBtn.onclick = sendMessage;

messageInput.addEventListener("keydown", e => {
    if (e.key === "Enter") sendMessage();
});

async function sendMessage() {

    const msg = messageInput.value.trim();
    if (!msg) return;

    await connection.invoke("SendMessage", userName, chatRoom, msg);

    messageInput.value = "";
}

// ================= LEAVE =================
leaveBtn.onclick = async () => {

    if (connection) {
        await connection.invoke("LeaveChat", userName, chatRoom);

        await connection.stop();

        connection = null;
        isConnected = false;
    }

    location.reload();
};

// ================= NOTIFICATIONS =================
function addNotification(user, message) {

    const card = document.createElement("div");
    card.className = "notification-card";

    card.innerHTML = `
        <div class="notification-sender">${user}</div>
        <div class="notification-message">${message}</div>
    `;

    notificationsDiv.prepend(card);

    unread++;
    updateBadge();
}

function updateBadge() {

    if (unread > 0) {
        notificationBadge.classList.remove("hidden");
        notificationBadge.textContent = unread;
    }
}

// ================= OPEN / CLOSE =================
notificationBtn.onclick = () => {

    notificationDrawer.classList.remove("translate-x-full");

    unread = 0;
    notificationBadge.classList.add("hidden");
};

closeNotificationBtn.onclick = () => {
    notificationDrawer.classList.add("translate-x-full");
};
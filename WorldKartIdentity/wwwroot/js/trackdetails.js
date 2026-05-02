window.process = window.process || { env: { NODE_ENV: "production" } }

import * as P from "../lib/pintura/pintura.js";

const userRes = await fetch('/user/me');
const user = await userRes.json();
let editor = null;

//pintura
if (user.authenticated) {
    console.log(window.trackContext.trackBase64);
    P.setPlugins(P.plugin_crop, P.plugin_annotate);

    editor = P.appendEditor(".drawing-canvas", {
        src: `data:image/png;base64,${window.trackContext.trackBase64}`,
        enableCanvasRenderer: false,
        imageReader: P.createDefaultImageReader(),
        imageWriter: P.createDefaultImageWriter(),

        ...P.markup_editor_defaults,
        shapePreprocessor: P.createDefaultShapePreprocessor(),

        // keep the needed tools
        markupEditorToolbar: P.createMarkupEditorToolbar([
            "sharpie",
            "arrow",
            "rectangle",
            "eraser",
            "path"
        ]),
        locale: {
            ...P.locale_en_gb,
            ...P.plugin_crop_locale_en_gb,
            ...P.plugin_annotate_locale_en_gb,
            ...P.markup_editor_locale_en_gb
        }
    });

    //on done editing
    editor.on("process", async (result) => {
        const blob = result.dest; //proccessed image(as blob)
        const imageBase64 = await blobToBase64(blob);

        const res = await fetch("/Track/CreateTrajectory", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                trackId: window.trackContext.trackId,
                base64: imageBase64
            })
        });

    });
}
else {
    const canvas = document.querySelector(".drawing-canvas");
    canvas.innerHTML = `
     <img alt="Track Layout Schematic" class="object-contain drop-shadow-[0_0_30px_rgba(225,29,72,0.1)] rounded-3 h-full w-full" src="data:image/png;base64,${window.trackContext.trackBase64}"/>
    `;

}


//ai chat
const chatHistory = document.getElementById('apex-chat-history');
const chatInput = document.getElementById('apex-chat-input');
const sendBtn = document.getElementById('apex-chat-send');

function scrollToBottom() {
    chatHistory.scrollTop = chatHistory.scrollHeight;
}

function addMessage(text, type = 'user') {
    const time = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    const messageDiv = document.createElement('div');
    messageDiv.className = 'space-y-1 message-animate' + (type === 'user' ? ' flex flex-col items-end' : '');

    const bubbleClass = type === 'user'
        ? 'bg-surface-container-highest p-3 rounded-lg rounded-tr-none border-r-2 border-primary-container text-right'
        : 'bg-surface-container-low p-3 rounded-lg rounded-tl-none border-l-2 border-primary-container';

    const label = type === 'user' ? 'YOU' : 'SYSTEM';

    messageDiv.innerHTML = `
      <div class="${bubbleClass}">
<p class="text-sm text-on-surface leading-relaxed">${text}</p>
</div>
<span class="text-[8px] font-bold text-outline uppercase tracking-widest ${type === 'user' ? 'mr-1' : 'ml-1'}">${label} • ${time}</span>
    `;

    chatHistory.appendChild(messageDiv);
    scrollToBottom();
}

function showTypingIndicator() {
    const id = 'typing-indicator';
    if (document.getElementById(id)) return;

    const indicator = document.createElement('div');
    indicator.id = id;
    indicator.className = 'space-y-1 message-animate';
    indicator.innerHTML = `
      <div class="bg-surface-container-low p-3 rounded-lg rounded-tl-none border-l-2 border-primary-container inline-flex items-center gap-1 text-primary-container">
<span class="typing-dot"></span>
<span class="typing-dot"></span>
<span class="typing-dot"></span>
</div>
    `;
    chatHistory.appendChild(indicator);
    scrollToBottom();
}

function removeTypingIndicator() {
    const indicator = document.getElementById('typing-indicator');
    if (indicator) indicator.remove();
}

async function handleSend() {
    const text = chatInput.value.trim();
    const { dest } = await editor.processImage();

    if (!text) return;

    addMessage(text, 'user');
    chatInput.value = '';
    showTypingIndicator();
    const fd = new FormData();

    fd.append("text", text);
    fd.append("image", dest, "track.png");
    console.log(fd);

    const res = await fetch("/Track/GetAdviceOnTrack", {
        method: "POST",
        body:fd
    });

    const data = await res.json();
    console.log(data);
    removeTypingIndicator();
    addMessage(data.response, 'ai');


}

sendBtn.addEventListener('click', await handleSend);
chatInput.addEventListener('keypress', async (e) => {
    if (e.key === 'Enter') await handleSend();
});



const worktime = window.trackContext.trackWorktime; // e.g. "9:00 - 20:00"

const dot = document.getElementById('statusDot');
const statusText = document.getElementById('statusText');
const label = document.getElementById('countdownLabel');
const countdown = document.getElementById('countdown');

function pad(n) { return String(n).padStart(2, '0'); }

function parseMinutes(str) {
    const [h, m] = str.trim().split(':').map(Number);
    return h * 60 + m;
}

function update() {
    const parts = worktime.split('-');
    const openMins = parseMinutes(parts[0]);
    const closeMins = parseMinutes(parts[1]);

    const now = new Date();
    const nowMins = now.getHours() * 60 + now.getMinutes();
    const isOpen = nowMins >= openMins && nowMins < closeMins;

    if (isOpen) {
        dot.style.background = '#22c55e';   // green
        statusText.textContent = 'ОТВОРЕНО';
        label.textContent = 'Затваря след';

        const totalSecs = (closeMins - nowMins) * 60 - now.getSeconds();
        countdown.textContent = `${pad(Math.floor(totalSecs / 3600))}h:${pad(Math.floor((totalSecs % 3600) / 60))}m`;
    } else {
        dot.style.background = '#ef4444';   // red
        statusText.textContent = 'ЗАТВОРЕНО';
        label.textContent = 'Отваря след';

        const targetMins = nowMins < openMins ? openMins : openMins + 1440;
        const totalSecs = (targetMins - nowMins) * 60 - now.getSeconds();
        countdown.textContent = `${pad(Math.floor(totalSecs / 3600))}h:${pad(Math.floor((totalSecs % 3600) / 60))}h}`;
    }
}

update();
setInterval(update, 60000);

const likeIcon = document.querySelector('.like-icon');
const likeBtn = likeIcon.parentElement;
const likeCount = document.querySelector('.likes');

if (likeBtn) {
    likeBtn.addEventListener('click', async () => {
        likeBtn.classList.toggle('bg-surface-container-highest');
        likeBtn.classList.toggle('bg-rose-600');

        likeIcon.classList.toggle('text-primary-container');
        likeIcon.classList.toggle('text-white');

        const res = await fetch(`/Track/ToggleLike?trackId=${window.trackContext.trackId}`, { method: "POST" });
        const resData = await res.json();
        likeCount.textContent = resData.likes;
         
    });
}


async function blobToBase64(blob) {
    const arrayBuffer = await blob.arrayBuffer();
    const bytes = new Uint8Array(arrayBuffer);

    let binary = "";
    bytes.forEach(b => binary += String.fromCharCode(b));

    return btoa(binary);
}


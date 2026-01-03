const fileInput = document.getElementById('file-input');
const previewContainer = document.getElementById('preview-container');
const postBtn = document.getElementById('post-btn');
const postText = document.getElementById('post-text');

let selectedFiles = [];

// Handle File Selection and Previews
fileInput.addEventListener('change', (e) => {
    const files = Array.from(e.target.files);
    files.forEach(file => {
        if (selectedFiles.length < 4) {
            selectedFiles.push(file);
            renderPreviews();
        }
    });
    updatePostButton();
});

function renderPreviews() {
    previewContainer.innerHTML = '';
    selectedFiles.forEach((file, index) => {
        const reader = new FileReader();
        reader.onload = (e) => {
            const div = document.createElement('div');
            div.className = 'preview-wrapper';
            div.innerHTML = `
                    <img src="${e.target.result}" class="preview-img shadow-sm">
                    <button class="remove-img" onclick="removeImage(${index})"><i class="bi bi-x-lg"></i></button>
                `;
            previewContainer.appendChild(div);
        };
        reader.readAsDataURL(file);
    });
}

window.removeImage = (index) => {
    selectedFiles.splice(index, 1);
    renderPreviews();
    updatePostButton();
};

// Button Logic
function updatePostButton() {
    const hasText = postText.value.trim().length > 0;
    const hasMedia = selectedFiles.length > 0;

    if (hasText || hasMedia) {
        postBtn.disabled = false;
        postBtn.classList.remove('opacity-50');
    } else {
        postBtn.disabled = true;
        postBtn.classList.add('opacity-50');
    }
}

postText.addEventListener('input', updatePostButton);

// Reset modal on close
const myModalEl = document.getElementById('threadModal');
myModalEl.addEventListener('hidden.bs.modal', () => {
    postText.value = '';
    selectedFiles = [];
    renderPreviews();
    updatePostButton();
});





//like/remove-like functionality

const heartBtn = document.getElementById('heartBtn');

heartBtn.addEventListener('click', async () => {
    heartBtn.classList.toggle('is-liked');
    console.log(heartBtn.getAttribute("data-bid"));
    const res = await fetch(`/Blog/ToggleLike?bid=${Number(heartBtn.getAttribute('data-bid'))}`, { method: 'POST' });
    const resBody = await res.json();
    heartBtn.querySelector(".likes-count").textContent = resBody.likes;
});
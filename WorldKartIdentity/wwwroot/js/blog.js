
//const fileInput = document.getElementById('file-input');
//const previewContainer = document.getElementById('preview-container');
//const postBtn = document.getElementById('post-btn');
//const postText = document.getElementById('post-text');

//let selectedFiles = [];

//// Handle File Selection and Previews
//fileInput.addEventListener('change', (e) => {
//    const files = Array.from(e.target.files);
//    files.forEach(file => {
//        if (selectedFiles.length < 4) {
//            selectedFiles.push(file);
//            renderPreviews();
//        }
//    });
//    updatePostButton();
//});

//function renderPreviews() {
//    previewContainer.innerHTML = '';
//    selectedFiles.forEach((file, index) => {
//        const reader = new FileReader();
//        reader.onload = (e) => {
//            const div = document.createElement('div');
//            div.className = 'preview-wrapper';
//            div.innerHTML = `
//                    <img src="${e.target.result}" class="preview-img shadow-sm">
//                    <button class="remove-img" onclick="removeImage(${index})"><i class="bi bi-x-lg"></i></button>
//                `;
//            previewContainer.appendChild(div);
//        };
//        reader.readAsDataURL(file);
//    });
//}

//window.removeImage = (index) => {
//    selectedFiles.splice(index, 1);
//    renderPreviews();
//    updatePostButton();
//};

//// Button Logic
//function updatePostButton() {
//    const hasText = postText.value.trim().length > 0;
//    const hasMedia = selectedFiles.length > 0;

//    if (hasText || hasMedia) {
//        postBtn.disabled = false;
//        postBtn.classList.remove('opacity-50');
//    } else {
//        postBtn.disabled = true;
//        postBtn.classList.add('opacity-50');
//    }
//}

//postText.addEventListener('input', updatePostButton);

//// Reset modal on close
//const myModalEl = document.getElementById('threadModal');
//myModalEl.addEventListener('hidden.bs.modal', () => {
//    postText.value = '';
//    selectedFiles = [];
//    renderPreviews();
//    updatePostButton();
//});





////toggle like button
//const heartButtons = document.querySelectorAll('.heart-btn');


//heartButtons.forEach(heartBtn =>
//{
//    heartBtn.addEventListener('click', async () => {
//        heartBtn.classList.toggle('is-liked');
//        console.log(heartBtn.getAttribute("data-bid"));
//        const res = await fetch(`/Blog/ToggleLike?bid=${Number(heartBtn.getAttribute('data-bid'))}`, { method: 'POST' });
//        const resBody = await res.json();
//        heartBtn.querySelector(".likes-count").textContent = resBody.likes;
//    });
//});


//const buttons = document.querySelectorAll('.bi-chat');
//const commentSection = document.querySelector('.comment-section');
//const postBtnWrap = document.querySelector('.post-btn-wrap');
//const blogContainer = document.querySelector('.blog-container');

//buttons.forEach(b => {
//    b.addEventListener('click', () => {
//        commentSection.classList.toggle('open');
//        blogContainer.classList.toggle('retracted');
//        postBtnWrap.classList.toggle('retracted');
//    });
//});


//document.querySelector('.close-btn').addEventListener('click', () => {
//    commentSection.classList.toggle('open');
//    blogContainer.classList.toggle('retracted');
//});

//const AVATAR_COLORS = [
//    { bg: '#E6F1FB', text: '#0C447C' },
//    { bg: '#EAF3DE', text: '#3B6D11' },
//    { bg: '#FAEEDA', text: '#854F0B' },
//    { bg: '#FBEAF0', text: '#993556' },
//    { bg: '#E1F5EE', text: '#0F6E56' },
//];//used for pfps, remove in .net mvc when using actual images


////in .net mvc when loading comments get them from a fetch req;
//let comments = [
//    { id: 1, name: 'Alex Morgan', time: '2h ago', text: 'Really insightful — the optimization section clicked for me.', likes: 4, liked: false },
//    { id: 2, name: 'Jamie Park', time: '5h ago', text: 'Great write-up! Any chance of a follow-up on edge cases?', likes: 2, liked: false },
//    { id: 3, name: 'Sofia Reyes', time: '1d ago', text: 'Bookmarked. Clearest explanation I\'ve found on this topic so far.', likes: 7, liked: false },
//];// sample data, replace with fetch from server in .net mvc
//let nextId = 4;

////in .net mvc when loading comments get them from a fetch req;



//function getInitials(name) {
//    return name.split(' ').map(w => w[0]).join('').toUpperCase();
//}//used for pfps, remove in .net mvc when using actual images

//function getAvatarStyle(name) {
//    const i = name.charCodeAt(0) % AVATAR_COLORS.length;
//    const c = AVATAR_COLORS[i];
//    return `background:${c.bg}; color:${c.text}`;
//}//used for pfps, remove in .net mvc when using actual imagesk


//function renderComments() {
//    const list = document.getElementById('comments-list');
//    list.innerHTML = '';

//    comments.forEach((c, i) => {
//        // Comment node
//        const comment = document.createElement('div');
//        comment.className = 'comment';
//        comment.style.animationDelay = `${i * 0.05}s`;
//        comment.innerHTML = `
//          <div class="avatar" style="${getAvatarStyle(c.name)}">${getInitials(c.name)}</div>
//          <div class="comment-body">
//            <div class="comment-meta">
//              <span class="comment-name">${c.name}</span>
//              <span class="comment-time">${c.time}</span>
//            </div>
//            <p class="comment-text">${c.text}</p>
//            <div class="comment-actions">
//              <button class="action-btn ${c.liked ? 'liked' : ''}" onclick="toggleLike(${c.id})">
//                ${c.liked ? '♥' : '♡'} ${c.likes}
//              </button>
//              <button class="action-btn">Reply</button>
//            </div>
//          </div>
//        `;
//        list.appendChild(comment);

//        // Divider between comments
//        if (i < comments.length - 1) {
//            const div = document.createElement('div');
//            div.className = 'divider';
//            list.appendChild(div);
//        }
//    });

//    updateCount();
//}

//function updateCount() {
//    const n = comments.length;
//    document.getElementById('count-badge').textContent = n + ' comment' + (n !== 1 ? 's' : '');
//    document.getElementById('main-count').textContent = n;
//}

//function toggleLike(id) {
//    const c = comments.find(x => x.id === id);
//    if (!c) return;
//    c.liked = !c.liked;
//    c.likes += c.liked ? 1 : -1;
//    renderComments();
//}


//function postComment() {
//    const ta = document.getElementById('new-comment');
//    const text = ta.value.trim();
//    if (!text) return;
//    comments.unshift({ id: nextId++, name: 'You', time: 'just now', text, likes: 0, liked: false });
//    ta.value = '';
//    document.getElementById('send-btn').disabled = true;
//    renderComments();
//    document.getElementById('comments-list').scrollTop = 0;
//}

//document.getElementById('new-comment').addEventListener('input', function () {
//    document.getElementById('send-btn').disabled = !this.value.trim();
//});

//// Keyboard: Escape closes .comment-section
//document.addEventListener('keydown', e => {
//    if (e.key === 'Escape') {
//        commentSection.classList.remove('open');
//        blogContainer.classList.remove('retracted');
//    }
//});

//renderComments();

function toggleComments(title) {
    const drawer = document.getElementById('commentDrawer');
    const overlay = document.getElementById('drawerOverlay');
    const drawerTitle = document.getElementById('drawerTitle');
    const drawerSubtitle = document.getElementById('drawerSubtitle');

    drawerTitle.innerText = "COMMENTS";
    drawerSubtitle.innerText = title.toUpperCase();

    drawer.classList.toggle('active');
    overlay.classList.toggle('active');

    if (drawer.classList.contains('active')) {
        document.body.style.overflow = 'hidden';
    } else {
        document.body.style.overflow = '';
    }
}

function closeComments() {
    document.getElementById('commentDrawer').classList.remove('active');
    document.getElementById('drawerOverlay').classList.remove('active');
    document.body.style.overflow = '';
}

function togglePostModal() {
    const modal = document.getElementById('postModal');
    const content = document.getElementById('modalContent');

    modal.classList.toggle('active');
    content.classList.toggle('active');

    if (modal.classList.contains('active')) {
        document.body.style.overflow = 'hidden';
    } else {
        document.body.style.overflow = '';
    }
}

function handleImagePreview(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            const previewArea = document.getElementById('imagePreview');
            const previewImg = document.getElementById('previewImg');
            previewImg.src = e.target.result;
            previewArea.classList.remove('hidden');
        };
        reader.readAsDataURL(input.files[0]);
    }
}

function removeImage() {
    const previewArea = document.getElementById('imagePreview');
    const previewImg = document.getElementById('previewImg');
    const input = document.getElementById('imageInput');

    input.value = '';
    previewImg.src = '';
    previewArea.classList.add('hidden');
}


document.querySelectorAll(".like-buttons").forEach(likeBtn =>
{
    likeBtn.addEventListener('click', async () => {
        likeBtn.classList.toggle('text-rose-600');

        const res = await fetch(`/Blog/ToggleLike?bid=${Number(likeBtn.getAttribute('data-bid'))}`, { method: 'POST' });
        const resBody = await res.json();
        likeBtn.querySelector(".likes").textContent = resBody.likes;
    });
});
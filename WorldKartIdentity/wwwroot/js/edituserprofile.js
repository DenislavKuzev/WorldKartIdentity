document.getElementById('profile-upload').addEventListener('change', function (event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            // Update both the large preview and the top bar avatar for consistency
            document.getElementById('profile-preview').src = e.target.result;
            const topAvatar = document.querySelector('header img');
            if (topAvatar) topAvatar.src = e.target.result;
        };
        reader.readAsDataURL(file);
    }
});
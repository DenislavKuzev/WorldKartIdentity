document.addEventListener("DOMContentLoaded", function () {
    const linkInput = document.getElementById("locationInput");
    const mapContainer = document.getElementById("mapPreviewContainer");

    function hideMap() {
        mapContainer.style.display = "none";
        mapContainer.innerHTML = "";
    }

    function showMap(html) {
        mapContainer.innerHTML = html;
        mapContainer.style.display = "block";
    }

    linkInput.addEventListener("input", function () {
        const url = linkInput.value.trim();
        if (!url) {
            hideMap();
            return;
        }

        if (!url.includes("google.com") && !url.includes("maps.app.goo.gl")) {
            hideMap();
            return;
        }

        const embedMatch = url.match(/\/maps\/embed\?pb=/);
        if (embedMatch) {
            showMap(`<iframe src="${url}" allowfullscreen="" loading="lazy"></iframe>`);
            return;
        }

        const atMatch = url.match(/@(-?\d+\.\d+),(-?\d+\.\d+)/);
        if (atMatch) {
            const [_, lat, lng] = atMatch;
            const src = `https://maps.google.com/maps?q=${lat},${lng}&z=15&output=embed`;
            showMap(`<iframe src="${src}" allowfullscreen="" loading="lazy"></iframe>`);
            return;
        }

        hideMap();
    });
});

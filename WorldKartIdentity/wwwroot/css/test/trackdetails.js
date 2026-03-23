document.addEventListener("DOMContentLoaded", () => {
    const animatedBlocks = document.querySelectorAll(
        ".panel, .stat-card, .benefit-card, .timeline-item"
    );

    animatedBlocks.forEach((element) => {
        element.classList.add("reveal");
    });

    const observer = new IntersectionObserver(
        (entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                    observer.unobserve(entry.target);
                }
            });
        },
        {
            threshold: 0.12,
            rootMargin: "0px 0px -30px 0px",
        }
    );

    animatedBlocks.forEach((element) => observer.observe(element));

    const backButton = document.querySelector(".back-button");
    if (backButton) {
        backButton.addEventListener("click", (event) => {
            if (backButton.getAttribute("href") === "#") {
                event.preventDefault();
                window.history.back();
            }
        });
    }
});
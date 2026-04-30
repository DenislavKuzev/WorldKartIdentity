document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('track-search');
    const trackGrid = document.getElementById('track-grid');
    const trackCards = trackGrid.querySelectorAll('.group');
    const filterBtn = document.getElementById('filter-btn');
    const sortBtn = document.getElementById('sort-btn');

    // Search Functionality
    searchInput.addEventListener('input', (e) => {
        const query = e.target.value.toLowerCase().trim();
        trackCards.forEach(card => {
            const title = card.querySelector('h3').textContent.toLowerCase();
            if (title.includes(query)) {
                card.style.display = 'flex';
            } else {
                card.style.display = 'none';
            }
        });
    });

    // Button Feedback
    filterBtn.addEventListener('click', () => {
        console.log('Filter menu clicked');
        filterBtn.classList.add('ring-2', 'ring-primary');
        setTimeout(() => filterBtn.classList.remove('ring-2', 'ring-primary'), 500);
    });

    sortBtn.addEventListener('click', () => {
        console.log('Sort menu clicked');
        sortBtn.classList.add('ring-2', 'ring-primary');
        setTimeout(() => sortBtn.classList.remove('ring-2', 'ring-primary'), 500);
    });
});
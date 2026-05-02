function toggleAccordion(header) {
    const panel = header.closest('.accordion-panel');
    const content = panel.querySelector('.accordion-content');
    const chevron = panel.querySelector('.accordion-chevron');

    if (content.classList.contains('hidden')) {
        content.classList.remove('hidden');
        chevron.classList.add('rotate-180');
    } else {
        content.classList.add('hidden');
        chevron.classList.remove('rotate-180');
    }
}